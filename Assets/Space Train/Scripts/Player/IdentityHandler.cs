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

	    // The renderer of the player.
	    public Renderer myPlayerRenderer;
	    
	    // This is all the current player values.
	    [Header("Current Player Values")]
	    // This is the current Player Identity.
	    public PlayerIdentity currentPlayerIdentity;
	    // This is the current player material.
	    public Texture2D currentPlayerMaterial;
	    // This is the UI of the current selected Identity.
	    public GameObject selectedIdentiyUI;

	    [Header("None Identity")]
	    // The Material of the none identity.
	    public Texture2D nonePlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject noneCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject noneSelectedIdentiyUI;
	    
	    [Header("Passenger Identity")]
	    // The Material of the Passenger identity.
	    public Texture2D passengerPlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject passengerCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject passengerSelectedIdentiyUI;
	    
	    [Header("Worker Identity")]
	    // The Material of the Worker identity.
	    public Texture2D workerPlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject workerCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject workerSelectedIdentiyUI;
	    
	    [Header("Guard Identity")]
	    // The Material of the Guard identity.
	    public Texture2D guardPlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject guardCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject guardSelectedIdentiyUI;
	    
	    private void Awake()
	    {
		    // Basically Change the player to none identity.
		    ChangeToNoneIdentity();
		    // Select No Idenetity.
		    SelectingNoIdentity();
	    }


	    #region Selecting Identities

		    // Select No Identity.
		    private void SelectingNoIdentity()
		    {
			    noneSelectedIdentiyUI.gameObject.SetActive(true);
			    passengerSelectedIdentiyUI.gameObject.SetActive(false);
			    guardSelectedIdentiyUI.gameObject.SetActive(false);
			    workerSelectedIdentiyUI.gameObject.SetActive(false);
		    }
		    
		    // Select Citizen Identity.
		    private void SelectingCitizenIdentity()
		    {
			    noneSelectedIdentiyUI.gameObject.SetActive(false);
			    passengerSelectedIdentiyUI.gameObject.SetActive(true);
			    guardSelectedIdentiyUI.gameObject.SetActive(false);
			    workerSelectedIdentiyUI.gameObject.SetActive(false);
		    }
		    
		    // Select Guard Identity.
		    private void SelectingGuardIdentity()
		    {
			    noneSelectedIdentiyUI.gameObject.SetActive(false);
			    passengerSelectedIdentiyUI.gameObject.SetActive(false);
			    guardSelectedIdentiyUI.gameObject.SetActive(true);
			    workerSelectedIdentiyUI.gameObject.SetActive(false);
		    }
		    
		    // Select Worker Identity.
		    private void SelectingWorkerIdentity()
		    {
			    noneSelectedIdentiyUI.gameObject.SetActive(false);
			    passengerSelectedIdentiyUI.gameObject.SetActive(false);
			    guardSelectedIdentiyUI.gameObject.SetActive(false);
			    workerSelectedIdentiyUI.gameObject.SetActive(true);
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
		    
		    // This is for when you change to no identity.
		    public void ChangeToGuardIdentity()
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