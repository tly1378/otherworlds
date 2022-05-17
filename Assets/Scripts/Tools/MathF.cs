using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathF
{
    public static float Angle(Transform from, Vector2 to)
    {
        float angle = Vector3.Angle(from.up, to - (Vector2)from.position);
        Vector3 normal = Vector3.Cross(from.up, to - (Vector2)from.position);
        angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.forward));
        return angle;
    }

    public static void FaceTo(Transform from, Vector3 to, float speed)
    {
        float rotateAngle = Angle(from, to);
        if (Mathf.Abs(rotateAngle) > speed * Time.deltaTime)
            rotateAngle = (speed * Time.deltaTime) * (rotateAngle > 0 ? 1 : -1);
        float targetAngle = from.rotation.eulerAngles.z + rotateAngle;
        Vector3 targetDirection = new Vector3(0, 0, targetAngle);
        from.eulerAngles = targetDirection;
    }

    public static void FaceToMouse(Transform transform, float speed)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//求得世界坐标 
        FaceTo(transform, mousePosition, speed);
    }

    public static void FaceTo(Transform from, Vector3 to)
    {
        Vector2 direction = to - from.position;//朝向鼠标的向量
        float targetAngle = Vector2.Angle(direction, Vector3.up);//需要旋转的度数
        if (to.x > from.position.x)
        {
            targetAngle = -targetAngle;
        }
        Vector3 targetDirection = new Vector3(0, 0, targetAngle);
        from.eulerAngles = targetDirection;
    }

    //下列Squeeze的运算均假设circle的offsize为(0, 0)
    public static Vector2 SqueezeOut(CircleCollider2D circle, CircleCollider2D fixedCircle)
    {
        Vector2 x = fixedCircle.transform.right;//圆的正方向
        Vector2 position = (Vector2)fixedCircle.transform.position + Projection(fixedCircle.offset, mirro(x));
        float distance = fixedCircle.radius * fixedCircle.transform.localScale.z + circle.radius * circle.transform.localScale.z;
        Vector2 move = ((Vector2)circle.transform.position - position).normalized * distance;
        return position + move;
    }

    //未考虑缩放
    public static Vector2 SqueezeOut(CircleCollider2D circle, BoxCollider2D fixedBox)
    {
        Vector2 x = fixedBox.transform.right;//方的正方向
        //计算挤出后位置
        Vector2 cp = circle.transform.position;//圆的位置（世界）
        Vector2 bp = (Vector2)fixedBox.transform.position + Projection(fixedBox.offset, mirro(x));//方的位置（世界）
        Vector2 rp = cp - bp;//圆的相对位置（世界）
        Vector2 zp = Projection(rp, x);//将圆的相对位置投射到方的正方向上（x）
        Vector2 wh = fixedBox.size / 2;//方的宽高（x）

        Vector2 direction = Normal(zp);//移动的方向（x）
        Vector2 corner = wh * direction;//边的相对位置（x）
        Vector2 fm;
        Vector2 rm;
        if ((zp.x > wh.x && zp.y > wh.y) || (zp.x < -wh.x && zp.y > wh.y) || (zp.x < -wh.x && zp.y < -wh.y) || (zp.x > wh.x && zp.y < -wh.y))
        {

            rm = (zp - corner).normalized * (circle.radius - (zp - corner).magnitude);
            fm = Projection(rm, mirro(x));
            return fm;
        }
        Vector2 margin = corner - (zp - direction * circle.radius);//边距（x）
        //移动的方向+距离（x）
        rm = margin;
        if (Mathf.Abs(margin.x) > Mathf.Abs(margin.y))
        {
            rm *= Vector2.up;
        }
        else
        {
            rm *= Vector2.right;
        }
        //将圆的相对位置投射回世界正方向（世界）
        fm = Projection(rm, mirro(x));
        return fm;
    }


    private static Vector2 Normal(Vector2 vector2)
    {
        Vector2 result = Vector2.zero;
        if (vector2.x > 0)
        {
            result.x = 1;
        }
        else if (vector2.x < 0)
        {
            result.x = -1;
        }

        if (vector2.y > 0)
        {
            result.y = 1;
        }
        else if (vector2.y < 0)
        {
            result.y = -1;
        }
        return result;
    }

    /// <summary>
    /// 点乘
    /// </summary>
    /// <param name="a">二维向量</param>
    /// <param name="b">二维向量</param>
    /// <returns></returns>
    private static float Dot(Vector2 a, Vector2 b) => (a.x * b.x + a.y * b.y);

    /// <summary>
    /// 求a投射到以x为正方向的新坐标后的值，x的长度必须为1。
    /// </summary>
    /// <param name="a">原向量</param>
    /// <param name="x">新坐标轴</param>
    /// <returns>新向量</returns>
    private static Vector2 Projection(Vector2 a, Vector2 x)
    {
        return new Vector2(Dot(a, x), Dot(a, rotate90(x)));
    }

    /// <summary>
    /// 将x逆时针旋转90度
    /// </summary>
    /// <param name="x">二维向量</param>
    /// <returns>逆时针旋转后的二维向量</returns>
    private static Vector2 rotate90(Vector2 x)
    {
        return new Vector2(-x.y, x.x);
    }

    /// <summary>
    /// 以y=0为镜面，获取向量x的镜像向量；
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static Vector2 mirro(Vector2 x)
    {
        return new Vector2(x.x, -x.y);
    }
}
