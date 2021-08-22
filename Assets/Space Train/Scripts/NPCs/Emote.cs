using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmoteType
{
	Exclaimation,
	Question,
	Angry,
	Happy,
	Sad,
	Laugh,
	Whistle,
}
public class Emote : MonoBehaviour
{
	[Header("-- Emote Materials --")] 
	[SerializeField] private Material exclaimationMaterial;
	[SerializeField] private Material questionMaterial;
	[SerializeField] private Material angryMaterial;
	[SerializeField] private Material happyMaterial;
	[SerializeField] private Material sadMaterial;
	[SerializeField] private Material laughMaterial;
	[SerializeField] private Material whistleMaterial;
	
	[Header("-- Emote Components --")]
	[SerializeField] private MeshRenderer emoteMeshRenderer;

	[Header("-- Emote Positioning --")] [SerializeField]
	private Vector3 position;
	private Vector3 rotation;

	private Material EmoteTypeToMat(EmoteType _emoteType)
	{
		switch(_emoteType)
		{
			case EmoteType.Exclaimation:
				return exclaimationMaterial;
			case EmoteType.Question:
				return questionMaterial;
			case EmoteType.Angry:
				return angryMaterial;
			case EmoteType.Happy:
				return happyMaterial;
			case EmoteType.Sad:
				return sadMaterial;
			case EmoteType.Laugh:
				return laughMaterial;
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
