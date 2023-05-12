using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleStageCtr : MonoBehaviour
{
    // =================�ϐ�=================
    // �C���X�^���X
    public static BattleStageCtr instance;
    // �v���C���[�G���A
    public BoxCollider PlayerArea;
    // �G�l�~�[�G���A
    public BoxCollider EnemyArea;
    // �J�[�h�G���A�I�u�W�F�N�g
    public GameObject cardAreaPrefab;
    // �c�ɕ��ԃJ�[�h����
    [SerializeField]
    private int heightNum = 0;
    // ���ɕ��ԃJ�[�h����
    [SerializeField]
    private int widthNum = 0;
    // �J�[�h�Ԋu
    [SerializeField]
    private float interval_H;
    [SerializeField]
    private float interval_W;

    // �J�[�h�G���A
    GameObject CardArea;

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

        Debug.LogError("BattleStageCtr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }

    // �J�[�h�G���A�쐬
    public void CreateCardArea(List<CardArea> _cardAreaList)
    {
        //// ����W
        //Vector3 baseVec = new Vector3();

        //BoxCollider cardCollider = cardAreaPrefab.GetComponent<BoxCollider>();

        //// ����̎�O
        //Vector3 vecCardLeftTopUp = Common.GetBoxCollideVertices(cardCollider)[0];
        //// ����̉�
        //Vector3 vecCardLeftBottomUp = Common.GetBoxCollideVertices(cardCollider)[6];
        //// �E��̎�O
        //Vector3 vecCardRightTopUp = Common.GetBoxCollideVertices(cardCollider)[1];

        //// �J�[�h�̔���������Ɉړ��B
        //baseVec.z += Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) / 2;
        //// �J�[�h�Ԃ̕��̔�����Ɉړ��B
        //baseVec.z += interval_H / 2;

        //// �J�[�h2�������Ɉړ��B
        //baseVec.x += Vector3.Distance(vecCardLeftTopUp, vecCardRightTopUp) * 2;

        //Vector3 vecLeftTopUp = Common.GetBoxCollideVertices(PlayerArea)[0];
        //Vector3 vecLeftBottomUp = Common.GetBoxCollideVertices(PlayerArea)[6];

        //baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;



        //// �c��̕���
        //for (int j = 0; j < heightNum; j++)
        //{
        //    // ����̕���
        //    for (int k = 0; k < widthNum; k++)
        //    {
        //        // �G���A�̕\���ʒu
        //        Vector3 areaPos = baseVec;

        //        areaPos.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
        //        areaPos.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

        //        GameObject cardAreaClone = Instantiate(cardAreaPrefab, areaPos, Quaternion.identity);

        //        // �J�[�h�G���A�̏��ݒ�
        //        CardArea cardArea = cardAreaClone.GetComponent<CardArea>();
        //        cardArea.SetPosiiton((ePosition)(j + k));
        //        _cardAreaList.Add(cardArea);
        //    }
        //}

        // ����Ǝ����̕���
        for (int i = 0; i < (int)Side.eSide_Max; i++)
        {
            // ����W
            Vector3 baseVec = new Vector3();

            int playerIndex = (int)Side.eSide_Player;
            int enemyIndex = (int)Side.eSide_Enemy;

            BoxCollider cardCollider = cardAreaPrefab.GetComponent<BoxCollider>();

            // ����̎�O
            Vector3 vecCardLeftTopUp = Common.GetBoxCollideVertices(cardCollider)[0];
            // ����̉�
            Vector3 vecCardLeftBottomUp = Common.GetBoxCollideVertices(cardCollider)[6];
            // �E��̎�O
            Vector3 vecCardRightTopUp = Common.GetBoxCollideVertices(cardCollider)[1];

            // �J�[�h�̔���������Ɉړ��B
            baseVec.z += Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) / 2;
            // �J�[�h�Ԃ̕��̔�����Ɉړ��B
            baseVec.z += interval_H / 2;

            // �J�[�h2�������Ɉړ��B
            baseVec.x += Vector3.Distance(vecCardLeftTopUp, vecCardRightTopUp) * 2;

            // �v���C���[��
            if (i == playerIndex)
            {
                Vector3 vecLeftTopUp = Common.GetBoxCollideVertices(PlayerArea)[0];
                Vector3 vecLeftBottomUp = Common.GetBoxCollideVertices(PlayerArea)[6];

                baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;
            }
            // �G��
            else if (i == enemyIndex)
            {
                Vector3 vecLeftTopUp = Common.GetBoxCollideVertices(EnemyArea)[0];
                Vector3 vecLeftBottomUp = Common.GetBoxCollideVertices(EnemyArea)[6];

                baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;
            }



            // �c��̕���
            for (int j = 0; j < heightNum; j++)
            {
                // ����̕���
                for (int k = 0; k < widthNum; k++)
                {
                    // �G���A�̕\���ʒu
                    Vector3 areaPos = baseVec;

                    areaPos.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
                    areaPos.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

                    GameObject cardAreaClone = Instantiate(cardAreaPrefab, areaPos, Quaternion.identity);

                    // �J�[�h�G���A�̏��ݒ�
                    CardArea cardArea = cardAreaClone.GetComponent<CardArea>();
                    cardArea.SetPosiiton((Position)(j + k));
                    _cardAreaList.Add(cardArea);
                }
            }
        }
    }
}
