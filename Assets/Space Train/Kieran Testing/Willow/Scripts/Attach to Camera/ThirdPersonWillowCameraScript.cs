using System;
using UnityEngine;

namespace Willow
{
    /// <summary>
    /// Attach to the Camera and then drag and drop Willow.
    /// This will give a Third person controller if WillowInputHandler,
    /// and WillowTopDownCharacterMover are attached to the Player
    /// </summary>
    [RequireComponent(typeof(Camera))]
    
    public class ThirdPersonWillowCameraScript : MonoBehaviour
    {
        /// <summary>
        /// The Object on the player it will follow.
        /// In this case best to have an Vector3 object in the Player hirarchy to folllow.
        /// </summary>
        public GameObject player;

        /// <summary>
        /// Distace from the Seclected GameObject Above.
        /// </summary>
        public float distance = 10.0f;

        /// <summary>
        /// The X Speed to follow the Player.
        /// </summary>
        public float xSpeed = 250.0f;
        
        /// <summary>
        /// The Y Speed to follow the Player.
        /// </summary>
        public float ySpeed = 120.0f;

        /// <summary>
        /// Min distance before the Camera follows.
        /// </summary>
        public float yMinLimit = -20;
        
        /// <summary>
        /// Max distance before the Camera follows.
        /// </summary>
        public float yMaxLimit = 80;


        private float x = 0.0f;
        private float y = 0.0f;

        private float prevDistance;
        
        /// <summary>
        /// Gets the Camera Angles and Save it in X and Y.
        /// </summary>
        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }


        /// <summary>
        /// Done in Late update since its following.
        /// </summary>
        private void LateUpdate()
        {
            if(distance < 2)
            {
                distance = 2;
            }

            distance -= Input.GetAxis("Mouse ScrollWheel") * 2;

            Vector3 pos = Input.mousePosition;
            
            // Comment out these two lines if you don't want to hide mouse curser.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + player.transform.position;
            transform.rotation = rotation;
            transform.position = position;


            if(Math.Abs(prevDistance - distance) > 0.001f)
            {
                prevDistance = distance;
                Quaternion rot = Quaternion.Euler(y, x, 0);
                Vector3 po = rot * new Vector3(0.0f, 0.0f, -distance) + player.transform.position;
                transform.rotation = rot;
                transform.position = po;
            }
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if(angle < -360)
            {
                angle += 360;
            }

            if(angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}