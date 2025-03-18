using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CanEditMultipleObjects]
[CustomEditor(typeof(EnemyHitBox))]
public class EnemyHitBoxEditor : Editor
{
    EnemyHitBox tgt;
    void OnEnable(){
        tgt=target as EnemyHitBox;
    }
    public override void OnInspectorGUI()
    {
        tgt.connectedEnemy=(EnemyBase)EditorGUILayout.ObjectField("Connected Enemy", tgt.connectedEnemy, typeof(EnemyBase), true);
    }
}
