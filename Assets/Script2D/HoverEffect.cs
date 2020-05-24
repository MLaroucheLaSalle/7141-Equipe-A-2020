using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [SerializeField] private float hoverAmount; /*Valeur de zoom quand le curseur survole une tile*/

    private void OnMouseEnter()
    {
        transform.localScale += Vector3.one * hoverAmount; /*Zoom in. vector3.one veut dire vector3(1,1,1)*/
    }

    private void OnMouseExit()
    {
        transform.localScale -= Vector3.one * hoverAmount; /*Zoom out*/
    }
}
