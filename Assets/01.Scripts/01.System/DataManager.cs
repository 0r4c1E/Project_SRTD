using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{ 
    public static DataManager instance;

    public Player playerData = new Player();

    // ����׽�Ʈ�� �ӽ� Ÿ�� ����Ʈ
    public List<TowerObject> towerTire1 = new List<TowerObject>();
    public List<TowerObject> towerTire2 = new List<TowerObject>();
    public List<TowerObject> towerTire3 = new List<TowerObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Application.runInBackground = true;
    }

    public TowerObject ReturnTowerObject(int level)
    {
        if (level == 1)
        {
            return towerTire1[Random.RandomRange(0, towerTire1.Count)];
        }
        else if (level == 2)
        {
            return towerTire2[Random.RandomRange(0, towerTire2.Count)];
        }
        else if (level == 3)
        {
            return towerTire3[Random.RandomRange(0, towerTire3.Count)];
        }
        else return null;
    }

    public bool ReturnNextTowerTier(int level)
    {
        if (level < 3) return true;
        else return false;
    }
}


[System.Serializable]
public class Player
{
    public int stageLevel = 1;
    public int playerHP;
    public int playerMaxHP;

    public int money;
}

[System.Serializable]
public class Status
{
    public string key; // Ÿ�� ������ ���� Ű��
    public string name;
    public int level;
    public double maxHealth;
    public double hp;
    public int attack;
    public double attackSpeed;
    public int attackCount = 1; // ����ȸ�� (���ð���)
    public double defense;
    public double speed;
    public double critical;

}