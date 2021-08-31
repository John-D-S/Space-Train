using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Audio;

namespace SpaceTrain.UI
{
    /// <summary> Holds the Static menuHandler. </summary>
    public static class TheMenuHandler
    {
        public static MenuHandler theMenuHandler;
    }

    public class MenuHandler : MonoBehaviour
    {
        [Header("-- Scene Name Variables --")]
        [SerializeField, Tooltip("The name of the Menu Scene")]
        private string menuSceneName = "Menu";
        [SerializeField, Tooltip("The name of the Game Scene")]
        private string gameSceneName = "Main";
        [SerializeField, Tooltip("The name of the Greybox Scene")]
        private string greyboxSceneName = "Greybox";

        [Header("-- Resolution Settings --")]
        [Tooltip("the resolution dropdown in the options menu")]
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [Header("-- Audio --")]
        [Tooltip("The master audio mixer")]
        public AudioMixer masterMixer;

        [Header("-- Menu Objects --")]
        [SerializeField, Tooltip("The options menu.")]
        private GameObject OptionsMenu;
        [SerializeField, Tooltip("The pause/main menu that is not the options menu")]
        private GameObject HomeMenu;

        #region Pausing
        private bool paused = false;
        public bool Paused
        {
            get
            {
                return paused;
            }
        }
        
        /// <summary>
        /// pauses the game
        /// </summary>
        public void Pause()
        {
            Time.timeScale = 0;
            if (HomeMenu)
                HomeMenu.SetActive(true);
            paused = true;
        }

        /// <summary>
        /// unpauses the game
        /// </summary>
        public void Unpause()
        {
            paused = false;
            if (HomeMenu)
                HomeMenu.SetActive(false);
            Time.timeScale = 1;
        }

        /// <summary>
        /// if the options menu is open, go back to the menu, if not exit that menu
        /// </summary>
        public void MenuGoBack()
        {
            EventSystem.current.SetSelectedGameObject(null);
            if (OptionsMenu.activeInHierarchy)
                CloseOptionsMenu();
            else
            {
                if (!paused)
                    Pause();
                else
                    Unpause();
            }
        }
        #endregion

        #region Options
        /// <summary>
        /// toggles the options menu being activeInHierarchy
        /// </summary>
        public void ToggleOptionsMenu()
        {
            if (HomeMenu.activeInHierarchy && !OptionsMenu.activeInHierarchy)
            {
                OpenOptionsMenu();
            }
            else if (!HomeMenu.activeInHierarchy && OptionsMenu.activeInHierarchy)
            {
                CloseOptionsMenu();
            }
        }

        /// <summary>
        /// sets the options menu as active and the home menu as inactive
        /// </summary>
        public void OpenOptionsMenu()
        {
            if (OptionsMenu && HomeMenu)
            {
                OptionsMenu.SetActive(true);
                HomeMenu.SetActive(false);
            }
        }

        /// <summary>
        /// sets the options menu to be inactive and the home menu to be active
        /// </summary>
        public void CloseOptionsMenu()
        {
            if (OptionsMenu && HomeMenu)
            {
                OptionsMenu.SetActive(false);
                HomeMenu.SetActive(true);
            }
        }

        #region Volume
        public void ChangeSFXVolume(float _volume)
        {
            if (masterMixer)
            {
                masterMixer.SetFloat("SFXVolume", _volume);
                PlayerPrefs.SetFloat("SFXVolume", _volume);
                PlayerPrefs.Save();
            }
        }

        public void ChangeMusicVolume(float _volume)
        {
            if (masterMixer)
            {
                masterMixer.SetFloat("MusicVolume", _volume);
                PlayerPrefs.SetFloat("MusicVolume", _volume);
                PlayerPrefs.Save();
            }
        }

        public void SetMute(bool isMuted)
        {
            if (masterMixer)
            {
                if (isMuted)
                    masterMixer.SetFloat("MasterVolume", -80f);
                else
                    masterMixer.SetFloat("MasterVolume", 0f);
                PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        #endregion

        #region Resolution
        public void SetResolution(int ResolutionIndex)
        {
            Resolution res = resolutions[ResolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        #endregion

        #endregion

        #region SceneSwitching
        /// <summary>
        /// Loads the game scene
        /// </summary>
        public void StartGame() => SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        /// <summary>
        /// Loads the game scene
        /// </summary>
        public void StartGreybox() => SceneManager.LoadScene(greyboxSceneName, LoadSceneMode.Single);
        /// <summary>
        /// Loads the main menu scene
        /// </summary>
        public void ReturnToMainMenu() => SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
        #endregion

        #region Initialization
        /// <summary>
        /// Sets all the options in the resolutions Dropdown and sets te function.
        /// </summary>
        private void InitializeResolutions()
        {
            // set the list of resolutions to all the possible resolutions
            resolutions = Screen.resolutions;
            // clear the options in the dropdown
            resolutionDropdown.ClearOptions();
            // initialise the resolution options as a new list of strings
            List<string> resolutionOptions = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                resolutionOptions.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            // add all the resolutions in the resolution Options list
            resolutionDropdown.AddOptions(resolutionOptions);
            //set the currently sellected resolution to the current resolution of the screen
            resolutionDropdown.value = currentResolutionIndex;
            //refresh the shown value so that it displays correctly
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// initializes all the volume variables from playerprefs
        /// </summary>
        private void InitializeVolume()
        {
            if (PlayerPrefs.HasKey("MusicVolume"))
                ChangeMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
            if (PlayerPrefs.HasKey("SFXVolume"))
                ChangeSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
            if (PlayerPrefs.HasKey("IsMuted"))
                SetMute(PlayerPrefs.GetInt("IsMuted") == 1);
        }
        #endregion

        #region Saving
        /// <summary>
        /// doesn't work
        /// </summary>
        public void Save()
        {
            Debug.Log("Not Yet Implemented");
        }

        /// <summary>
        /// doesn't work
        /// </summary>
        public void Load()
        {
            Debug.Log("Not Yet Implemented");
        }
        #endregion

        /// <summary>
        /// if in the game scene, set the timescale to 1 and go to the menu scene
        /// if in the menu scene, quit the game
        /// </summary>
        public void Quit()
        {
            Time.timeScale = 1;
            if (gameObject.scene.name == gameSceneName)
                ReturnToMainMenu();
            else
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                MenuGoBack();
        }

        private void Start()
        {
            InitializeVolume();
        }

        private void Awake()
        {
            InitializeResolutions();
            TheMenuHandler.theMenuHandler = this;
        }
    }
}
