using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFighterController : FighterController {
    protected override void Update()
    {
        base.Update();
        if (canRecordInput)
        {
            if (!recordedAlready)
            {
                CheckCommand((Enums.FightCommands)Random.Range(0, (int)Enums.FightCommands.NUM_COMMANDS));
            }
        }
    }
}
