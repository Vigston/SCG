using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

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

            Side side = BattleMgr.instance.GetSide(i);

            BoxCollider cardCollider = cardAreaPrefab.GetComponent<BoxCollider>();

            // 左上の手前
            Vector3 vecCardLeftTopUp = Common.GetBoxCollideVertices(cardCollider)[0];
            // 左上の奥
            Vector3 vecCardLeftBottomUp = Common.GetBoxCollideVertices(cardCollider)[6];
            // 右上の手前
            Vector3 vecCardRightTopUp = Common.GetBoxCollideVertices(cardCollider)[1];

            // カードの半分だけ上に移動。
            baseVec.z += Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) / 2;
            // カード間の幅の半分上に移動。
            baseVec.z += interval_H / 2;

            // カード2枚分横に移動。
            baseVec.x += Vector3.Distance(vecCardLeftTopUp, vecCardRightTopUp) * 2;

            // プレイヤー側
            if (side == Side.eSide_Player)
            {
                Vector3 vecLeftTopUp = Common.GetBoxCollideVertices(PlayerArea)[0];
                Vector3 vecLeftBottomUp = Common.GetBoxCollideVertices(PlayerArea)[6];

                baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;
            }
            // 敵側
            else if (side == Side.eSide_Enemy)
            {
                Vector3 vecLeftTopUp = Common.GetBoxCollideVertices(EnemyArea)[0];
                Vector3 vecLeftBottomUp = Common.GetBoxCollideVertices(EnemyArea)[6];

                baseVec += (vecLeftTopUp + vecLeftBottomUp) / 2;
            }


            // プレイヤー側のエリア作成
            if(side == Side.eSide_Player)
            {
                // 縦列の分回す
                for (int j = 0; j < heightNum; j++)
                {
                    // 横列の分回す
                    for (int k = 0; k < widthNum; k++)
                    {
                        // エリアの表示位置
                        Vector3 areaPos = baseVec;

                        areaPos.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
                        areaPos.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

                        GameObject cardAreaClone = Instantiate(cardAreaPrefab, areaPos, Quaternion.identity);

                        // カードエリアの情報設定
                        CardArea cardArea = cardAreaClone.GetComponent<CardArea>();
                        cardArea.SetSide(side);
                        cardArea.SetPosiiton((Position)((j * widthNum) + k));

                        BattleStageMgr.instance.AddArea(cardArea);
                    }
                }
            }

            // 敵側のエリア作成
            if (side == Side.eSide_Enemy)
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
                        Vector3 areaPos = baseVec;

                        areaPos.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
                        areaPos.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

                        GameObject cardAreaClone = Instantiate(cardAreaPrefab, areaPos, Quaternion.identity);

                        // カードエリアの情報設定
                        CardArea cardArea = cardAreaClone.GetComponent<CardArea>();
                        cardArea.SetSide(side);
                        cardArea.SetPosiiton((Position)(((jrev) * widthNum) + krev));

                        BattleStageMgr.instance.AddArea(cardArea);
                    }
                }
            }
        }
    }
}
