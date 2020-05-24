using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private GameManager manager;
    private Unit info;
    [SerializeField] private Unit target;
    private bool canHealOnlyOnce;
    private bool action = false;
    [SerializeField] private Transform spaceToGo;
    private Transform temp;
    private Tile tileInfo;

    private bool pop = false;

    public Unit Target { get => target; set => target = value; }
    public Transform SpaceToGo { get => spaceToGo; set => spaceToGo = value; }

    // Start is called before the first frame update
    void Start()
    {
        manager = GameManager.instance;
        info = GetComponent<Unit>();
        canHealOnlyOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            if (SpaceToGo != null)
            {
                if (manager.playerTurn == 1)
                {
                    if (canHealOnlyOnce && info.health < 5 && !action)
                    {
                        info.health += 2;
                        canHealOnlyOnce = false;
                        action = true;
                        Debug.Log("Has heal");
                    }
                    else if (!action)
                    {
                        if (Mathf.Abs(transform.position.x - Target.transform.position.x) + Mathf.Abs(transform.position.y - Target.transform.position.y) > info.AttackRange)
                        {
                            ChooseWhereToGo();
                            info.Move(temp);
                        }

                        else if ((Mathf.Abs(transform.position.x - Target.transform.position.x) + Mathf.Abs(transform.position.y - Target.transform.position.y) <= info.AttackRange))
                        {
                            if (!info.hasAttacked)
                            {
                                info.Attack(Target);
                                action = true;
                                Debug.Log("Peu atk");
                            }
                        }
                    }
                }
                else if (manager.playerTurn == 2)
                {
                    action = false;
                    pop = false;
                }
            }
            else if (SpaceToGo == null)
            {
                manager.Position(Target);
            }
        }
        else if (Target == null)
        {
            RNGTraget();
        }
    }

    private void RNGTraget()
    {
        float RNG = Random.Range(0, 3);
        switch (RNG)
        {
            case 0:
                Target = manager.listOfPlayer[0];
                break;
            case 1:
                Target = manager.listOfPlayer[1];
                break;
            case 2:
                Target = manager.listOfPlayer[2];
                break;
            case 3:
                Target = manager.listOfPlayer[3];
                break;
            default:
                Debug.Log("Il n'y a pas de target...");
                break;
        }
    }

    private void ChooseWhereToGo()
    {
        if (!pop)
        {
            temp = SpaceToGo;
            pop = true;
        }
    }
}