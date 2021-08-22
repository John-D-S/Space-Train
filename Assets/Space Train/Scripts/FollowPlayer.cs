using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private float heightAbovePlayer;
	[SerializeField] private Vector3 cameraRotation;

	private void Update()
	{
		gameObject.transform.position = player.transform.position + Vector3.up * heightAbovePlayer;
		gameObject.transform.rotation = Quaternion.Euler(cameraRotation);
	}
}
