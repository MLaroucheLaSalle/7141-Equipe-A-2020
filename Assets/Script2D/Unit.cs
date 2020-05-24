using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Header("Statistiques des personnages")]
    [SerializeField] private int attackRange;/*nombre de tiles pour attaquer*/
    public int health;
    [SerializeField] private int damage;
    [SerializeField] private int defense;
    [SerializeField] private int armor;
    public int cost;
    [SerializeField] public GameObject weaponIcon; /*pour activer l'icone d'attaque quand on a le curseur survolant une cible qu'on peut attaquer*/

    List<Unit> enemiesInRange = new List<Unit>(); /*va stocker les ennmies attaquables*/
    public bool hasAttacked;/*check si l'unité a attaqué pendant le tour*/

    private GameManager manager; /*reference au game manager*/
    public bool selected; /*Connaitre l'unité sélectionnée*/
    public int tileSpeed; /*Nombre de tiles où l'unité peut se déplacer*/
    public bool hasMoved; /*vérifie si l'unité s'est déplacée ou pas durant le tour courant*/
    public float moveSpeed; /*Vitesse à laquelle l'unité se déplace*/
    public int playerNumber; /*Pour séparer les unités en équipe*/
    public DamageIcon damageIcon; /*pour le visuel des degats*/
    public Transform[] pos;

    public int posInList;

    /*hp du roi*/
    public Text kingHealth;
    public bool isKing;

    public int AttackRange { get => attackRange; set => attackRange = value; }

    void Start()
    {
        manager = GameManager.instance; /*Instantiation du Game Manager*/
        UpdateKingHealth(); 
    }

    public void UpdateKingHealth()
    {
        if (isKing == true)
        {
            kingHealth.text = health.ToString();/*update les hp sur l'ecran*/
        }
    }

    private void Update()
    {
        if (manager.playerTurn == 2)
            posInList = manager.listOfPlayer.FindIndex(IsThere);
        else
            posInList = manager.listOfEnemy.FindIndex(IsAlsoThere);
    }

    private bool IsThere(Unit i)
    {
        return i.name.StartsWith(i.name);
    }

    private bool IsAlsoThere(EnemyAI e)
    {
        return e.name.StartsWith(e.name);
    }

    private void OnMouseDown() /*pour sélectionner et déselectionner une unité*/
    {
        ResetWeaponIcons();

        if (selected == true)
        {   /*Déselectionne une unité si déjà sélectionnée*/
            selected = !selected;
            manager.selectedUnit = null;
            manager.ResetTiles(); /*enlève le highlighting*/
        }
        else
        {
            if (playerNumber == manager.playerTurn) /*Pour que la sélection ne soit possible que si playerTurn et playerNumber concordent*/
            {
                if (manager.selectedUnit != null)
                {
                    manager.selectedUnit.selected = false;
                }
                selected = true;
                manager.selectedUnit = this; /*this = instance courante du script Unit de l'objet player, en gros on sélectionne l'unité sur laquelle on a cliqué*/

                manager.ResetTiles(); /*prévient le déplacement d'une unité*/
                GetEnemies();/*check quels ennemis je peux attaquer*/
                GetWalkableTiles();
            }

        }
        /*Sélectionne une unité à attaquer*/
        Collider2D collider = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit unit = collider.GetComponent<Unit>();
        if (manager.selectedUnit != null)
        {
            if (manager.selectedUnit.enemiesInRange.Contains(unit) && manager.selectedUnit.hasAttacked == false) /*check si l'ennemi sélectionné est "in range" + s'assure que selectedUnit n'a pas attaqué*/
            {
                manager.selectedUnit.Attack(unit);
            }
        }
    }

    public void Attack(Unit enemy)
    {
        hasAttacked = true;
        int enemyDamage = damage - enemy.armor;
        int myDamage = enemy.defense - armor;

        if (enemyDamage >= 1)
        {
            enemy.health -= enemyDamage;
            enemy.UpdateKingHealth();
            DamageIcon d = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamage);
        }
        if (myDamage >= 1)
        {
            health -= myDamage;
            UpdateKingHealth();
            DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
            d.Setup(myDamage);
        }
        if (enemy.health <= 0)
        {
            Destroy(enemy.gameObject);
            GetWalkableTiles();
            if (enemy.playerNumber == 1)
            {
                manager.listOfEnemy.RemoveAt(posInList);
            }
            else if (enemy.playerNumber == 2)
            {
                manager.listOfPlayer.RemoveAt(posInList);
            }
        }
        if (health <= 0)
        {
            manager.ResetTiles();
            Destroy(this.gameObject);
            if (playerNumber == 1)
            {
                manager.listOfEnemy.RemoveAt(posInList);
            }
            else if (playerNumber == 2)
            {
                manager.listOfPlayer.RemoveAt(posInList);
            }
        }
    }

    void GetWalkableTiles() /*Affiche/retourne les tiles vers lesquelles l'unité sélectionnée peut se déplacer*/
    {
        if (hasMoved)
        {
            return; /*skip le reste du code vu que le déplacement a déjà été complété*/
        }
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed) /*déplacement tile par tile*/
            {
                if (tile.IsClear() == true)
                {
                    tile.Highlight();
                }
            }
        }
    }

    public void Move(Transform tilePos)/*Méthode qui permet le déplacement*/
    {
        manager.ResetTiles(); /*enlève le highlighting avant de commencer à se déplacer pour éviter des problèmes de multi-clique*/
        if (hasMoved == false) /*test pour voir si je peux éviter le déplacement plusieurs fois*/
        {
            StartCoroutine(StartMovement(tilePos)); /*Enclenche la coroutine*/
            hasMoved = true;/*Empêche le déplacement une fois effectué*/
            ResetWeaponIcons();
            GetEnemies();/*check quels ennemis je peux attaquer*/
        }
    }
    IEnumerator StartMovement(Transform tilePos) /*Coroutine en question*/
    {   /*Déplacement se fait en deux temps, d'abord se positionne sur X puis sur Y*/
        while (transform.position.x != tilePos.position.x)/*Déplacement sur l'axe des X*/
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(tilePos.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.position.y != tilePos.position.y)/*Déplacement sur l'axe des Y*/
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, tilePos.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

    }

    public void ResetWeaponIcons()/*cache les icones d'épées*/
    {
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }

    void GetEnemies()/*appelée au début du tour de chaque unité pour check s'il y a des ennemis à distance*/
    {
        enemiesInRange.Clear(); /**/
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y) <= AttackRange) /*Vérifie si on est à distance d'attaque*/
            {
                if (unit.playerNumber != manager.playerTurn && hasAttacked == false) /*évite que l'unité puisse attaquer son allié ou attaquer plusieurs fois*/
                {
                    enemiesInRange.Add(unit);/*incrémente la liste d'ennemis à distance d'attaque*/
                    unit.weaponIcon.SetActive(true);/*active l'icone d'épée*/
                }
            }
        }
    }

}
