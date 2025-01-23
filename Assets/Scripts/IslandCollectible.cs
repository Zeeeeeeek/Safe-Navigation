using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IslandEnterEvent : UnityEvent<IslandDTO> {}

public class IslandCollectible : MonoBehaviour
{
    public string islandId;
    public IslandEnterEvent onPlayerEnter;
    public Answer answer;
    public JsonResourcesReader.Content content;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        #if !UNITY_EDITOR && UNITY_WEBGL
            SetScore(1000);
        #endif
        onPlayerEnter?.Invoke(new IslandDTO(islandId, answer, content));
    }
}