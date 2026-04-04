using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SimpleLockerSettings
{
    public string displayName = "Category";
    public Sprite[] icons;               // 0-based: icons[0] is default
    public GameObject rowPrefab;         // prefab with Item0..ItemN-1 children (or Item1..)
    public Transform parentContent;      // content transform for rows
    public int itemsPerRow = 4;
}

public class LockerManager : MonoBehaviour
{
    [Header("Simple categories")]
    public SimpleLockerSettings skins;
    public SimpleLockerSettings trails;

    [Header("Optional UI")]
    public Text debugText;

    // runtime caches
    private List<GameObject> skinRows = new List<GameObject>();
    private List<GameObject> trailRows = new List<GameObject>();

    void Start()
    {
        RegenerateAll();
    }

    // PUBLIC: regenerate both categories (call after purchases)
    public void RegenerateAll()
    {
        GenerateCategory(skins, skinRows, true);   // true = isSkin
        GenerateCategory(trails, trailRows, false); // false = isTrail
    }

    // helper to regenerate single category on demand
    public void RegenerateSkins() => GenerateCategory(skins, skinRows, true);
    public void RegenerateTrails() => GenerateCategory(trails, trailRows, false);

    // Equip helper - immediately equip chosen item after purchase or click
    public void EquipSkin(int localIndex)
    {
        // call ButtonL.SelectCharacter() if present on displayed slot
        TryCallButtonL(skins, skinRows, localIndex);
    }

    public void EquipTrail(int localIndex)
    {
        // write PlayerPrefs "Trail" so your LineRendererScript reads it
        PlayerPrefs.SetInt("Trail", localIndex);
        PlayerPrefs.Save();

        // call ButtonT.SelectTrail() if present on displayed slot (optional)
        TryCallButtonT(trails, trailRows, localIndex);
    }

    // Scene change helper
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    // ----------------------------
    // INTERNAL: category generation
    // ----------------------------
    private void GenerateCategory(SimpleLockerSettings cfg, List<GameObject> rowsList, bool isSkin)
    {
        if (cfg == null)
        {
            Debug.LogWarning("LockerManager: category config null.");
            return;
        }
        if (cfg.icons == null || cfg.icons.Length == 0)
        {
            Debug.LogWarning($"LockerManager: {cfg.displayName} icons empty.");
            ClearRows(rowsList);
            return;
        }
        if (cfg.rowPrefab == null || cfg.parentContent == null)
        {
            Debug.LogError($"LockerManager: {cfg.displayName} missing rowPrefab or parentContent.");
            ClearRows(rowsList);
            return;
        }

        // clear existing rows
        ClearRows(rowsList);

        // create first row
        GameObject curRow = Instantiate(cfg.rowPrefab, cfg.parentContent);
        curRow.name = cfg.displayName + "_row_0";
        rowsList.Add(curRow);

        bool zeroIndexed = curRow.transform.Find("Item0") != null;

        // disable all slots initially
        DisableAllSlotsInRow(curRow, cfg.itemsPerRow, zeroIndexed);

        int filled = 0;

        // always show default index 0 in first slot
        SetupSlot(curRow, 0, cfg, 0, isSkin);
        filled = 1;

        // show purchased items sequentially (starting from index 1)
        for (int i = 1; i < cfg.icons.Length; i++)
        {
            bool purchased = isSkin ? PlayerPrefs.GetInt("Spurchased" + i, 0) == 1
                                    : PlayerPrefs.GetInt("Tpurchased" + i, 0) == 1;
            if (!purchased) continue;

            if (filled >= cfg.itemsPerRow)
            {
                // create new row
                curRow = Instantiate(cfg.rowPrefab, cfg.parentContent);
                curRow.name = cfg.displayName + "_row_" + (rowsList.Count);
                rowsList.Add(curRow);
                DisableAllSlotsInRow(curRow, cfg.itemsPerRow, zeroIndexed);
                filled = 0;
            }

            SetupSlot(curRow, filled, cfg, i, isSkin);
            filled++;
        }

        // hide unused slots across rows
        foreach (var r in rowsList)
            HideUnusedSlotsInRow(r, cfg.itemsPerRow, zeroIndexed);

        if (debugText != null)
            debugText.text = $"{cfg.displayName}: rows={rowsList.Count}";
    }

    private void SetupSlot(GameObject row, int slotIndex, SimpleLockerSettings cfg, int localIndex, bool isSkin)
    {
        bool zeroIndexed = row.transform.Find("Item0") != null;
        Transform slotT = GetSlotTransform(row, slotIndex, zeroIndexed);
        if (slotT == null)
        {
            Debug.LogWarning($"LockerManager.SetupSlot: slot not found in row '{row.name}' index {slotIndex}");
            return;
        }

        GameObject slotGO = slotT.gameObject;
        slotGO.SetActive(true);

        // set icon
        Sprite sp = (localIndex >= 0 && localIndex < cfg.icons.Length) ? cfg.icons[localIndex] : null;
        if (sp != null) ApplyIconRobust(slotGO, sp);

        // set existing ButtonL or ButtonT ItemIDs if present (do NOT add components)
        if (isSkin)
        {
            var bl = slotGO.GetComponent<ButtonL>() ?? slotGO.GetComponentInChildren<ButtonL>(true);
            if (bl != null) bl.ItemID = localIndex;
        }
        else
        {
            var bt = slotGO.GetComponent<ButtonT>() ?? slotGO.GetComponentInChildren<ButtonT>(true);
            if (bt != null) bt.ItemID = localIndex;
        }

        // ensure clicking the slot button calls equip (adds runtime listener but does not remove editor listeners)
        var btn = slotGO.GetComponent<Button>() ?? slotGO.GetComponentInChildren<Button>(true);
        if (btn != null)
        {
            int li = localIndex;
            if (isSkin) btn.onClick.AddListener(() => EquipSkin(li));
            else btn.onClick.AddListener(() => EquipTrail(li));
        }
    }

    // find child by name Item{n} or fallback to nth child
    private Transform GetSlotTransform(GameObject row, int slotIndex, bool zeroIndexed)
    {
        string name = zeroIndexed ? $"Item{slotIndex}" : $"Item{slotIndex + 1}";
        Transform t = row.transform.Find(name);
        if (t != null) return t;

        // try opposite index name too (tolerance)
        string alt = zeroIndexed ? $"Item{slotIndex + 1}" : $"Item{slotIndex}";
        t = row.transform.Find(alt);
        if (t != null) return t;

        // fallback to nth child
        if (slotIndex < row.transform.childCount) return row.transform.GetChild(slotIndex);
        return null;
    }

    private void DisableAllSlotsInRow(GameObject row, int count, bool zeroIndexed)
    {
        for (int i = 0; i < count; i++)
        {
            Transform t = GetSlotTransform(row, i, zeroIndexed);
            if (t != null) t.gameObject.SetActive(false);
        }
    }

    private void HideUnusedSlotsInRow(GameObject row, int count, bool zeroIndexed)
    {
        for (int i = 0; i < count; i++)
        {
            Transform t = GetSlotTransform(row, i, zeroIndexed);
            if (t == null) continue;

            // consider a slot used if it has non-empty Image sprite or ButtonL/ButtonT present
            bool keep = false;
            var img = t.GetComponentInChildren<Image>(true);
            if (img != null && img.sprite != null) keep = true;
            if (!keep && (t.GetComponent<ButtonL>() != null || t.GetComponentInChildren<ButtonL>(true) != null)) keep = true;
            if (!keep && (t.GetComponent<ButtonT>() != null || t.GetComponentInChildren<ButtonT>(true) != null)) keep = true;

            t.gameObject.SetActive(keep);
        }
    }

    private void ClearRows(List<GameObject> rows)
    {
        if (rows == null) return;
        foreach (var r in rows) if (r != null) Destroy(r);
        rows.Clear();
    }

    private void TryCallButtonL(SimpleLockerSettings cfg, List<GameObject> rows, int localIndex)
    {
        if (rows == null) return;
        foreach (var row in rows)
        {
            foreach (Transform slot in row.transform)
            {
                var bl = slot.GetComponent<ButtonL>() ?? slot.GetComponentInChildren<ButtonL>(true);
                var img = slot.GetComponentInChildren<Image>(true);
                // try to identify by image sprite matching cfg.icons[localIndex]
                if (bl != null)
                {
                    // set id then call
                    bl.ItemID = localIndex;
                    bl.SelectCharacter();
                    return;
                }
            }
        }
    }

    private void TryCallButtonT(SimpleLockerSettings cfg, List<GameObject> rows, int localIndex)
    {
        if (rows == null) return;
        foreach (var row in rows)
        {
            foreach (Transform slot in row.transform)
            {
                var bt = slot.GetComponent<ButtonT>() ?? slot.GetComponentInChildren<ButtonT>(true);
                if (bt != null)
                {
                    bt.ItemID = localIndex;
                    bt.SelectTrail();
                    return;
                }
            }
        }
    }

    // Robust icon setter: Icon child -> any Image -> SpriteRenderer; sets native size & alpha
    private void ApplyIconRobust(GameObject slot, Sprite sprite)
    {
        if (sprite == null) return;

        Transform iconChild = slot.transform.Find("Icon");
        if (iconChild != null)
        {
            var img = iconChild.GetComponent<Image>();
            if (img != null) { img.sprite = sprite; img.SetNativeSize(); img.color = new Color(1,1,1,1); return; }
            var sr = iconChild.GetComponent<SpriteRenderer>();
            if (sr != null) { sr.sprite = sprite; return; }
        }

        var images = slot.GetComponentsInChildren<Image>(true);
        if (images != null && images.Length > 0)
        {
            // pick first image that looks like an icon (best-effort)
            Image chosen = images[0];
            foreach (var im in images)
            {
                if (im.gameObject.name.ToLower().Contains("icon")) { chosen = im; break; }
            }
            chosen.sprite = sprite;
            chosen.SetNativeSize();
            chosen.color = new Color(1,1,1,1);
            return;
        }

        var srs = slot.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs != null && srs.Length > 0)
        {
            srs[0].sprite = sprite;
            return;
        }

        Debug.LogWarning($"LockerManager: could not apply sprite '{sprite?.name}' to slot '{slot.name}' (no Image or SpriteRenderer).");
    }
}