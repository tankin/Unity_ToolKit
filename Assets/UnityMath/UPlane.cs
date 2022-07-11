using UnityEngine;
using System.Collections;
using System;

namespace UnityMathUtility
{
    //平面本身就是一个单位法向量
    public class UPlane {

        public float    m_W;            //原点到平面的距离
        public Vector3  m_Normal;       //平面法向量(单位向量)

        /**
        * Constructor.
        *
        * @param InNormal 平面法向量.
        * @param InW 原点到平面距离.
        */
        public UPlane(Vector3 InNormal, float InW)
        {
            m_Normal = InNormal.normalized;
            m_W = InW;
        }

        /**
        * Constructor.
        *
        * @param InBase 平面内一点.
        * @param InNormal 平面法向量.
        */
        public UPlane(Vector3 InBase, Vector3 InNormal)
        {
            m_Normal = InNormal.normalized;
            m_W = Vector3.Dot(InBase, m_Normal);
        }

        /**
        * Constructor.
        *
        * @param A First point in the plane.
        * @param B Second point in the plane.
        * @param C Third point in the plane.
        */
        //注意不能三点共线
        public UPlane(Vector3 A, Vector3 B, Vector3 C)
        {
            Vector3 normal = Vector3.Cross((B-A), (C-A));
            if (normal.sqrMagnitude == 0)
            {
                //三点共线
                throw new Exception("构造平面错误：三点共线");
            }

            m_Normal = normal.normalized;
            m_W = Vector3.Dot(A, m_Normal);
        }

        /**
        * 任意点到平面的距离
        * 
        * @param Point 任意点.
        */
        public float PointPlaneDistance(Vector3 Point)
        {
            return Vector3.Dot(Point, m_Normal) - m_W;
        }
    
        //比较平面是否相等
        public static bool operator == (UPlane A, UPlane B)
        {
            return (A.m_Normal.x == B.m_Normal.x) && (A.m_Normal.y == B.m_Normal.y) && (A.m_Normal.z == B.m_Normal.z) && (A.m_W == B.m_W);
        }

        //比较平面是否不相等
        public static bool operator !=(UPlane A, UPlane B)
        {
            return (A.m_Normal.x != B.m_Normal.x) || (A.m_Normal.y != B.m_Normal.y) || (A.m_Normal.z != B.m_Normal.z) || (A.m_W != B.m_W);
        }
    }
}

