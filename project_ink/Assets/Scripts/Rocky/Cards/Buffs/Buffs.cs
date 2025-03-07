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
    int activateCount=1;
    public override void Enable()
    {
        base.Enable();
        activateCount++;
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
        for(int i=0;i<activateCount;++i){
            while(action.MoveNext()){
                yield return action.Current;
            }
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
            CardSlotManager.inst.InstantiateProjectile(3, true).gameObject.name="buff 5";
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

//行星环绕效果的Effects
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
            bool hasSaturn=PlanetManager.inst.HasPlanet(PlanetType.Saturn);
            CardLog.PlanetOrbitEffect("Mercury: decrease anticipation by 20%", hasSaturn);
            //saturn effect
            if(hasSaturn)
                return anticipation*.6f;
            else
                return anticipation*.8f;
        }
        return anticipation;
    }
    //auto shoot a 1-damage card for every damage card shot
    public void Venus(Card cardShot, List<IEnumerator> actions){
        if(PlanetManager.inst.HasPlanet(PlanetType.Venus) && cardShot.damage>0){
            bool hasSaturn=PlanetManager.inst.HasPlanet(PlanetType.Saturn);
            CardLog.PlanetOrbitEffect("Venus: auto shoot a 1-damage card for every damage card shot", hasSaturn);
            //saturn effect
            if(hasSaturn)
                actions.Add(DelayShoot(2));
            else
                actions.Add(DelayShoot(1));
        }
    }
    public void Uranus(){
        bool hasSaturn=PlanetManager.inst.HasPlanet(PlanetType.Saturn);
        CardLog.PlanetOrbitEffect("Uranus: charge 1", hasSaturn);
        //saturn effect
        if(hasSaturn){
            foreach(Uranus u in PlanetVisualizer.inst.uranuses)
                u.charge+=2;
        }
        else{
            foreach(Uranus u in PlanetVisualizer.inst.uranuses)
                u.charge+=1;
        }
    }
    int marsEffectCounter=0;
    
    ///<summary>
    ///activate the next damage card for every 3 damage card shot. 
    ///</summary>
    public void Mars(Card cardShot, List<IEnumerator> actions){
        if(PlanetManager.inst.HasPlanet(PlanetType.Mars) && cardShot.damage>0){
            if(marsEffectCounter>3){
                marsEffectCounter=0;
                bool hasSaturn=PlanetManager.inst.HasPlanet(PlanetType.Saturn);
                CardLog.PlanetOrbitEffect("Mars: activate the next damage card for every 3 damage card shot.",hasSaturn);
                //saturn effect
                if(hasSaturn)
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
                bool hasSaturn=PlanetManager.inst.HasPlanet(PlanetType.Saturn);
                CardLog.PlanetOrbitEffect("Jupiter: auto fire the next card in discard pile for every 3 cards shot", hasSaturn);
                //saturn effect
                if(hasSaturn)
                    actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
                actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
            }
            ++jupiterEffectCounter;
        }
    }
    //destroy the closest planet and double activate its effect
    public void Earth(){
        Planet planet=PlanetManager.inst.RandomPlanet();
        if(planet!=null){
            CardLog.PlanetOrbitEffect("Earth: destroy the closest planet and double activate its effect",false);
            CardLog.Indent();
            //double activate the planet
            planet.DoubleActivate();
            //destroy the planet
            PlanetManager.inst.RemovePlanet(planet);
            CardLog.UnIndent();
        }
    }
}

/// <summary>
/// Venus: deal a 2-damage card every 3 cards are shot
/// </summary>
[System.Serializable]
public class BuffP_3 : Buff{
    int count;
    int damage;
    public override void Enable()
    {
        base.Enable();
        count=3;
        damage=2;
    }
    public void DoubleEnable()
    {
        Enable();
        damage=4;
    }
    IEnumerator DelayShoot(){
		yield return new WaitForSeconds(0.3f);
		CardSlotManager.inst.InstantiateProjectile(damage, true).gameObject.name="venus buff activated";
    }
    public void Activate(Card cardShot, List<IEnumerator> actions, bool forceActivate=false){
        if(!forceActivate){
            if(!enabled) return;
            if(cardShot.damage==0) return;
            --count;
        }
        actions.Add(DelayShoot());
        CardLog.ActivatePlanetEffect("Venus: deal a 2-damage card every 3 cards are shot", damage/3==2);
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
    bool doubleActivate;
    public override void Enable()
    {
        base.Enable();
        count=2;
        doubleActivate=false;
    }
    public void DoubleEnable(){
        Enable();
        doubleActivate=true;
    }
    public void Activate(Card cardShot, List<IEnumerator> actions, bool forceActivate=false){
        if(!forceActivate&&!enabled) return;
        if(cardShot.damage==0) return;
        if(!forceActivate){
            --count;
            if(count==0){
                Disable();
            }
        }
        actions.Add(cardShot.Activate());
        if(doubleActivate)
            actions.Add(cardShot.Activate());
        CardLog.ActivatePlanetEffect("Mars: activate next two damage card when shot", doubleActivate);
    }
}

/// <summary>
/// Sun: destroy all planets of sun, then shoot a 5*n damage fire ball
/// </summary>
[System.Serializable]
public class BuffP_6 : Buff{
    public void Activate(){
        PlanetManager.inst.sun.Activate();
        PlanetManager.inst.RemovePlanet(PlanetManager.inst.sun);
    }
}

/// <summary>
/// Jupiter activation effect: auto fire the next two cards in the discard pile
/// </summary>
[System.Serializable]
public class BuffP_7 : Buff{
    bool doubleActivate;
    public override void Enable()
    {
        base.Enable();
        doubleActivate=false;
    }
    public void DoubleEnable(){
        base.Enable();
        doubleActivate=true;
    }
    public void Activate(List<IEnumerator> actions, bool forceActivate=false){
        if(!forceActivate&&!enabled) return;
        actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
        actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
        if(doubleActivate){
            actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
            actions.Add(CardSlotManager.inst.cardDealer.GetCard().AutoFire());
        }
        CardLog.ActivatePlanetEffect("Jupiter: auto fire the next two cards in the discard pile", doubleActivate);
        Disable();
    }
}

/// <summary>
/// Saturn activation effect: randomly activates three planet effects
/// </summary>
[System.Serializable]
public class BuffP_8 : Buff{
    public void Activate(bool doubleActivate=false){
        if(PlanetManager.inst.sun!=null)
            PlanetManager.inst.sun.charge+=3;
        CardLog.ActivatePlanetEffect("Saturn activation effect: randomly activates three planet effects", doubleActivate);
        CardLog.Indent();
        for(int i=doubleActivate?6:3;i>0;--i){
            switch(UnityEngine.Random.Range(1,9)){
                //TODO: fill in the effects for saturn activation effect
                case 1: //earth effect
                    break;
                case 2: //mercury effect
                    CardSlotManager.inst.buffP_2.Enable();
                    break;
                case 3: //venus effect
                    CardSlotManager.inst.buffP_3.Enable();
                    break;
                case 4: //uranus effect
                    //randomly choose a uranus and activate its effect
                    if(PlanetVisualizer.inst.uranuses.Count>0)
                        PlanetVisualizer.inst.uranuses[UnityEngine.Random.Range(0, PlanetVisualizer.inst.uranuses.Count)].Activate();
                    break;
                case 5: //mars effect
                    CardSlotManager.inst.buffP_5.Enable();
                    break;
                case 6: //sun effect
                    if(PlanetManager.inst.sun==null){ //if doesn't have sun, don't activate sun effect
                        --i;
                        break;
                    }
                    CardSlotManager.inst.buffP_6.Activate();
                    break;
                case 7: //jupter effect
                    CardSlotManager.inst.buffP_7.Enable();
                    break;
                case 8: //saturn effect
                    Activate(doubleActivate);
                    break;
            }
        }
        CardLog.UnIndent();
    }
}