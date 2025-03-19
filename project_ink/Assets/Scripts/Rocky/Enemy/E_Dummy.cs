using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class E_Dummy : EnemyBase {
    [SerializeField] TMP_Text dps_text, dp10s_text, damage_text;

    Queue<Tuple<float,int>> q;
    float dps, dp10s;
    public override int Dir { get => base.Dir; set{return;} }
    internal override void Start()
    {
        base.Start();
        q=new Queue<Tuple<float,int>>();
    }
    public override void OnDamaged(int damage){
        UpdateDamageQueue(damage);
        damage_text.text=damage.ToString();
        dps_text.text=dps.ToString();
        dp10s_text.text=dp10s.ToString();
    }
    void UpdateDamageQueue(int damage){
        q.Enqueue(new Tuple<float,int>(Time.time, damage));
        while(q.Count>0 && Time.time-q.Peek().Item1>10)
            q.Dequeue();
        dp10s=0;
        dps=0;
        foreach(var t in q){
            dp10s+=t.Item2;
            if(Time.time-t.Item1<=1)
                dps+=t.Item2;
        }
    }
}