using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    public int health = 100;
    public Slider healthBar;

    private void Awake() { instance = this; }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = (float)health / 100f;
        Debug.Log("Player took " + damage + " damage! HP: " + health);
    }
}
