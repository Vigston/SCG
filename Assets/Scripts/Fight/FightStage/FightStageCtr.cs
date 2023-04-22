using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightStageCtr : MonoBehaviour
{
    // =================�ϐ�=================
    // �v���C���[�G���A
    public GameObject PlayerArea;
    // �G�l�~�[�G���A
    public GameObject EnemyArea;
    // �J�[�h�I�u�W�F�N�g
    public GameObject cardPrefab;
    // �c�ɕ��ԃJ�[�h����
    [SerializeField]
    private int heightNum = 0;
    // ���ɕ��ԃJ�[�h����
    [SerializeField]
    private int widthNum = 0;
    // �J�[�h�Ԋu
    [SerializeField]
    private float interval_H = 0.0f;
    [SerializeField]
    private float interval_W = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // ����Ǝ����̕���
        for(int i = 0; i < (int)FightMgr.eSide.eSide_Max; i++)
        {
            // ����W
            Vector3 baseVec = PlayerArea.transform.position;

            // �v���C���[��
            if((int)FightMgr.eSide.eSide_Player == i)

            // �c��̕���
            for (int j = 0; j < heightNum; j++)
            {
                // ����̕���
                for (int k = 0; k < widthNum; k++)
                {
                    Instantiate(cardPrefab, new Vector3((k * cardPrefab.transform.localScale.x) + (k * interval_W), 0.0f, j * cardPrefab.transform.localScale.z + (j * interval_H)), Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
