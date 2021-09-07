using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	[SerializeField, Tooltip("The player GameObject.")] private GameObject player;
	[SerializeField, Tooltip("The height above the player to set this gameObject's position.")] private float heightAbovePlayer;
	[SerializeField, Tooltip("The rotation to keep.")] private Vector3 cameraRotation;

	private void Update()
	{
		gameObject.transform.position = player.transform.position + Vector3.up * heightAbovePlayer;
		gameObject.transform.rotation = Quaternion.Euler(cameraRotation);
	}
}
