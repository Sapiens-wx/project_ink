using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CardInventory cardInventory;
    public LayerMask groundMixLayer, groundLayer, platformLayer; //groundLayer: platform|ground, platformLayer: platform
    [NonSerialized] public int platformLayerIdx; //not layermask, this is the layer.
    public LayerMask enemyLayer;
    public LayerMask enemyBulletLayer;
    public float distEpsilon;
    public static GameManager inst;
    void Awake()
    {
        inst=this;
        platformLayerIdx=MaskToLayer(platformLayer);
        cardInventory.Init();
    }
    void OnDisable(){
        cardInventory.SaveRunTimeCards();
    }
    public static bool IsLayer(LayerMask mask, int layer){
        return (mask.value&(1<<layer))!=0;
    }
    int MaskToLayer(LayerMask layerMask){
        int layer=0;
        int mask=layerMask.value>>1;
        while(mask!=0){
            layer++;
            mask>>=1;
        }
        return layer;
    }
}
