using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private EnemyObject target;
    private double damage;

    // 투사체의 목표와 공격력을 설정하는 메서드
    public void SetTarget(EnemyObject target, double damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    // 목표에 도달했을 때 호출되는 메서드
    private void HitTarget()
    {
        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
