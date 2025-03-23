using System.Collections;
using UnityEngine;

public class CharacterSkills : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject shieldPrefab;
    public Transform fireballSpawnPoint;
    public Transform playerTransform;

    [SerializeField] Actions actions;
    public void UseSkill(string skillName)
    {
        if (skillName.ToLower() == "fireball" )
        {

            StartCoroutine(CastFireball());
        }
        else if (skillName.ToLower() == "shield")
        {
            GameObject shield = Instantiate(shieldPrefab, playerTransform.position, Quaternion.identity);
            shield.transform.SetParent(playerTransform);
            Destroy(shield, 5f);
            Debug.Log("Using Shield!");
        }
    }

    private IEnumerator CastFireball()
    {
        actions.Attack();

        AudioManager.instance.PlayShootSound();
        Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        Debug.Log("Casting Fireball!");

        
        yield return new WaitForSeconds(1f);

      
        actions.Stay(); 
       
    }

}
