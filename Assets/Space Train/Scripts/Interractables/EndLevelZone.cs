using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelZone : MonoBehaviour
{
	[SerializeField] private int menuSceneNumber;
	[SerializeField] private GameObject endGameScreen;
	private bool gameHasEnded = false;
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(!gameHasEnded)
			{
				gameHasEnded = true;
				StartCoroutine(EndGame());
			}
		}
	}

	private void Start()
	{
		endGameScreen.SetActive(false);
	}

	private IEnumerator EndGame()
	{
		endGameScreen.SetActive(true);
		yield return new WaitForSeconds(5);
		SceneManager.LoadScene(menuSceneNumber);
	}
}
