using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : FighterController {
    protected override void Update()
    {
        base.Update();
        if (canRecordInput)
        {
            if (!recordedAlready)
            {
                if (P.GetButton("Fake"))
                    fakeHeld = true;
                else
                    fakeHeld = false;
                
                if (P.GetButtonDown("Right"))
                    CheckCommand(Enums.FightCommands.MoveRight);

                else if (P.GetButtonDown("Left"))
                    CheckCommand(Enums.FightCommands.MoveLeft);
                
                else if (P.GetButtonDown("Down"))
                    CheckCommand(Enums.FightCommands.Crouch);

                else if (P.GetButtonDown("Up"))
                    CheckCommand(Enums.FightCommands.StandUp);

                else if (P.GetButtonDown("LightAttack"))
                {
                     CheckCommand(Enums.FightCommands.LightAttack);
                }
                else if (P.GetButtonDown("Block"))
                {
                    if(fightCommands.Count > 0)
                    {
                        if (fightCommands[fightCommands.Count - 1] == Enums.FightCommands.Block)
                            CheckCommand(Enums.FightCommands.BrokenBlock);
                        else
                            CheckCommand(Enums.FightCommands.Block);
                    }
                    else
                        CheckCommand(Enums.FightCommands.Block);
                }
            }
        }
    }

    private Player P
    {
        get { return ReInput.players.GetPlayer(playerIndex); }
    }
}
