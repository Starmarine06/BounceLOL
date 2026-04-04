using UnityEngine;

public class ButtonT : MonoBehaviour
{
    public int ItemID;

    public void SelectTrail()
    {
        PlayerPrefs.SetInt("Trail", ItemID);
    }
}
