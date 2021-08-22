using System;
using UnityEngine;

namespace TDLN.CameraControllers
{
    public class OrbitCamera : MonoBehaviour
    {
        // The player to follow.
        public GameObject player;

        // Distace from the camera
        public float distance = 10.0f;

        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20;
        public float yMaxLimit = 80;

        float x = 0.0f;
        float y = 0.0f;

        void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        float prevDistance;

        // Done in Late update since its following.
        void LateUpdate()
        {
            if(distance < 2)
            {
                distance = 2;
            }

            distance -= Input.GetAxis("Mouse ScrollWheel") * 2;

            Vector3 pos = Input.mousePosition;
            
            // This was changed from a previous script I used to lock the mouse to the player.


            // Comment out these two lines if you don't want to hide mouse curser Kieran.
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

        static float ClampAngle(float angle, float min, float max)
        {
            if(angle < -360)
                angle += 360;
            if(angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}