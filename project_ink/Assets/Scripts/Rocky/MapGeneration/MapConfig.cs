using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "map config", menuName = "Inventory/MapConfig")]
public class MapConfig : ScriptableObject
{
    public string initialTiless;
    [SerializeField] string ltTopTiless, midTopTiless, rtTopTiless, ltMidTiless, rtMidTiless, ltBotTiless, midBotTiless, rtBotTiless;
    public List<Element> arr;

    [HideInInspector] public int[] initialTiles, ltTopTiles, midTopTiles, rtTopTiles, ltMidTiles, rtMidTiles, ltBotTiles, midBotTiles, rtBotTiles;
    public void Initialize()
    {
        initialTiles = StringToArray(initialTiless);
        ltTopTiles = StringToArray(ltTopTiless);
        midTopTiles = StringToArray(midTopTiless);
        rtTopTiles = StringToArray(rtTopTiless);
        ltMidTiles = StringToArray(ltMidTiless);
        rtMidTiles = StringToArray(rtMidTiless);
        ltBotTiles = StringToArray(ltBotTiless);
        midBotTiles = StringToArray(midBotTiless);
        rtBotTiles = StringToArray(rtBotTiless);
        for(int i = 0; i < arr.Count; ++i)
        {
            arr[i].Initialize();
        }
    }
    /// <summary>
    /// format: "1,2,3..."
    /// </summary>
    /// <param name="s">input</param>
    /// <returns></returns>
    public static int[] StringToArray(string s)
    {
        int[] ret;
        int len = 1;
        for(int i = 0; i < s.Length; ++i)
        {
            if (s[i] == ',') ++len;
        }
        ret = new int[len];
        int l = 0, r=0;
        for(int i=0; i<len; ++i)
        {
            while (r < s.Length && s[r] != ',') ++r;
            ret[i] = int.Parse(s.Substring(l, r - l));
            l = r + 1;
            r = l;
        }
        return ret;
    }

    [System.Serializable]
    public class Element
    {
        [SerializeField] public string name;
        [SerializeField] public int maxAmount;
        [SerializeField] private string sltTop, smidTop, srtTop, sltMid, srtMid, sltBot, smidBot, srtBot;
        [HideInInspector]
        public int[] ltTop, midTop, rtTop, ltMid, midMid, rtMid, ltBot, midBot, rtBot;
        public void Initialize()
        {
            ltTop = StringToArray(sltTop);
            midTop = StringToArray(smidTop);
            rtTop = StringToArray(srtTop);
            ltMid = StringToArray(sltMid);
            rtMid = StringToArray(srtMid);
            ltBot = StringToArray(sltBot);
            midBot = StringToArray(smidBot);
            rtBot = StringToArray(srtBot);
        }
        public void Collapse(List<int> list, int[] arr)
        {
            int i,j;
            for(i=0,j=0;i<list.Count && j<arr.Length;){
                if(list[i]>arr[j]){
                    while(j<arr.Length && arr[j]<list[i])
                        ++j;
                    if(j>=arr.Length || list[i]!=arr[j])
                        list.RemoveAt(i);
                    else{
                        ++i;++j;
                    }
                } else if(list[i]<arr[j]){
                    while(i<list.Count && list[i]<arr[j])
                        list.RemoveAt(i);
                } else{
                    ++i;++j;
                }
            }
            if(i<list.Count)
                list.RemoveRange(i, list.Count-i);
        }
    }
}
