using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCtr : MonoBehaviour
{
    // =================変数================
    // インスタンス
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

    // =================関数================
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

        Debug.LogError("BattleCtrのインスタンスが生成できませんでした");
        return false;
    }
}
