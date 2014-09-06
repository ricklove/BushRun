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
            transform.FindChild("LevelSelection").gameObject.SetActive(true);
        }
        else if (fromState == MenuState.LevelSelection)
        {
            transform.FindChild("LevelSelection").gameObject.SetActive(false);
            // Change to game scene
            var game = transform.parent.FindChild("Game").gameObject;
            game.SetActive(true);
            SubjectController.Instance.GoStartOfLevel();
        }
    }

    public void ReturnFromGame()
    {
        transform.FindChild("CharacterSelection").gameObject.SetActive(true);
        
        var game = transform.parent.FindChild("Game").gameObject;
        game.SetActive(false);

        // Move to camera
        transform.position = new Vector3(Camera.main.transform.position.x, 0, 0);
    }
}

public enum MenuState
{
    CharacterSelection,
    LevelSelection,
    Game,
}