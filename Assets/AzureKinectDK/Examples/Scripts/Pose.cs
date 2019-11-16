using UnityEngine;

public enum PoseName
{
    ArmsUp = 0,
    ArmsHalfwayUp,
    Tpose,
    ArmsHalfwayDown,
    ArmsDown,
}
public enum PoseType
{
    Standing =0,
    Sitting,
    Count
}
//Right click to make a new Pose
[CreateAssetMenu(menuName = "Settings/Pose")]
public class Pose : ScriptableObject
{
    //A Pose has a name: Tpose
    public PoseName poseName;
    //A Pose has a type: Standing pose
    public PoseType poseType;
    //A Pose has a status
    public bool isCaptured;
    //Eventually, when we make a pose match game, a pose will have a 'perfect score' position to match
}
