#region
using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using VeeBulletHell.Data;

#endregion
namespace VeeBulletHell.Base
{
    public abstract class BHCollisionShape
    {
        public abstract BHEntity Parent { get; set; }
        public abstract bool IsColliding(BHCollisionShape mShape);
    }

    public class BHCSPoint : BHCollisionShape
    {
        public BHCSPoint(BHEntity mParent) { Parent = mParent; }

        public override sealed BHEntity Parent { get; set; }

        public override bool IsColliding(BHCollisionShape mShape) { return mShape.IsColliding(this); }
    }
    public class BHCSCircle : BHCollisionShape
    {
        public BHCSCircle(BHEntity mParent, long mRadiusSquared)
        {
            Parent = mParent;
            RadiusSquared = mRadiusSquared;
        }

        public long RadiusSquared { get; set; }
        public override sealed BHEntity Parent { get; set; }

        public override bool IsColliding(BHCollisionShape mShape)
        {
            if (mShape is BHCSPoint)
            {
                double dx = Parent.Position.X - mShape.Parent.Position.X;
                double dy = Parent.Position.Y - mShape.Parent.Position.Y;

                return dx*dx + dy*dy < RadiusSquared;
            }
            if (mShape is BHCSCircle)
            {
                BHCSCircle circle = (BHCSCircle) mShape;

                double dx = Parent.Position.X - mShape.Parent.Position.X;
                double dy = Parent.Position.Y - mShape.Parent.Position.Y;

                return dx*dx + dy*dy < RadiusSquared + circle.RadiusSquared;
            }

            return mShape.IsColliding(this);
        }
    }
    public class BHCSLine : BHCollisionShape
    {
        public BHCSLine(BHEntity mParent, float mDegrees, int mLength, long mRadiusSquared)
        {
            Parent = mParent;
            Degrees = mDegrees;
            Length = mLength;
            RadiusSquared = mRadiusSquared;
        }

        public float Degrees { get; set; }
        public long Length { get; set; }
        public long RadiusSquared { get; set; }
        public override sealed BHEntity Parent { get; set; }

        public override bool IsColliding(BHCollisionShape mShape)
        {
            if (mShape is BHCSPoint)
            {
                Vector2i p1 = Parent.Position;
                var angleVector = Utils.Math.Vectors.OrbitDegrees(new SSVector2F(p1.X, p1.Y), Degrees, Length);
                Vector2i p2 = new Vector2i((int) angleVector.X, (int) angleVector.Y);
                Vector2i p3 = mShape.Parent.Position;

                double xDelta = p2.X - p1.X;
                double yDelta = p2.Y - p1.Y;

                double u = ((p3.X - p1.X)*xDelta + (p3.Y - p1.Y)*yDelta)/(xDelta*xDelta + yDelta*yDelta);

                Vector2i closestPoint;
                if (u < 0) closestPoint = p1;
                else if (u > 1) closestPoint = p2;
                else closestPoint = new Vector2i((int) (p1.X + u*xDelta), (int) (p1.Y + u*yDelta));

                double dx = closestPoint.X - p3.X;
                double dy = closestPoint.Y - p3.Y;

                return dx*dx + dy*dy < RadiusSquared;
            }
            if (mShape is BHCSCircle) return false;
            if (mShape is BHCSLine) return false;

            return mShape.IsColliding(this);
        }
    }
    public class BHCSPolygon : BHCollisionShape
    {
        public BHCSPolygon(BHEntity mParent, params Vector2i[] mVertices)
        {
            Parent = mParent;
            Vertices = mVertices.ToList();
        }

        public List<Vector2i> Vertices { get; set; }
        public bool IsFilled { get; set; }
        public override sealed BHEntity Parent { get; set; }

        public override bool IsColliding(BHCollisionShape mShape)
        {
            if (mShape is BHCSPoint)
            {
                Vector2i point = mShape.Parent.Position/BHUtils.Unit;

                int i, j;
                bool intersection = false;
                for (i = 0, j = Vertices.Count - 1; i < Vertices.Count; j = i++)
                {
                    if (((Vertices[i].Y.ToPixels() > point.Y) != (Vertices[j].Y.ToPixels() > point.Y)) &&
                        (point.X < (Vertices[j].X.ToPixels() - Vertices[i].X.ToPixels())*(point.Y - Vertices[i].Y.ToPixels())/
                         (Vertices[j].Y.ToPixels() - Vertices[i].Y.ToPixels()) + Vertices[i].X.ToPixels()))
                    {
                        mShape.Parent.Parameters["polygonvertex1"] = Vertices[i]*BHUtils.Unit;
                        mShape.Parent.Parameters["polygonvertex2"] = Vertices[j]*BHUtils.Unit;

                        intersection = !intersection;
                    }
                }

                return intersection;
            }
            if (mShape is BHCSCircle)
            {
                BHCSCircle circle = (BHCSCircle) mShape;

                for (var k = 0; k < Vertices.Count; k++)
                {
                    Vector2i p1 = Vertices[k];
                    int nextIndex = k + 1;
                    if (k == Vertices.Count - 1) nextIndex = 0;
                    Vector2i p2 = Vertices[nextIndex];
                    Vector2i p3 = mShape.Parent.Position;

                    double xDelta = p2.X - p1.X;
                    double yDelta = p2.Y - p1.Y;

                    double u = ((p3.X - p1.X)*xDelta + (p3.Y - p1.Y)*yDelta)/(xDelta*xDelta + yDelta*yDelta);

                    Vector2i closestPoint;
                    if (u < 0) closestPoint = p1;
                    else if (u > 1) closestPoint = p2;
                    else closestPoint = new Vector2i((int) (p1.X + u*xDelta), (int) (p1.Y + u*yDelta));

                    double dx = closestPoint.X - p3.X;
                    double dy = closestPoint.Y - p3.Y;

                    if (dx*dx + dy*dy < circle.RadiusSquared)
                    {
                        mShape.Parent.Parameters["polygonvertex1"] = p1;
                        mShape.Parent.Parameters["polygonvertex2"] = p2;
                        return true;
                    }
                }

                if (!IsFilled) return false;

                Vector2i point = mShape.Parent.Position/BHUtils.Unit;

                int i, j;
                bool intersection = false;
                for (i = 0, j = Vertices.Count - 1; i < Vertices.Count; j = i++)
                {
                    if (((Vertices[i].Y.ToPixels() > point.Y) != (Vertices[j].Y.ToPixels() > point.Y)) &&
                        (point.X < (Vertices[j].X.ToPixels() - Vertices[i].X.ToPixels())*(point.Y - Vertices[i].Y.ToPixels())/
                         (Vertices[j].Y.ToPixels() - Vertices[i].Y.ToPixels()) + Vertices[i].X.ToPixels())) intersection = !intersection;
                }

                return intersection;
            }
            if (mShape is BHCSLine) return false;
            if (mShape is BHCSPolygon) return false;

            return false;
        }
    }
}