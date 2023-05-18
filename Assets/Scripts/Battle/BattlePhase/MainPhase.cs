using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;

public class MainPhase : MonoBehaviour
{
    // ===�\����===
    public enum State
    {
        eState_Init,
        eState_End,
    }

    // ===�ϐ�===
    // ���݂̃X�e�[�g
    State m_State;
    // ���̃X�e�[�g
    State m_NextState;

    // �X�e�[�g���Ƃ̎��s��
    int m_StateValue = 0;

    // ===�t���O===
    // �X�e�[�g�̍X�V�t���O
    bool m_NextStateFlag = false;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // �������X�e�[�g����n�߂�
        SetState(State.eState_Init);
        // �X�e�[�g���[�v����
        StateLoop().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        m_StateValue++;
        switch (m_State)
        {
            case State.eState_Init:
                Init();
                break;
            case State.eState_End:
                End();
                break;
            default:
                Debug.Log("�o�^����Ă��Ȃ��X�e�[�g�ł��B");
                break;
        }
    }

    // ������
    void Init()
    {
        if(m_StateValue == 1)
        {
            Debug.Log("�������X�e�[�g�����J�n");
        }

        // �^�[���G���h�t���O�������Ă���Ȃ�I������B
        if (BattleMgr.instance.IsTurnEndFlag())
        {
            // �I����
            SetNextStateAndFlag(State.eState_End);
            Debug.Log("�������X�e�[�g�����I��");
        }
    }

    // �I��
    void End()
    {
        Debug.Log("�I���X�e�[�g�����J�n");
        // �G���h�t�F�C�Y�Ɉړ��B
        BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_End);
        Debug.Log("�I���X�e�[�g�����I��");
    }

    // --�V�X�e��--
    async UniTask StateLoop()
    {
        Debug.Log("StateLoop�N��");
        while (true)
        {
            // �X�e�[�g�X�V����
            Debug.Log("�X�e�[�g�X�V�I�I");
            // ���̃X�e�[�g�X�V(���̃X�e�[�g�ɐݒ�)
            SetNextState(m_State);
            // ���̃X�e�[�g
            State nextState = GetNextState();
            // ���̃X�e�[�g�Ɉړ�����t���O������
            m_NextStateFlag = false;

            switch (m_State)
            {
                case State.eState_Init:
                    Debug.Log("�������X�e�[�g");
                    nextState = await StateInit();
                    Debug.Log("���̃X�e�[�g��");
                    break;
                case State.eState_End:
                    Debug.Log("�I���X�e�[�g");
                    nextState = await StateEnd();
                    Debug.Log("���̃X�e�[�g��");
                    break;
                default:
                    Debug.Log("StateLoop�ɋL�ڂ���Ă��Ȃ��X�e�[�g�ɑJ�ڂ��悤�Ƃ��Ă��܂�");
                    break;
            }

            // ���̃X�e�[�g�����݂Ɠ����Ȃ�͂���
            if (nextState == m_State) { continue; }

            // ���̃X�e�[�g��ݒ�
            SetState(nextState);
        }
    }
    // ������
    async UniTask<State> StateInit()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());
        // ���̃X�e�[�g��
        return GetNextState();
    }
    // �I��
    async UniTask<State> StateEnd()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());

        // ���̃X�e�[�g��
        return GetNextState();
    }

    // --�X�e�[�g--
    // �X�e�[�g�ݒ�
    public void SetState(State _state)
    {
        m_State = _state;
    }
    public void SetNextState(State _nextState)
    {
        m_NextState = _nextState;
    }
    // �X�e�[�g�擾
    public State GetState()
    {
        return m_State;
    }
    public State GetNextState()
    {
        return m_NextState;
    }
    // �w��̃X�e�[�g��
    public bool IsState(State _state)
    {
        return m_State == _state;
    }
    public bool IsNextState(State _nextState)
    {
        return m_NextState == _nextState;
    }

    // --�t���O--
    public void SetNextStateFlag()
    {
        m_NextStateFlag = true;
        m_StateValue = 0;
    }
    public bool IsNextStateFlag()
    {
        return m_NextStateFlag;
    }

    // ���̃X�e�[�g�ƃt���O��ݒ�
    public void SetNextStateAndFlag(State _nextState)
    {
        SetNextState(_nextState);
        SetNextStateFlag();
    }
}
