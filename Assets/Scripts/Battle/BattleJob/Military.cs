using UnityEngine;

[System.Serializable]
public class Military : Job
{
    /////////構造体///////////
    public enum Status
    {
        // 非武装
        eStatus_Unarmed,
        // 武装
        eStatus_Armed,
    }

    //////////変数////////////
    // ステータス
    private Status m_Status;

    public Military(Status _status, bool isEnable = true)
    {
        //職業設定
        SetUpJob(isEnable);

        // ステータス設定
        SetStatus(_status);

        Debug.Log("Militaryクラスが生成されました");
    }

    // 更新
    public void Update()
    {

    }

    //////////関数/////////////
    // ===ステータス===
    // ステータス設定
    public void SetStatus(Status _status)
    {
        m_Status = _status;
    }
    // ステータス取得
    public Status GetStatus()
    {
        return m_Status;
    }
    // 指定のステータスか
    public bool IsStatus(Status _status)
    {
        return m_Status == _status;
    }
}