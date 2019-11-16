using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Stahle.Utility;
using APRLM.Game;
using APRLM.Utilities;
using UnityEngine.UI;
using System.Collections;

public class DebugRenderer : PersistantSingleton<DebugRenderer>
{
    [SerializeField] public List<Skeleton> skeletons = new List<Skeleton>();
    [SerializeField] GameObject[] blockmanArray;
    GameObject[] blockman_averagedResults;
    public GameObject CapturedResultsRoot;
    public GameObject blockmanCaptured_parent;
#if UNITY_EDITOR_WIN
    Device device;
    BodyTracker tracker;
#endif
    Skeleton skeleton;

    public Renderer renderer;
	public Text JointPositionArea_Text;

    [HideInInspector]public bool canUpdate = false;
    // public UnityEngine.UI.Image recordPoseToggleImage; //dragged in manually
    public Toggle recordPoseToggle;//dragged in manually
    public GameObject countdownParent;//manually dragged in
    public Text countdownText; //manually dragged in
    public Text displayCapturedPose; //manually dragged in
    public Text RecordNextPoseToggleText; //manually dragged in 
    protected override void Awake()
    {
		print("DebugRenderer Awake");
        base.Awake();
    }

	public void Start()
	{
		print("DebugRenderer start");
		if (CapturedResultsRoot.activeInHierarchy)
		{
			CapturedResultsRoot.SetActive(false);
		}
		blockmanArray = GameManager.Instance.blockman;
		foreach (GameObject go in blockmanArray)
		{
			go.SetActive(true);
		}
        print("Blockman was fetched from GM and set active here in DebugRenderer");
#if UNITY_EDITOR_WIN
        InitCamera();
#endif
    }
#if UNITY_EDITOR_WIN
    void InitCamera()
    {
        this.device = Device.Open(0);
		var config = new DeviceConfiguration
		{
			ColorResolution = ColorResolution.r720p,
			ColorFormat = ImageFormat.ColorBGRA32,
			DepthMode = DepthMode.NFOV_Unbinned
        };
        device.StartCameras(config);

        //declare and initialize a calibration for the camera
        var calibration = device.GetCalibration(config.DepthMode, config.ColorResolution);
        //initialize a tracker with the calibration we just made
        this.tracker = BodyTracker.Create(calibration);
        renderer = GetComponent<Renderer>();
    }
#endif
    void Update()
    {
        if (canUpdate)
        {
#if UNITY_EDITOR_WIN
            StreamCameraAsTexture();
            CaptureSkeletonsFromCameraFrame();
#endif

#if UNITY_EDITOR_OSX
            CaptureSkeletonsFromFakeRandomData();
#endif
            // if (skeletons.Count > 4)
            // {
            //     Debug.Log("we have enough skeletons");
            //     //Disable this script's Update loop's logic from running
            //     canUpdate = false;
            //     //Activate the parent GO containing the averaged position blockman
            //     CapturedResultsRoot.SetActive(true);
            //     //Clear the text, which is currently holding the last avg vector3 positions of each joint
            //     JointPositionArea_Text.text = "";
            //     //Find the avg of the current joints of the 5 skele's captured
            //     FindAverageSkeletalPosition();
            //     //Enable capturedBlockman, disable first blockman
            //     EnableBlockman(true);
            //     //display what pose was captured
            //     //"Displaying captured results for: " + [pose]
            //     //clear
            //     displayCapturedPose.text = "";
            //     displayCapturedPose.text += "Displaying averaged joint positions in: " + GameManager.Instance.currentPose.name;
            // }

        }//end if(canUpdate) 
    }//end Update()

#if UNITY_EDITOR_WIN
	void StreamCameraAsTexture()
	{
		//this streams camera output as a texture to a plane in the scene
		using (Capture capture = device.GetCapture())
		{
			tracker.EnqueueCapture(capture);
			var color = capture.Color;
			if (color.WidthPixels > 0)
			{
				Texture2D tex = new Texture2D(color.WidthPixels, color.HeightPixels, TextureFormat.BGRA32, false);
				tex.LoadRawTextureData(color.GetBufferCopy());
				tex.Apply();
				renderer.material.mainTexture = tex;
			}
		}
	}
#endif

    void CaptureSkeletonsFromFakeRandomData()
    {
        //0 Make a new object that will make us a skeleton
        MakeRandomSkeleton makeSkeleton = new MakeRandomSkeleton();
        //1 fill this.skeleton with a skeleton
        this.skeleton = makeSkeleton.MakeSkeleton();
        //2 add the skele to this list
        skeletons.Add(this.skeleton);
        //pull out each joint from the skele, transform the data to assign to a vector3 and quaternion
        for (var i = 0; i < (int)JointId.Count; i++)
        {
            var joint = this.skeleton.Joints[i];
            var pos = joint.Position;
            Debug.Log("pos: " + (JointId)i + " " + pos[0] + " " + pos[1] + " " + pos[2]);
            var rot = joint.Orientation;
            Debug.Log("rot " + (JointId)i + " " + rot[0] + " " + rot[1] + " " + rot[2] + " " + rot[3]); // Length 4
            var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
            var r = new Quaternion(rot[1], rot[2], rot[3], rot[0]);
            var obj = blockmanArray[i];
            obj.transform.SetPositionAndRotation(v, r);
        }
    }
#if UNITY_EDITOR_WIN
    //Gets skeletal data from frames, pulls individual joint data from a skeleton, applies pos/rot to blocks representing joints
    void CaptureSkeletonsFromCameraFrame()
	{
		using (var frame = tracker.PopResult())
		{
			Debug.LogFormat("{0} bodies found.", frame.NumBodies);
			if (frame.NumBodies > 0)
			{
				var bodyId = frame.GetBodyId(0);
				//Debug.LogFormat("bodyId={0}", bodyId);
				this.skeleton = frame.GetSkeleton(0);
				skeletons.Add(this.skeleton);
				for (var i = 0; i < (int)JointId.Count; i++)
				{
					var joint = this.skeleton.Joints[i];
					var pos = joint.Position;
					// Debug.Log("pos: " + (JointId)i + " " + pos[0] + " " + pos[1] + " " + pos[2]);
					var rot = joint.Orientation;
					// Debug.Log("rot " + (JointId)i + " " + rot[0] + " " + rot[1] + " " + rot[2] + " " + rot[3]); // Length 4
					var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
					var r = new Quaternion(rot[1], rot[2], rot[3], rot[0]);
					var obj = blockmanArray[i];
					obj.transform.SetPositionAndRotation(v, r);
				}
			}
		}
	}
#endif
	void FindAverageSkeletalPosition()
	{
		Debug.Log("activating blockman captured blocks");
        //blockman_averagedResults is the array of GO's
		blockman_averagedResults = GameManager.Instance.blockmanCaptured;

		// skeletons is a List<Skeleton> of size 5
		for (int i = 0; i < (int)JointId.Count; i++)
		{
			List<Vector3> positionsOfSameJointPositions = new List<Vector3>();

			for (int j = 0; j < skeletons.Count; j++)
			{
				float[] pos = skeletons[j].Joints[i].Position;

				//for fake data, you need to multiply by something bigger than .3 on windows we did * .004
				float scalingFactor = 0.004f;
#if UNITY_EDITOR_OSX
				Debug.Log("TODO -------------------------------------------- verify this");
				scalingFactor = 0.4f;
#endif
				Vector3 posV3 = new Vector3(pos[0], -pos[1], pos[2]) * scalingFactor;
				positionsOfSameJointPositions.Add(posV3);
			}
			Vector3 averageOfSingleJointI = Vector3Helper.FindAveragePosition(positionsOfSameJointPositions);

			float[] rot = skeletons[0].Joints[i].Orientation;
			Quaternion rotationOfFirstSkeleton = new Quaternion(rot[1], rot[2], rot[3], rot[0]);

			var jointCube = blockman_averagedResults[i];
			jointCube.transform.SetPositionAndRotation(averageOfSingleJointI, rotationOfFirstSkeleton);

			JointPositionArea_Text.text += JointDataStringFormatter.formatJointDataToText(averageOfSingleJointI, (JointId)i);
		}
	}

	void EnableBlockman(bool enable)
	{
        //make a parent GO for captured(averaged) blockman
		if (blockmanCaptured_parent == null)
		{
            //make an empty parent obj to store blockman in hierarchy
			blockmanCaptured_parent = new GameObject();
            //give that empty parent a name
            blockmanCaptured_parent.name = "BlockmanCaptured";
        }
        //turn blockman1 off
		foreach (GameObject go in GameManager.Instance.blockman)
		{
			go.SetActive(!enable);
		}
        //turn blockman2 on and set its blocks into the parent object
		foreach (GameObject go in GameManager.Instance.blockmanCaptured)
		{
            go.transform.SetParent(blockmanCaptured_parent.transform);
            go.SetActive(enable);
		}
        //move it slightly to the left for visibility
        blockmanCaptured_parent.transform.Translate(new Vector3(-1.5f,0,0));
	}

	void ClearSkeletonsList()
	{
		skeletons.Clear();
	}

#if UNITY_EDITOR_WIN
    private void OnDisable()
    {
        //todo test if only called once at the end of the program, if so, renable the below
        print("DebugRenderer onDisable was called");
		device.StopCameras();
		//k4a_device_close(device) here.
		if (tracker != null)
		{
			tracker.Dispose();
		}
		if (device != null)
		{
			device.Dispose();
		}
	}
#endif
    //it's a toggle because it's either recording or not recording
    public void RecordPose_LinkedToToggle()//todo mess with .interactable to prevent click abuse
    //todo second time pressing this button it will say "recording stopped!", it should say started if we want to clikc the button every time
    //so we should mark this toggle as off again after we complete a pose
    {
		//change
		canUpdate = !canUpdate;
		// /change

        // if (GameManager.Instance.poseList.Count > 0)
        // {
			
            // Color recordingRed = new Color(255, 0, 0);
            // Color disabledGrey = new Color(211, 211, 211);

            // ColorBlock cb = recordPoseToggle.colors;

            // if (!recordPoseToggle.isOn)
            // {
            //     //then we are pressing it off, change selected color to disabled grey
            //     cb.selectedColor = disabledGrey;
            //     //todo implement a text change to "stop recording"
            //     RecordNextPoseToggleText.text = "Record Next Pose";

            //     print("Recording stopped!");
            // }
            // else //comes here first time we press
            // {
            //     //then we are pressing it on, change selected color to recording red
            //     cb.selectedColor = recordingRed;
            //     RecordNextPoseToggleText.text = "Stop Recording"; //todo implement killswitch
            //     print("recording started!");
            // }
            // //assign the toggle the color
            // recordPoseToggle.colors = cb;

            // StartCoroutine(StartCountdown());
        // }
    }


    // IEnumerator StartCountdown()
    // {
    //     countdownParent.SetActive(true);
    //     countdownText.text = "3...";
    //     yield return new WaitForSeconds(.5f); //!sped up for testing!
    //     countdownText.text += "2...";
    //     yield return new WaitForSeconds(.5f);
    //     countdownText.text += "1...";
    //     yield return new WaitForSeconds(.5f);
    //     countdownParent.SetActive(false);

    //     canUpdate = true;
    // }

    // public void PoseAccepted_linkToButton()
    // {
	// 	Debug.Log("pose accepted. Writing data to file...");
    //     countdownText.text = "";
    //     WriteDataToFile();
    //     CapturedResultsRoot.SetActive(false);
	// 	EnableBlockman(false);
	// 	ClearSkeletonsList();

    //     GameManager.Instance.MarkCurrentPoseCompleted();
	// }
    // void WriteDataToFile()
    // {
    //     //this assumes makeFile.cs is dragged onto the same obj as DebugRenderer.cs
    //     //0 get reference to the file making script
    //     MakeFile makeFile = GetComponent<MakeFile>();
    //     //1 give makeFile a string that it will write to a .txt file
    //     makeFile.WriteToFile(displayCapturedPose.text +"\n"+ JointPositionArea_Text.text);
    // }
	// public void PoseDeclined_linkToButton() 
	// {
	// 	Debug.Log("pose declined. clearing captured data");
    //     countdownText.text = "";
    //     //hide the captured results panel
    //     CapturedResultsRoot.SetActive(false);
    //     //disable captured blockman, enable live blockman
	// 	EnableBlockman(false);
    //     //reset the skeletons list that we just captured
	// 	ClearSkeletonsList();
	// }
}
