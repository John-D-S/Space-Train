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

    private void Start()
    {
        requiredNumberOfKeys++;
    }

    public void Interract()
    {
        numberOfKeysCollected++;
        Destroy(gameObject);
    }
}
