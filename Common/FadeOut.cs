using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {

	float timeLeft;
	public float MessageDuration = 2;
	Text myText;

	TextMesh myTextMesh;

	public bool ShowOnStart;
	public string StartMessage;
	
	private TextMeshPro myTMP;
	private TextMeshProUGUI myTMPGui;
	private Action _onFinishAction;

	// Use this for initialization
	void Start () {
		myTMP = GetComponent<TextMeshPro>();
		myTMPGui = GetComponent<TextMeshProUGUI>();
		myText = gameObject.GetComponent<Text> ();
		myTextMesh = gameObject.GetComponent<TextMesh> ();
	
		if(ShowOnStart) DisplayMessage(StartMessage);
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (timeLeft < 0.01f)
	    {
	        if (myText != null)
	            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, 0);
	        if (myTextMesh != null)
		        myTextMesh.color = new Color(myTextMesh.color.r, myTextMesh.color.g, myTextMesh.color.b, 0);
	        if (myTMP != null)
		        myTMP.color = new Color(myTMP.color.r, myTMP.color.g, myTMP.color.b, 0);
	        if (myTMPGui != null)
		        myTMPGui.color = new Color(myTMPGui.color.r, myTMPGui.color.g, myTMPGui.color.b, 0);

	        _onFinishAction?.Invoke();
	        //_onFinishAction = null;

	    }
        else {
			if(myText != null)
				myText.color = new Color (myText.color.r, myText.color.g,myText.color.b, timeLeft / MessageDuration);
			if(myTextMesh != null)
				myTextMesh.color = new Color (myTextMesh.color.r, myTextMesh.color.g,myTextMesh.color.b, timeLeft / MessageDuration);
			if (myTMP != null)
			{
				myTMP.color = new Color (myTMP.color.r, myTMP.color.g,myTMP.color.b, timeLeft / MessageDuration);
				myTMP.outlineColor = new Color (myTMP.outlineColor.r, myTMP.outlineColor.g,myTMP.outlineColor.b, timeLeft / MessageDuration);
				myTMP.faceColor = new Color (myTMP.faceColor.r, myTMP.faceColor.g,myTMP.faceColor.b, timeLeft / MessageDuration);
			}
			if (myTMPGui != null)
			{
				myTMPGui.color = new Color (myTMPGui.color.r, myTMPGui.color.g,myTMPGui.color.b, timeLeft / MessageDuration);
				myTMPGui.outlineColor = new Color (myTMPGui.outlineColor.r, myTMPGui.outlineColor.g,myTMPGui.outlineColor.b, timeLeft / MessageDuration);
				myTMPGui.faceColor = new Color (myTMPGui.faceColor.r, myTMPGui.faceColor.g,myTMPGui.faceColor.b, timeLeft / MessageDuration);
			}


			timeLeft -= Time.fixedDeltaTime;
		}
	}

	public void DisplayMessage(string message, float fadeTime = -1, Action onFinish = null )
	{
		if (fadeTime > -1) MessageDuration = fadeTime;
		gameObject.SetActive (true);

		_onFinishAction = onFinish;

		if(	gameObject.GetComponent<Text> () != null)
			gameObject.GetComponent<Text> ().text = message;
		if(	gameObject.GetComponent<TextMesh> () != null)
			gameObject.GetComponent<TextMesh> ().text = message;
		if(	gameObject.GetComponent<TextMeshPro> () != null)
			gameObject.GetComponent<TextMeshPro> ().text = message;
		if(	gameObject.GetComponent<TextMeshProUGUI> () != null)
			gameObject.GetComponent<TextMeshProUGUI> ().text = message;
		
		timeLeft = MessageDuration;
	}

}
