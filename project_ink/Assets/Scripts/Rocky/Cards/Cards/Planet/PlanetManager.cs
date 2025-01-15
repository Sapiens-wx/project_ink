using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        //update mask
        Planet planet=Planet.FromType(planetType);
        mask|=(int)planetType;

StringBuilder sb=new StringBuilder();
        int idx=-1, nullIdx=-1;
        for(int i=0;i<planets.Length;++i){
            if(planets[i]==null)
                sb.Append($"i={i}, planets[i]=null\n");
            else
                sb.Append($"i={i}, planets[i]={planets[i].type}\n");
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
        //if the planet is sun
        if(planet.type==PlanetType.Sun){
            //activate the previous sun
            if(sun!=null){
                CardSlotManager.inst.buffP_6.Activate();
            }
            sun=(Sun)planet;
            if(idx!=-1) //there is a existing sun, then use this sun's position
                planets[idx]=planet;
            else
                planets[nullIdx]=planet;
            PlanetVisualizer.inst.AddPlanet(planet);
            return;
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
        if(planet==null) return;
        int idx=-1;
        mask&=~(int)planet.type;
        for(int i=0;i<planets.Length;++i){
            //find the index of the planet with the same type if there is any
            if(planets[i]==planet){
                idx=i;
                break;
            }
        }
        PlanetVisualizer.inst.RemovePlanet(planets[idx]);
        planets[idx]=null;
        if(planet.type==PlanetType.Sun)
            sun=null;
    }
}