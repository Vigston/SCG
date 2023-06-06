using UnityEngine;

[System.Serializable]
public class Merchant : Job
{
    public void Start()
    {
        //職業設定
        SetUpJob();

        Debug.Log("Merchantクラスが生成されました");
    }

    // 更新
    public void Update()
    {

    }
}
