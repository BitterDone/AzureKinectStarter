using UnityEngine;
using UnityEngine.SceneManagement;
using Stahle.Utility;
using System.Collections.Generic;
using UnityEditor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System;

namespace APRLM.Game
{
    public enum GameState
    {
    }

    public class GameManager : PersistantSingleton<GameManager>
    {

        [Header("Drag Poses you've created onto the 'Pose List'.",order =0)]
        [Space(-10,order = 1)]
        [Header("Multiple Poses can be dropped at once.",order = 2)]

        [Tooltip("Right click in any folder in Project window to make a new Pose.")]
		
		public GameObject[] blockman; //todo refactor into a blockmanMaker.cs
		public GameObject blockPrefab;

        protected override void Awake()
        {
			print("GM awake");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            base.Awake();
            MakeBlockMan();
        }
        void Start()
        {
            Debug.Log("GM start called");
        }

		void MakeBlockMan()
		{
			int size = (int)JointId.Count;

			blockman = new GameObject[size];

			for (var i = 0; i < size; i++)
			{
				//make a cube for every joint
				//var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				GameObject cube = Instantiate(blockPrefab, transform);
				//deactivate it - (its Start() or OnEnable() won't be called)
				cube.SetActive(false);
				//give cube a name of matching joint
				cube.name = Enum.GetName(typeof(JointId), i);
				//why do we multiply by .4?  idk
				cube.transform.localScale = Vector3.one * 0.4f;
				//add our cube to the skeleton[]
				blockman[i] = cube;
			}
			print("Blockman was created in GM");
		}
		

		void OnDisable()
		{
			print("OnDisabled GM");
		}
	}
}