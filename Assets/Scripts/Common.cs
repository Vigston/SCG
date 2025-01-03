﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public static class Common
{
    // バトルの共通定数
    public static class BattleConst
    {
        public const int ADD_GOLD_EVERY_MERCHANT = 10;
    }

    // BoxCollisionの頂点座標を取得
    /*
     * 0 = 左上(上)
     * 1 = 右上(上)
     * 2 = 右上(下)
     * 3 = 左上(下)
     * 4 = 右下(下)
     * 5 = 左下(下)
     * 6 = 左下(上)
     * 7 = 右下(上)
     */
    public static Vector3[] GetBoxCollideVertices(BoxCollider Col)
    {
        Transform trs = Col.transform;
        Vector3 sc = trs.lossyScale;

        sc.x *= Col.size.x;
        sc.y *= Col.size.y;
        sc.z *= Col.size.z;

        sc *= 0.5f;

        Vector3 cp = trs.TransformPoint(Col.center);

        Vector3 vx = trs.right * sc.x;
        Vector3 vy = trs.up * sc.y;
        Vector3 vz = trs.forward * sc.z;

        Vector3 p1 = -vx + vy + vz;
        Vector3 p2 = vx + vy + vz;
        Vector3 p3 = vx + -vy + vz;
        Vector3 p4 = -vx + -vy + vz;

        Vector3[] vertices = new Vector3[8];

        vertices[0] = cp + p1;
        vertices[1] = cp + p2;
        vertices[2] = cp + p3;
        vertices[3] = cp + p4;

        vertices[4] = cp - p1;
        vertices[5] = cp - p2;
        vertices[6] = cp - p3;
        vertices[7] = cp - p4;

        return vertices;
    }

    // スクリーン座標からワールド座標に変換
    public static Vector3 GetWorldPositionFromScreenPosition(Canvas _canvas, RectTransform _rect)
    {
        //UI座標からスクリーン座標に変換
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _rect.position);

        //ワールド座標
        Vector3 worldPos = Vector3.zero;

        //スクリーン座標からワールド座標に変換
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_rect, screenPos, _canvas.worldCamera, out worldPos);

        return worldPos;
    }

    // 逆の側を取得
    public static Side GetRevSide(Side _side)
    {
        // プレイヤー
        if(_side == Side.eSide_Player)
        {
            return Side.eSide_Enemy;
        }
        // 敵
        else if(_side == Side.eSide_Enemy)
        {
            return Side.eSide_Player;
        }

        return Side.eSide_None;
    }

    // 自分のターンで自分のカードか
    public static bool IsMyTurnAndMyCard(BattleCard battleCard)
    {
        if(battleCard == null) { return false; }

        Side turnSide = BattleMgr.instance.GetSetTurnSide;
        Side userSide = BattleUserMgr.instance.GetSetOperateUserSide;
        Side cardSide = battleCard.GetSetSide;

        return turnSide == userSide && cardSide == userSide;
    }
}
