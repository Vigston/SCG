using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCtr : MonoBehaviour
{
    // =================�ϐ�================
    // �C���X�^���X
    public static BattleCtr instance;

    void Awake()
    {
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // =================�֐�================
    // �C���X�^���X���쐬
    public bool CreateInstance()
    {
        // ���ɃC���X�^���X���쐬����Ă��Ȃ���΍쐬����
        if (instance == null)
        {
            // �쐬
            instance = this;
        }

        // �C���X�^���X���쐬�ς݂Ȃ�I��
        if (instance != null) { return true; }

        Debug.LogError("BattleCtr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }
}
