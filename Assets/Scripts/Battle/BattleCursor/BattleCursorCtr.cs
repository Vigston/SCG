using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BattleCursorCtr : MonoBehaviour
{
    // =================変数================
    // インスタンス
    public static BattleCursorCtr instance;

    // 選択しているオブジェクト
    [SerializeField]
    private GameObject m_SelectedObj;

    private void Awake()
    {
        CreateInstance();
        CheckClickedObj().Forget();
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

        Debug.LogError("BattleCursorCtrのインスタンスが生成できませんでした");
        return false;
    }

    // クリック処理のループ
    async UniTask CheckClickedObj()
    {
        while (true)
        {
            // クリックされた選択可能オブジェクトが取得できるまではじく
            GameObject clickedObject = await GetClickedObject();

            await UniTask.WaitUntil(() => clickedObject != null);

            // 選択しているオブジェクト設定
            SetSelectedObj(clickedObject);

            Debug.Log($"{m_SelectedObj}を選択しました。");
        }
    }

    // クリックされた選択可能オブジェクトを取得
    async UniTask<GameObject> GetClickedObject()
    {
        // 選択可能なオブジェクトがクリックされるまではじく
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));

        GameObject clickedObj = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            clickedObj = hit.collider.gameObject;
        }

        if (clickedObj != null)
        {
            // ↓下記に選択可能なオブジェクトを設定↓
            // カード
            if (clickedObj.GetComponent<BattleCard>() != null)
            {
                Debug.Log("カードをクリックしました。");
            }
            // カードエリア
            else if (clickedObj.GetComponent<CardArea>() != null)
            {
                Debug.Log("カードエリアをクリックしました。");
            }
            // 職業付与カード
            else if (clickedObj.GetComponent<GiveJobDrag>() != null)
            {
                Debug.Log("職業付与カードをクリックしました。");
            }
            // 選択できないオブジェクトをクリックしているので初期化する
            else
            {
                clickedObj = null;
                Debug.Log("選択できないオブジェクトをクリックしました。");
            }
        }

        return clickedObj;
    }

    // 選択可能なオブジェクトをクリックしたか
    public bool IsClickedSelectableObj()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedObj = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                clickedObj = hit.collider.gameObject;
            }

            if(clickedObj != null)
            {
                // ↓下記に選択可能なオブジェクトを設定↓
                // カード
                if (clickedObj.GetComponent<BattleCard>() != null)
                {
                    Debug.Log("カードをクリックしました。");
                    return true;
                }
                // カードエリア
                else if (clickedObj.GetComponent<CardArea>() != null)
                {
                    Debug.Log("カードエリアをクリックしました。");
                    return true;
                }
                // 職業付与カード
                else if(clickedObj.GetComponent<GiveJobDrag>() != null)
                {
                    Debug.Log("職業付与カードをクリックしました。");
                    return true;
                }
            }
        }

        // 選択可能なオブジェクトをクリックしていない
        return false;
    }

    // 選択しているオブジェクト設定
    public void SetSelectedObj(GameObject _obj)
    {
        m_SelectedObj = _obj;
    }

    // 選択しているオブジェクト取得
    public GameObject GetSelectedObj()
    {
        return m_SelectedObj;
    }
}
