using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public static class Common
{
    // BoxCollision�̒��_���W���擾
    /*
     * 0 = ����(��)
     * 1 = �E��(��)
     * 2 = �E��(��)
     * 3 = ����(��)
     * 4 = �E��(��)
     * 5 = ����(��)
     * 6 = ����(��)
     * 7 = �E��(��)
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

    // �X�N���[�����W���烏�[���h���W�ɕϊ�
    public static Vector3 GetWorldPositionFromScreenPosition(Canvas _canvas, RectTransform _rect)
    {
        //UI���W����X�N���[�����W�ɕϊ�
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _rect.position);

        //���[���h���W
        Vector3 worldPos = Vector3.zero;

        //�X�N���[�����W���烏�[���h���W�ɕϊ�
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_rect, screenPos, _canvas.worldCamera, out worldPos);

        return worldPos;
    }

    // �t�̑����擾
    public static Side GetRevSide(Side _side)
    {
        // �v���C���[
        if(_side == Side.eSide_Player)
        {
            return Side.eSide_Enemy;
        }
        // �G
        else if(_side == Side.eSide_Enemy)
        {
            return Side.eSide_Player;
        }

        return Side.eSide_None;
    }
}
