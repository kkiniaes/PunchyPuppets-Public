using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUtil : MonoBehaviour {

    [SerializeField]
    [EnumsFlags]
    private GameManager.PauseMode pauseRegister = (GameManager.PauseMode)(-1);

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameManager.PauseEvent += HandlePause;
        GameManager.UnpauseEvent += HandleUnpause;
    }
    private void OnDisable()
    {
        GameManager.PauseEvent -= HandlePause;
        GameManager.UnpauseEvent -= HandleUnpause;
    }

    private void HandlePause(GameManager.PauseMode mode)
    {
        if (mode == GameManager.PauseMode.Pause)
        {
            anim.speed = 0;
        }
    }

    private void HandleUnpause(GameManager.PauseMode mode)
    {
        if (mode == GameManager.PauseMode.Pause)
        {
            anim.speed = 1;
        }
    }
}
