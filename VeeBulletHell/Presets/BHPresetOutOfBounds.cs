#region
using System.Drawing;
using SFML.Window;
using VeeBulletHell.Base;

#endregion

namespace VeeBulletHell.Presets
{
    public static class BHPresetOutOfBounds
    {
        public static void Destroy(BHEntity mEntity, Vector2i mDirection, Rectangle mBounds, int mBoundsOffset) { mEntity.Destroy(); }
        public static void Stop(BHEntity mEntity, Vector2i mDirection, Rectangle mBounds, int mBoundsOffset)
        {
            if (mDirection.X == -1) mEntity.Position = new Vector2i(mBounds.X + mBoundsOffset, mEntity.Position.Y);
            if (mDirection.X == 1) mEntity.Position = new Vector2i(mBounds.X + mBounds.Width - mBoundsOffset, mEntity.Position.Y);
            if (mDirection.Y == -1) mEntity.Position = new Vector2i(mEntity.Position.X, mBounds.Y + mBoundsOffset);
            if (mDirection.Y == 1) mEntity.Position = new Vector2i(mEntity.Position.X, mBounds.Y - mBoundsOffset + mBounds.Height);
        }
    }
}