using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlanetVisualizer:MonoBehaviour{
    [SerializeField] PlanetViConfig config;
    [SerializeField] float planetDist, planetSunDist;

    public static PlanetVisualizer inst;
    Transform playerTransform;
    List<Planet> planets, planetSun;
    List<GameObject> planetObjs, planetSunObjs;
    Sun sun;
    GameObject sunObj;

    //Uranus
    [HideInInspector] public Dictionary<EnemyBase, Tuple<Planet, GameObject>> uranusesDict;
    [HideInInspector] public List<Planet> uranuses;

    void Awake(){
        inst=this;
        planets=new List<Planet>(6);
        planetSun=new List<Planet>(6);
        planetObjs=new List<GameObject>(6);
        planetSunObjs=new List<GameObject>(6);
        uranuses=new List<Planet>();
    }
    void Start(){
        playerTransform=PlayerShootingController.inst.transform;
        uranusesDict=new Dictionary<EnemyBase, Tuple<Planet, GameObject>>(RoomManager.inst.enemies.Count);
    }
    void UpdatePlanetPos(Transform planetObj, Transform center){
        planetObj.position=(Vector2)center.position+new Vector2(planetDist,0);
    }
    void UpdatePlanetsPos(){
        float theta=0, dtheta=Mathf.PI*2/planetObjs.Count;
        for(int i=0;i<planetObjs.Count;++i){
            planetObjs[i].transform.position=(Vector2)playerTransform.position+MathUtil.Rotate(new Vector2(planetDist,0), theta);
            theta+=dtheta;
        }
    }
    void UpdatePlanetsSunPos(){
        float theta=0, dtheta=Mathf.PI*2/planetSunObjs.Count;
        for(int i=0;i<planetSunObjs.Count;++i){
            planetSunObjs[i].transform.position=(Vector2)sunObj.transform.position+MathUtil.Rotate(new Vector2(planetSunDist,0), theta);
            theta+=dtheta;
        }
    }
    public void AddPlanet(Planet planet){
        GameObject go=Instantiate(config.GetPlanet(planet.type));
        go.transform.SetParent(playerTransform);
        planets.Add(planet);
        planetObjs.Add(go);
        UpdatePlanetsPos();
        //if is sun
        if(planet.type==PlanetType.Sun){
            sun=planet as Sun;
            sunObj=go;
        }
    }
    public void AddPlanetToSun(Planet planet){
        GameObject go=Instantiate(config.GetPlanet(planet.type));
        go.transform.SetParent(playerTransform);
        planetSun.Add(planet);
        planetSunObjs.Add(go);
        UpdatePlanetsSunPos();
    }
    void RemovePlanetsOfSun(){
        foreach(GameObject go in planetSunObjs){
            Destroy(go);
        }
        planetSun.Clear();
        planetSunObjs.Clear();
    }
    public void RemovePlanet(Planet planet){
        for(int i=planets.Count-1;i>-1;--i){
            if(planets[i]==planet){
                if(planets[i].type==PlanetType.Sun){ //if removes the sun, remove all the planets of the sun
                    RemovePlanetsOfSun();
                    sun=null;
                }
                Destroy(planetObjs[i]);
                planetObjs.RemoveAt(i);
                planets.RemoveAt(i);
                break;
            }
        }
        UpdatePlanetsPos();
    }
    public void RemoveSunPlanet(Planet planet){
        for(int i=planetSun.Count-1;i>-1;--i){
            if(planetSun[i]==planet){
                planetSun.RemoveAt(i);
                Destroy(planetSunObjs[i]);
                planetSunObjs.RemoveAt(i);
            }
        }
        UpdatePlanetsSunPos();
    }
    public void AddUranus(EnemyBase enemy, Uranus planet){
        //try to choose the enemy that is hit. if is it surrounded by a planet, then randomly choose an enemy
        if(uranusesDict.ContainsKey(enemy)){
            foreach(EnemyBase e in RoomManager.CurrentRoom.enemies){
                if(!uranusesDict.ContainsKey(e)){
                    enemy=e;
                }
            }
        }
        //if all the enemies are surrounded by a planet
        if(uranusesDict.ContainsKey(enemy)){
            //choose the closest enemy and activate uranus effect
            enemy=RoomManager.CurrentRoom.ClosestEnemy(playerTransform);
            uranusesDict[enemy].Item1.Activate();
            planet.surroundingEnemy=enemy;
            uranuses.Remove(uranusesDict[enemy].Item1);
            uranusesDict[enemy]=new Tuple<Planet, GameObject>(planet, uranusesDict[enemy].Item2);
            uranuses.Add(planet);
        } else{ //there is an enemy available
            GameObject go=Instantiate(config.GetPlanet(PlanetType.Uranus));
            go.transform.SetParent(enemy.transform);
            UpdatePlanetPos(go.transform, enemy.transform);
            planet.surroundingEnemy=enemy;
            uranusesDict.Add(enemy, new Tuple<Planet, GameObject>(planet, go));
            uranuses.Add(planet);
        }
    }
    public void RemoveUranus(EnemyBase enemy){
        uranuses.Remove(uranusesDict[enemy].Item1);
        Destroy(uranusesDict[enemy].Item2);
        uranusesDict.Remove(enemy);
    }
}