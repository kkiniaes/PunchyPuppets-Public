using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bitwise {
    public static bool IsBitOn(int reg, int bitPlace)
    {
        return (((1 << bitPlace) & reg) > 0);
    }

    public static int SetBit(int reg, int bitPlace)
    {
        return reg | (1 << bitPlace);
    }

    public static int ClearBit(int reg, int bitPlace)
    {
        return reg & (~(1 << bitPlace));
    }
}
