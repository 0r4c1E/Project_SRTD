using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerObject : MonoBehaviour
{
    public DataBox dataBox;

    public Status status; // 타워 오브젝트의 스탯

    public EnemyObject[] nowTarget; // 현재 타겟으로 지정된 적

    public List<EnemyObject> enemies = new List<EnemyObject>();

    public bool battleState = true;

    private void Start()
    {
        dataBox.SetName(status.name);
        dataBox.hpBar.transform.parent.gameObject.SetActive(false);

        StartCoroutine(TowerCoroutine());
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
    //    {
    //        battleState = false;
    //    }
    //    else if (Input.GetMouseButtonUp(0) && !battleState)
    //    {
    //        battleState = true;
    //    }
    //}

    public void SetLayerPos()
    {
        Vector3 myPos = transform.position;
        myPos.z = myPos.y;
        transform.position = myPos;
    }

    // 2D 콜라이더 내에 들어온 적을 리스트에 추가
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //BarParu.Palog.Log(collision.name);
        if (collision.CompareTag("Enemy"))
        {
            enemies.Add(collision.GetComponent<EnemyObject>());
        }
    }
    // 2D 콜라이더 밖으로 나간 적을 리스트에서 제거
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyObject enemy = collision.GetComponent<EnemyObject>();
            enemies.Remove(enemy);
            // 현재 타겟이 방금 나간 적이라면 타겟을 null로 초기화
            if (nowTarget.Contains(enemy))
            {
                nowTarget = Array.FindAll(nowTarget, i => i != enemy).ToArray();
            }
        }
    }

    public GameObject projectilePrefab; // 투사체 프리팹 (임시)
    public Transform firePoint; // 투사체가 발사되는 위치
    public float fireRate = 1f; // 발사 간격
    private float fireCooldown = 0f;
    private void AttackEnemy()
    {
        EnemyObject[] targets = enemies.ToArray();

        for (int i = 0; i < status.attackCount; i++)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            if (i < targets.Length)
            {
                //if (targets[i] != null)
                //{
                    projectile.SetTarget(targets[i], status.attack);
                //}
            }
        }
    }

    IEnumerator TowerCoroutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(.1f);
        float attackCooldown = 0;
        while (true)
        {
            yield return waitTime;
            if (enemies.Count > 0 && battleState)
            {
                attackCooldown += .1f;
                if (attackCooldown >= status.attackSpeed)
                {
                    // 타겟이 attackCount만큼 설정되지 않은 경우
                    if (nowTarget.Length < status.attackCount)
                    {
                        // 타겟 수와 공격 가능한 최대 수 중 더 작은 값을 설정
                        int targetCount = Mathf.Min(status.attackCount, enemies.Count);

                        nowTarget = new EnemyObject[targetCount];

                        // 타겟 설정
                        for (int i = 0; i < targetCount; i++)
                        {
                            nowTarget[i] = enemies[i]; // enemies 리스트에서 순차적으로 선택
                        }
                    }

                    // 타겟이 설정되면 공격 실행
                    if (nowTarget != null && nowTarget.Length > 0)
                    {
                        AttackEnemy();
                    }

                    attackCooldown = 0;
                }
            }
        }
    }
}
