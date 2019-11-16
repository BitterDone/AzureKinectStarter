using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Stahle.Utility;
using UnityEngine.UI;
using System.Collections;

public class DebugRenderer : PersistantSingleton<DebugRenderer>
{
    [HideInInspector]public bool canUpdate = false;
    [SerializeField] public List<Skeleton> skeletons = new List<Skeleton>();
    [SerializeField] GameObject[] blockmanArray;
    public Toggle recordPoseToggle; //dragged in manually 
    public Renderer renderer;
	public GameObject blockPrefab;
	Skeleton skeleton;
#if UNITY_EDITOR_WIN
    Device device;
    BodyTracker tracker;
#endif

    protected override void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		base.Awake();
		MakeBlockMan();
	}

	public void Start()
	{
		foreach (GameObject go in blockmanArray)
		{
			go.SetActive(true);
		}
		recordPoseToggle.onValueChanged.AddListener(OnToggleValueChanged);
#if UNITY_EDITOR_WIN
		InitCamera();
#endif
    }

	void MakeBlockMan()
	{
		int numberOfJoints = (int)JointId.Count;

		blockmanArray = new GameObject[numberOfJoints];

		for (var i = 0; i < numberOfJoints; i++)
		{
			GameObject jointCube = Instantiate(blockPrefab, transform);
			//deactivate it - (its Start() or OnEnable() won't be called)
			jointCube.SetActive(false);
			jointCube.name = Enum.GetName(typeof(JointId), i);
			//why do we multiply by .4?  idk
			jointCube.transform.localScale = Vector3.one * 0.4f;
			blockmanArray[i] = jointCube;
		}
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

        var calibration = device.GetCalibration(config.DepthMode, config.ColorResolution);
        this.tracker = BodyTracker.Create(calibration);

		GameObject cameraFeed = GameObject.FindWithTag("CameraFeed");
		renderer = cameraFeed.GetComponent<Renderer>();
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
        }
    }

#if UNITY_EDITOR_WIN
	void StreamCameraAsTexture()
	{
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

#if UNITY_EDITOR_WIN
    void CaptureSkeletonsFromCameraFrame()
	{
		using (var frame = tracker.PopResult())
		{
			Debug.LogFormat("{0} bodies found.", frame.NumBodies);
			if (frame.NumBodies > 0)
			{
				var bodyId = frame.GetBodyId(0);
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
	
    public void RecordPose_LinkedToToggle()
    {
		canUpdate = !canUpdate;
    }

	void OnToggleValueChanged(bool isOn)
	{
		ColorBlock cb = recordPoseToggle.colors;
		cb.normalColor = Color.white; // blue;
		cb.highlightedColor = Color.white; // green;

		cb.selectedColor = Color.white;
		cb.pressedColor = Color.red;

		if (isOn)
		{
			cb.selectedColor = Color.red;
			cb.pressedColor = Color.white;
		}

		recordPoseToggle.colors = cb;
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
		//device.StopCameras();
		//k4a_device_close(device) here.
		if (tracker != null)
		{
			tracker.Dispose();
		}
		if (device != null)
		{
			device.StopCameras();
			device.Dispose();
		}
	}
#endif
}
