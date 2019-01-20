using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour {
    protected Fighter fighter;
    protected bool canRecordInput, executingInput, recordedAlready;
    protected bool canPerformInput = true;

    protected float recordingInterval = 0.5f;

    protected List<Enums.FightCommands> fightCommands;
    protected float commandWaitTime = 0.25f;

    private float commandIntervalTimer = 0f;

    protected bool fakeHeld = false;

    [EnumsFlags]
    public GameManager.PauseMode pauseRegister = (GameManager.PauseMode)(-1);

    protected int playerIndex = 0;

    private void Start()
    {
        fighter = GetComponent<Fighter>();
        fighter.Controller = this;
        fightCommands = new List<Enums.FightCommands>();

        if (GameManager.Instance.gameMode == Enums.GameMode.Mayhem)
            recordingInterval = 0.5f;
        else if(GameManager.Instance.gameMode == Enums.GameMode.Strategy)
            recordingInterval = 2f;

        //LevelManager.Instance.AddFighterController(this);
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
        //if(mode == GameManager.PauseMode.Sleep)
        //{
            fighter.Animator.speed = 0;
        //}
    }

    private void HandleUnpause(GameManager.PauseMode mode)
    {
        //if (mode == GameManager.PauseMode.Sleep)
        //{
            fighter.Animator.speed = 1;
        //}
    }

    protected virtual void Update()
    {
        if (canRecordInput)
        {

            if (GameManager.Instance.gameMode == Enums.GameMode.Mayhem)
            {
                commandIntervalTimer += GameManager.Instance.DeltaTime(pauseRegister);
                UIManager.Instance.UpdatePlayerInputDelay(playerIndex, commandIntervalTimer / recordingInterval);
                if (commandIntervalTimer >= recordingInterval)
                {
                    commandIntervalTimer = 0f;
                    if (!recordedAlready) CheckCommand(Enums.FightCommands.None);
                    recordedAlready = false;
                    SoundEffectManager.Instance.PlayCountdown();
                }
            }
        }
    }

    public void EndRecordingSegmentEarly()
    {
        recordedAlready = false;
        if (fightCommands.Count > 0) UIManager.Instance.SetCommandSprite(playerIndex, fightCommands[fightCommands.Count - 1]);
        SoundEffectManager.Instance.PlayCountdown();
    }

    public virtual void EndRecordingInput()
    {
        canRecordInput = false;
    }

    public virtual void ExecuteCommands()
    {
        StartCoroutine(ExecuteFightingCommands());
    }

    protected IEnumerator ExecuteFightingCommands()
    {
        executingInput = true;
        for (int i = 0; i < fightCommands.Count; i++)
        {
            LevelManager.Instance.NextInput(fightCommands[i], playerIndex);

            float timer = 0f;
            while(timer < commandWaitTime)
            {
                timer += GameManager.Instance.DeltaTime(pauseRegister);
                yield return null;
            }
        }
        executingInput = false;
        fightCommands.Clear();
    }
    
    protected void CheckCommand(Enums.FightCommands command)
    {
        if (fakeHeld)
        {
            fightCommands.Add(Enums.FightCommands.Fake);
            //Debug.Log("Fake has been recorded");
        }
        else
        {
            fightCommands.Add(command);
            // Debug.Log(command.ToString() + " has been recorded");
        }
        if(GameManager.Instance.gameMode == Enums.GameMode.Mayhem) UIManager.Instance.SetCommandSprite(playerIndex, command);
        recordedAlready = true;
    }

    public void ForceEndRound()
    {
        StopAllCoroutines();
    }

    public void StartRecordingInput()
    {
        canRecordInput = true;
    }

    public bool CanRecordInput
    {
        get { return canRecordInput; }
    }

    public int PlayerIndex
    {
        get { return playerIndex; }
        set { playerIndex = value; }
    }

    public Fighter Fighter
    {
        get { return fighter; }
    }

    public bool CanPerformInput
    {
        get { return canPerformInput; }
    }

    public bool RecordedAlready
    {
        get { return recordedAlready; }
    }

    public int CommandsCount
    {
        get { return fightCommands.Count; }
    }
}
