using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour {

	public Image myImage;

	public bool FadeOnStart = true;
	
	[DisableIf("@!FadeOnStart"),SerializeField, Indent] private float startFadeTime = 1;


	// Use this for initialization
	void Start () {
		myImage = GetComponent<Image> ();
		if(FadeOnStart) FadeIn(Color.white, startFadeTime, null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FadeIn(Color startColour, float fadeTime, Action onFinish){
		StartCoroutine(FadeInCoroutine(startColour, fadeTime, onFinish));
	}
		
	public void FadeOut(Color startColour, float fadeTime, Action onFinish){
		StartCoroutine(FadeOutCoroutine(startColour, fadeTime, onFinish));
	}

	//from colour to trans
	IEnumerator FadeInCoroutine(Color startColour, float fadeTime, Action onFinish){
		float t = 0;
		myImage.enabled = true;

		while (t < fadeTime) {
			t += Time.deltaTime;
			
			myImage.color = new Color (startColour.r,startColour.g,startColour.b, 1- t/fadeTime);
			yield return null;
		}
		myImage.enabled = false;
		onFinish?.Invoke();
		yield break;
	}


	//from trans to colour
	IEnumerator FadeOutCoroutine(Color startColour, float fadeTime, Action onFinish){
		float t = 0;
		myImage.enabled = true;

		while (t < fadeTime) {
			t += Time.deltaTime;
			myImage.color = new Color (startColour.r,startColour.g,startColour.b, t/fadeTime);
			yield return null;
		}
	//	myImage.enabled = false;
		onFinish?.Invoke();
		yield break;
	}

}
