using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailFighterController : FighterController {
    public List<Enums.FightCommands> defaultCommands = new List<Enums.FightCommands>(20);

    private int tempIndex = 0;

    protected override void Update()
    {
        base.Update();
        if (canRecordInput)
        {
            if (!recordedAlready)
            {
                if(tempIndex < defaultCommands.Count) {
                    CheckCommand(defaultCommands[tempIndex++]);
                }
            }
        }
    }

    public override void EndRecordingInput()
    {
        fightCommands.Clear();
        for(int i = 0; i < defaultCommands.Count; i++)
        {
            fightCommands.Add(defaultCommands[i]);
            
        }
        base.EndRecordingInput();
    }
}
