using UnityEngine;
using UnityEngine.EventSystems;

public class GiveJobDrag : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField]
    private BattleCard.Kind m_giveKind;
    [SerializeField]
    private GameObject dragObj;

    [SerializeField]
    private GameObject dropCard;
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragObj = gameObject;
        dropCard = null;
        Debug.Log($"{dragObj}をドラッグしました。");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        foreach(RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (hit.collider.tag == "BattleCard")
            {
                dropCard = hit.transform.gameObject;
                Debug.Log(hit.transform.gameObject.ToString());
            }
        }

        if(dropCard != null)
        {
            // カード種類を設定
            BattleCard battleCard = dropCard.GetComponent<BattleCard>();
            battleCard.SetKind(m_giveKind);
        }

        dragObj = null;

        Debug.Log($"ドラッグ終了。");
    }

}