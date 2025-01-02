using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Buff{
    public GameObject indicator;
    internal bool enabled;
    public virtual void Disable(){
        enabled=false;
        if(indicator!=null)
            indicator.SetActive(false);
    }
    public virtual void Enable(){
        enabled=true;
        if(indicator!=null)
            indicator.SetActive(true);
    }
    internal void Activated(){
        CardSlotManager.inst.StartCoroutine(ActivatedAnim());
    }
    IEnumerator ActivatedAnim(){
        if(indicator==null) yield break;
        float totalT=.3f;
        float time=0;
        float t;
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(time<totalT){
            t=time/totalT;
            t=Mathf.Sin(3.14f*t)*.4f+1;
            indicator.transform.localScale=new Vector3(t,t,t);
            time+=Time.fixedDeltaTime;
            yield return wait;
        }
        indicator.transform.localScale=Vector3.one;
    }
}
/// <summary>
/// reduce anticipation time by [anticReduceAmount]%
/// </summary>
[System.Serializable]
public class Buff_ReduceAntic : Buff{
    int count;
    float anticReduceAmount;
    public void Enable(int count, float anticReduceAmount){
        Enable();
        this.count=count;
        this.anticReduceAmount=anticReduceAmount;
    }
    public float Activate(float anticipation){
        if(enabled){
            --count;
            if(count==0) Disable();
            Activated();
            return anticipation*(1-anticReduceAmount);
        }
        return anticipation;
    }
}
[System.Serializable]
/// <summary>
/// the first discarded card in every new round is activated
/// </summary>
public class Buff1_4 : Buff
{
    [HideInInspector] public bool firstCardOfRound;
    public override void Enable()
    {
        base.Enable();
    }
    public override void Disable()
    {
        base.Disable();
    }
    public IEnumerator Activate(IEnumerator action){
        if(!enabled) yield break;
        if(!firstCardOfRound) yield break;
        Activated();
        firstCardOfRound=false;
        while(action.MoveNext()){
            yield return action.Current;
        }
    }
}
[System.Serializable]
public class Buff1_5 : Buff{
    List<int> buffs;
    public void AddBuff(int val){
        if(buffs==null) buffs=new List<int>();
        if(!enabled) Enable();
        buffs.Add(val);
    }
    IEnumerator DelayShoot(int shootCount){
        for(int i=0;i<shootCount;++i){
            Activated();
            yield return new WaitForSeconds(0.3f);
            CardSlotManager.inst.InstantiateProjectile(2, true).gameObject.name="buff 5";
        }
    }
    public void Activate(){
        if(!enabled) return;
        int shootCount=0;
        for(int i=0;i<buffs.Count;){
            shootCount++;
            --buffs[i];
            if(buffs[i]==0){
                buffs[i]=buffs[^1];
                buffs.RemoveAt(buffs.Count-1);
                if(buffs.Count==0) Disable();
            }else ++i;
        }
        CardSlotManager.inst.StartCoroutine(DelayShoot(shootCount));
    }
}

[System.Serializable]
public class PlanetBuff : Buff{
    IEnumerator DelayShoot(){
		yield return new WaitForSeconds(0.3f);
		CardSlotManager.inst.InstantiateProjectile(1, true).gameObject.name="venus buff";
    }
    //dec anticipation by 20%
    public float Mercury(float anticipation){
        if(PlanetManager.inst.hasPlanet(PlanetType.Mercury))
            return anticipation*.8f;
        return anticipation;
    }
    //auto shoot a 1-damage card for every damage card shot
    public void Venus(Card cardShot){
        if(PlanetManager.inst.hasPlanet(PlanetType.Venus) && cardShot.damage>0){
            CardSlotManager.inst.StartCoroutine(DelayShoot());
        }
    }
    public void Uranus(){

    }
}

/// <summary>
/// deal a 3-damage card after the next 3 cards are dealt
/// </summary>
[System.Serializable]
public class BuffP_3 : Buff{
    int count;
    public override void Enable()
    {
        base.Enable();
        count=3;
    }
    IEnumerator DelayShoot(){
		yield return new WaitForSeconds(0.3f);
		CardSlotManager.inst.InstantiateProjectile(3, true).gameObject.name="venus buff activated";
    }
    public void Activate(Card cardShot){
        if(!enabled) return;
        if(cardShot.damage==0) return;
        --count;
        CardSlotManager.inst.StartCoroutine(DelayShoot());
        if(count==0){
            Disable();
        }
    }
}