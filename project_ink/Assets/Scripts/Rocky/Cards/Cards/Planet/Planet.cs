using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet {
    public PlanetType type;
    internal Planet(PlanetType type){
        this.type=type;
    }
    public virtual void Activate(){
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
            case PlanetType.Mars:
            case PlanetType.Sun:
            case PlanetType.Jupiter:
            case PlanetType.Saturn:
                break;
            default: throw new System.Exception("planet type undefined");
        }
    }
    public static Planet FromType(PlanetType type){
        switch(type){
            case PlanetType.Sun: return new Sun();
            default: return new Planet(type);
        }
    }
}
public class Sun : Planet{
    public int charge;
    Planet[] planets;
    internal Sun():base(PlanetType.Sun){
        charge=0;
        type=PlanetType.Sun;
        planets=new Planet[6];
    }
    public override void Activate()
    {
        
    }
    public void AddPlanet(Planet planet){
        int idx=-1, nullIdx=-1;
        for(int i=0;i<planets.Length;++i){
            //find the first null slot
            if(nullIdx==-1&&planets[i]==null)
                nullIdx=i;
            //find the index of the planet with the same type if there is any
            if(planets[i].type==planet.type){
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