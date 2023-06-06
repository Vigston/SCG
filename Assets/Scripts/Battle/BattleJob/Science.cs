using UnityEngine;

[System.Serializable]
public class Science : Job
{
    public void Start()
    {
        //職業設定
        SetUpJob();

        Debug.Log("Scienceクラスが生成されました");
    }

    // 更新
    public void Update()
    {

    }
}
