using UnityEngine;

[System.Serializable]
public class Spy : Job
{
    public void Start()
    {
        //職業設定
        SetUpJob();

        Debug.Log("Spyクラスが生成されました");
    }

    // 更新
    public void Update()
    {

    }
}