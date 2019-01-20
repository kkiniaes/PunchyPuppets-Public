using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;

public class PauseManager : SingletonMonobehaviour<PauseManager> {

    private bool pause;
	void Update () {
        if (ReInput.players.GetPlayer(0).GetButtonDown("Pause") || ReInput.players.GetPlayer(1).GetButtonDown("Pause"))
        {
            //pause = !pause;
            //if (pause) GameManager.Instance.Pause(GameManager.PauseMode.Pause);
            //else GameManager.Instance.Unpause(GameManager.PauseMode.Pause);
            SceneManager.LoadScene(0);
        }
	}

    protected override bool Persistent
    {
        get
        {
            return false;
        }
    }
}
