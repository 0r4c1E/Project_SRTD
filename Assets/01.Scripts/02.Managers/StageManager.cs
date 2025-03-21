using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public Text life;
    private string lifeBase = "³²Àº ¸ñ¼û : {0}/{1}";

    public void InitializeLife()
    {
        Player data = DataManager.instance.playerData;
        data.playerHP = data.playerMaxHP;

        life.text = string.Format(lifeBase, data.playerHP, data.playerMaxHP);
    }
    public void SetLife()
    {
        life.text = string.Format(lifeBase, DataManager.instance.playerData.playerHP, DataManager.instance.playerData.playerMaxHP);
    }
    public void GetDamage(int count)
    {
        DataManager.instance.playerData.playerHP -= count;
        SetLife();
    }
}
