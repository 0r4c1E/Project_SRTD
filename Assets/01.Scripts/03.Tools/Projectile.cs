using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private EnemyObject target;
    private double damage;

    // ����ü�� ��ǥ�� ���ݷ��� �����ϴ� �޼���
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

    // ��ǥ�� �������� �� ȣ��Ǵ� �޼���
    private void HitTarget()
    {
        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
