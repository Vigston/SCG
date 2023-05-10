using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardCtr : MonoBehaviour
{
    // インスタンス
    public static BattleCardCtr instance;

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
            return true;
        }

        Debug.LogError("BattleCardCtrのインスタンスが生成できませんでした");
        return false;
    }

    public void CreateCard()
    {

    }
}
