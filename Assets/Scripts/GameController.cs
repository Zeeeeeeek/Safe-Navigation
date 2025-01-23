using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public GameObject[] islandPrefabs;
    public int islandCount;
    public float minDistance = 2.0f;
    public int maxAttempts = 10;
    private readonly List<Vector2> _islandPositions = new();
    public GameObject resourceCanvas;
    public GameObject endGameCanvas;
    private Answer _currentAnswer = Answer.UNKNOWN;
    private int _currentIsland = 0;
    
    private void Start()
    {
        PopulatePositions();
        SpawnIslands();
    }

    private void SpawnIslands()
    {
        for (var i = 0; i < _islandPositions.Count; i++)
        {
            var islandPrefab = islandPrefabs[Random.Range(0, islandPrefabs.Length)];
            var island = Instantiate(islandPrefab, _islandPositions[i], Quaternion.identity);

            var collectible = island.GetComponent<IslandCollectible>();
            if (collectible == null) continue;
            collectible.islandId = $"Island_{i + 1}";
            collectible.onPlayerEnter.AddListener(OnPlayerEnterIsland);
            collectible.answer = Answer.SAFE;
        }
    }
    
    private void OnPlayerEnterIsland(IslandDTO islandDto)
    {
        if (resourceCanvas.activeSelf) return;
        resourceCanvas.SetActive(true);
        resourceCanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = islandDto.islandId;
        var player = FindFirstObjectByType<PlayerController>();
        player.CanMove = false;
        _currentAnswer = islandDto.answer;
        _currentIsland = int.Parse(islandDto.islandId.Split('_')[1]);
    }
    
    public void SafeAnswer()
    {
        SubmitAnswer(Answer.SAFE);
    }

    public void SpamAnswer()
    {
        SubmitAnswer(Answer.SPAM);
    }
    
    private void SubmitAnswer(Answer answer)
    {
        if (_currentAnswer == Answer.UNKNOWN)
        {
            Debug.Log("No current answer set");
        }
        else
        {

            if (_currentAnswer != answer)
            {
                var healthDisplay = FindFirstObjectByType<HealthDisplay>();
                healthDisplay.DecreaseHealth();
            }
            CloseCanvas();
            var islands = FindObjectsByType<IslandCollectible>(FindObjectsSortMode.None);
            foreach (var island in islands)
            {
                if (island.islandId != $"Island_{_currentIsland}") continue;
                Destroy(island.gameObject);
                var islandDisplay = FindFirstObjectByType<IslandDisplay>();
                islandDisplay.DecreaseIslandCount();
                break;
            }
            CheckForEndGame();
        }
    }

    private void CheckForEndGame()
    {
        var islands = FindFirstObjectByType<IslandDisplay>().islandCount;
        var hp = FindFirstObjectByType<HealthDisplay>().health;
        if (hp == 0)
        {
            Debug.Log("Game Over");
        }
        else if (islands == 0)
        {
            endGameCanvas.SetActive(true);
            var player = FindFirstObjectByType<PlayerController>();
            player.CanMove = false;
            endGameCanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                hp * 100/islandCount >= 60 ? "Hai vinto!" : "Hai perso!";
        }
        
    }

    private void CloseCanvas()
    {
        resourceCanvas.SetActive(false);
        var player = FindFirstObjectByType<PlayerController>();
        player.CanMove = true;
        _currentAnswer = Answer.UNKNOWN;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && resourceCanvas.activeSelf)
        {
            CloseCanvas();
        }
    }


    private void PopulatePositions()
    {
        for (var i = 0; i < islandCount; i++)
        {
            var placed = false;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var x = Random.Range(-16, 17);
                var y = Random.Range(-11, 12);
                var pos = new Vector2(x, y);

                if (!IsPositionValid(pos)) continue;
                _islandPositions.Add(pos);
                placed = true;
                break;
            }
            
            if (!placed)
            {
                Debug.LogWarning($"Impossibile posizionare l'isola {i + 1} dopo {maxAttempts} tentativi.");
            }
        }
    }

    private bool IsPositionValid(Vector2 newPos)
    {
        return newPos != Vector2.zero && _islandPositions.All(pos => !(Vector2.Distance(pos, newPos) < minDistance));
    }

    public void onClickEndButton()
    {
        var hp = FindFirstObjectByType<HealthDisplay>().health;
        Debug.Log($"Setting score to {hp*100/islandCount}");
    }
}