#region
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using VeeBulletHell.Base;
using VeeBulletHell.Data;

#endregion

namespace VeeBulletHell.Presets
{
    public static class BHPresetBullets
    {
        public static BHEntity Pellet(BHGame mGame, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0)
        {
            BHEntity result = BHPresetBase.Bullet(mGame, mPosition, mAngle, mSpeed, 3.4f.ToUnits());
            result.Sprite = new Sprite(Assets.GetTexture("b_pellet"));
            return result;
        }
        public static BHEntity RoundSmall(BHGame mGame, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0)
        {
            BHEntity result = BHPresetBase.Bullet(mGame, mPosition, mAngle, mSpeed, 5.ToUnits());
            result.Sprite = new Sprite(Assets.GetTexture("b_circlesmall"));
            result.IsSpriteFixed = true;
            return result;
        }
        public static BHEntity RoundFull(BHGame mGame, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0)
        {
            BHEntity result = BHPresetBase.Bullet(mGame, mPosition, mAngle, mSpeed, 5.ToUnits());
            result.Sprite = new Sprite(Assets.GetTexture("b_circlesmallfull"));
            result.IsSpriteFixed = true;
            return result;
        }
        public static BHEntity Glow1(BHGame mGame, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0)
        {
            BHEntity result = BHPresetBase.Bullet(mGame, mPosition, mAngle, mSpeed, 7.ToUnits());
            result.Sprite = new Sprite(Assets.GetTexture("b_glow1"));
            result.IsSpriteFixed = true;
            return result;
        }
    }
}