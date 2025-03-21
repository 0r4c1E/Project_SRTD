using BarParu;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap�� �����Ϳ��� �Ҵ�
    public Grid grid; // Grid�� �����Ϳ��� �Ҵ�

    private bool onState = false;
    private TowerObject towerObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ����
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);

            // Ÿ���� ���� ���� Ȯ��
            if (tilemap.HasTile(tilePos))
            {
                onState = true;
                // Ÿ���� ��ǥ���� ����Ͽ� ���ϴ� �۾� ����
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

            // Ÿ���� ���� ���� Ȯ��
            if (tilemap.HasTile(tilePos) && towerObject != null)
            {
                // Ÿ���� ��ǥ���� ����Ͽ� ���ϴ� �۾� ����
                TileManager.instance.MoveTower(towerObject, tilePos);
            }

            onState = false;
            towerObject = null;
        }

    }
}
