using UnityEngine;

[CreateAssetMenu(fileName="PathFindingConfig", menuName="GameConfig/PathFindingConfig")]
public class PathFindingConfig : ScriptableObject{
    public int jumpXmin, jumpXmax, jumpY, horizontalJumpXMax;
}