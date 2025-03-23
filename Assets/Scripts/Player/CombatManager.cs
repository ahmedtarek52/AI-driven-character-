using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public IdleCombat idleCombat;
    public CharacterSkills characterSkills;
    public static CombatManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void ProcessCommand(string command)
    {
        if (command.ToLower() == "enemy1")
        {
            idleCombat.SetTarget(GameObject.Find("Enemy1").GetComponent<Enemy>());
        }
        else if (command.ToLower() == "enemy2")
        {
            idleCombat.SetTarget(GameObject.Find("Enemy2").GetComponent<Enemy>());
        }
        else if (command.ToLower() == "fireball")
        {
            characterSkills.UseSkill("fireball");
        }
        else if (command.ToLower() == "shield")
        {
            characterSkills.UseSkill("shield");
        }
        else
        {
            Debug.Log("Unknown command: " + command);
        }
    }


}
