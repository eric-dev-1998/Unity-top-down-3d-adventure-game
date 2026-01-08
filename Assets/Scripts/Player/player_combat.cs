using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_combat : MonoBehaviour
{
    // GAME OBJECT TO STORE THE ATTACKS AND COLLIDERS
    public GameObject attackContainer;
    public GameObject[] attackObjects;

    bool canAttack = true;
    int currentDirection = 0;

    // UPDATE METHOD
    void Update()
    {
        // PRESSED THE ATTACK KEY
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(canAttack)
            {
                // SET ATTACK POSITION BASED ON THE PLAYER FACING DIRECTION
                switch (currentDirection)
                {
                    case 0:
                        attackContainer.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
                        Attack(0);
                        break;
                    case 1:
                        attackContainer.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                        Attack(0);
                        break;
                    case 2:
                        attackContainer.transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                        Attack(0);
                        break;
                    case 3:
                        attackContainer.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                        Attack(0);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // SET UP ATTACK PARAMETERS BEFOR SHOWING THE EFFECTS AND TRIGGERS
    void Attack(int type)
    {
        switch (type)
        {
            // NEUTRAL ATTACK
            case 0:
                StartCoroutine(doAttack());
                break;
            default:
                return;
        }
    }

    // SET CURRENT DIRECTION, CURRENTLY FROM THE PLAYER ANIMATION'S SCRIPT
    public void setCurrentDirection(int dir)
    {
        currentDirection = dir;
    }

    // COROUTINE TO PLAY THE ATTACK ANIMATIONS
    public IEnumerator doAttack()
    {
        attackObjects[0].SetActive(true);
        yield return new WaitForSeconds(1);
    }
}
