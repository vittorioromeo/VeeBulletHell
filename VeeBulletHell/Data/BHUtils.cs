#region
using System;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using VeeBulletHell.Base;

#endregion
namespace VeeBulletHell.Data
{
    public static class BHUtils
    {
        public static int Unit = 100;
        public static int ToUnits(this int mPixels) { return mPixels*Unit; }
        public static int ToPixels(this int mUnits) { return mUnits/Unit; }
        public static int ToUnits(this float mPixels) { return (int) (mPixels*Unit); }
        public static float ToPixels(this float mUnits) { return mUnits/Unit; }
        public static long ToUnits(this long mPixels) { return mPixels*Unit; }
        public static long ToPixels(this long mUnits) { return mUnits/Unit; }

        public static float GetAngleTowards(BHEntity mStart, BHEntity mEnd) { return (float) Math.Atan2(mEnd.Position.Y - mStart.Position.Y, mEnd.Position.X - mStart.Position.X)*57.3f; }
        public static Vector2i CalculateVelocity(float mDegrees, int mSpeed)
        {
            var direction = Utils.Math.Angles.ToVectorDegrees(mDegrees);
            return new Vector2i((int) (direction.X*mSpeed), (int) (direction.Y*mSpeed));
        }
    }
}