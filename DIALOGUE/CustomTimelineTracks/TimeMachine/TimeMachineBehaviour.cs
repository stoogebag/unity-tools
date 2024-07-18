using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimeMachineBehaviour : PlayableBehaviour
{
	public TimeMachineAction action;
	public Condition condition;
	public string markerToJumpTo, markerLabel;
	public float timeToJumpTo;
	public TimelineConditionProvider ConditionProvider;

	[HideInInspector]
	public bool clipExecuted = false; //the user shouldn't author this, the Mixer does

	
	//BC: LAG BREAKS THIS! probably deal with that at some time in the future!
	//how? well if theres a lag spike (happens in editor quite a bit) then t he timeline will just
	//skip over this clip and then kapow 
	
	
	public bool ConditionMet()
	{
		switch(condition)
		{
			case Condition.Always:
				return true;
				
			case Condition.Conditional:
				//The Timeline will jump to the label or time if a specific Platoon still has at least 1 unit alive
				if(ConditionProvider != null)
				{
					return !ConditionProvider.ConditionMet();
				}
				else
				{
					return false;
				}

			case Condition.Never:
			default:
				return false;
		}
	}

	public enum TimeMachineAction
	{
		Marker,
		JumpToTime,
		JumpToMarker,
		Pause,
	}

	public enum Condition
	{
		Always,
		Never,
		Conditional
		
	}
}
