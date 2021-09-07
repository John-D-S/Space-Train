using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelZone : MonoBehaviour
{
	[SerializeField, Tooltip("The Build index of the menu scene")] private int menuSceneNumber;
	[SerializeField, Tooltip("The UI element Screen that pops up when the game finishes")] private GameObject endGameScreen;
	private bool gameHasEnded = false;
	
	// When the player walks into the trigger, show the screen that says the game has ended, then end it by going back to the main menu after 5 seconds.
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
