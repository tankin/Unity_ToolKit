using UnityEngine;
using System.Collections;

namespace UnityMathUtility{

    public class UBox
    {
        public Vector3  m_Min;          //最小值顶点
        public Vector3  m_Max;          //最大值顶点
        public int      m_IsValid;      //是否有效

        /** 
        * Utility function to build an AABB from Origin and Extent 
        *
        * @param Origin The location of the bounding box.
        * @param Extent Half size of the bounding box.
        * @return A new axis-aligned bounding box.
        */
        //创建一个AABB包围盒
        public static UBox BuildAABB(Vector3 Origin, Vector3 Extent)
        {
            UBox NewBox = new UBox(Origin - Extent, Origin + Extent);
		    return NewBox;
        }

        /** Default constructor (no initialization). */
        public UBox()
        {
            Init();
        }

        /**
        * Creates and initializes a new box from the specified extents.
        *
        * @param InMin The box's minimum point.
        * @param InMax The box's maximum point.
        */
        public UBox(Vector3 InMin, Vector3 InMax)
        {
            m_Min = InMin;
            m_Max = InMax;
            m_IsValid = 1;
        }

        /**
        * Creates and initializes a new box from an array of points.
        *
        * @param Points Array of Points to create for the bounding volume.
        */
        public UBox(Vector3[] Points)
        {
            for (int i = 0; i < Points.Length; ++i)
            {
                AddPoint(Points[i]);
            }
        }

        /**
        * Adds to this bounding box to include a given point.
        *
        * @param Other the point to increase the bounding volume to.
        * @return Reference to this bounding box after resizing to include the other point.
        */
        //加入一个点以扩充包围盒
        public void AddPoint(Vector3 Point)
        {
            if (m_IsValid == 1)
            {
                m_Min.x = UMath.Min(m_Min.x, Point.x);
                m_Min.y = UMath.Min(m_Min.y, Point.y);
                m_Min.z = UMath.Min(m_Min.z, Point.z);

                m_Max.x = UMath.Max(m_Max.x, Point.x);
                m_Max.y = UMath.Max(m_Max.y, Point.y);
                m_Max.z = UMath.Max(m_Max.z, Point.z);
            }
            else
            {
                m_Min = Point;
                m_Max = Point;
                m_IsValid = 1;
            }
        }

        void Init()
        {
            m_Min = Vector3.zero;
            m_Max = Vector3.zero;
            m_IsValid = 0;
        }

        /** 
        * Increases the box size.
        *
        * @param W The size to increase the volume by.
        * @return A new bounding box.
        */
        //扩充包围盒体积
        public UBox ExpandBy(float W)
        {
            m_Min -= new Vector3(W, W, W);
            m_Max += new Vector3(W, W, W);
            return this;
        }

        /**
        * Increases the box size.
        *
        * @param V The size to increase the volume by.
        * @return A new bounding box.
        */
        public UBox ExpandBy(Vector3 V)
        {
            m_Min -= V;
            m_Max += V;
            return this;
        }

        /** 
        * Shifts the bounding box position.
        *
        * @param Offset The vector to shift the box by.
        * @return A new bounding box.
        */
        //移动包围盒位置
        public UBox ShiftBy(Vector3 Offset)
        {
            m_Min += Offset;
            m_Max += Offset;
            return this;
        }

        /** 
        * Moves the center of bounding box to new destination.
        *
        * @param The destination point to move center of box to.
        * @return A new bounding box.
        */
        //盒子移动到新的目标点
        public UBox MoveTo(Vector3 Destination)
        {
            Vector3 Offset = Destination - GetCenter();
            m_Min += Offset;
            m_Max += Offset;
            return this;
        }

        /**
        * Gets the center point of this box.
        *
        * @return The center point.
        * @see GetCenterAndExtents, GetExtent, GetSize, GetVolume
        */
        //获取包围盒中心点
        public Vector3 GetCenter()
        {
            return (m_Min+m_Max) * 0.5f;
        }

        /**
        * Gets the extents of this box.
        *
        * @return The box extents.
        * @see GetCenter, GetCenterAndExtents, GetSize, GetVolume
        */
        //获取包围盒半径向量,也就是范围
        public Vector3 GetExtent()
        {
            return (m_Max - m_Min) * 0.5f;
        }

        /**
        * Gets the size of this box.
        *
        * @return The box size.
        * @see GetCenter, GetCenterAndExtents, GetExtent, GetVolume
        */
        //获取包围盒大小
        public Vector3 GetSize()
        {
            return m_Max - m_Min;
        }

        /**
        * Gets the volume of this box.
        *
        * @return The box volume.
        * @see GetCenter, GetCenterAndExtents, GetExtent, GetSize
        */
        //获取包围盒体积
        public float GetVolume()
        {
            return (m_Max.x-m_Min.x)*(m_Max.y-m_Min.y)*(m_Max.z-m_Min.z);
        }

        /**
        * Checks whether the given bounding box intersects this bounding box.
        *
        * @param Other The bounding box to intersect with.
        * @return true if the boxes intersect, false otherwise.
        */
        //判断两个包围盒是否相交
        public bool Intersect(UBox Other)
        {
            if ((m_Min.x > Other.m_Max.x) || (Other.m_Min.x > m_Max.x))
            {
                return false;
            }

            if ((m_Min.y > Other.m_Max.y) || (Other.m_Min.y > m_Max.y))
            {
                return false;
            }

            if ((m_Min.z > Other.m_Max.z) || (Other.m_Min.z > m_Max.z))
            {
                return false;
            }

            return true;
        }

        /**
        * Calculates the closest point on or inside the box to a given point in space.
        *
        * @param Point The point in space.
        * @return The closest point on or inside the box.
        */
        //取得某个点到包围盒的最近点
        public Vector3 GetClosestPointTo(Vector3 Point)
        {
            // start by considering the point inside the box
            Vector3 ClosestPoint = Point;

            // now clamp to inside box if it's outside
            if (Point.x < m_Min.x)
            {
                ClosestPoint.x = m_Min.x;
            }
            else if (Point.x > m_Max.x)
            {
                ClosestPoint.x = m_Max.x;
            }

            // now clamp to inside box if it's outside
            if (Point.y < m_Min.y)
            {
                ClosestPoint.y = m_Min.y;
            }
            else if (Point.y > m_Max.y)
            {
                ClosestPoint.y = m_Max.y;
            }

            // Now clamp to inside box if it's outside.
            if (Point.z < m_Min.z)
            {
                ClosestPoint.z = m_Min.z;
            }
            else if (Point.z > m_Max.z)
            {
                ClosestPoint.z = m_Max.z;
            }

            return ClosestPoint;
        }

        /**
	    * Returns the overlap FBox of two box
	    *
	    * @param Other The bounding box to test overlap
	    * @return the overlap box. It can be 0 if they don't overlap
	    */
        //返回包含两个相交包围盒的新包围盒
        public UBox Overlap(UBox Other)
        {
            if(Intersect(Other) == false)
	        {
		        UBox EmptyBox = new UBox();
		        return EmptyBox;
	        }

	        // otherwise they overlap
	        // so find overlapping box
	        Vector3 MinVector, MaxVector;

	        MinVector.x = UMath.Max(m_Min.x, Other.m_Min.x);
	        MaxVector.x = UMath.Min(m_Max.x, Other.m_Max.x);

	        MinVector.y = UMath.Max(m_Min.y, Other.m_Min.y);
	        MaxVector.y = UMath.Min(m_Max.y, Other.m_Max.y);

	        MinVector.z = UMath.Max(m_Min.z, Other.m_Min.z);
	        MaxVector.z = UMath.Min(m_Max.z, Other.m_Max.z);

	        return new UBox(MinVector, MaxVector);
        }

        /** 
        * Checks whether the given location is inside this box.
        * 
        * @param In The location to test for inside the bounding volume.
        * @return true if location is inside this volume.
        * @see IsInsideXY
        */
        //检测盒子是否包含某个点
        public bool IsInside(Vector3 Point)
        {
            return ((Point.x > m_Min.x) && (Point.x < m_Max.x) && (Point.y > m_Min.y) && (Point.y < m_Max.y) && (Point.z > m_Min.z) && (Point.z < m_Max.z));
        }

        /** 
        * Checks whether the given location is inside or on this box.
        * 
        * @param In The location to test for inside the bounding volume.
        * @return true if location is inside this volume.
        * @see IsInsideXY
        */
        public bool IsInsideOrOn(Vector3 Point)
        {
            return ((Point.x >= m_Min.x) && (Point.x <= m_Max.x) && (Point.y >= m_Min.y) && (Point.y <= m_Max.y) && (Point.z >= m_Min.z) && (Point.z <= m_Max.z));
        }

        /** 
        * Checks whether a given box is fully encapsulated by this box.
        * 
        * @param Other The box to test for encapsulation within the bounding volume.
        * @return true if box is inside this volume.
        */
        //检测盒子是否完全包含另一个盒子
        public bool IsInside(UBox Other)
        {
            return (IsInside(Other.m_Min) && IsInside(Other.m_Max));
        }
    }
}
