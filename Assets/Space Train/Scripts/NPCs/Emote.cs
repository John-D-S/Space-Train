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
		[SerializeField, Tooltip("The exclaimation material of the emote.")] private Material exclaimationMaterial;
		[SerializeField, Tooltip("The question material of the emote.")] private Material questionMaterial;
		[SerializeField, Tooltip("The talk material of the emote.")] private Material talkMaterial;
		[SerializeField, Tooltip("The listen material of the emote.")] private Material listenMaterial;
		[SerializeField, Tooltip("The whistle material of the emote.")] private Material whistleMaterial;
		
		[Header("-- Emote Components --")]
		[SerializeField, Tooltip("The meshrenderer that displays the emote material.")] private MeshRenderer emoteMeshRenderer;

		[Header("-- Emote Positioning --")] 
		[SerializeField, Tooltip("The target position of the emote gameobject relative to this gameobject.")] private Vector3 position;
		[SerializeField, Tooltip("The target rotation of the emote gameobject relative to this gameobject.")] private Vector3 rotation;

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
