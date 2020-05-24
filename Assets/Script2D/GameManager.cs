using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Unit selectedUnit; /*variable pour  y stocker quel personnage est sélectionné*/
    public static GameManager instance = null; /*reference au singleton*/
    public int playerTurn = 1; /*Tour*/

    /*Pour changer l'indicateur de tour*/
    [SerializeField] private Image indicator;
    [SerializeField] private Sprite playerIndicator;
    [SerializeField] private Sprite enemyIndicator;

    /*gestion de l'or*/
    public int gold = 100;
    public Text goldText;
    public Unit purchasedItem;

    public List<EnemyAI> listOfEnemy = new List<EnemyAI>();
    public List<Unit> listOfPlayer = new List<Unit>();

    public void UpdateGoldText()
    {
        goldText.text = gold.ToString();
    }

    public void GetGoldIncome(int playerTurn) /*attribue de l'or au joueur a chaque tour*/
    {
        foreach(camp camp in FindObjectsOfType<camp>())
        {
            if(playerTurn == 1)
            {
                gold += camp.goldPerTurn;
            }
        }
        UpdateGoldText();
    }


    void Awake()
    {   /*singleton.*/
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        GetGoldIncome(1);
    }

    public void ResetTiles() /*Annule le highlighting*/
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.Reset();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) /*penser à trouver une alternative au hard coding*/
        {
            EndTurn();
        }


        EnemyPlace();
    }

    void EndTurn() /*Change de tour*/
    {
        if (playerTurn == 1)
        {
            playerTurn = 2;
            indicator.sprite = playerIndicator; /*change l'indic*/
        }
        else if (playerTurn == 2)
        {
            playerTurn = 1;
            indicator.sprite = enemyIndicator; /*change l'indic*/
        }

        GetGoldIncome(playerTurn);

        if (selectedUnit != null)/*Désélectionne une unité après un tour fini*/
        {
            selectedUnit.selected = false;
            selectedUnit = null;
        }

        ResetTiles();

        foreach (Unit unit in FindObjectsOfType<Unit>())/*permet de se déplacer au tour suivant*/
        {
            unit.hasMoved = false;
            unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
        }

        GetComponent<Shop>().CloseMenu(); /*ferme le shop*/
    }

    public void EnemyPlace()
    {
        int j = 0;
        int k = 0;
        for (int i = 0; i <= listOfEnemy.Count; i++)
        {
            listOfEnemy[i].Target = listOfPlayer[j];
            Debug.Log("Target chosen + " + j);
            listOfEnemy[i].SpaceToGo = listOfPlayer[j].pos[k];
            Debug.Log("Square chosen + " + k);
            k++;
            if (k > listOfPlayer[j].pos.Length)
            {
                k = 0;
                j++;
            }
        }
    }

    public void Position(Unit enemy)
    {
        int k = 0;
        for (int i = 0; i <= listOfEnemy.Count; i++)
        {
            listOfEnemy[i].SpaceToGo = enemy.pos[k];
            Debug.Log("Square chosen + " + k);
            k++;
            if (k > enemy.pos.Length)
            {
                k = 0;
            }
        }
    }
}
