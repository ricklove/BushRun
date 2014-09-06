using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;
    
    void Awake()
    {
        Instance = this;
    }

    public void ReturnFromMenu(MenuState fromState)
    {
        if (fromState == MenuState.CharacterSelection)
        {
            transform.FindChild("CharacterSelection").gameObject.SetActive(false);
            
            // Change to game scene
            var game = transform.parent.FindChild("Game").gameObject;
            game.SetActive(true);
        }
    }

}

public enum MenuState
{
    CharacterSelection,
    Game,
}