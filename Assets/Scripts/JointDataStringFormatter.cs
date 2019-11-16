using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;

public static class JointDataStringFormatter
{
	public static string formatJointDataToText(Vector3 singleJointPosition, JointId joint)
	{
		string formattedLine = "Avg " + joint.ToString() + ": " + singleJointPosition.ToString() + "\n";
		return formattedLine;
	}
}
