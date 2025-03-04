using UnityEngine;
using System.IO;
using System.Text;

public class CardLog : Singleton<CardLog>{
    public string path;
    [TextArea] public string log;
    StringBuilder sb;
    int indent;
    void Start(){
        sb=new StringBuilder();
        indent=0;
    }
    void OnDisable(){
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
    public static void Log(string msg){
        for(int i=0;i<inst.indent;++i)
            inst.sb.Append("    ");
        inst.sb.Append(msg);
        inst.sb.Append('\n');
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