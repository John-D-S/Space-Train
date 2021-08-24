using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Willow
{
    /// <summary>
    /// Attach to the Camera and then drag and drop Willow.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraScriptFollowWillow : MonoBehaviour
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
}

