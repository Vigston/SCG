using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightStageCtr : MonoBehaviour
{
    // =================変数=================
    // プレイヤーエリア
    public GameObject PlayerArea;
    // エネミーエリア
    public GameObject EnemyArea;
    // カードオブジェクト
    public GameObject cardPrefab;
    // 縦に並ぶカード枚数
    [SerializeField]
    private int heightNum = 0;
    // 横に並ぶカード枚数
    [SerializeField]
    private int widthNum = 0;
    // カード間隔
    [SerializeField]
    private float interval_H = 0.0f;
    [SerializeField]
    private float interval_W = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // 相手と自分の分回す
        for(int i = 0; i < (int)FightMgr.eSide.eSide_Max; i++)
        {
            // 基準座標
            Vector3 baseVec = PlayerArea.transform.position;

            // プレイヤー側
            if((int)FightMgr.eSide.eSide_Player == i)

            // 縦列の分回す
            for (int j = 0; j < heightNum; j++)
            {
                // 横列の分回す
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
