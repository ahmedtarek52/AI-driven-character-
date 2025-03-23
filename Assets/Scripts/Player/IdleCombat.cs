using UnityEngine;

public class IdleCombat : MonoBehaviour
{
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    private Enemy currentEnemy;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Actions action;
    void Update()
    {
        if (Time.time >= nextAttackTime && currentEnemy != null)
        {
            AttackEnemy();
            nextAttackTime = Time.time + attackRate;
        }
    }

    public void SetTarget(Enemy enemy)
    {
        currentEnemy = enemy;
    }

    void AttackEnemy()
    {
        if (currentEnemy != null && Enemy.instance.health > 0)
        {
            action.Attack();
            Transform EnemyTransform = Enemy.instance.transform;
            transform.LookAt(EnemyTransform.position );
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            AudioManager.instance.PlayShootSound();
            Debug.Log("Attacked " + currentEnemy.name);
        }
        else{
            action.Stay();
        }
    }
}
