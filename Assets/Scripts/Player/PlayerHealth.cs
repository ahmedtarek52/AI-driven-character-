using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    public int health = 100;
    public Slider healthBar;
    Actions action;
    
    // Movement variables
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    private Rigidbody rb;
    private bool isMoving = false;

    // Camera variables
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    private bool isFirstPerson = true;
    
    private void Awake() 
    { 
        instance = this;
        rb = GetComponent<Rigidbody>();
        action = GetComponent<Actions>();
        
        // Ensure only one camera is active at start
        if (firstPersonCamera != null && thirdPersonCamera != null)
        {
            firstPersonCamera.enabled = true;
            thirdPersonCamera.enabled = false;
        }
    }

    private void Update()
    {
        // Handle movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = transform.forward * verticalInput * moveSpeed;
        transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
        
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        // Handle walk animation
        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(horizontalInput) > 0.1f;
        
        if (isMoving && !wasMoving)
        {
            action.Walk();
        }
        else if (!isMoving && wasMoving)
        {
            action.Stay();
        }

        // Handle camera switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToFirstPerson();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToThirdPerson();
        }
    }

    private void SwitchToFirstPerson()
    {
        if (firstPersonCamera != null && thirdPersonCamera != null)
        {
            firstPersonCamera.enabled = true;
            thirdPersonCamera.enabled = false;
            isFirstPerson = true;
        }
    }

    private void SwitchToThirdPerson()
    {
        if (firstPersonCamera != null && thirdPersonCamera != null)
        {
            firstPersonCamera.enabled = false;
            thirdPersonCamera.enabled = true;
            isFirstPerson = false;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = (float)health / 100f;
        Debug.Log("Player took " + damage + " damage! HP: " + health);
    }
}
