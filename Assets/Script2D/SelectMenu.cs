using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Selectable[] button;

    // Start is called before the first frame update
    void Start() // RÉCIT 8
    {
        if (panels[0].name == "Main") // RÉCIT 8
        {
            Invoke("PanelToggle", 0.01f);
        }

    }

    public void PanelToggle(int position)
    {
        Input.ResetInputAxes();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(position == i);
            if (position == i)
            {
                button[i].Select();
            }
        }
    }
}
