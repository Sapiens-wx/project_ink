using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerEffects : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] SpriteRenderer dashEffect;
    [SerializeField] float dashInterval;
    [SerializeField] Sprite[] dashEffectSprs;
    [Header("Hit")]
    [SerializeField] SpriteRenderer hitSpr;
    [SerializeField] float hitInterval;
    [SerializeField] Sprite[] hitEffectSprs;
    
    public static PlayerEffects inst;
    Vector3 dashEffectOffset, hitEffectOffset;
    void Awake(){
        inst=this;
    }
    void Start()
    {
        //dash effect
        dashEffectOffset=dashEffect.transform.position-transform.position;
        dashEffect.gameObject.SetActive(false);
        //hit effect
        hitEffectOffset=hitSpr.transform.position-transform.position;
        hitSpr.gameObject.SetActive(false);
    }

    public void PlayEffect(EffectType type){
        switch(type){
            case EffectType.Dash:
                    dashEffectOffset.x=Mathf.Abs(dashEffectOffset.x)*-PlayerCtrl.inst.dir;
                    PlayEffect(dashEffect, dashInterval, dashEffectSprs, dashEffectOffset);
                break;
            case EffectType.Hit:
                    //PlayEffect(hitSpr, hitInterval, hitEffectSprs, hitEffectOffset);
                break;
        }
    }
    private void PlayEffect(SpriteRenderer spr, float frameDuration, Sprite[] frames, Vector2 offset){
        offset.x=Mathf.Abs(offset.x)*-PlayerCtrl.inst.dir;
        spr.transform.localScale=new Vector3(PlayerCtrl.inst.dir,1,1);
        spr.transform.position=(Vector2)transform.position+offset;
        spr.gameObject.SetActive(true);
        StartCoroutine(PlayAnim(spr,frames,frameDuration));
    }
    public void PlayDashEffect(){
        dashEffectOffset.x=Mathf.Abs(dashEffectOffset.x)*-PlayerCtrl.inst.dir;
        dashEffect.transform.localScale=new Vector3(PlayerCtrl.inst.dir,1,1);
        dashEffect.transform.position=transform.position+dashEffectOffset;
        dashEffect.gameObject.SetActive(true);
        StartCoroutine(DashEffectAnim());
    }
    IEnumerator PlayAnim(SpriteRenderer spr, Sprite[] frames, float frameDuration){
        WaitForSeconds wait=new WaitForSeconds(frameDuration);
        for(int i=0;i<frames.Length;++i){
            spr.sprite=frames[i];
            yield return wait;
        }
        spr.gameObject.SetActive(false);
    }
    IEnumerator DashEffectAnim(){
        WaitForSeconds wait=new WaitForSeconds(dashInterval);
        for(int i=0;i<dashEffectSprs.Length;++i){
            dashEffect.sprite=dashEffectSprs[i];
            yield return wait;
        }
        dashEffect.gameObject.SetActive(false);
    }
    public enum EffectType{
        Dash,
        Hit
    }
}
