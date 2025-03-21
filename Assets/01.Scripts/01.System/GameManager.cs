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

    public Tilemap tilemap; // Tilemap�� �����Ϳ��� �Ҵ�
    public GameObject objectPrefab; // ������ ������Ʈ�� �������� �����Ϳ��� �Ҵ�
    public List<Vector3Int> path; // ��� Ÿ�� ��ǥ ����Ʈ�� �����Ϳ��� ����
    public int numberOfObjects = 20; // ������ ������Ʈ�� ��
    public float spawnDelay = 0.5f; // ������Ʈ ���� ����

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
            // ù ��° Ÿ���� �߾ӿ� ������Ʈ ����
            Vector3 startPosition = tilemap.GetCellCenterWorld(path[0]);
            startPosition.z = startPosition.y;
            EnemyObject movingObject = Instantiate(objectPrefab, startPosition, Quaternion.identity).GetComponent<EnemyObject>();
            movingObject.SetPath(path);

            // ������Ʈ�� ��θ� ���� �����̱� ����
            movingObject.StartMoving();

            // ������ ������ �� ���� ������Ʈ ����
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
