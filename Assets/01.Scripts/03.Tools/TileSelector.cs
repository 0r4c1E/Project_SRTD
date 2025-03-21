using BarParu;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap을 에디터에서 할당
    public Grid grid; // Grid를 에디터에서 할당

    private bool onState = false;
    private TowerObject towerObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);

            // 타일의 존재 여부 확인
            if (tilemap.HasTile(tilePos))
            {
                onState = true;
                // 타일의 좌표값을 사용하여 원하는 작업 수행
                if (TileManager.instance.GetTowerState(tilePos))
                {
                    towerObject = TileManager.instance.ReturnTowerObejct(tilePos);
                }
            }
        }
        if(Input.GetMouseButtonUp(0) && onState)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);

            // 타일의 존재 여부 확인
            if (tilemap.HasTile(tilePos) && towerObject != null)
            {
                // 타일의 좌표값을 사용하여 원하는 작업 수행
                TileManager.instance.MoveTower(towerObject, tilePos);
            }

            onState = false;
            towerObject = null;
        }

    }
}
