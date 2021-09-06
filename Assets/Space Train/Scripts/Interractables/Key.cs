using NpcAi;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class Key : MonoBehaviour, IInterractable
{
    public static int numberOfKeysCollected = 0;
    public static int requiredNumberOfKeys = 0;
    [SerializeField] private Transform keyTransform;
    [SerializeField] private float keyRotateSpeed;

    private void Awake()
    {
        numberOfKeysCollected = 0;
        requiredNumberOfKeys = 0;
    }

    private void Start()
    {
        requiredNumberOfKeys++;
    }

    public void Interract()
    {
        numberOfKeysCollected++;
        Destroy(gameObject);
    }

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
