using UnityEngine;
using System;

namespace UnityMathUtility   
{
    class UMath
    {
        public static float PI = 3.1415926535897932f;
        public static float HALF_PI = 1.57079632679f;
        public static float KINDA_SMALL_NUMBER = 1.0E-4F;
        public static float SMALL_NUMBER = 1.0E-8F;

#region 通用函数

        //绝对值
        public static int Abs(int A)
        {
            return (A >= 0) ? A : -A;
        }

        public static float Abs(float A)
        {
            return (A >= 0) ? A : -A;
        }

        //取值的符号
        public static int Sign(int A)
        {
            return (A > 0) ? 1 : ((A < 0) ? -1 : 0); 
        }

        public static float Sign(float A)
        {
            return (A > 0) ? 1 : ((A < 0) ? -1 : 0);
        }

        //最大值
        public static float Max(float A, float B)
        {
            return (A >= B) ? A : B;
        }

        public static float Max3(float A, float B, float C)
        {
            return Max(Max(A, B), C);
        }

        //最小值
        public static float Min(float A, float B)
        {
            return (A <= B) ? A : B;
        }

        public static float Min3(float A, float B, float C)
        {
            return Min( Min(A, B), C);
        }

        //平方
        public static int Square(int A)
        {
            return A * A;
        }

        public static float Square(float A)
        {
            return A * A;
        }

        //裁切值
        public static int Clamp(int X, int Min, int Max)
        {
            return X < Min ? Min : X < Max ? X : Max; 
        }

        public static float Clamp(float X, float Min, float Max)
        {
            return X < Min ? Min : X < Max ? X : Max;
        }

        //笛卡尔坐标系转换为极坐标系
        public static void CartesianToPolar(float X, float Y, ref float OutRad, ref float OutAngle)
        {
            OutRad = (float)Math.Sqrt(Square(X)+Square(Y));
            OutAngle = (float)Math.Atan2(Y, X);
        }

        //笛卡尔坐标系转换为极坐标系
        public static void CartesianToPolar(Vector2 InCart, Vector2 OutPolar)
        {
            OutPolar.x = (float)Math.Sqrt(Square(InCart.x) + Square(InCart.y));
            OutPolar.y = (float)Math.Atan2(InCart.y, InCart.x);
        }

        //极坐标系转换为笛卡尔坐标系
        public static void PolarToCartesian(float Rad, float Ang, ref float OutX, ref float OutY)
        {
            OutX = Rad * (float)Math.Cos(Ang);
            OutY = Rad * (float)Math.Sin(Ang);
        }

        //极坐标系转换为笛卡尔坐标系
        public static void PolarToCartesian(Vector2 InPolar, Vector2 OutCart)
        {
            OutCart.x = InPolar.x * (float)Math.Cos(InPolar.y);
            OutCart.y = InPolar.x * (float)Math.Sin(InPolar.y);
        }

#endregion

#region 插值函数

        //线性插值，Alpha为样条距离
        public static Vector3 Lerp(Vector3 A, Vector3 B, float Alpha)
        {
            return (A + Alpha * (B - A));
        }

        public static float Lerp(float A, float B, float Alpha)
        {
            return (A + Alpha * (B - A));
        }

        public static Quaternion Lerp(Quaternion A, Quaternion B, float Alpha)
        {
            return Quaternion.Lerp(A, B, Alpha);
        }

        public static Quaternion SLerp(Quaternion A, Quaternion B, float Alpha)
        {
            return Quaternion.Slerp(A, B, Alpha);
        }

        //稳定线性插值
        public static Vector3 LerpStable(Vector3 A, Vector3 B, float Alpha)
        {
            return ((A * (1.0f - Alpha)) + (B * Alpha));
        }

        public static float LerpStable(float A, float B, float Alpha)
        {
            return ((A * (1.0f - Alpha)) + (B * Alpha));
        }

        //二维线性插值，用四个点进行插值，先在X方向进行插值，再在Y方向进行插值
        public static Vector3 BiLerp(Vector3 P00, Vector3 P10, Vector3 P01, Vector3 P11, float FracX, float FracY)
        {
            return Lerp(
                Lerp(P00, P10, FracX),
                Lerp(P01, P11, FracX),
                FracY
                );
        }

        //入口处缓动插值，exp控制曲线角度
        public static Vector3 InterpEaseIn(Vector3 A, Vector3 B, float Alpha, float Exp)
        {
            float modifiedAlpha = (float)Math.Pow(Alpha, Exp);
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpEaseIn(float A, float B, float Alpha, float Exp)
        {
            float modifiedAlpha = (float)Math.Pow(Alpha, Exp);
            return Lerp(A, B, modifiedAlpha);
        }

        //出口处缓动插值
        public static Vector3 InterpEaseOut(Vector3 A, Vector3 B, float Alpha, float Exp)
        {
            float modifiedAlpha = 1.0f - (float)Math.Pow(1.0f - Alpha, Exp);
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpEaseOut(float A, float B, float Alpha, float Exp)
        {
            float modifiedAlpha = 1.0f - (float)Math.Pow(1.0f - Alpha, Exp);
            return Lerp(A, B, modifiedAlpha);
        }

        //出入口两边都有缓动
        public static Vector3 InterpEaseInOut(Vector3 A, Vector3 B, float Alpha, float Exp)
        {
            return Lerp(A, B, (Alpha < 0.5f) ? 
                InterpEaseIn(0f, 1.0f, Alpha*2.0f, Exp) * 0.5f :
                InterpEaseOut(0f, 1.0f, Alpha*2.0f - 1.0f, Exp)*0.5f + 0.5f
                );
        }

        public static float InterpEaseInOut(float A, float B, float Alpha, float Exp)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
                InterpEaseIn(0f, 1.0f, Alpha * 2.0f, Exp) * 0.5f :
                InterpEaseOut(0f, 1.0f, Alpha * 2.0f - 1.0f, Exp) * 0.5f + 0.5f
                );
        }

        //入口处正弦曲线插值
        public static Vector3 InterpSinIn(Vector3 A, Vector3 B, float Alpha)
        {
            float modifiedAlpha = -1.0f * (float)Math.Cos(Alpha * HALF_PI) + 1.0f;
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpSinIn(float A, float B, float Alpha)
        {
            float modifiedAlpha = -1.0f * (float)Math.Cos(Alpha * HALF_PI) + 1.0f;
            return Lerp(A, B, modifiedAlpha);
        }

        //出口处正弦曲线插值
        public static Vector3 InterpSinOut(Vector3 A, Vector3 B, float Alpha)
        {
            float modifiedAlpha = (float)Math.Sin(Alpha * HALF_PI);
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpSinOut(float A, float B, float Alpha)
        {
            float modifiedAlpha = (float)Math.Sin(Alpha * HALF_PI);
            return Lerp(A, B, modifiedAlpha);
        }

        //出入口正弦曲线插值
        public static Vector3 InterpSinInOut(Vector3 A, Vector3 B, float Alpha)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
            InterpSinIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpSinOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        public static float InterpSinInOut(float A, float B, float Alpha)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
            InterpSinIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpSinOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        //指数插值
        public static Vector3 InterpExpoIn(Vector3 A, Vector3 B, float Alpha)
        {
            float modifiedAlpha = (Alpha == 0.0f) ? 0.0f : (float)Math.Pow(2.0f, 10.0f * (Alpha - 1.0f));
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpExpoIn(float A, float B, float Alpha)
        {
            float modifiedAlpha = (Alpha == 0.0f) ? 0.0f : (float)Math.Pow(2.0f, 10.0f * (Alpha - 1.0f));
            return Lerp(A, B, modifiedAlpha);
        }

        public static Vector3 InterpExpoOut(Vector3 A, Vector3 B, float Alpha)
        {
            float modifiedAlpha = (Alpha == 1.0f) ? 1.0f : -(float)Math.Pow(2.0f, -10.0f * Alpha) + 1.0f;
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpExpoOut(float A, float B, float Alpha)
        {
            float modifiedAlpha = (Alpha == 1.0f) ? 1.0f : -(float)Math.Pow(2.0f, -10.0f * Alpha) + 1.0f;
            return Lerp(A, B, modifiedAlpha);
        }

        public static Vector3 InterpExpoInOut(Vector3 A, Vector3 B, float Alpha)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
            InterpExpoIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpExpoOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        public static float InterpExpoInOut(float A, float B, float Alpha)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
            InterpExpoIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpExpoOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        //圆形插值
        public static Vector3 InterpCircularIn(Vector3 A, Vector3 B, float Alpha)
        {
            float modifiedAlpha = -1.0f * ((float)Math.Sqrt(1.0f - Alpha * Alpha) - 1.0f);
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpCircularIn(float A, float B, float Alpha)
        {
            float modifiedAlpha = -1.0f * ((float)Math.Sqrt(1.0f - Alpha * Alpha) - 1.0f);
            return Lerp(A, B, modifiedAlpha);
        }

        public static Vector3 InterpCircularOut(Vector3 A, Vector3 B, float Alpha)
        {
            Alpha -= 1.0f;
		    float modifiedAlpha = (float)Math.Sqrt(1.0f - Alpha  * Alpha);
            return Lerp(A, B, modifiedAlpha);
        }

        public static float InterpCircularOut(float A, float B, float Alpha)
        {
            Alpha -= 1.0f;
            float modifiedAlpha = (float)Math.Sqrt(1.0f - Alpha * Alpha);
            return Lerp(A, B, modifiedAlpha);
        }

        public static Vector3 InterpCircularInOut(Vector3 A, Vector3 B, float Alpha)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
            InterpCircularIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpCircularOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        public static float InterpCircularInOut(float A, float B, float Alpha)
        {
            return Lerp(A, B, (Alpha < 0.5f) ?
            InterpCircularIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpCircularOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        //对当前位置和目标位置做插值
        public static Vector3 InterpTo(Vector3 Current, Vector3 Target, float DeltaTime, float InterpSpeed)
        {
            // If no interp speed, jump to target value
	        if( InterpSpeed <= 0.0f )
	        {
		        return Target;
	        }

	        // Distance to reach
	        Vector3 Dist = Target - Current;

	        // If distance is too small, just set the desired location
	        if( Dist.sqrMagnitude < KINDA_SMALL_NUMBER )
	        {
		        return Target;
	        }

	        // Delta Move, Clamp so we do not over shoot.
	        Vector3 deltaMove = Dist * Clamp(DeltaTime * InterpSpeed, 0.0f, 1.0f);
            return Current + deltaMove;
        }

#endregion

#region    碰撞检测相关
        /**
	    * Find the intersection of a line and an offset plane. Assumes that the
	    * line and plane do indeed intersect; you must make sure they're not
	    * parallel before calling.
	    *
	    * @param Point1 the first point defining the line (直线上一点)
	    * @param Point2 the second point defining the line （直线的方向）
	    * @param PlaneOrigin the origin of the plane    （平面上一点）
	    * @param PlaneNormal the normal of the plane    （平面法向量）
	    *
	    * @return The point of intersection between the line and the plane.
	    */
        //获得直线与平面的交点，使用前要确定线面不是平行的(可通过直线向量点乘平面法线向量是否为0来判断)
        public static Vector3 LinePlaneIntersection(Vector3 Point1, Vector3 Point2, Vector3 PlaneOrigin, Vector3 PlaneNormal)
        {
            return Point1
                + (Point2 - Point1)
                * (Vector3.Dot((PlaneOrigin - Point1), PlaneNormal) / Vector3.Dot((Point2 - Point1), PlaneNormal));
        }

        /**
        * Calculates distance between plane and a point.
        *
        * @param Point The other point.                (任意点)
        * @param PlaneOrigin The point of the panel.  （平面上一点）
        * @param PlaneNormal The normal of the panel. （平面法向量）
        * @return >0: point is in front of the plane, <0: behind, =0: on the plane.
        */
        //任意点到平面的距离
        public static float PointPlaneDistance(Vector3 Point, Vector3 PlaneOrigin, Vector3 PlaneNormal)
        {
            return Vector3.Dot(Point, PlaneNormal) - Vector3.Dot(PlaneOrigin, PlaneNormal);
        }

        /**
        * Calculates distance between plane and a point.
        *
        * @param Point The other point.               (任意点)
        * @param Plane The panel.                     （平面）
        * @return >0: point is in front of the plane, <0: behind, =0: on the plane.
        */
        //任意点到平面的距离
        public static float PointPlaneDistance(Vector3 Point, UPlane Plane)
        {
            return Plane.PointPlaneDistance(Point);
        }

        /**
        * Calculate the projection of a point on the given plane.
        *
        * @param Point The point to project onto the plane  (需要投影到平面上的点)
        * @param PlaneOrigin The point of the panel.        （平面上一点）
        * @param PlaneNormal The normal of the panel.       （平面法向量）
        * @return Projection of Point onto Plane             （投影点）
        */
        //任意点在平面上的投影
        public static Vector3 PointPlaneProject(Vector3 Point, Vector3 PlaneOrigin, Vector3 PlaneNormal)
        {
            return Point - PointPlaneDistance(Point, PlaneOrigin, PlaneNormal) * PlaneNormal;
        }

        /**
        * Calculates the distance of a given Point in world space to a given line,
        * defined by the vector couple (Origin, Direction).
        *
        * @param	Point				Point to check distance to line   (需要计算到直线距离的点)
        * @param	Direction			Vector indicating the direction of the line. Not required to be normalized. （直线向量）
        * @param	Origin				Point of reference used to calculate distance （直线上某一点）
        * @param	OutClosestPoint	optional point that represents the closest point projected onto Axis （点到直线的最近点）
        *
        * @return	distance of Point from line defined by (Origin, Direction)
        */
        //点到直线的最近点及距离
        public static float PointDistToLine(Vector3 Point, Vector3 Direction, Vector3 Origin, Vector3 OutClosestPoint)
        {
            Vector3 normalDir = Direction.normalized;
            OutClosestPoint = Origin + (normalDir * (Vector3.Dot((Point - Origin), normalDir)));
            return (OutClosestPoint - Point).magnitude;
        }

        /**
        * Returns closest point on a segment to a given point.
        * The idea is to project point on line formed by segment.
        * Then we see if the closest point on the line is outside of segment or inside.
        *
        * @param	Point			point for which we find the closest point on the segment （需要计算到线段距离的点）
        * @param	StartPoint		StartPoint of segment （线段起点）
        * @param	EndPoint		EndPoint of segment  （线段终点）
        *
        * @return	point on the segment defined by (StartPoint, EndPoint) that is closest to Point.
        */
        //点到线段的最近点
        public static Vector3 ClosestPointOnSegment(Vector3 Point, Vector3 StartPoint, Vector3 EndPoint)
        {
            Vector3 Segment = EndPoint - StartPoint;
            Vector3 VectToPoint = Point - StartPoint;

            // See if closest point is before StartPoint
            float Dot1 = Vector3.Dot(VectToPoint, Segment);
            if (Dot1 <= 0)
            {
                return StartPoint;
            }

            // See if closest point is beyond EndPoint
            float Dot2 = Vector3.Dot(Segment, Segment);
            if (Dot2 <= Dot1)
            {
                return EndPoint;
            }

            // Closest Point is within segment
            return StartPoint + Segment * (Dot1 / Dot2);
        }

        /**
        * Returns distance from a point to the closest point on a segment.
        *
        * @param	Point			point to check distance for 
        * @param	StartPoint		StartPoint of segment
        * @param	EndPoint		EndPoint of segment
        *
        * @return	closest distance from Point to segment defined by (StartPoint, EndPoint).
        */
        //点到线段的距离
        public static float PointDistToSegment(Vector3 Point, Vector3 StartPoint, Vector3 EndPoint)
        {
            Vector3 ClosestPoint = ClosestPointOnSegment(Point, StartPoint, EndPoint);
	        return (Point - ClosestPoint).magnitude;
        }

        /**
        * returns the time (t) of the intersection of the passed segment and a plane (could be <0 or >1)  
        * @param StartPoint - start point of segment  
        * @param EndPoint   - end point of segment
        * @param PlaneOrigin The point of the panel.
        * @param PlaneNormal The normal of the panel.
        * @return time(T) of intersection
        */
        //p(t)=p0+td，(p0+td)*n=d, t=d-p0*n/d*n  (n为平面法向量，d为原点到平面的距离，p0为线段上一点)
        //得到线段跟平面相交点的T值(调用此函数前必须保证线段跟平面不是平行的)
        public static float GetTForSegmentPlaneIntersect(Vector3 StartPoint, Vector3 EndPoint, Vector3 PlaneOrigin, Vector3 PlaneNormal)
        {
            return (Vector3.Dot(PlaneOrigin, PlaneNormal) - Vector3.Dot(StartPoint, PlaneNormal)) / (Vector3.Dot((EndPoint - StartPoint), PlaneNormal));	
        }

        /**
        * Returns true if there is an intersection between the segment specified by StartPoint and Endpoint, and
        * the plane on which polygon Plane lies. If there is an intersection, the point is placed in out_IntersectionPoint
        * @param StartPoint - start point of segment
        * @param EndPoint   - end point of segment
        * @param PlaneOrigin The point of the panel.
        * @param PlaneNormal The normal of the panel.
        * @param out_IntersectionPoint - out var for the point on the segment that intersects the mesh (if any)
        * @return true if intersection occurred
        */
        //判断线段跟平面是否相交,并得到相交的点
        public static bool SegmentPlaneIntersection(Vector3 StartPoint, Vector3 EndPoint, Vector3 PlaneOrigin, Vector3 PlaneNormal, Vector3 out_IntersectionPoint)
        {
            float T = GetTForSegmentPlaneIntersect(StartPoint, EndPoint, PlaneOrigin, PlaneNormal);
            // If the parameter value is not between 0 and 1, there is no intersection
            if (T > -KINDA_SMALL_NUMBER && T < 1.0f + KINDA_SMALL_NUMBER)
            {
                out_IntersectionPoint = StartPoint + T * (EndPoint - StartPoint);
                return true;
            }
            return false;
        }

        /**
        * Returns true if there is an intersection between the segment specified by StartPoint and Endpoint, and
        * the Triangle defined by A, B and C. If there is an intersection, the point is placed in out_IntersectionPoint
        * @param StartPoint - start point of segment
        * @param EndPoint   - end point of segment
        * @param A, B, C	- points defining the triangle 
        * @param OutIntersectPoint - out var for the point on the segment that intersects the triangle (if any)
        * @param OutNormal - out var for the triangle normal
        * @return true if intersection occurred
        */
        //判断线段跟三角形是否相交
        public static bool SegmentTriangleIntersection(Vector3 StartPoint, Vector3 EndPoint, Vector3 A, Vector3 B, Vector3 C, Vector3 OutIntersectPoint, Vector3 OutTriangleNormal)
        {
            Vector3 BA = A - B;
            Vector3 CB = B - C;
            Vector3 TriNormal = Vector3.Cross(BA, CB).normalized;

            bool bCollide = SegmentPlaneIntersection(StartPoint, EndPoint, A, TriNormal, OutIntersectPoint);
            if (!bCollide)
            {
                return false;
            }

            Vector3 BaryCentric = ComputeBaryCentric2D(OutIntersectPoint, A, B, C);
            if (BaryCentric.x > 0.0f && BaryCentric.y > 0.0f && BaryCentric.z > 0.0f)
            {
                OutTriangleNormal = TriNormal;
                return true;
            }
            return false;
        }

        static Vector3 ComputeBaryCentric2D(Vector3 Point, Vector3 A, Vector3 B, Vector3 C)
        {
            // Compute the normal of the triangle
            Vector3 TriNorm = Vector3.Cross((B - A), (C - A));

	        Vector3 N = TriNorm.normalized;

	        // Compute twice area of triangle ABC
	        float AreaABCInv = 1.0f / Vector3.Dot(N, TriNorm);

	        // Compute a contribution
	        float AreaPBC = Vector3.Dot(N, (Vector3.Cross((B-Point), (C-Point))));
	        float a = AreaPBC * AreaABCInv;

	        // Compute b contribution
	        float AreaPCA = Vector3.Dot(N, (Vector3.Cross((C-Point), (A-Point))));
	        float b = AreaPCA * AreaABCInv;

	        // Compute c contribution
	        return new Vector3(a, b, 1.0f - a - b);
        }

        //点是否在某个三角型内(分离轴法为基本原理)
        public static bool PointInTriangle(Vector3 Point, Vector3 A, Vector3 B, Vector3 C)
        {
            //Figure out what region the point is in and compare against that "point" or "edge"
	        Vector3 BA = A - B;
	        Vector3 AC = C - A;
	        Vector3 CB = B - C;
	        Vector3 TriNormal = Vector3.Cross(BA, CB);

	        // Get the planes that define this triangle
	        // edges BA, AC, BC with normals perpendicular to the edges facing outward
            UPlane[] panels = new UPlane[3] { 
                new UPlane(B, Vector3.Cross(TriNormal, BA)), 
                new UPlane(A, Vector3.Cross(TriNormal, AC)), 
                new UPlane(C, Vector3.Cross(TriNormal, CB)) 
            };
	        int PlaneHalfspaceBitmask = 0;

	        //Determine which side of each plane the test point exists
	        for (int i=0; i<3; i++)
	        {
                //分离轴法
                if (PointPlaneDistance(Point, panels[i]) > 0.0f)
		        {
			        PlaneHalfspaceBitmask |= (1 << i);
		        }
	        }

	        if (PlaneHalfspaceBitmask == 0)
	        {
                return true;
	        }

            return false;
        }

        //点是否在某个四边形内
        public static bool PointInTetrahedron(Vector3 Point, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
        {
            //Figure out what region the point is in and compare against that "point" or "edge"
            Vector3 BA = A - B;
            Vector3 CB = B - C;
            Vector3 DC = C - D;
            Vector3 AD = D - A;
            Vector3 TriNormal = Vector3.Cross(BA, CB);

            // Get the planes that define this triangle
            // edges BA, AC, BC with normals perpendicular to the edges facing outward
            UPlane[] panels = new UPlane[4] { 
                new UPlane(B, Vector3.Cross(TriNormal, BA)), 
                new UPlane(C, Vector3.Cross(TriNormal, CB)), 
                new UPlane(D, Vector3.Cross(TriNormal, DC)),
                new UPlane(A, Vector3.Cross(TriNormal, AD)) 
            };
            int PlaneHalfspaceBitmask = 0;

            //Determine which side of each plane the test point exists
            for (int i = 0; i < 4; i++)
            {
                if (PointPlaneDistance(Point, panels[i]) > 0.0f)
                {
                    PlaneHalfspaceBitmask |= (1 << i);
                }
            }

            if (PlaneHalfspaceBitmask == 0)
            {
                return true;
            }

            return false;
        }

        /**
        * Returns closest point on a triangle to a point.
        * The idea is to identify the halfplanes that the point is
        * in relative to each triangle segment "plane"
        *
        * @param	Point			point to check distance for
        * @param	A,B,C			counter clockwise ordering of points defining a triangle
        *
        * @return	Point on triangle ABC closest to given point
        */
        //点到三角形的最近点
        public static Vector3 ClosestPointOnTriangleToPoint(Vector3 Point, Vector3 A, Vector3 B, Vector3 C)
        {
            //Figure out what region the point is in and compare against that "point" or "edge"
	        Vector3 BA = A - B;
	        Vector3 AC = C - A;
	        Vector3 CB = B - C;
	        Vector3 TriNormal = Vector3.Cross(BA, CB);

	        // Get the planes that define this triangle
	        // edges BA, AC, BC with normals perpendicular to the edges facing outward
            UPlane[] panels = new UPlane[3] { 
                new UPlane(B, Vector3.Cross(TriNormal, BA)), 
                new UPlane(A, Vector3.Cross(TriNormal, AC)), 
                new UPlane(C, Vector3.Cross(TriNormal, CB)) 
            };
	        int PlaneHalfspaceBitmask = 0;

	        //Determine which side of each plane the test point exists
	        for (int i=0; i<3; i++)
	        {
                if (PointPlaneDistance(Point, panels[i]) > 0.0f)
		        {
			        PlaneHalfspaceBitmask |= (1 << i);
		        }
	        }

	        Vector3 Result = new Vector3(Point.x, Point.y, Point.z);
	        switch (PlaneHalfspaceBitmask)
	        {
	        case 0: //000 Inside
		        return PointPlaneProject(Point, A, TriNormal);
	        case 1:	//001 Segment BA
		        Result = ClosestPointOnSegment(Point, B, A);
		        break;
	        case 2:	//010 Segment AC
		        Result = ClosestPointOnSegment(Point, A, C);
		        break;
	        case 3:	//011 point A
		        return A;
	        case 4: //100 Segment BC
		        Result = ClosestPointOnSegment(Point, B, C);
		        break;
	        case 5: //101 point B
		        return B;
	        case 6: //110 point C
		        return C;
	        default:
		        break;
	        }

	        return Result;
        }

        //判断多个点是否共面
        public static bool PointsAreCoplanar(Vector3[] Points, float Tolerance = 0.1f)
        {
            //less than 4 points = coplanar
	        if (Points.Length < 4)
	        {
		        return true;
	        }

	        //Get the Normal for plane determined by first 3 points
	        Vector3 Normal = Vector3.Cross(Points[2] - Points[0], Points[1] - Points[0]).normalized;

	        int Total = Points.Length;
	        for (int v = 3; v < Total; v++)
	        {
		        //Abs of PointPlaneDist, dist should be 0
                if (Abs(PointPlaneDistance(Points[v], Points[0], Normal)) > Tolerance)
		        {
			        return false;
		        }
	        }

	        return true;
        }

        //二次贝塞尔曲线，有3个控制点
        public static Vector3 Bezier2(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;
            return p;
        }

        //三次贝塞尔曲线，有4个控制点 
        public static Vector3 Bezier3(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
#endregion
    }
}
