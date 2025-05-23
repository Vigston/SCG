﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Photon.Pun;

public class BattleStageCtr : MonoBehaviour
{
    // =================変数=================
    // インスタンス
    public static BattleStageCtr instance;
    // プレイヤーエリア
    public BoxCollider PlayerArea;
    // エネミーエリア
    public BoxCollider EnemyArea;
    // カードエリアオブジェクト
    public GameObject cardAreaPrefab;
    // 縦に並ぶカード枚数
    [SerializeField]
    private int heightNum = 0;
    // 横に並ぶカード枚数
    [SerializeField]
    private int widthNum = 0;
    // カード間隔
    [SerializeField]
    private float interval_H;
    [SerializeField]
    private float interval_W;

    // カードエリアプレハブ
    private string m_CardAreaPrehabName = "CardArea";

    // カードエリア
    GameObject CardArea;

    private void Awake()
    {
        CreateInstance();
    }

    // インスタンスを作成
    public bool CreateInstance()
    {
        // 既にインスタンスが作成されていなければ作成する
        if (instance == null)
        {
            // 作成
            instance = this;
        }

        // インスタンスが作成済みなら終了
        if (instance != null) { return true; }

        Debug.LogError("BattleStageCtrのインスタンスが生成できませんでした");
        return false;
    }

    // カードエリア作成
    public void CreateCardArea()
    {
        // 相手と自分の分回す
        for (int i = 0; i < (int)Side.eSide_Max; i++)
        {
            // 基準座標
            Vector3 baseVec = new Vector3();

            // 側
            Side Side = BattleMgr.instance.GetSide(i);

            BoxCollider cardCollider = cardAreaPrefab.GetComponent<BoxCollider>();

            // 左上の手前
            Vector3 vecCardLeftTopUp = Common.GetBoxColliderVertices(cardCollider)[0];
            // 左上の奥
            Vector3 vecCardLeftTopDown = Common.GetBoxColliderVertices(cardCollider)[3];
            // 左下の手前
            Vector3 vecCardLeftBottomUp = Common.GetBoxColliderVertices(cardCollider)[6];
            // 右上の手前
            Vector3 vecCardRightTopUp = Common.GetBoxColliderVertices(cardCollider)[1];

            // カードの半分だけ上に移動。
            baseVec.z += Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) / 2;
            // カード間の幅の半分上に移動。
            baseVec.z += interval_H / 2;

            // カード2枚分横に移動。
            baseVec.x += Vector3.Distance(vecCardLeftTopUp, vecCardRightTopUp) * 2;

            // プレイヤー側
            if (Side == Side.eSide_Player)
            {
                Vector3 vecLeftTopUp = Common.GetBoxColliderVertices(PlayerArea)[0];
                Vector3 vecLeftBottomUp = Common.GetBoxColliderVertices(PlayerArea)[6];

                baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;
            }
            // 敵側
            else if (Side == Side.eSide_Enemy)
            {
                Vector3 vecLeftTopUp = Common.GetBoxColliderVertices(EnemyArea)[0];
                Vector3 vecLeftBottomUp = Common.GetBoxColliderVertices(EnemyArea)[6];

                baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;
            }

            // カードの厚み
            float CardHeightSize = Vector3.Distance(vecCardLeftTopUp, vecCardLeftTopDown) / 2;

            // プレイヤー側のエリア作成
            if (Side == Side.eSide_Player)
            {
                // 縦列の分回す
                for (int j = 0; j < heightNum; j++)
                {
                    // 横列の分回す
                    for (int k = 0; k < widthNum; k++)
                    {
                        // エリアの表示位置
                        Vector3 areaVec = baseVec;
						// エリア位置
						Position areaPos = (Position)((j * widthNum) + k);

						areaVec.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
						areaVec.y += CardHeightSize;
						areaVec.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

                        // カード追加
		                BattleStageMgr.instance.AddCardArea(areaVec, (int)Side, (int)areaPos);
                    }
                }
            }

            // 敵側のエリア作成
            if (Side == Side.eSide_Enemy)
            {
                // 縦列の分回す
                for (int j = 0; j < heightNum; j++)
                {
                    int jrev = heightNum - j - 1;
                    // 横列の分回す
                    for (int k = 0; k < widthNum; k++)
                    {
                        int krev = widthNum - k - 1;
                        // エリアの表示位置
                        Vector3 areaVec = baseVec;
						// エリア位置
						Position areaPos = (Position)(((jrev) * widthNum) + krev);

						areaVec.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
						areaVec.y += CardHeightSize;
						areaVec.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

                        // カード追加
                        BattleStageMgr.instance.AddCardArea(areaVec, (int)Side, (int)areaPos);
                    }
                }
            }
        }
    }
}
