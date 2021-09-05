using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
	public enum EmoteType
	{
		Exclaimation,
		Question,
		Talk,
		Listen,
		Whistle
	}
	public class Emote : MonoBehaviour
	{
		[Header("-- Emote Materials --")] 
		[SerializeField] private Material exclaimationMaterial;
		[SerializeField] private Material questionMaterial;
		[SerializeField] private Material talkMaterial;
		[SerializeField] private Material listenMaterial;
		[SerializeField] private Material whistleMaterial;
		
		[Header("-- Emote Components --")]
		[SerializeField] private MeshRenderer emoteMeshRenderer;

		[Header("-- Emote Positioning --")] 
		[SerializeField] private Vector3 position;
		[SerializeField] private Vector3 rotation;

		private Material EmoteTypeToMat(EmoteType _emoteType)
		{
			switch(_emoteType)
			{
				case EmoteType.Exclaimation:
					return exclaimationMaterial;
				case EmoteType.Question:
					return questionMaterial;
				case EmoteType.Talk:
					return talkMaterial;
				case EmoteType.Listen:
					return listenMaterial;
				case EmoteType.Whistle:
					return whistleMaterial;
			}
			return null;
		}
		
		public void ShowEmote(EmoteType _emoteType)
		{
			emoteMeshRenderer.enabled = true;
			emoteMeshRenderer.material = EmoteTypeToMat(_emoteType);
		}
		
		public void ShowEmote(EmoteType _emoteType, float _secondsDisplayed)
		{
			Debug.Log("triedToShowEmote");
			StopAllCoroutines();
			StartCoroutine(ShowEmoteForTime(_emoteType, _secondsDisplayed));
		}
		
		public void HideEmote()
		{
			emoteMeshRenderer.enabled = false;
		}
		
		private IEnumerator ShowEmoteForTime(EmoteType _emoteType, float _secondsDisplayed)
		{
			ShowEmote(_emoteType);
			yield return new WaitForSeconds(_secondsDisplayed);
			HideEmote();
		}

		private void Start()
		{
			HideEmote();
		}

		private void Update()
		{
			if(emoteMeshRenderer.enabled && transform.parent != null)
			{
				transform.SetPositionAndRotation(transform.parent.position + position, Quaternion.Euler(rotation));
			}
		}
	}
}
