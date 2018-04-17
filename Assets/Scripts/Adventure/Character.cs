using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    //skills
    public Ability[] abilities = new Ability[6];
    public Ability[] curAbilities = new Ability[4];

    //varrying stats
    public int maxHP;
    public int curHP;

    //background attributes
    public string characterName;
    public bool isEnemy;
    public GameManager gameManager;

    //attributes
    public int speed;

    public virtual void OnTurnStart() {
        
    }

}
