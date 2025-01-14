using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager:MonoBehaviour{

    public static PlanetManager inst;
    Planet[] planets;
    [HideInInspector] public Sun sun;
    [HideInInspector] public int mask;

    void Awake(){
        inst=this;
    }
    void Start(){
        Initialize();
    }
    void Initialize(){
        planets=new Planet[6];
    }
    public bool HasPlanet(PlanetType type){
        return (mask&(int)type)!=0;
    }
    public void AddPlanet(PlanetType planetType){
        if(planetType==PlanetType.Sun){
            //activate the previous sun
            if(sun!=null){
                CardSlotManager.inst.buffP_6.Activate();
            }
            //create a new sun
            sun=new Sun();
            PlanetVisualizer.inst.AddPlanet(sun);
            return;
        }
        //update mask
        Planet planet=Planet.FromType(planetType);
        mask|=(int)planetType;

        int idx=-1, nullIdx=-1;
        for(int i=0;i<planets.Length;++i){
            //find the first null slot
            if(planets[i]==null){
                if(nullIdx==-1)
                    nullIdx=i;
            }
            //find the index of the planet with the same type if there is any
            else if(planets[i].type==planetType){
                idx=i;
                break;
            }
        }
        if(idx>=0){ //has that planet in orbits
            if(sun!=null){
                //try to add the planet to the sun
                sun.AddPlanet(planet);
            } else{
                //destroy the original planet
                ActivatePlanet(planet);
            }
        } else{
            //assign the planet
            planets[nullIdx]=planet;
            PlanetVisualizer.inst.AddPlanet(planet);
        }
    }
    void ActivatePlanet(Planet planet){
        if(sun!=null) sun.charge++;
        planet.Activate();
    }
    public void RemovePlanet(Planet planet){
        int idx=-1;
        for(int i=0;i<planets.Length;++i){
            //find the index of the planet with the same type if there is any
            if(planets[i]==planet){
                idx=i;
                break;
            }
        }
        PlanetVisualizer.inst.RemovePlanet(planets[idx]);
        planets[idx]=null;
    }
}