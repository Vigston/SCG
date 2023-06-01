public class Job
{
    // 有効か
    public bool m_IsEnable;

    // 各種設定
    public virtual void SetUp(bool _isEnable = true)
    {
        m_IsEnable = _isEnable;
    }
}
