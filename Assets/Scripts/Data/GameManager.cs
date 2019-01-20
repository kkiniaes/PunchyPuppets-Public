using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager> {
    private float deltaTime, fixedDeltaTime;
    public enum PauseMode { None, Pause, Loading, Sleep }

    private Enums.FighterControllerType player1Type = Enums.FighterControllerType.None, player2Type = Enums.FighterControllerType.None;

    private float currentSlowMotionFactor = 1, slowMotionFactor = 8;

    public Enums.GameMode gameMode = Enums.GameMode.Mayhem;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(TimeUpdate());
    }

    private IEnumerator TimeUpdate()
    {
        while (true)
        {
            deltaTime = Time.deltaTime/currentSlowMotionFactor;
            fixedDeltaTime = Time.fixedDeltaTime;
            yield return null;
        }
    }

    private int pauseRegister;
    public int PauseRegister
    {
        get { return pauseRegister; }
    }

    public float DeltaTime(int objRegister)
    {
        return (objRegister & pauseRegister) != 0 ? 0 : deltaTime;
    }

    public float DeltaTime(PauseMode objRegister)
    {
        return DeltaTime((int)objRegister);
    }

    public float FixedDeltaTime(int objRegister)
    {
        return (objRegister & pauseRegister) != 0 ? 0 : fixedDeltaTime;
    }

    public float FixedDeltaTime(PauseMode objRegister)
    {
        return FixedDeltaTime((int)objRegister);
    }

    public delegate void PauseAction(PauseMode mode);
    public static event PauseAction PauseEvent, UnpauseEvent;

    public void Pause(PauseMode mode)
    {
        pauseRegister = Bitwise.SetBit(pauseRegister, (int)mode);
        if (PauseEvent != null) PauseEvent(mode);
    }

    public void Unpause(PauseMode mode)
    {
        pauseRegister = Bitwise.ClearBit(pauseRegister, (int)mode);
        if (UnpauseEvent != null) UnpauseEvent(mode);
    }

    public void Sleep(float seconds)
    {
        StartCoroutine(SleepForSeconds(seconds));
    }

    private IEnumerator SleepForSeconds(float seconds)
    {
        float timer = 0f;
        while(timer <= 0.12f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Pause(PauseMode.Sleep);
        timer = 0f;

        while (timer <= seconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Unpause(PauseMode.Sleep);
    }

    public void ActivateSlowMotion()
    {
        StartCoroutine(SlowMotion());
    }

    private IEnumerator SlowMotion()
    {
        float timer = 0f;
        while(timer < 0.06f)
        {
            timer += Time.deltaTime;
            currentSlowMotionFactor = Mathf.Lerp(currentSlowMotionFactor, slowMotionFactor, Time.deltaTime * 20);
            yield return null;
        }

        timer = 0f;
        currentSlowMotionFactor = slowMotionFactor;

        while(timer <= 2)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < 0.06f)
        {
            timer += Time.deltaTime;
            currentSlowMotionFactor = Mathf.Lerp(currentSlowMotionFactor, 1, Time.deltaTime * 5);
            yield return null;
        }
        currentSlowMotionFactor = 1;
    }

    public Enums.FighterControllerType Player1Type
    {
        get { return player1Type; }
        set { player1Type = value; }
    }

    public Enums.FighterControllerType Player2Type
    {
        get { return player2Type; }
        set { player2Type = value; }
    }

    public float CurrentSlowMotionFactor
    {
        get { return currentSlowMotionFactor; }
    }
}
