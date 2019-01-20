using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums {
    public enum FightCommands {
        LightAttack,
        Block, BrokenBlock, MoveRight, MoveLeft, Crouch, StandUp, Fake,
        None, NUM_COMMANDS, NULL
    }

    public enum FighterControllerType { Player, AI, Dummy, Rail, None }

    public enum GameMode { Mayhem, Strategy }
}
