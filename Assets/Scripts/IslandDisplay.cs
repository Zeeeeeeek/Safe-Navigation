using TMPro;
using UnityEngine;

public class IslandDisplay : MonoBehaviour
{
    
    public int islandCount = 10;
    public TextMeshProUGUI islandText;
    
    public void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        islandText.text = $"{islandCount}";
    }
    
    public void DecreaseIslandCount()
    {
        if (islandCount <= 0) return;
        islandCount--;
        UpdateText();
    }
   
}
