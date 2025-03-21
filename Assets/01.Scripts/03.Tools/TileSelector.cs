using BarParu;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap�� �����Ϳ��� �Ҵ�
    public Grid grid; // Grid�� �����Ϳ��� �Ҵ�

    private bool onState = false;
    private TowerObject towerObject;
    private Vector3 originalPosition; // ���� ��ġ ����

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
                    towerObject.battleState = false;
                    originalPosition = towerObject.transform.position; // ���� ��ġ ����
                }
            }
        }

        if (Input.GetMouseButton(0) && onState && towerObject != null) // ���콺�� Ŭ���� ���¿��� �̵�
        {
            Vector3 nowMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = new Vector3(nowMouseWorldPos.x, nowMouseWorldPos.y, towerObject.transform.position.z);
            towerObject.transform.position = newPosition; // ������Ʈ�� ���콺�� ����ٴϰ� ��
        }

        if (Input.GetMouseButtonUp(0) && onState)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);

            // Ÿ���� ���� ���� Ȯ��
            if (tilemap.HasTile(tilePos) && towerObject != null)
            {
                // Ÿ���� ��ǥ���� ����Ͽ� ���ϴ� �۾� ����
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

    // �߸��� ��ġ�� ������ �� ���� ��ġ�� �������� �޼���
    public void ReturnToOriginalPosition()
    {
        if (towerObject != null)
        {
            towerObject.battleState = true;
            towerObject.transform.position = originalPosition;
        }
    }
}
