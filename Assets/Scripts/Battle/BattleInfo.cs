using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfo : MonoBehaviour
{
    // �C���X�^���X
    public static BattleInfo instance;

    private void Awake()
    {
        CreateInstance();
    }

    // �C���X�^���X���쐬
    public bool CreateInstance()
    {
        // ���ɃC���X�^���X���쐬����Ă��Ȃ���΍쐬����
        if (instance == null)
        {
            // �쐬
            instance = this;
            return true;
        }

        Debug.LogError("BattleInfo�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }
}
