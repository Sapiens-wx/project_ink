using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/EnemyList")]
public class EnemyList : ScriptableObject {
    [SerializeField] EnemyPrefab[] rawList;
    GameObject[] prefabs;
    public void Init(){
        prefabs=new GameObject[rawList.Length];
        foreach(EnemyPrefab e in rawList){
            prefabs[(int)e.type]=e.prefab;
        }
    }
    public GameObject this[EnemyType type]{
        get=>prefabs[(int)type];
    }
    [System.Serializable]
    public class EnemyPrefab {
        public EnemyType type;
        public GameObject prefab;
    }
}

public enum EnemyType{
    Club,
    Heart,
    Spade,
    Diamond,
}