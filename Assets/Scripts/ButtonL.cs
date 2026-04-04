using UnityEngine;

public class ButtonL : MonoBehaviour
{
    public int ItemID;

    public void SelectCharacter()
    {
        PlayerPrefs.SetInt("selectedOption", ItemID);
    }
    
}
