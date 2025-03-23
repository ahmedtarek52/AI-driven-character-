using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    [SerializeField] Slider healthBar;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    public Transform[] patrolPoints; 
    public float patrolSpeed = 2f;  
    private int currentPointIndex = 0;  
    bool isPatoll = true;
    [SerializeField] Actions action;

    private void Awake()
    {

    }

    private void Start()
    {
        if (patrolPoints.Length > 0)
        {
            MoveToNextPoint();
        }
    }

    private void Update()
    {
        // Check if the patrolPoints array has been set
        if (patrolPoints.Length > 0)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length > 0 && isPatoll)
        {
            action.Walk();
            Vector3 direction = patrolPoints[currentPointIndex].position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, patrolSpeed * Time.deltaTime);
            if (transform.position == patrolPoints[currentPointIndex].position)
            {
                MoveToNextPoint();
            }
        }
    }

    void MoveToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    public void TakeDamage(int damage)
    {
        isPatoll = false;
        AttackPlayer();
        health -= damage;
        Debug.Log(name + " took " + damage + " damage!");
        healthBar.value = (float)health / 50f; 
        if (health <= 0)
        {
            Die();
        }      
        
    }

    void AttackPlayer()
    {
        action.Attack();

        transform.LookAt(new Vector3(PlayerHealth.instance.transform.position.x, transform.position.y, PlayerHealth.instance.transform.position.z));
        firePoint.LookAt(PlayerHealth.instance.transform.position);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        AudioManager.instance.PlayHitSound();
        Debug.Log(name + " attacked the player!");
    }


    void Die()
    {
        action.Death();
        AudioManager.instance.PlayDeathSound();
        Debug.Log(name + " has been defeated!");
        Destroy(gameObject ,3);
    }
}
