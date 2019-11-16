using UnityEngine;
using System.Collections.Generic;
using APRLM.Game;
using UnityEngine.UI;
using System;

public class PosePanel : MonoBehaviour
{
	//dragged in manually, else is last child
	[Tooltip("Dragged in manually!")]
	public Text poseText;

    public Text capturedPoseText;//dragged in manually
    string firstLineCached;
    string firstLineUpdated;
    void Start()
	{
		//Request pose data from GM
		ParsePoseList(GameManager.Instance.poseList);
    }

	void ParsePoseList(List<Pose> poseList)
	{
		foreach (Pose p in poseList)
		{
			poseText.text += p.poseName.ToString() + "\n";
		}
	}

    void UpdatePoseList(List<Pose> poseList)
    {
        foreach (Pose p in poseList)
        {
            if(p.isCaptured)
            {
                //move it over to the completed text box
            }
            else
            {

            }
        }
    }

    //todo mess with .interactable to prevent click abuse
    //todo this doesn't doesn't work when we go to add the next succesfull pose 
    public void MarkPoseAsSuccesfullyCaptured_LinkedToButton()//(accept pose) save data to file button
    {
        //take the first line from pose text and put it in captured pose text
        capturedPoseText.text += firstLineCached + "\n";
        //delete firstline from pose text
        poseText.text = poseText.text.Replace(firstLineUpdated, "");
        //trim the whitespace away
        poseText.text = poseText.text.Trim();
        //clear the caches (CASH-SHAY'S...don't @ me)
        firstLineCached = "";
        firstLineUpdated = "";
    }

    //todo mess with .interactable to prevent click abuse
    public void HighlightCurrentPose_LinkedToButton()//record pose button 
    {

        //todo change to if else structure
        //if(poseText.text.IndexOf("\n",System.StringComparison.CurrentCulture) > -1)
        //get the first line(first pose)
        //todo i think this doesn't work when theres only one pose in there
        try
        {
            //add an if check to see if a newline character is there or keep up with pose list.Count to know when we are on the last line
            firstLineCached = poseText.text.Substring(0, poseText.text.IndexOf("\n", System.StringComparison.CurrentCulture));
        }
        catch(Exception e) //todo put exact exception here
        {
            print("Couldn't cache first line of string!" + e
                + "...bc there was no \n escape char detected bc there was only a single pose left in the textbox!");
            //get the remaining single pose in the text box
            firstLineCached = poseText.text.Substring(0);
        }
        //bug this is gonna get just the "" the second and subsequent times
        firstLineUpdated = firstLineCached;
        //add html code to change the color of this string
        firstLineUpdated = "<color='red'>" + firstLineUpdated + "</color>";
        //assign the firstline back with the color html code (strings are immutable, bro)
        poseText.text = poseText.text.Replace(firstLineCached,firstLineUpdated);
    }

}
