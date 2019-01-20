using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Rewired;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    private GameObject titlePanel = null, playerPanel = null, p1Buttons = null, p2Buttons = null, infoPanel = null, creditsPanel;
    [SerializeField]
    private Button p1KeyboardButton = null, p2KeyboardButton = null, p1ControllerButton = null, p2ControllerButton = null;
    [SerializeField]
    private TextMeshProUGUI player1Text = null, player2Text;

    private bool p1UseController = false, p2UseController = false;

    private Enums.FighterControllerType p1Type = Enums.FighterControllerType.Player, p2Type = Enums.FighterControllerType.AI;

    public Animator mainMenuAnimator;

    private IEnumerator Start()
    {
        GameManager.Instance.Player1Type = Enums.FighterControllerType.Player;
        GameManager.Instance.Player2Type = Enums.FighterControllerType.AI;

        buttonColor = p1ControllerButton.transform.GetChild(0).GetComponent<Image>().color;
        ChangeP1ToKeyboard();

        yield return new WaitForSeconds(0.2f);
        
        if (ReInput.controllers.joystickCount > 0)
            p1Buttons.SetActive(true);
    }

    public void StrategyPressed() {
        GameManager.Instance.gameMode = Enums.GameMode.Strategy;
    }

    public void MayhemPressed() {
        GameManager.Instance.gameMode = Enums.GameMode.Mayhem;
    }

    public void PlayPressed()
    {
        titlePanel.SetActive(false);
        playerPanel.SetActive(true);
        mainMenuAnimator.SetBool("GoToIdle", true);
    }

    public void BackToMain() {
        titlePanel.SetActive(true);
        playerPanel.SetActive(false);
        creditsPanel.SetActive(false);
        infoPanel.SetActive(false);
        mainMenuAnimator.SetBool("GoToIdle", true);
    }

    public void CreditsPressed() {
        titlePanel.SetActive(false);
        creditsPanel.SetActive(true);
        mainMenuAnimator.SetBool("GoToIdle", true);
    }

    public void InfoPressed() {
        titlePanel.SetActive(false);
        infoPanel.SetActive(true);
        mainMenuAnimator.SetBool("GoToIdle", true);
    }

    public void Quit() {
        Application.Quit();
    }

    public void PlayGame()
    {
        GameManager.Instance.Player1Type = p1Type;
        GameManager.Instance.Player2Type = p2Type;
        
        if(ReInput.controllers.joystickCount == 0)
        {
            ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Controllers[0], false);
            ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Controllers[0], false);
        }
        else if(ReInput.controllers.joystickCount == 1)
        {
            if(p1Type == Enums.FighterControllerType.Player)
            {
                if (p1UseController)
                {
                    ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                    ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Controllers[0], true);
                }
                else
                {
                    ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Controllers[0], true);
                    if(p2UseController)
                        ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                    else
                        ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Controllers[0], true);
                }
            }
            //P1 is AI
            else
            {
                if (p2UseController)
                    ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                else
                    ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Controllers[0], true);
            }
        }
        else
        {
            // Both are players
            if (p1Type == Enums.FighterControllerType.Player && p2Type == Enums.FighterControllerType.Player)
            {
                // P1 use first controller
                if (p1UseController)
                {
                    ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                    //P2 Use second controller
                    if (p2UseController)
                        ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Joysticks[1], true);
                    //P2 use Keyboard
                    else
                        ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Controllers[0], true);
                }
                // P1 use keyboard
                else
                {
                    ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Controllers[0], true);
                    // P2 use first controller
                    if(p2UseController)
                        ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                    // P2 also wants to use keyboard UH OH
                    //else

                }
            }
            // P2 is AI
            else if(p1Type == Enums.FighterControllerType.Player)
            {
                if (p1UseController)
                    ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                else
                    ReInput.players.GetPlayer(0).controllers.AddController(ReInput.controllers.Controllers[0], true);
            }
            //P1 is AI
            else
            {
                if (p2UseController)
                    ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Joysticks[0], true);
                else
                    ReInput.players.GetPlayer(1).controllers.AddController(ReInput.controllers.Controllers[0], true);
            }
        }
        SceneManager.LoadScene("kartiktest");
    }

    public void Player1Right()
    {
        p1Type = p1Type == Enums.FighterControllerType.Player ? Enums.FighterControllerType.AI : Enums.FighterControllerType.Player;
        player1Text.text = p1Type == Enums.FighterControllerType.Player ? "Player 1" : "CPU";
        p1Buttons.SetActive(p1Type == Enums.FighterControllerType.Player);
        if (ReInput.controllers.joystickCount == 0)
            p1Buttons.SetActive(false);
    }

    public void Player1Left()
    {
        p1Type = p1Type == Enums.FighterControllerType.Player ? Enums.FighterControllerType.AI : Enums.FighterControllerType.Player;
        player1Text.text = p1Type == Enums.FighterControllerType.Player ? "Player 1" : "CPU";
        p1Buttons.SetActive(p1Type == Enums.FighterControllerType.Player);
        if (ReInput.controllers.joystickCount == 0)
            p1Buttons.SetActive(false);
    }

    public void Player2Right()
    {
        p2Type = p2Type == Enums.FighterControllerType.Player ? Enums.FighterControllerType.AI : Enums.FighterControllerType.Player;
        player2Text.text = p2Type == Enums.FighterControllerType.Player ? "Player 2" : "CPU";
        p2Buttons.SetActive(p2Type == Enums.FighterControllerType.Player);
        if (ReInput.controllers.joystickCount == 0)
            p2Buttons.SetActive(false);
    }

    public void Player2Left()
    {
        p2Type = p2Type == Enums.FighterControllerType.Player ? Enums.FighterControllerType.AI : Enums.FighterControllerType.Player;
        player2Text.text = p2Type == Enums.FighterControllerType.Player ? "Player 2" : "CPU";
        p2Buttons.SetActive(p2Type == Enums.FighterControllerType.Player);
        if (ReInput.controllers.joystickCount == 0)
            p2Buttons.SetActive(false);
    }


    private Color buttonColor;
    public void ChangeP1ToKeyboard()
    {
        p1KeyboardButton.transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        p1ControllerButton.interactable = true;
        p1KeyboardButton.interactable = false;
        p1ControllerButton.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
        p1UseController = false;
    }

    public void ChangeP1ToController()
    {
        p1KeyboardButton.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
        p1ControllerButton.interactable = false;
        p1KeyboardButton.interactable = true;
        p1ControllerButton.transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        p1UseController = true;
    }

    public void ChangeP2ToKeyboard()
    {
        p2KeyboardButton.transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        p2ControllerButton.interactable = false;
        p2KeyboardButton.interactable = true;
        p2ControllerButton.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
        p2UseController = false;
    }

    public void ChangeP2ToController()
    {
        p2KeyboardButton.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
        p2ControllerButton.interactable = true;
        p2KeyboardButton.interactable = false;
        p2ControllerButton.transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        p2UseController = true;
    }
}
