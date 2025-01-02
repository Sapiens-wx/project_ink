using UnityEngine;

[CreateAssetMenu(fileName = "planetViConfig", menuName = "GameConfig/PlanetViConfig")]
public class PlanetViConfig : ScriptableObject {
    [SerializeField] GameObject earth;
    [SerializeField] GameObject mercury;
    [SerializeField] GameObject venus;
    [SerializeField] GameObject uranus;
    [SerializeField] GameObject mars;
    [SerializeField] GameObject sun;
    [SerializeField] GameObject jupiter;
    [SerializeField] GameObject saturn;
    public GameObject GetPlanet(PlanetType type){
        switch(type){
            case PlanetType.Earth: return earth;
            case PlanetType.Mercury: return mercury;
            case PlanetType.Venus: return venus;
            case PlanetType.Uranus: return uranus;
            case PlanetType.Mars: return mars;
            case PlanetType.Sun: return sun;
            case PlanetType.Jupiter: return jupiter;
            case PlanetType.Saturn: return saturn;
            default: throw new System.Exception("planet type undefined: "+type.ToString());
        }
    }
}