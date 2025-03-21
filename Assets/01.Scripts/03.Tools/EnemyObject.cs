using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyObject : MonoBehaviour
{
    public DataBox dataBox;

    public Status status; // �� ������Ʈ�� ����

    public float moveSpeed = 2f; // ������Ʈ�� �̵� �ӵ�

    private List<Vector3Int> path; // ��� Ÿ�� ��ǥ ����Ʈ
    private Tilemap tilemap;

    // ��ο� Ÿ�ϸ� ���� �޼���
    public void SetPath(List<Vector3Int> path)
    {
        this.path = path;
        tilemap = FindObjectOfType<Tilemap>();
    }

    // �̵� ���� �޼���
    public void StartMoving()
    {
        dataBox.nameBox.gameObject.SetActive(false);
        status.hp = status.maxHealth;

        StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        foreach (Vector3Int tilePos in path)
        {
            Vector3 targetPosition = tilemap.GetCellCenterWorld(tilePos);

            // ������Ʈ�� Ÿ�� ��ġ�� ������ ������ �̵�
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                yield return null;
            }

            // ��Ȯ�� ��ġ�� ���߱�
            transform.position = targetPosition;
        }

        // ����� ������ ��ġ�� �������� ��
        OnReachDestination();
    }

    // �������� �������� �� ȣ��Ǵ� �޼���
    private void OnReachDestination()
    {
        GameManager.instance.stageManager.GetDamage(status.attack);
        // �߰� �۾��� ���� �޼���
        gameObject.SetActive(false);
    }

    // ü���� ���ҽ�Ű�� �޼���
    public void TakeDamage(double damage)
    {
        status.hp -= damage;
        dataBox.SetHP((float)(status.hp / status.maxHealth));
        if (status.hp <= 0)
        {
            Die();
        }
    }

    // ���� �׾��� �� ȣ��Ǵ� �޼���
    private void Die()
    {
        // �� ��Ȱ��ȭ Ȥ�� �ı�
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
