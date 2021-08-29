using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace SpaceTrain.UI
{
    public enum MenuButtonType
    {
        Start,
        Greybox,
        Return,
        Load_Game,
        Save_Game,
        Options,
        Save_And_Quit,
        Quit
    }

    public class MenuButton : MonoBehaviour
    {
        [SerializeField, Tooltip("The Type of Button This Is")]
        private MenuButtonType buttonType = MenuButtonType.Start;
        
        //The button and button text.
        [SerializeField, HideInInspector]
        private Button button;

        private MenuHandler menuHandler;

        private void OnValidate()
        {

            if (!button)
                button = GetComponent<Button>();
            
            //remove all the listners of the button because that will cause an error
            button.onClick.RemoveAllListeners();
        }

        //i dont think any of this is used
        #region FindParentObjectWithTag()
        GameObject FindParentObjectWithTag(string _tag)
        {
            if (transform.parent == null)
                return null;
            else if (transform.parent.CompareTag(_tag))
                return transform.parent.gameObject;
            else
                return FindParentObjectWithTag(_tag, transform.parent);
        }
        
        GameObject FindParentObjectWithTag(string _tag, Transform _transform)
        {
            if (_transform.parent == null)
                return null;
            else if (_transform.parent.CompareTag(_tag))
                return _transform.parent.gameObject;
            else
                return FindParentObjectWithTag(_tag, _transform.parent);
        }
        #endregion

        private void Start()
        {
            //set the menu handler
            menuHandler = TheMenuHandler.theMenuHandler;
            //set the onclick listener
            button.onClick.AddListener(PerformFunction);
        }

        private void PerformFunction()
        {
            //perform the appropriate function of the button according to the selected buttontype
            switch (buttonType)
            {
                case MenuButtonType.Start:
                    menuHandler.StartGame();
                    break;
                case MenuButtonType.Greybox:
                    menuHandler.StartGreybox();
                    break;
                case MenuButtonType.Return:
                    menuHandler.MenuGoBack();
                    break;
                case MenuButtonType.Load_Game:
                    menuHandler.Load();
                    break;
                case MenuButtonType.Save_Game:
                    menuHandler.Save();
                    break;
                case MenuButtonType.Options:
                    menuHandler.OpenOptionsMenu();
                    break;
                case MenuButtonType.Save_And_Quit:
                    menuHandler.Save();
                    menuHandler.Quit();
                    break;
                case MenuButtonType.Quit:
                    menuHandler.Quit();
                    break;
                default:
                    break;
            }
        }
    }
}