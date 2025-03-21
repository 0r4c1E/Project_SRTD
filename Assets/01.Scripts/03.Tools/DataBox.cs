using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBox : MonoBehaviour
{
    public Text nameBox;

    public Image hpBar;

    public void SetName(string name)
    {
        nameBox.text = name;
    }

    public void SetHP(float hp)
    {
        hpBar.fillAmount = hp;
    }
}
