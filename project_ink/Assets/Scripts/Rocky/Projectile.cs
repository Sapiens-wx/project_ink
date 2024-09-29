using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rgb;
    int damage;
    // Start is called before the first frame update
    void Start()
    {
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5);
    }
    public void InitProjectile(Card card, Vector2 velocity){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        damage=card.damage;
        rgb.velocity=velocity;
    }
    public void InitProjectile(int damage, Vector2 velocity){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.damage=damage;
        rgb.velocity=velocity;
    }
}
