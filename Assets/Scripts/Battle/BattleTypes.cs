using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace battleTypes
{
    // 場(Field)位置
    public enum Position
    {
        ePosition_FF_Left,
        ePosition_FS_Left,
        ePosition_F_Center,
        ePosition_FF_Right,
        ePosition_FS_Right,
        ePosition_BF_Left,
        ePosition_BS_Left,
        ePosition_B_Center,
        ePosition_BF_Right,
        ePosition_BS_Right,
        ePosiiton_Max,

        Exclusion = -1,
    }

    // 側
    public enum Side
    {
        eSide_Player,
        eSide_Enemy,
        eSide_Max,

        eSide_None = -1,
    }

    // フェイズ
    public enum PhaseType
    {
        ePhaseType_Start,
        ePhaseType_Join,
        ePhaseType_Main,
        ePhaseType_End,
    }
}
