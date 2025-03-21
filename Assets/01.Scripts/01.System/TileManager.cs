using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using BarParu;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public Tilemap tilemap;
    public TileBase pathTile; // '��' Ÿ��

    private bool[,] tileUsed; // Ÿ�� ��� ���θ� üũ�ϴ� �迭
    private TowerObject[,] towerObjects;
    private int mapMin = -6; // Ÿ�ϸ� �ּ� ��ǥ
    private int mapMax = 5; // Ÿ�ϸ� �ִ� ��ǥ

    public TowerObject towerPrefab; // Ÿ�� ������ (�ӽ�)

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Ÿ�� ��� ���� �迭 �ʱ�ȭ (ũ��: 12x12)
        tileUsed = new bool[mapMax - mapMin + 1, mapMax - mapMin + 1];
        towerObjects = new TowerObject[mapMax - mapMin + 1, mapMax - mapMin + 1];   
        InitializeTileUsage();
    }

    // Ÿ�� ��� ���� �迭 �ʱ�ȭ �޼���
    private void InitializeTileUsage()
    {
        for (int x = mapMin; x <= mapMax; x++)
        {
            for (int y = mapMin; y <= mapMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePos);
                if (tile == pathTile)
                {
                    tileUsed[x - mapMin, y - mapMin] = true; // '��' Ÿ���� ��� ������ ǥ��
                }
                else
                {
                    tileUsed[x - mapMin, y - mapMin] = false; // '�ٴ�' Ÿ���� ������ �������� ǥ��
                }
            }
        }
    }

    // Ÿ���� 0,0�� �߽����� ����� ��ġ�� ��ġ�ϴ� �޼���
    public void PlaceTower()
    {
        List<Vector3Int> availablePositions = new List<Vector3Int>();
        for (int x = mapMin; x <= mapMax; x++)
        {
            for (int y = mapMin; y <= mapMax; y++)
            {
                if (!tileUsed[x - mapMin, y - mapMin])
                {
                    availablePositions.Add(new Vector3Int(x, y, y));
                }
            }
        }

        // 0,0�� �߽����� ����� ��ġ���� ����
        availablePositions.Sort((a, b) => Vector3Int.Distance(a, Vector3Int.zero).CompareTo(Vector3Int.Distance(b, Vector3Int.zero)));

        if (availablePositions.Count > 0)
        {
            Vector3Int closestPosition = availablePositions[0];
            Vector3 worldPosition = tilemap.GetCellCenterWorld(closestPosition);
            TowerObject tower = Instantiate(DataManager.instance.ReturnTowerObject(1), worldPosition, Quaternion.identity);
            tower.SetLayerPos();

            tileUsed[closestPosition.x - mapMin, closestPosition.y - mapMin] = true; // ��� ������ ǥ��
            towerObjects[closestPosition.x - mapMin, closestPosition.y - mapMin] = tower; // �ش� ��ġ�� Ÿ���� �߰�
        }
    }

    // Ÿ���� �̵��� �� ȣ��Ǵ� �޼���
    public void MoveTower(TowerObject tower, Vector3Int newTilePos, Vector3 originalPos)
    {
        Vector3Int currentTilePos = tilemap.WorldToCell(originalPos);
        //Vector3Int currentTilePos = tilemap.WorldToCell(tower.transform.position);
        if (!tileUsed[newTilePos.x - mapMin, newTilePos.y - mapMin])
        {
            tileUsed[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = false; // ���� ��ġ�� ������ �������� ǥ��
            towerObjects[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = null; // �ش� ��ġ�� Ÿ���� �ʱ�ȭ
            Palog.Log("�ʱ�ȭ �Ϸ� : " + tileUsed[currentTilePos.x - mapMin, currentTilePos.y - mapMin] + " / " + towerObjects[currentTilePos.x - mapMin, currentTilePos.y - mapMin]);
            Palog.Log(string.Format("�ʱ�ȭ�� ��ǥ x:{0}, y:{1}", currentTilePos.x - mapMin, currentTilePos.y - mapMin));
            tower.transform.position = tilemap.GetCellCenterWorld(newTilePos);
            tower.SetLayerPos();
            tileUsed[newTilePos.x - mapMin, newTilePos.y - mapMin] = true; // ���ο� ��ġ�� ��� ������ ǥ��
            towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin] = tower; // �ش� ��ġ�� Ÿ���� �߰�
            tower.battleState = true;   // �������·� ��Ȱ��ȭ
        }
        else if (towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin] != null)
        {
            if (string.Equals(towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin].status.key, tower.status.key) && tower != towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin])
            {
                Palog.Log("�ռ��� �����մϴ�.");
                int upgradeLevel = tower.status.level;

                if (DataManager.instance.ReturnNextTowerTier(upgradeLevel))
                {
                    TowerObject newTower = Instantiate(DataManager.instance.ReturnTowerObject(upgradeLevel + 1), tilemap.GetCellCenterWorld(newTilePos), Quaternion.identity);

                    if (newTower != null)
                    {
                        Palog.Log("���ο� Ÿ�� Ȯ��. ��ġ�մϴ�.");
                        //Destroy(towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin]);
                        //Destroy(tower);
                        towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin].gameObject.SetActive(false);
                        tower.gameObject.SetActive(false);

                        tileUsed[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = false; // ���� ��ġ�� ������ �������� ǥ��
                        towerObjects[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = null; // �ش� ��ġ�� Ÿ���� �ʱ�ȭ
                        Palog.Log("�ʱ�ȭ �Ϸ�");
                        tileUsed[newTilePos.x - mapMin, newTilePos.y - mapMin] = true; // ���ο� ��ġ�� ��� ������ ǥ��
                        towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin] = newTower; // �ش� ��ġ�� Ÿ���� �߰�
                        newTower.gameObject.SetActive(true);
                        newTower.SetLayerPos();
                    }
                }
                else
                {
                    Palog.Log("�� �̻� ���׷��̵� �� �� �����ϴ�.");
                    GameManager.instance.tileSelector.ReturnToOriginalPosition();
                }
            }
            else
            {
                Palog.Log("�ش� ��ġ�� �̹� Ÿ���� �ֽ��ϴ�.");
                GameManager.instance.tileSelector.ReturnToOriginalPosition();
            }
        }
        else
        {
            Palog.Log("�ش� ��ġ�� �̹� Ÿ���� �ֽ��ϴ�.");
            GameManager.instance.tileSelector.ReturnToOriginalPosition();
        }
    }

    // Ÿ���� ���� ���θ� Ȯ�����ִ� �޼���
    public bool GetTowerState(Vector3Int tilePos)
    {
        Palog.Log(string.Format("���õ� ��ǥ x:{0}, y:{1}", tilePos.x - mapMin, tilePos.y - mapMin));
        if (tileUsed[tilePos.x - mapMin, tilePos.y - mapMin] == true && towerObjects[tilePos.x - mapMin, tilePos.y - mapMin] != null)
        {
            Palog.Log("Ÿ��������Ȳ : True");
            return true;
        }
        else
        {
            Palog.Log("Ÿ��������Ȳ : False");
            return false;
        }
    }

    // Ÿ�� ������Ʈ�� ����
    public TowerObject ReturnTowerObejct(Vector3Int tilePos)
    {
        if (towerObjects[tilePos.x - mapMin, tilePos.y - mapMin])
        {
            return towerObjects[tilePos.x - mapMin, tilePos.y - mapMin];
        }
        else return null;
    }
}
