using UnityEngine;
using System.Collections;

public class B1_1_Platform : Singleton<B1_1_Platform>{
    [SerializeField] float platformDisappearInterval;

    Animator animator;
    void Start(){
        animator=GetComponent<Animator>();
    }
    public void BeginAlternatePlatform(){
        StartCoroutine(AlternatePlatform());
    }
    IEnumerator AlternatePlatform(){
        WaitForSeconds wait=new WaitForSeconds(platformDisappearInterval);
        bool isPlatform1Active=true;
        while(true){
            animator.SetTrigger(isPlatform1Active?"to_platform2":"to_platform1");
            isPlatform1Active=!isPlatform1Active;
            yield return wait;
        }
    }
}