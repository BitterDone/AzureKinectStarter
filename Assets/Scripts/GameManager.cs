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
        PlayScenePressed =0, //Read the Poses we want to use for this session and give them to canvas
        PoseListPopulated, //The canvas has written the poses to UI fed to it by this GameManager
        WaitingForUserToPressStart, //nothing to do, waiting for user
        StartButtonPressed, //The user has pressed the start button to begin capturing pose data
        ReadyNext,
        ReadyNextCountDownOver,
        CaptureCompleted
            //TODO finish game states
            //TODO delegates for game states and message broadcasting, static events...etc
    }

    public class GameManager : PersistantSingleton<GameManager>
    {

        [Header("Drag Poses you've created onto the 'Pose List'.",order =0)]
        [Space(-10,order = 1)]
        [Header("Multiple Poses can be dropped at once.",order = 2)]

        [Tooltip("Right click in any folder in Project window to make a new Pose.")]

        public List<Pose> poseList;
        public GameState currentState;
        public Pose currentPose;
		public GameObject[] blockman; //todo refactor into a blockmanMaker.cs
		public GameObject[] blockmanCaptured; //todo refactor into a blockmanMaker.cs
		public GameObject blockPrefab;
        public int currentPoseIndex;

        protected override void Awake()
        {
			print("GM awake");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            base.Awake();
            currentState = GameState.PlayScenePressed;
            CheckSettings();
            MakeBlockMan();
			MakeBlockManCaptured();
        }
        void Start()
        {
            Debug.Log("GM start called");
        }
        public List<Pose> GetPoseList()
        {
            return poseList;
        }
        void CheckSettings()
        {
            if(poseList.Count < 1)
            {
                Debug.Log("!No poses were dragged into the Pose List!");
                EditorApplication.isPlaying = false;
            }
            else
            {
                //currentPose gets set to first Pose in the list
                currentPoseIndex = 0;
                currentPose = poseList[currentPoseIndex];
            }
        }

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			print(scene.name + " loaded");
		}

		bool CheckForPoses()
        {
            if (poseList.Count < 1)
            {
                Debug.Log("!No poses were dragged into the Pose List!");
                return false;
            }
            else
            {
                return true;
            }
        }
        public void MarkCurrentPoseCompleted()
        {
            //transfer the first line of pose text to capturedPose text
            //check if there's another pose in in the poselist
            if(currentPoseIndex + 1 < poseList.Count)
            {
                currentPose.isCaptured = true;
                //if there is, then increment the index
                currentPoseIndex++;
                //assign currentPose to next
                currentPose = poseList[currentPoseIndex];
            }
            else
            {
                print("no more poses in list!"); //we want the app to stop after this, or at least disable the record pose
            }

        }
        public void LoadSceneAdditively(int scene)
        {
            if (CheckForPoses())
            {
                //Load another scene on different thread
                SceneManager.LoadSceneAsync(scene,LoadSceneMode.Additive);
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
        }
        
		//todo put block man under this GameManager so they dont dissapear
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

		public void MakeBlockManCaptured()
		{
			int size = (int)JointId.Count;

			blockmanCaptured = new GameObject[size];

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
				blockmanCaptured[i] = cube;
			}
			print("BlockmanCaptured was created in GM");
		}

		void OnDisable()//a persistant singleton class will only have this called once, when program ending.
		{
			print("OnDisabled GM");
		}
	}
}