using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace SpaceTrain.UI
{
    public enum MenuToggleType
    {
        Fullscreen,
        Mute
    }

    public class MenuToggle : MonoBehaviour
    {
        [SerializeField] private MenuToggleType toggleType = MenuToggleType.Fullscreen;
        //The toggle and Toggle text
        [SerializeField, HideInInspector] private Toggle toggle;
        [SerializeField, HideInInspector] private TextMeshProUGUI toggleText;

        private MenuHandler menuHandler;

        private void OnValidate()
        {
            if(!toggle)
                toggle = GetComponent<Toggle>();

            if(!toggleText)
                toggleText = GetComponentInChildren<TextMeshProUGUI>();

            //set the text of the toggle to be the selected menuToggleTypeText
            toggleText.text = toggleType.ToString().Replace("_", " ");
            //remove all the listeners to avoid errors
            toggle.onValueChanged.RemoveAllListeners();
        }

        private void Start()
        {
            //Set the menu handler
            menuHandler = TheMenuHandler.theMenuHandler;

            //set the current toggle from the playerSettings
            switch(toggleType)
            {
                case MenuToggleType.Fullscreen:
                    if(PlayerPrefs.HasKey("isFullScreen"))
                        toggle.isOn = PlayerPrefs.GetInt("IsFullScreen") == 1;
                    else
                        toggle.isOn = false;

                    break;
                case MenuToggleType.Mute:
                    if(PlayerPrefs.HasKey("isMuted"))
                        toggle.isOn = PlayerPrefs.GetInt("IsMuted") == 1;
                    else
                        toggle.isOn = false;

                    break;
                default:
                    break;
            }

            toggle.onValueChanged.RemoveAllListeners();
            //set the onValuechanged listner
            toggle.onValueChanged.AddListener(delegate { PerformFunction(toggle.isOn); });
        }

        private void PerformFunction(bool _value)
        {
            //perform the appropriate function of the toggle according to the selected toggleType
            switch(toggleType)
            {
                case MenuToggleType.Fullscreen:
                    menuHandler.SetFullscreen(_value);
                    break;
                case MenuToggleType.Mute:
                    menuHandler.SetMute(_value);
                    break;
                default:
                    break;
            }
        }
    }
}