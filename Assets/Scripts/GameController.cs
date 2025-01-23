using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public GameObject[] islandPrefabs;
    public int islandCount;
    public float minDistance = 2.0f;
    public int maxAttempts = 10;
    private readonly List<Vector2> _islandPositions = new();
    public GameObject emailCanvas;
    public GameObject linkCanvas;
    public GameObject endGameCanvas;
    private Answer _currentAnswer = Answer.UNKNOWN;
    private int _currentIsland = 0;
    
    private List<JsonResourcesReader.Content> _resources;
    
    [DllImport("__Internal")]
    private static extern void SetFinalScore(int points);
    
    private void Start()
    {
        var jsonReader = new JsonResourcesReader();

        StartCoroutine(jsonReader.ReadResources(resources =>
        {
            _resources = resources.OrderBy(x => Random.value).ToList();
            if (_resources.Count > 0)
            {
                PopulatePositions();
                SpawnIslands();
            }
            else
            {
                Debug.LogError("No resources were loaded. Check the JSON file.");
            }
        }));
        
    }

    private void SpawnIslands()
    {
        for (var i = 0; i < _islandPositions.Count; i++)
        {
            var islandPrefab = islandPrefabs[Random.Range(0, islandPrefabs.Length)];
            var island = Instantiate(islandPrefab, _islandPositions[i], Quaternion.identity);

            var collectible = island.GetComponent<IslandCollectible>();
            var resource = _resources[i];
            if (collectible == null) continue;
            collectible.islandId = $"Island_{i + 1}";
            collectible.onPlayerEnter.AddListener(OnPlayerEnterIsland);
            collectible.answer = resource.GetAnswerEnum();
            collectible.content = resource;
        }
    }
    
    private void OnPlayerEnterIsland(IslandDTO islandDto)
    {
        if (emailCanvas.activeSelf || linkCanvas.activeSelf) return;
        if (islandDto.content.GetTypeEnum() == JsonResourcesReader.Type.Email)
        {
            var texts = emailCanvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            for (var i = 0; i < texts.Length; i++)
            {
                texts[i].text = islandDto.content.elements[i];
            }
            emailCanvas.SetActive(true);
        }
        else
        {
            linkCanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = islandDto.content.elements[0];
            linkCanvas.SetActive(true);
        }
        
        var player = FindFirstObjectByType<PlayerController>();
        player.CanMove = false;
        
        _currentAnswer = islandDto.content.GetAnswerEnum();
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
        linkCanvas.SetActive(false);
        emailCanvas.SetActive(false);
        foreach (var t in emailCanvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>())
        {
            t.text = "";
        }

        foreach (var t in linkCanvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>())
        {
            t.text = "";
        }
        var player = FindFirstObjectByType<PlayerController>();
        player.CanMove = true;
        _currentAnswer = Answer.UNKNOWN;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (emailCanvas.activeSelf || linkCanvas.activeSelf))
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
                var playerPosition = FindFirstObjectByType<PlayerController>().transform.position;
                if (!IsPositionValid(pos) || Vector2.Distance(pos, playerPosition) < 5.0f) continue;
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
        var mark = hp * 100 / islandCount;
#if !UNITY_EDITOR && UNITY_WEBGL
        SetFinalScore(mark);
#else
        Debug.Log($"Il tuo punteggio è {mark}");
#endif
    }
    
}