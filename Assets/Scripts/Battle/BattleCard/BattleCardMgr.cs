using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCardMgr : MonoBehaviour
{
    // �C���X�^���X
    public static BattleCardMgr instance;

    // �J�[�h��Prehab
    public GameObject battleCardPrehab;

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
        }

        // �C���X�^���X���쐬�ς݂Ȃ�I��
        if (instance != null) { return true; }

        Debug.LogError("BattleCardMgr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
