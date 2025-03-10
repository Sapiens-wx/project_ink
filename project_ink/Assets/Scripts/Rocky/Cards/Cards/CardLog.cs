using UnityEngine;
using System.IO;
using System.Text;

public class CardLog : Singleton<CardLog>{
    public string path;
    public bool log;
    StringBuilder sb;
    int indent;
    internal override void Awake()
    {
        base.Awake();
        sb=new StringBuilder();
        indent=0;
    }
    void OnDisable(){
        if(log)
            Write();
    }
    public static void Indent(){
        ++inst.indent;
    }
    public static void UnIndent(){
        --inst.indent;
    }
    public static void LogFire(Card card){
        Log($"F {card.type}, Dmg={card.damage}");
    }
    public static void LogAutoFire(Card card){
        Log($"A {card.type}, Dmg={card.damage}");
    }
    private static void _Log(string msg){
        for(int i=0;i<inst.indent;++i)
            inst.sb.Append("    ");
        inst.sb.Append(msg);
        inst.sb.Append('\n');
    }
    public static void Log(string msg){
        if(inst.log){
            LogCurCardSlotState();
            _Log(msg);
        }
    }
    public static void LogCurCardSlotState(){
        _Log("CS: "+CardSlotManager.inst.CurCardSlotState());
    }
    public static void MouseFire(){
        Log("-----Mouse Click-----");
    }
    public static void CardProjectileInstantiated(Card proj){
        Log($"Instantiate Projectile of card {proj.type} Dmg={proj.damage}");
    }
    public static void ProjectileInstantiated(Projectile proj){
        Log($"Instantiate Projectile Dmg={proj.damage}");
    }
    public static void ActivatePlanetEffect(string msg, bool doubleActivate=false){
        if(doubleActivate)
            Log("Double Activate "+msg);
        else
            Log("Activate "+msg);
    }
    public static void PlanetOrbitEffect(string msg, bool doubleEffect){
        if(doubleEffect)
            Log("Double Orbit Effect "+msg);
        else
            Log("Orbit Effect "+msg);
    }
    public static void DiscardCardEffect(string msg){
        Log("E" + msg);
    }
    public static void Buff(string msg){
        Log("B "+msg);
    }
    public static void DealCard(Card card){
        Log("D "+card.type);
    }
    public static void ReturnCard(Card card){
        Log("R "+card.type);
    }
    public void Write()
    {
        try
        {
            // Append the string to the file
            string fullPath=Application.dataPath+path;
            using (StreamWriter writer = new StreamWriter(fullPath, false))
            {
                writer.Write(sb);
                FileInfo f=new FileInfo(fullPath);
                Debug.Log("Successfully wrote to the file: " + f.FullName);
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to write to the file: " + e.Message);
        }
    }
}