using System;
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
    IEnumerator DelayShoot(int shootCount){
        for(int i=0;i<shootCount;++i){
            yield return new WaitForSeconds(0.3f);
            CardSlotManager.inst.InstantiateProjectile(1, true).gameObject.name="venus buff";
        }
    }
    //dec anticipation by 20%
    public float Mercury(float anticipation){
        if(PlanetManager.inst.HasPlanet(PlanetType.Mercury)){
            //saturn effect
            if(PlanetManager.inst.HasPlanet(PlanetType.Saturn))
                return anticipation*.6f;
            else
                return anticipation*.8f;
        }
        return anticipation;
    }
    //auto shoot a 1-damage card for every damage card shot
    public void Venus(Card cardShot, List<IEnumerator> actions){
        if(PlanetManager.inst.HasPlanet(PlanetType.Venus) && cardShot.damage>0){
            //saturn effect
            if(PlanetManager.inst.HasPlanet(PlanetType.Saturn))
                actions.Add(DelayShoot(2));
            else
                actions.Add(DelayShoot(1));
        }
    }
    public void Uranus(){

    }
    int marsEffectCounter=0;
    
    ///<summary>
    ///activate the next damage card for every 3 damage card shot. 
    ///returns true if needs to activate the damage card for an extra time
    ///</summary>
    public void Mars(Card cardShot, List<IEnumerator> actions){
        if(PlanetManager.inst.HasPlanet(PlanetType.Mars) && cardShot.damage>0){
            if(marsEffectCounter>3){
                marsEffectCounter=0;
                //saturn effect
                if(PlanetManager.inst.HasPlanet(PlanetType.Saturn))
                    actions.Add(cardShot.Activate());
                actions.Add(cardShot.Activate());
            }
            ++marsEffectCounter;
        }
    }

    int jupiterEffectCounter=0;
    /// <summary>
    /// auto fire the next card in discard pile for every 3 cards shot
    /// </summary>
    public void Jupiter(List<IEnumerator> actions){
        if(PlanetManager.inst.HasPlanet(PlanetType.Jupiter)){
            if(jupiterEffectCounter>2){
                jupiterEffectCounter=0;
                //saturn effect
                if(PlanetManager.inst.HasPlanet(PlanetType.Saturn))
                    actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
                actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
            }
            ++jupiterEffectCounter;
        }
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
    public void Activate(Card cardShot, List<IEnumerator> actions){
        if(!enabled) return;
        if(cardShot.damage==0) return;
        --count;
        actions.Add(DelayShoot());
        if(count==0){
            Disable();
        }
    }
}

/// <summary>
/// Mars activation effect: activate the next two damage card when shot
/// </summary>
[System.Serializable]
public class BuffP_5 : Buff{
    int count;
    public override void Enable()
    {
        base.Enable();
        count=2;
    }
    public void Activate(Card cardShot, List<IEnumerator> actions){
        if(!enabled) return;
        if(cardShot.damage==0) return;
        --count;
        if(count==0){
            Disable();
        }
        actions.Add(cardShot.Activate());
    }
}

/// <summary>
/// Jupiter activation effect: auto fire the next two cards in the discard pile
/// </summary>
[System.Serializable]
public class BuffP_6 : Buff{
    public override void Enable()
    {
        base.Enable();
    }
    public void Activate(List<IEnumerator> actions){
        if(!enabled) return;
        actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
        actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
        Disable();
    }
}