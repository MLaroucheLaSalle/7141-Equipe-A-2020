using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameManager manager; /*reference au game manager*/
    private SpriteRenderer sr;
    [SerializeField] private Sprite[] tileGraphics; /*Tableau pour stocker les tiles, juste pour l'allure so far.*/
    [SerializeField] private LayerMask obstacleLayer; /*va être utilisée pour détecter un obstacle (joueur, ennemis, ojets)*/
    [SerializeField] private Color highlightedColor; /*couleur de la tile survolée par le curseur*/
    public bool isWalkable; /*pour identifier les tiles sur lesquelles on peut se déplacer*/
    [SerializeField] private Color CreatableColor;
    public bool isCreatable;

    void Start()
    {
        manager = GameManager.instance; /*Instantiation du Game Manager*/
        sr = GetComponent<SpriteRenderer>(); /*On va chercher le spriteRenderer des tiles*/
        int randTile = Random.Range(0, tileGraphics.Length); /*Pioche au hasard une tile de la liste des tiles*/
        sr.sprite = tileGraphics[randTile]; /*attribue la tile au prefab Tile, donne une map à tiles randomisées*/

    }

    public void SetCreatable()
    {
        sr.color = CreatableColor;
        isCreatable = true;
    }

    public bool IsClear() /*Vérifie si la case est clear ou pas*/
    {   /*Créé une var "cercle" quand il y a une cellule avec un objet du layer obstacle*/
        Collider2D obstacle = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
        if (obstacle != null)
        {
            return false;
        }
        else {
            return true;
        }
    }

    public void Highlight()/*change la couleur de la tile survolée*/
    {
        sr.color = highlightedColor;
        isWalkable = true;
    }

    public void Reset()/*inverse de Highlight()*/
    {
        sr.color = Color.white;/*Reset la couleur*/
        isWalkable = false;
        isCreatable = false;
    }

    private void OnMouseDown() /*Clique pour déplacer*/
    {
        if (isWalkable && manager.selectedUnit != null)
        {
            manager.selectedUnit.Move(this.transform);
        }
        else if (isCreatable == true && manager.purchasedItem!=null)
        {
            Unit unit = Instantiate(manager.purchasedItem, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
            unit.hasMoved = true;
            unit.hasAttacked = true;
            manager.ResetTiles();
            manager.purchasedItem = null;
        }
    }
}
