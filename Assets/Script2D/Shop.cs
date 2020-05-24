using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    public Button playerToggleButton;

    public GameObject playerMenu;

    GameManager manager;

    private void Start()
    {
        manager = GameManager.instance; /*Instantiation du Game Manager*/
    }

    private void Update()
    {
        if(manager.playerTurn == 2) /*rend le UI du shop activable si c'est le tour du joueur*/
        {
            playerToggleButton.interactable = true;
        }
        else
        {
            playerToggleButton.interactable = false;
        }
    }

    public void ToggleMenu(GameObject menu)/*active et désactive le shop*/
    {
        menu.SetActive(!menu.activeSelf);
    }

    public void CloseMenu()
    {
        playerMenu.SetActive(false);
    }

    public void BuyItem(Unit unit)
    {
        if (manager.playerTurn == 1 && unit.cost <= manager.gold)
        {
            manager.gold -= unit.cost;
            playerMenu.SetActive(false);
        }
        else
        {
            print("not enough gold");
            return;
        }

        DeselectUnit();

        manager.UpdateGoldText();
        manager.purchasedItem = unit;
        SetCreatableTiles();
    }

    private void DeselectUnit()
    {
        if (manager.selectedUnit != null) /*Déselectionne une unité si sélectionnée pour pouvoir drop le unit acheté*/
        {
            manager.selectedUnit.selected = false;
            manager.selectedUnit = null;
        }
    }

    public void SetCreatableTiles() /*va changer la couleur des tiles sur lesquelles on peut drop notre unit acheté*/
    {
        manager.ResetTiles();
        foreach(Tile tile in FindObjectsOfType<Tile>())
        {
            if (tile.IsClear())
            {
                tile.SetCreatable();
            }
        }
    }
}
