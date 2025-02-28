using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet {
    public PlanetType type;
    internal Planet(PlanetType type){
        this.type=type;
    }
    public void DoubleActivate(){
        if(PlanetManager.inst.sun!=null)
            PlanetManager.inst.sun.charge+=2;
        CardSlotManager.inst.planetBuff.Uranus();
        switch(type){
            case PlanetType.Earth:
                Debug.LogError("impossible situation: double activate earth effect");
                break;
            case PlanetType.Mercury:
                CardSlotManager.inst.buffP_2.Enable(3, .36f);
                break;
            case PlanetType.Venus:
                CardSlotManager.inst.buffP_3.DoubleEnable();
                break;
            case PlanetType.Uranus:
                Uranus u = (Uranus)this;
                u.surroundingEnemy.OnDamaged(2*u.charge);
                break;
            case PlanetType.Mars:
                CardSlotManager.inst.buffP_5.DoubleEnable();
                break;
            case PlanetType.Sun:
                ((Sun)this).DoubleActivate_Sun();
                break;
            case PlanetType.Jupiter:
                CardSlotManager.inst.buffP_7.DoubleEnable();
                break;
            case PlanetType.Saturn:
                if(PlanetManager.inst.sun!=null)
                    --PlanetManager.inst.sun.charge; //the next line of the code will charge the sun by 3
                CardSlotManager.inst.buffP_8.Activate(true);
                break;
            default: throw new System.Exception("planet type undefined");
        }
    }
    public virtual void Activate(){
        if(PlanetManager.inst.sun!=null)
            ++PlanetManager.inst.sun.charge;
        CardSlotManager.inst.planetBuff.Uranus();
        switch(type){
            case PlanetType.Earth:
                break;
            case PlanetType.Mercury:
                CardSlotManager.inst.buffP_2.Enable(3, .6f);
                break;
            case PlanetType.Venus:
                CardSlotManager.inst.buffP_3.Enable();
                break;
            case PlanetType.Uranus:
                Uranus u = (Uranus)this;
                u.surroundingEnemy.OnDamaged(u.charge);
                break;
            case PlanetType.Mars:
                CardSlotManager.inst.buffP_5.Enable();
                break;
            case PlanetType.Sun:
                break;
            case PlanetType.Jupiter:
                CardSlotManager.inst.buffP_7.Enable();
                break;
            case PlanetType.Saturn:
                if(PlanetManager.inst.sun!=null)
                    --PlanetManager.inst.sun.charge; //the next line of the code will charge the sun by 3
                CardSlotManager.inst.buffP_8.Activate();
                break;
            default: throw new System.Exception("planet type undefined");
        }
    }
    public static Planet FromType(PlanetType type){
        switch(type){
            case PlanetType.Sun: return new Sun();
            case PlanetType.Uranus: return new Uranus();
            default: return new Planet(type);
        }
    }
}
public class Uranus : Planet{
    public int charge;
    public EnemyBase surroundingEnemy;
    internal Uranus():base(PlanetType.Uranus){
        charge=0;
    }
}
public class Sun : Planet{
    public int charge;
    Planet[] planets;
    internal Sun():base(PlanetType.Sun){
        charge=1;
        planets=new Planet[6];
    }
    public override void Activate()
    {
        foreach(Planet p in planets){
            if(p!=null)
                p.Activate();
        }
        CardSlotManager.inst.InstantiateProjectile(5*charge, true);
    }
    public void DoubleActivate_Sun(){
        foreach(Planet p in planets){
            if(p!=null)
                p.Activate();
        }
        CardSlotManager.inst.InstantiateProjectile(10*charge, true);
    }
    public void AddPlanet(Planet planet){
        int idx=-1, nullIdx=-1;
        for(int i=0;i<planets.Length;++i){
            //find the first null slot
            if(planets[i]==null){
                if(nullIdx==-1)
                    nullIdx=i;
            }
            //find the index of the planet with the same type if there is any
            else if(planets[i].type==planet.type){
                idx=i;
                break;
            }
        }
        if(idx>=0){ //has that planet in orbits
            //destroy the original planet
            planet.Activate();
        } else{
            //assign the planet
            planets[nullIdx]=planet;
            PlanetVisualizer.inst.AddPlanetToSun(planet);
        }
    }
}

public enum PlanetType{
    Earth=1,
    Mercury=2,
    Venus=4,
    Uranus=8,
    Mars=16,
    Sun=32,
    Jupiter=64,
    Saturn=128
}