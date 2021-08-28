using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceTrain.Player
{
    /// <summary> This will hold all the Identity Information.</summary>
    public class IdentityHandler : MonoBehaviour
    {
	    // OK THIS IS SO ANNOYING WITHOUT ODIN AHHHHH!!!
	    // FFS HERE YOU GO
	    
	    // This is all the current player values.
	    [Header("Current Player Values")]
	    // This is the current Player Identity.
	    public PlayerIdentity currentPlayerIdentity;
	    // This is the current player material.
	    public Material currentPlayerMaterial;
	    // This is the UI of the current selected Identity.
	    public GameObject selectedIdentiyUI;

	    [Header("None Identity")]
	    // The Material of the none identity.
	    public Material nonePlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject noneCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject noneSelectedIdentiyUI;

	    private void Awake()
	    {
		    // Basically Change the player to none identity.
		    ChangeToNoneIdentity();
		    // Select No Idenetity.
		    SelectNoIdentity();
	    }


	    #region MyRegion

		    // Select No Identity.
		    private void SelectNoIdentity()
		    {
			    noneSelectedIdentiyUI.gameObject.SetActive(true);
		    }


	    #endregion

	    #region Player Identities

		    // This is for when you change to no identity.
		    public void ChangeToNoneIdentity()
		    {
			    // Make the player None Identitity.
			    currentPlayerIdentity = PlayerIdentity.None;
			    // Make the players current material = none;
			    currentPlayerMaterial = nonePlayerMaterial;
			    // Make the UI of the Selected Material = On;
			    noneCurrentIdentityUI.gameObject.SetActive(true);
		    }
		    
	    #endregion
    }
    
    public enum PlayerIdentity
    {
	    None, Guard, Citizen, Worker
    }
    
    // ODIN NOT HERE!!
    // UNITY GO BRR
     /// <summary> This will be a class to hold all the different Player Identites.</summary>
     public class PlayerIdentitys
     {
	     [Header("Identity Name")]
	     // This is the name of the Identity.
	     public PlayerIdentity playerIdentityName;
	     [Header("Identity Material")]
	     // The Material of this identity.
	     public Material identityMaterial;
	     // This will be the UI Game Object for the identity when it is selected.
	     [Header("Selected Identity")]
	     public GameObject selectedIdentiyUI;
	     public bool isSelectedIdentity;
	     // Is this the UI game object of the current identity of the player; 
	     [Header("Current Identity")]
	     public GameObject currentIdentityUI;
	     public bool isCurrentIdentity;
     }
}