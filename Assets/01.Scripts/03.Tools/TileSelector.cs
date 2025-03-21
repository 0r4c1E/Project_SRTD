using BarParu;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap을 에디터에서 할당
    public Grid grid; // Grid를 에디터에서 할당

    private bool onState = false;
    private TowerObject towerObject;
    private Vector3 originalPosition; // 원래 위치 저장

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
                    towerObject.battleState = false;
                    originalPosition = towerObject.transform.position; // 원래 위치 저장
                }
            }
        }

        if (Input.GetMouseButton(0) && onState && towerObject != null) // 마우스를 클릭한 상태에서 이동
        {
            Vector3 nowMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = new Vector3(nowMouseWorldPos.x, nowMouseWorldPos.y, towerObject.transform.position.z);
            towerObject.transform.position = newPosition; // 오브젝트가 마우스를 따라다니게 함
        }

        if (Input.GetMouseButtonUp(0) && onState)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);

            // 타일의 존재 여부 확인
            if (tilemap.HasTile(tilePos) && towerObject != null)
            {
                // 타일의 좌표값을 사용하여 원하는 작업 수행
                TileManager.instance.MoveTower(towerObject, tilePos, originalPosition);
            }
            else
            {
                ReturnToOriginalPosition();
            }

            onState = false;
            towerObject = null;
        }

    }

    // 잘못된 위치에 놓았을 때 원래 위치로 돌려놓는 메서드
    public void ReturnToOriginalPosition()
    {
        if (towerObject != null)
        {
            towerObject.battleState = true;
            towerObject.transform.position = originalPosition;
        }
    }
}
