using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour  // https://www.youtube.com/watch?v=bcPqdCSGCls
{
    private Vector2 movementInput;
    private Vector3 direction;

    [SerializeField] private int maxUnitPoint;
    [SerializeField] private int currUnitPoint;


    private bool hasMoved;

    public int CurrUnitPoint { get => currUnitPoint; set => currUnitPoint = value; }
    public int MaxUnitPoint { get => maxUnitPoint; set => maxUnitPoint = value; }

    private void Start()
    {
        currUnitPoint = maxUnitPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "Player")
        {
            if (CurrUnitPoint > 0)
            {
                if (movementInput.x == 0 && movementInput.y == 0)
                    hasMoved = false;
                else if (movementInput.x != 0 || movementInput.y != 0)
                {
                    if (!hasMoved)
                    {
                        hasMoved = true;
                        GoTo();
                    }
                }
            }
            else
                Debug.Log("Plus de point de mouvement");
        }
    }

    public void GoTo()
    {
        if (movementInput.x > 0)
            direction = new Vector3(1f, 0);
        else if (movementInput.x < 0)
            direction = new Vector3(-1f, 0);
        else if (movementInput.y > 0)
            direction = new Vector3(0, 1f);
        else if (movementInput.y < 0)
            direction = new Vector3(0, -1f);
        transform.position += direction;
        CurrUnitPoint--;
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position -= direction;
        CurrUnitPoint++;
    }
}
