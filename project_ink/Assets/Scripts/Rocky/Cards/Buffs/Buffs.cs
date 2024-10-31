using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Buff{
    public GameObject indicator;
    internal bool enabled;
    public virtual void Disable(){
        enabled=false;
        indicator.SetActive(false);
    }
    public virtual void Enable(){
        enabled=true;
        indicator.SetActive(true);
    }
    internal void Activated(){
        CardSlotManager.inst.StartCoroutine(ActivatedAnim());
    }
    IEnumerator ActivatedAnim(){
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
[System.Serializable]
public class Buff1_3 : Buff{
    int count;
    float anticReduceAmount;
    public override void Enable()
    {
        base.Enable();
    }
    public override void Disable()
    {
        base.Disable();
    }
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
            return anticipation*anticReduceAmount;
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
    public override void Enable()
    {
        base.Enable();
    }
    public override void Disable()
    {
        base.Disable();
    }
    public void AddBuff(int val){
        if(buffs==null) buffs=new List<int>();
        if(!enabled) Enable();
        buffs.Add(val);
    }
    IEnumerator DelayShoot(){
		yield return new WaitForSeconds(0.3f);
		CardSlotManager.inst.InstantiateProjectile(2, true).gameObject.name="buff 5";
    }
    public void Activate(){
        if(!enabled) return;
        for(int i=0;i<buffs.Count;){
            Activated();
			//uncomment this line after adding enemies
            //CardSlotManager.instance.InstantiateProjectile(2).gameObject.name="buff 5";
            CardSlotManager.inst.StartCoroutine(DelayShoot());
            --buffs[i];
            if(buffs[i]==0){
                buffs[i]=buffs[^1];
                buffs.RemoveAt(buffs.Count-1);
                if(buffs.Count==0) Disable();
            }else ++i;
        }
    }
}
