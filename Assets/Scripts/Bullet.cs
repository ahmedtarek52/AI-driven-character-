using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float bulletSpeed = 40, lifeTime=3;
    public Rigidbody rb;
    [SerializeField] int damage =5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = transform.forward * bulletSpeed;
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy1" || other.gameObject.tag == "Enemy2")
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);

        }
           if (other.gameObject.tag == "Player" )
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);

        }
    
        Destroy(gameObject);
    }


}
