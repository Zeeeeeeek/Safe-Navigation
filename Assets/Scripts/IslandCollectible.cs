using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IslandEnterEvent : UnityEvent<string> {}

public class IslandCollectible : MonoBehaviour
{
    public string islandId;
    public IslandEnterEvent onPlayerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Lancia l'evento con l'ID dell'isola come parametro
        onPlayerEnter?.Invoke(islandId);
    }
}