using UnityEngine;

[System.Serializable]
public class Military : Job
{
    public Military(bool isEnable = true)
    {
        // 各種設定
        SetUp(isEnable);

        Debug.Log("Militaryクラスが生成されました");
    }

    // 各種設定
    public override void SetUp(bool _isEnable = true)
    {
        m_IsEnable = _isEnable;
    }
}