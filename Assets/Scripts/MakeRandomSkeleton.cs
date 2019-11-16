using Microsoft.Azure.Kinect.Sensor.BodyTracking;

public class MakeRandomSkeleton
{
    public Skeleton MakeSkeleton()
    {
        //Get length for looping
        int length = (int)JointId.Count;
        //Declare + init a new Skeleton
        Skeleton randomSkeleton = new Skeleton();
        //Declare + init a Joint[]
        Joint[] joints = new Joint[length];
        //Initialize the Skeleton's Joint[] with joints[]
        randomSkeleton.Joints = joints;

        for (int i = 0; i < length; i++)
        {
            //0 Declare and init a temp joint, and temp float[]'s to store joint data
            Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint randomJoint = new Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint();
            float[] pos = new float[3];
            float[] rot = new float[4];

            //1 Generate random floats to put into pos and rot
            for (int j = 0; j < pos.Length; j++)
            {
                pos[j] = UnityEngine.Random.Range(-5.0f, 5.0f);
            }
            for (int j = 0; j < rot.Length; j++)
            {
                rot[j] = UnityEngine.Random.Range(-5.0f, 5.0f);
            }
            //2 Assign pos and rot to a temp Joint
            randomJoint.Position = pos;
            randomJoint.Orientation = rot;
            //3 Assign the joint to the skeleton
            randomSkeleton.Joints[i] = randomJoint;
        }
        //4 Return the skeleton full of random joints
        return randomSkeleton;
    }
}
