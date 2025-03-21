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

    public Status status; // Ÿ�� ������Ʈ�� ����

    public EnemyObject[] nowTarget; // ���� Ÿ������ ������ ��

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
    //    if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ����
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

    // 2D �ݶ��̴� ���� ���� ���� ����Ʈ�� �߰�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //BarParu.Palog.Log(collision.name);
        if (collision.CompareTag("Enemy"))
        {
            enemies.Add(collision.GetComponent<EnemyObject>());
        }
    }
    // 2D �ݶ��̴� ������ ���� ���� ����Ʈ���� ����
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyObject enemy = collision.GetComponent<EnemyObject>();
            enemies.Remove(enemy);
            // ���� Ÿ���� ��� ���� ���̶�� Ÿ���� null�� �ʱ�ȭ
            if (nowTarget.Contains(enemy))
            {
                nowTarget = Array.FindAll(nowTarget, i => i != enemy).ToArray();
            }
        }
    }

    public GameObject projectilePrefab; // ����ü ������ (�ӽ�)
    public Transform firePoint; // ����ü�� �߻�Ǵ� ��ġ
    public float fireRate = 1f; // �߻� ����
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
                    // Ÿ���� attackCount��ŭ �������� ���� ���
                    if (nowTarget.Length < status.attackCount)
                    {
                        // Ÿ�� ���� ���� ������ �ִ� �� �� �� ���� ���� ����
                        int targetCount = Mathf.Min(status.attackCount, enemies.Count);

                        nowTarget = new EnemyObject[targetCount];

                        // Ÿ�� ����
                        for (int i = 0; i < targetCount; i++)
                        {
                            nowTarget[i] = enemies[i]; // enemies ����Ʈ���� ���������� ����
                        }
                    }

                    // Ÿ���� �����Ǹ� ���� ����
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
