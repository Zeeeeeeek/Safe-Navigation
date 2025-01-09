using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IslandEnterEvent : UnityEvent<string> {}

public class IslandCollectible : MonoBehaviour
{
    public string islandId;
    public IslandEnterEvent onPlayerEnter;
    [DllImport("__Internal")]
    private static extern void SetScore(int score);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        #if !UNITY_EDITOR && UNITY_WEBGL
            SetScore(1000);
        #else
            Debug.Log("Player entered island");
        #endif
        onPlayerEnter?.Invoke(islandId);
    }
}