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
        if (command.ToLower() == "target enemy 1")
        {
            idleCombat.SetTarget(GameObject.Find("Enemy1").GetComponent<Enemy>());
        }
        else if (command.ToLower() == "target enemy 2")
        {
            idleCombat.SetTarget(GameObject.Find("Enemy2").GetComponent<Enemy>());
        }
        else if (command.ToLower() == "use fireball")
        {
            characterSkills.UseSkill("fireball");
        }
        else if (command.ToLower() == "use shield")
        {
            characterSkills.UseSkill("shield");
        }
        else
        {
            Debug.Log("Unknown command: " + command);
        }
    }


}
