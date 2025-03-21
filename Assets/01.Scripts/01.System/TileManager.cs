using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using BarParu;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public Tilemap tilemap;
    public TileBase pathTile; // '길' 타일

    private bool[,] tileUsed; // 타일 사용 여부를 체크하는 배열
    private TowerObject[,] towerObjects;
    private int mapMin = -6; // 타일맵 최소 좌표
    private int mapMax = 5; // 타일맵 최대 좌표

    public TowerObject towerPrefab; // 타워 프리팹 (임시)

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 타일 사용 여부 배열 초기화 (크기: 12x12)
        tileUsed = new bool[mapMax - mapMin + 1, mapMax - mapMin + 1];
        towerObjects = new TowerObject[mapMax - mapMin + 1, mapMax - mapMin + 1];   
        InitializeTileUsage();
    }

    // 타일 사용 여부 배열 초기화 메서드
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
                    tileUsed[x - mapMin, y - mapMin] = true; // '길' 타일은 사용 중으로 표시
                }
                else
                {
                    tileUsed[x - mapMin, y - mapMin] = false; // '바닥' 타일은 사용되지 않음으로 표시
                }
            }
        }
    }

    // 타워를 0,0을 중심으로 가까운 위치에 배치하는 메서드
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

        // 0,0을 중심으로 가까운 위치부터 정렬
        availablePositions.Sort((a, b) => Vector3Int.Distance(a, Vector3Int.zero).CompareTo(Vector3Int.Distance(b, Vector3Int.zero)));

        if (availablePositions.Count > 0)
        {
            Vector3Int closestPosition = availablePositions[0];
            Vector3 worldPosition = tilemap.GetCellCenterWorld(closestPosition);
            TowerObject tower = Instantiate(DataManager.instance.ReturnTowerObject(1), worldPosition, Quaternion.identity);
            tower.SetLayerPos();

            tileUsed[closestPosition.x - mapMin, closestPosition.y - mapMin] = true; // 사용 중으로 표시
            towerObjects[closestPosition.x - mapMin, closestPosition.y - mapMin] = tower; // 해당 위치로 타워를 추가
        }
    }

    // 타워가 이동할 때 호출되는 메서드
    public void MoveTower(TowerObject tower, Vector3Int newTilePos, Vector3 originalPos)
    {
        Vector3Int currentTilePos = tilemap.WorldToCell(originalPos);
        //Vector3Int currentTilePos = tilemap.WorldToCell(tower.transform.position);
        if (!tileUsed[newTilePos.x - mapMin, newTilePos.y - mapMin])
        {
            tileUsed[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = false; // 이전 위치를 사용되지 않음으로 표시
            towerObjects[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = null; // 해당 위치의 타워를 초기화
            Palog.Log("초기화 완료 : " + tileUsed[currentTilePos.x - mapMin, currentTilePos.y - mapMin] + " / " + towerObjects[currentTilePos.x - mapMin, currentTilePos.y - mapMin]);
            Palog.Log(string.Format("초기화된 좌표 x:{0}, y:{1}", currentTilePos.x - mapMin, currentTilePos.y - mapMin));
            tower.transform.position = tilemap.GetCellCenterWorld(newTilePos);
            tower.SetLayerPos();
            tileUsed[newTilePos.x - mapMin, newTilePos.y - mapMin] = true; // 새로운 위치를 사용 중으로 표시
            towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin] = tower; // 해당 위치로 타워를 추가
            tower.battleState = true;   // 전투상태로 재활성화
        }
        else if (towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin] != null)
        {
            if (string.Equals(towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin].status.key, tower.status.key) && tower != towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin])
            {
                Palog.Log("합성을 시작합니다.");
                int upgradeLevel = tower.status.level;

                if (DataManager.instance.ReturnNextTowerTier(upgradeLevel))
                {
                    TowerObject newTower = Instantiate(DataManager.instance.ReturnTowerObject(upgradeLevel + 1), tilemap.GetCellCenterWorld(newTilePos), Quaternion.identity);

                    if (newTower != null)
                    {
                        Palog.Log("새로운 타워 확인. 설치합니다.");
                        //Destroy(towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin]);
                        //Destroy(tower);
                        towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin].gameObject.SetActive(false);
                        tower.gameObject.SetActive(false);

                        tileUsed[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = false; // 이전 위치를 사용되지 않음으로 표시
                        towerObjects[currentTilePos.x - mapMin, currentTilePos.y - mapMin] = null; // 해당 위치의 타워를 초기화
                        Palog.Log("초기화 완료");
                        tileUsed[newTilePos.x - mapMin, newTilePos.y - mapMin] = true; // 새로운 위치를 사용 중으로 표시
                        towerObjects[newTilePos.x - mapMin, newTilePos.y - mapMin] = newTower; // 해당 위치로 타워를 추가
                        newTower.gameObject.SetActive(true);
                        newTower.SetLayerPos();
                    }
                }
                else
                {
                    Palog.Log("이 이상 업그레이드 할 수 없습니다.");
                    GameManager.instance.tileSelector.ReturnToOriginalPosition();
                }
            }
            else
            {
                Palog.Log("해당 위치에 이미 타워가 있습니다.");
                GameManager.instance.tileSelector.ReturnToOriginalPosition();
            }
        }
        else
        {
            Palog.Log("해당 위치에 이미 타워가 있습니다.");
            GameManager.instance.tileSelector.ReturnToOriginalPosition();
        }
    }

    // 타워의 보유 여부를 확인해주는 메서드
    public bool GetTowerState(Vector3Int tilePos)
    {
        Palog.Log(string.Format("선택된 좌표 x:{0}, y:{1}", tilePos.x - mapMin, tilePos.y - mapMin));
        if (tileUsed[tilePos.x - mapMin, tilePos.y - mapMin] == true && towerObjects[tilePos.x - mapMin, tilePos.y - mapMin] != null)
        {
            Palog.Log("타워보유상황 : True");
            return true;
        }
        else
        {
            Palog.Log("타워보유상황 : False");
            return false;
        }
    }

    // 타워 오브젝트를 리턴
    public TowerObject ReturnTowerObejct(Vector3Int tilePos)
    {
        if (towerObjects[tilePos.x - mapMin, tilePos.y - mapMin])
        {
            return towerObjects[tilePos.x - mapMin, tilePos.y - mapMin];
        }
        else return null;
    }
}
