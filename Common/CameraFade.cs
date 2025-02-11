#if ODIN_INSPECTOR
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stoogebag.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag.Common
{
	public class CameraFade : Singleton<CameraFade> 
	{

		public Image myImage;

		public bool FadeOnStart = true;
	
		[DisableIf("@!FadeOnStart"),SerializeField, Indent] private float startFadeTime = 1;

		private Color startColour;
		

		// Use this for initialization
		void Start () {
			myImage = GetComponent<Image> ();
			startColour = myImage.color;
			if(FadeOnStart) FadeIn(startColour, startFadeTime, null);
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		public async UniTask FadeIn(Color startColour, float fadeTime, Action onFinish = null){
			await FadeInCoroutine(startColour, fadeTime, onFinish);
		}
		
		public async UniTask FadeOut(Color startColour, float fadeTime, Action onFinish = null){
			await FadeOutCoroutine(startColour, fadeTime, onFinish);
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
}
#endif