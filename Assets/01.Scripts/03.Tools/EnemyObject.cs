using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyObject : MonoBehaviour
{
    public DataBox dataBox;

    public Status status; // 적 오브젝트의 스탯

    public float moveSpeed = 2f; // 오브젝트의 이동 속도

    private List<Vector3Int> path; // 경로 타일 좌표 리스트
    private Tilemap tilemap;

    // 경로와 타일맵 설정 메서드
    public void SetPath(List<Vector3Int> path)
    {
        this.path = path;
        tilemap = FindObjectOfType<Tilemap>();
    }

    // 이동 시작 메서드
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

            // 오브젝트가 타겟 위치에 도달할 때까지 이동
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                yield return null;
            }

            // 정확한 위치로 맞추기
            transform.position = targetPosition;
        }

        // 경로의 마지막 위치에 도달했을 때
        OnReachDestination();
    }

    // 목적지에 도달했을 때 호출되는 메서드
    private void OnReachDestination()
    {
        GameManager.instance.stageManager.GetDamage(status.attack);
        // 추가 작업을 위한 메서드
        gameObject.SetActive(false);
    }

    // 체력을 감소시키는 메서드
    public void TakeDamage(double damage)
    {
        status.hp -= damage;
        dataBox.SetHP((float)(status.hp / status.maxHealth));
        if (status.hp <= 0)
        {
            Die();
        }
    }

    // 적이 죽었을 때 호출되는 메서드
    private void Die()
    {
        // 적 비활성화 혹은 파괴
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
