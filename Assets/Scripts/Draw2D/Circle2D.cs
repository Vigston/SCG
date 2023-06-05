using UnityEngine;
using UnityEngine.UI;
namespace BlueBreath.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class Circle2D : Graphic
    {
        [Tooltip("半径")] public float radius;
        [Tooltip("分割数")] [Range(2, 1000)] public int division;
        [Tooltip("描画を始める角度")] [Range(0f, 1f)] public float fillOrigin;
        [Tooltip("扇形の角度（1で360度）")] [Range(0f, 1f)] public float fillAmount;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (!IsActive()) return;
            vh.Clear();
            RectCache();
            Vector3 centerPos = new Vector3((pCLeft + pCRight) / 2, (pCTop + pCBottom) / 2);
            Vector3 startPos = centerPos + new Vector3(radius * Mathf.Cos(fillOrigin * 2f * Mathf.PI), radius * Mathf.Sin(fillOrigin * 2f * Mathf.PI));
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = this.color;
            vertex.position = centerPos;
            vh.AddVert(vertex);
            vertex.position = startPos;
            vh.AddVert(vertex);
            DrawCircle(vh, 0, 1, centerPos, startPos, Mathf.PI * 2f * fillAmount, division);
        }
        /// <summary>
        /// ベクトルを回転させて円を作成する
        /// </summary>
        /// <param name="vh">VertexHelper</param>
        /// <param name="centerVert">中心の番号</param>
        /// <param name="startVert">開始番号</param>
        /// <param name="centerPos">中心の座標</param>
        /// <param name="startPos">開始座標</param>
        /// <param name="rad">作成角度</param>
        /// <param name="div">分割数</param>
        /// <returns>最終番号の次</returns>
        private int DrawCircle(
            VertexHelper vh,
            int centerVert, int startVert,
            Vector3 centerPos, Vector3 startPos,
            float rad, int div
            )
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = this.color;
            float divRad = rad / (float)(div + 1);
            Vector3 startVector = startPos - centerPos;
            for (int i = startVert; i < startVert + 1 + div; i++)
            {
                vertex.position = new Vector3(
                    startVector.x * Mathf.Cos(divRad * (i - startVert + 1)) - startVector.y * Mathf.Sin(divRad * (i - startVert + 1)) + centerPos.x,
                    startVector.x * Mathf.Sin(divRad * (i - startVert + 1)) + startVector.y * Mathf.Cos(divRad * (i - startVert + 1)) + centerPos.y
                );
                vh.AddVert(vertex);
            }
            for (int i = startVert; i < startVert + 1 + div; i++)
            {
                vh.AddTriangle(centerVert, i + 1, i);
            }
            return startVert + 1 + div;
        }
        /* RectTransform Cache */
        float anchorLeft, anchorBottom, anchorTop, anchorRight;
        float pivotX, pivotY;
        float posX, posY, posZ;
        float width, height;
        float paddingTop, paddingBottom, paddingLeft, paddingRight;
        //Extensions
        float pCLeft, pCBottom, pCTop, pCRight;
        float nRectLeft, nRectBottom, nRectTop, nRectRight;
        private void RectCache()
        {
            //Anchors
            anchorLeft = rectTransform.anchorMin.x;
            anchorBottom = rectTransform.anchorMin.y;
            anchorTop = rectTransform.anchorMax.y;
            anchorRight = rectTransform.anchorMax.x;
            //pivots
            pivotX = rectTransform.pivot.x;
            pivotY = rectTransform.pivot.y;
            //width & height
            width = rectTransform.rect.width;
            height = rectTransform.rect.height;
            //一致したアンカーとピボットの位置の差
            posX = rectTransform.position.x;
            posY = rectTransform.position.y;
            posZ = rectTransform.position.z;
            //アンカーから内部方向へのパディング
            paddingLeft = rectTransform.offsetMin.x;
            paddingRight = rectTransform.offsetMax.x;
            paddingTop = rectTransform.offsetMax.y;
            paddingBottom = rectTransform.offsetMin.y;
            //ピボット中央に合わせる場合のUIVertex矩形座標
            pCLeft = -width / 2f;
            pCBottom = -height / 2f;
            pCTop = height / 2f;
            pCRight = width / 2f;
            //ピボットに影響されないUIVertex矩形座標
            nRectLeft = -pivotX * width;
            nRectRight = (1 - pivotX) * width;
            nRectTop = (1 - pivotY) * height;
            nRectBottom = -pivotY * height;
        }
    }
}