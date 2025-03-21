using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameOver = false;
    public bool isGameClear = false;

    public StageManager stageManager;
    public TileSelector tileSelector;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Application.runInBackground = true;
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void GameClear()
    {
        isGameClear = true;
    }

    public Tilemap tilemap; // Tilemap을 에디터에서 할당
    public GameObject objectPrefab; // 생성할 오브젝트의 프리팹을 에디터에서 할당
    public List<Vector3Int> path; // 경로 타일 좌표 리스트를 에디터에서 설정
    public int numberOfObjects = 20; // 생성할 오브젝트의 수
    public float spawnDelay = 0.5f; // 오브젝트 생성 간격

    void Start()
    {
        stageManager = GetComponent<StageManager>();
        tileSelector = GetComponent<TileSelector>();

        stageManager.InitializeLife();

        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // 첫 번째 타일의 중앙에 오브젝트 생성
            Vector3 startPosition = tilemap.GetCellCenterWorld(path[0]);
            startPosition.z = startPosition.y;
            EnemyObject movingObject = Instantiate(objectPrefab, startPosition, Quaternion.identity).GetComponent<EnemyObject>();
            movingObject.SetPath(path);

            // 오브젝트를 경로를 따라 움직이기 시작
            movingObject.StartMoving();

            // 지정된 딜레이 후 다음 오브젝트 생성
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
