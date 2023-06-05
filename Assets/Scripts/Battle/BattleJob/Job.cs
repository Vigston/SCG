public class Job
{
    /////////構造体/////////

    /////////変数/////////

    // 職業が付与されたターン


    // ===フラグ===
    // 有効か
    private bool m_IsEnable;

    // 職業設定
    public void SetUpJob(bool _isEnable = true)
    {
        SetEnable(_isEnable);
    }

    // 職業更新
    public void UpdateJob()
    {

    }

    // ===フラグ===
    // 有効フラグ設定
    public void SetEnable(bool _enable)
    {
        m_IsEnable = _enable;
    }
    // 有効フラグが立っているかどうか
    public bool IsEnable()
    {
        return m_IsEnable;
    }
}
