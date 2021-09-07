using NpcAi;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class Key : MonoBehaviour, IInterractable
{
    // the number of keys that the player has currently collected
    public static int numberOfKeysCollected = 0;
    // the number of keys the level was generated with and the number that the player needs to collect in order to open the end door.
    public static int requiredNumberOfKeys = 0;
    [SerializeField, Tooltip("The transform of the key gameobject")] private Transform keyTransform;
    [SerializeField, Tooltip("How fast the key spins")] private float keyRotateSpeed;

    private void Awake()
    {
        //set the number of keys to 0, so that the number doesn't carry over from the player's last attempt at the level
        numberOfKeysCollected = 0;
        requiredNumberOfKeys = 0;
    }

    private void Start()
    {
        //add one to the required number of keys so that the required number of keys equals the total number of keys after the level has been generated
        requiredNumberOfKeys++;
    }

    /// <summary>
    /// when the player interracts with the key, add to the number of keys collected and destroy the gameobject
    /// </summary>
    public void Interract()
    {
        numberOfKeysCollected++;
        Destroy(gameObject);
    }

    // let the player just pick up the keys by walking over them.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Interract();
        }
    }

    private void FixedUpdate()
    {
        if(keyTransform)
        {
            keyTransform.Rotate(Vector3.up * (keyRotateSpeed * Time.fixedDeltaTime));
        }
    }
}
