using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMonobehaviour<LevelManager> {
    private float slotDistance = 5;
    public const int NUM_SLOTS = 6;

    private int inputDuration = 10;
    private float roundWaitTime = 2f;
    private float battleTime = 5f;
    private float knockoutTime = 5f;
    private float roundWaitTimer = 0f, recordingTimer = 0f, battleTimer = 0f, knockoutTimer = 0f, fightAnimTimer = 0f;

    private int player1Score, player2Score;

    private GameManager.PauseMode pauseRegister = (GameManager.PauseMode)(-1);

    private List<FighterController> fighterControllers = new List<FighterController>();

    [SerializeField]
    private Enums.FighterControllerType[] defaultFighters =
        new Enums.FighterControllerType[] { Enums.FighterControllerType.Player, Enums.FighterControllerType.AI };

    private Enums.FighterControllerType[] fighters;

    private Enums.FightCommands p1Command = Enums.FightCommands.NULL,
                                p2Command = Enums.FightCommands.NULL;

    private FSM fsm;
    private State SetupState, WaitState, RecordState, BattleState, KnockoutState, FightAnimState;

    private GameObject[] fighterGameObjects = new GameObject[2];

    private void Start()
    {
        SetupState = new State(SetupRun, SetupEnter, null);
        WaitState = new State(WaitRun, WaitEnter, WaitExit);
        RecordState = new State(RecordRun, RecordEnter, RecordExit);
        BattleState = new State(BattleRun, BattleEnter, BattleExit);
        KnockoutState = new State(KnockoutRun, KnockoutEnter, KnockoutExit);
        FightAnimState = new State(FightAnimRun, FightAnimEnter, FightAnimExit);

        fsm = new FSM(new State[]{ SetupState, WaitState, RecordState, BattleState, FightAnimState });

        if (GameManager.Instance.gameMode == Enums.GameMode.Mayhem)
            inputDuration = 10;
        else if (GameManager.Instance.gameMode == Enums.GameMode.Strategy)
            inputDuration = 40;

        /*
        for (int i = 0; i < NUM_SLOTS; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            g.transform.position = new Vector3((i * Instance.SlotDistance) - (NUM_SLOTS / 2 * Instance.SlotDistance) + (SlotDistance / 2), -1, 5);
        }
        */
    }
    
    private void Update()
    {
        fsm.Run();
    }

    #region Setup State
    private void SetupEnter()
    {
        SetupFighters();
    }
    private void SetupRun()
    {
        fsm.ChangeState(WaitState);
    }
    #endregion

    #region Wait State
    private void WaitEnter()
    {
        roundWaitTimer = 0f;
    }
    private void WaitRun()
    {
        roundWaitTimer += GameManager.Instance.DeltaTime(pauseRegister);
        if (roundWaitTimer > roundWaitTime)
        {
            UIManager.Instance.ShowUI();
            fsm.ChangeState(RecordState);
        }
    }
    private void WaitExit()
    {
        roundWaitTimer = 0f;
    }
    #endregion

    #region Record State
    private void RecordEnter()
    {
        recordingTimer = 0f;
        StartRoundInput();
    }
    private void RecordRun()
    {
        recordingTimer += GameManager.Instance.DeltaTime(pauseRegister);
        if (GameManager.Instance.gameMode == Enums.GameMode.Mayhem)
        {
            UIManager.Instance.UpdateOverallTimer(recordingTimer / inputDuration);
            if (recordingTimer > inputDuration)
            {
                UIManager.Instance.HideUI();
                UIManager.Instance.PlayFightAnim();
                fsm.ChangeState(FightAnimState);
                SoundEffectManager.Instance.PlayRoundBegin();
            }
        }
        else if (GameManager.Instance.gameMode == Enums.GameMode.Strategy)
        {
            if (fighterControllers[0].RecordedAlready && fighterControllers[1].RecordedAlready)
            {
                fighterControllers[0].EndRecordingSegmentEarly();
                fighterControllers[1].EndRecordingSegmentEarly();
            }
            UIManager.Instance.UpdateOverallTimer(fighterControllers[0].CommandsCount / 20f);
            if(fighterControllers[0].CommandsCount == 20)
            {
                UIManager.Instance.HideUI();
                UIManager.Instance.PlayFightAnim();
                SoundEffectManager.Instance.PlayRoundBegin();
                fsm.ChangeState(FightAnimState);
            }
        }
    }

    private void RecordExit()
    {
        recordingTimer = 0f;
    }
    #endregion

    #region Fight Anim State

    private void FightAnimEnter()
    {
        fightAnimTimer = 0f;
        EndRoundInput();
    }
    private void FightAnimRun()
    {
        fightAnimTimer += GameManager.Instance.DeltaTime(pauseRegister);
        if (fightAnimTimer > 2f)
        {
            fsm.ChangeState(BattleState);
        }
    }
    private void FightAnimExit()
    {

    }
    #endregion

    #region Battle State
    private void BattleEnter()
    {
        battleTimer = 0f;
        Debug.Log("<color=#ff0000ff>battling</color>");
        PlayersExecuteCommands();
    }
    private void BattleRun()
    {
        battleTimer += GameManager.Instance.DeltaTime(pauseRegister);
        if(battleTimer > battleTime)
        {
            fsm.ChangeState(WaitState);
        }
    }
    private void BattleExit()
    {
        battleTimer = 0f;
    }
    #endregion

    #region Knockout State
    private void KnockoutEnter()
    {
        Debug.Log("<color=#FFFF00>A player was knocked out. Resetting in 5...</color>");
        knockoutTimer = 0f;
    }
    private void KnockoutRun()
    {
        knockoutTimer += GameManager.Instance.DeltaTime(pauseRegister);
        if (knockoutTimer > knockoutTime)
        {
            fsm.ChangeState(SetupState);
        }
    }
    private void KnockoutExit()
    {
        for(int i = 0; i < fighterGameObjects.Length; i++)
        {
            DestroyImmediate(fighterGameObjects[i]);
        }
    }
    #endregion

    private void StartRoundInput()
    {
        Debug.Log("<color=#ffff00ff>Recording Input Now</color>");
        for (int i = 0; i < fighterControllers.Count; i++)
        {
            fighterControllers[i].StartRecordingInput();
        }
    }

    private void EndRoundInput()
    {
        Debug.Log("<color=#ff00ffff>Stopping Recording Now</color>");
        for (int i = 0; i < fighterControllers.Count; i++)
        {
            fighterControllers[i].EndRecordingInput();
        }
    }

    private void PlayersExecuteCommands()
    {
        for (int i = 0; i < fighterControllers.Count; i++)
        {
            fighterControllers[i].ExecuteCommands();
        }
    }

    public void NextInput(Enums.FightCommands command, int player)
    {
        if(player == 0)
        {
            p1Command = command;
        }
        else
        {
            p2Command = command;
            ExecuteCommands();
        }
    }

    private void P1PunchP2()
    {
        if (F1.Slot - F0.Slot <= 1)
        {
            if (p2Command == Enums.FightCommands.Block)
            {
                if (F1.Crouched && !F0.Crouched)
                {
                    F1.Block(false);
                    F0.LightAttack(false, false);
                    SoundEffectManager.Instance.PlayAttackMissSounds();
                }
                else
                {
                    F1.Block(true);
                    F0.LightAttack(true, true);
                    GameManager.Instance.ActivateSlowMotion();
                }
            }
            else
            {
                if ((!F0.Crouched && !F1.Crouched) || (F0.Crouched && F1.Crouched))
                {
                    F1.TakeDamage(F0.Damage);
                    F0.AddHealth(F0.HealthGainedFromPunch);
                    F0.LightAttack(true, false);
                }
                // Opponent is crouched
                else if(F1.Crouched)
                {
                    if (p2Command == Enums.FightCommands.Crouch)
                        GameManager.Instance.ActivateSlowMotion();
                    F0.LightAttack(false, false);
                    SoundEffectManager.Instance.PlayAttackMissSounds();
                }
            }
        }
        else if (p2Command == Enums.FightCommands.Block)
        {
            F1.Block(false);
            F0.LightAttack(false, false);
            SoundEffectManager.Instance.PlayAttackMissSounds();
        }
        else
        {
            F0.LightAttack(false, false);
            SoundEffectManager.Instance.PlayAttackMissSounds();
        }
    }

    private void P2PunchP1()
    {
        if (F1.Slot - F0.Slot <= 1)
        {
            if (p1Command == Enums.FightCommands.Block)
            {
                if (F0.Crouched && !F1.Crouched)
                {
                    F0.Block(false);
                    F1.LightAttack(false, false);
                    SoundEffectManager.Instance.PlayAttackMissSounds();
                }
                else
                {
                    F0.Block(true);
                    GameManager.Instance.ActivateSlowMotion();
                    F1.LightAttack(true, true);
                }
            }
            else
            {
                if ((!F0.Crouched && !F1.Crouched) || (F1.Crouched && F0.Crouched))
                {
                    F0.TakeDamage(F1.Damage);
                    F1.AddHealth(F1.HealthGainedFromPunch);
                    F1.LightAttack(true, false);
                }
                else if (F0.Crouched)
                {
                    if (p1Command == Enums.FightCommands.Crouch)
                        GameManager.Instance.ActivateSlowMotion();
                    F1.LightAttack(false, false);
                    SoundEffectManager.Instance.PlayAttackMissSounds();
                }
            }
        }
        else if (p1Command == Enums.FightCommands.Block)
        {
            F0.Block(false);
            SoundEffectManager.Instance.PlayAttackMissSounds();
            F1.LightAttack(false, false);
        }
        else
        {
            F1.LightAttack(false, false);
            SoundEffectManager.Instance.PlayAttackMissSounds();
        }
    }

    private void PunchBoth()
    {
        if(F1.Slot - F0.Slot <= 1)
        {
            F0.LightAttackDraw();
            F1.LightAttackDraw();
        }
        else
        {
            SoundEffectManager.Instance.PlayAttackMissSounds();
            F0.LightAttack(false, false);
            F1.LightAttack(false, false);
        }
    }

    private void MoveCheck()
    {
        if (p1Command == Enums.FightCommands.MoveRight)
        {
            if (p2Command == Enums.FightCommands.MoveLeft)
            {
                if (F1.Slot - F0.Slot == 1 || F1.Slot - F0.Slot == 2)
                {
                    F0.MoveRightBump();
                    F1.MoveLeftBump();
                }
                else
                {
                    F0.MoveRight();
                    F1.MoveLeft();
                }
            }
            else
            {
                if (F1.Slot - F0.Slot == 1)
                {
                    if (F0.Slot != 4)
                    {
                        F0.BumpRight();
                        F1.ForceRight();
                    }
                }
                else
                {
                    F0.MoveRight();
                }
            }
        }
        else if (p1Command == Enums.FightCommands.MoveLeft)
        {
            F0.MoveLeft();
        }

        if (p2Command == Enums.FightCommands.MoveRight)
        {
            F1.MoveRight();
        }
        else if(p2Command == Enums.FightCommands.MoveLeft)
        {
            if (F1.Slot - F0.Slot == 1)
            {
                if (F1.Slot != 1)
                {
                    if (p1Command != Enums.FightCommands.MoveRight)
                    {
                        F0.ForceLeft();
                        F1.BumpLeft();
                    }
                }
            }
            else
            {
                if (p1Command != Enums.FightCommands.MoveRight)
                {
                    F1.MoveLeft();
                }
            }
        }
    }

    private void ExecuteCommands()
    {
        MoveCheck();

        if (p1Command == Enums.FightCommands.Crouch)
            F0.Crouch();
        else if (p1Command == Enums.FightCommands.StandUp)
            F0.StandUp();
        if (p2Command == Enums.FightCommands.Crouch)
            F1.Crouch();
        else if (p2Command == Enums.FightCommands.StandUp)
            F1.StandUp();

        if (p1Command == Enums.FightCommands.BrokenBlock)
            F0.BrokenBlock();
        if (p2Command == Enums.FightCommands.BrokenBlock)
            F1.BrokenBlock();

        if (p1Command == Enums.FightCommands.LightAttack)
        {
            if (p2Command != Enums.FightCommands.LightAttack)
                P1PunchP2();
            else
                PunchBoth();

        }
        else if(p2Command == Enums.FightCommands.LightAttack)
        {
            P2PunchP1();
        }
        else if(p1Command == Enums.FightCommands.Block)
        {
            F0.Block(false);
            if (p2Command == Enums.FightCommands.Block)
                F1.Block(false);
        }
        else if(p2Command == Enums.FightCommands.Block)
        {
            F1.Block(false);
        }


        p1Command = Enums.FightCommands.NULL;
        p2Command = Enums.FightCommands.NULL;
    }

    public void AddFighterController(FighterController controller)
    {
        fighterControllers.Add(controller);
    }

    private void SetupFighters()
    {
        if (GameManager.Instance.Player1Type != Enums.FighterControllerType.None)
        {
            fighters = new Enums.FighterControllerType[] { GameManager.Instance.Player1Type, GameManager.Instance.Player2Type };
        }
            // BAD
            FighterController[] fighterControllerArrays = FindObjectsOfType<FighterController>();
        if (fighterControllerArrays.Length != 0)
        {
            for (int i = 0; i < fighterControllerArrays.Length; i++)
            {

                fighterControllerArrays[i].PlayerIndex = i;
                Fighter f = fighterControllerArrays[i].GetComponent<Fighter>();
                f.Slot = i == 0 ? 1 : 4;
                fighterControllerArrays[i].transform.position = new Vector3((f.Slot * Instance.SlotDistance) - (NUM_SLOTS / 2 * Instance.SlotDistance) + (SlotDistance / 2), 0, 5);
                f.RecacheStartingPosition();
                AddFighterController(fighterControllerArrays[i]);
                if (i == 0)
                {
                    f.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
            }
        }
        else
        {
            //
            fighterGameObjects[0] = Instantiate((GameObject)Resources.Load("PlayerModel"));
            fighterGameObjects[1] = Instantiate((GameObject)Resources.Load("PlayerModel"));

            fighterControllers = new List<FighterController>();

            Enums.FighterControllerType[] init = fighters == null ? defaultFighters : fighters;
            for (int i = 0; i < defaultFighters.Length; i++)
            {
                CreateFighterController(fighterGameObjects[i], init[i], i);
            }
            Camera.main.GetComponent<Follow>().SetTargets(fighterGameObjects[0].transform, fighterGameObjects[1].transform);

        }
    }

    private void CreateFighterController(GameObject fighter, Enums.FighterControllerType controllerType, int player)
    {
        FighterController fc = null;
        switch (controllerType)
        {
            case Enums.FighterControllerType.Player:
                fc = fighter.AddComponent<PlayerController>();
                break;
            case Enums.FighterControllerType.AI:
                fc = fighter.AddComponent<AIFighterController>();
                break;
            case Enums.FighterControllerType.Dummy:
                fc = fighter.AddComponent<DummyFighterController>();
                break;
        }

        fc.PlayerIndex = player;
        Fighter f = fighter.AddComponent<Fighter>();
        f.Slot = player == 0 ? 1 : 4;
        if(player == 0) {
            f.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        fighter.transform.position = new Vector3((f.Slot * Instance.SlotDistance) - (NUM_SLOTS/2 * Instance.SlotDistance) + (SlotDistance/2), 0, 5);
        AddFighterController(fc);
    }

    public void PlayerDied(int playerIndex)
    {
        if (playerIndex == 0) {
            player2Score++;
        } else {
            player1Score++;
        }
        UIManager.Instance.ShowScoreUI();
        for(int i = 0; i < fighterControllers.Count; i++)
        {
            fighterControllers[i].ForceEndRound();
        }
        fsm.ChangeState(KnockoutState);
        SoundEffectManager.Instance.PlayMatchEnd();
    }

    public int GetScore(int playerIndex) {
        return playerIndex == 0 ? player1Score : player2Score;
    }

    protected override bool Persistent
    {
        get { return false; }
    }

    public float SlotDistance
    {
        get { return slotDistance; }
    }

    public int InputDuration
    {
        get { return inputDuration; }
        set { inputDuration = value; }
    }

    private Fighter F0
    {
        get { return fighterControllers[0].Fighter; }
    }

    private Fighter F1
    {
        get { return fighterControllers[1].Fighter; }
    }
}
