using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// stores room prefabs (GameObject) that can be instantiated based on a Room tree
/// </summary>
[CreateAssetMenu(fileName = "RoomSceneConfig", menuName = "GameConfig/RoomSceneConfig")]
public class RoomSceneConfig : ScriptableObject
{
    [System.Serializable]
    public class Element
    {
        public RoomType roomType;
        public List<GameObject> prefabs;
        public Element()
        {

        }
        public Element(RoomType roomType, params GameObject[] prefabsList)
        {
            this.roomType = roomType;
            prefabs = new List<GameObject>();
            foreach(GameObject prefab in prefabsList)
            {
                prefabs.Add(prefab);
            }
        }
    }
    /// <summary>
    /// unity cannot draw dictionary in inspector, so I use Array List instead, and use CustomEditor to convert the list into the dictionary.
    /// </summary>
    public List<Element> roomPrefabs_arrayList;
    /// <summary>
    /// stores prefabs of rooms based on its RoomType
    /// </summary>
    public Dictionary<RoomType, List<GameObject>> roomPrefabs_dictionary;
    /// <summary>
    /// Dictionary is not serializable, so call InitializeDictionary to update roomPrefabs_dictionary based on the contents in roomPrefabs_arrayList each time unity is loaded.
    /// </summary>
    public void InitializeDictionary()
    {
        roomPrefabs_dictionary = new Dictionary<RoomType, List<GameObject>>(roomPrefabs_arrayList.Count);
        foreach(Element element in roomPrefabs_arrayList)
        {
            roomPrefabs_dictionary.Add(element.roomType, element.prefabs);
        }
    }
}