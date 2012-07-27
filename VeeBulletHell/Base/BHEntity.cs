#region
using System.Collections.Generic;
using System.Drawing;
using SFML.Graphics;
using SFML.Window;
using SFMLStart;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeBulletHell.Data;

#endregion
namespace VeeBulletHell.Base
{
    public class BHEntity : Entity
    {
        #region Delegates
        public delegate void CollisionEvent(BHEntity mEntity, string mGroup);
        public delegate void OutOfBoundsEvent(BHEntity mEntity, Vector2i mDirection, Rectangle mBounds, int mBoundsOffset);
        #endregion
        private Sprite _sprite;

        public BHEntity(BHGame mGame, params string[] mGroups)
            : base(mGame.Manager, 0, mGroups)
        {
            Game = mGame;
            TimelinesUpdate = new List<Timeline>();
            TimelinesDrawBefore = new List<Timeline>();
            TimelinesDrawAfter = new List<Timeline>();
            Parameters = new Dictionary<string, object>();
            CollisionAgainstGroups = new List<string>();
            CollisionEntities = new List<BHEntity>();
        }

        public BHGame Game { get; set; }
        public List<Timeline> TimelinesUpdate { get; set; }
        public List<Timeline> TimelinesDrawBefore { get; set; }
        public List<Timeline> TimelinesDrawAfter { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Vector2i Position { get; set; }
        public Vector2i Velocity { get; set; }
        public BHCollisionShape CollisionShape { get; set; }
        public List<string> CollisionAgainstGroups { get; set; }
        public List<BHEntity> CollisionEntities { get; set; }
        public int BoundsOffset { get; set; }
        public bool IsIgnoringBounds { get; set; }
        public Sprite Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                _sprite.Origin = new Vector2f(Sprite.TextureRect.Width/2f, Sprite.TextureRect.Height/2f);
            }
        }
        public float SpriteRotation { get; set; }
        public Vector2i SpriteOffset { get; set; }
        public bool IsSpriteFixed { get; set; }
        public Animation Animation { get; set; }
        public event CollisionEvent OnCollision;
        public event OutOfBoundsEvent OnOutOfBounds;

        public void InvokeOnCollision(BHEntity mEntity, string mGroup)
        {
            CollisionEvent handler = OnCollision;
            if (handler != null) handler(mEntity, mGroup);
        }
        public void InvokeOnOutOfBounds(Vector2i mDirection)
        {
            OutOfBoundsEvent handler = OnOutOfBounds;
            if (handler != null) handler(this, mDirection, Game.Bounds, BoundsOffset);
        }

        public override void Update(float mFrameTime)
        {
            Position += new Vector2i((int) (Velocity.X*mFrameTime), (int) (Velocity.Y*mFrameTime));

            if (Animation != null)
            {
                Animation.Update(mFrameTime);
                Sprite.TextureRect = Animation.GetCurrentSubRect();
            }

            for (int i = TimelinesUpdate.Count - 1; i >= 0; i--)
            {
                Timeline timeline = TimelinesUpdate[i];
                timeline.Update(mFrameTime);
            }

            foreach (BHEntity entity in CollisionEntities) if (CollisionShape.IsColliding(entity.CollisionShape)) InvokeOnCollision(entity, "");
            foreach (string group in CollisionAgainstGroups) foreach (BHEntity entity in Manager.EntityDictionary[group]) if (CollisionShape.IsColliding(entity.CollisionShape)) InvokeOnCollision(entity, group);

            if (IsIgnoringBounds) return;
            if (Position.X < Game.Bounds.X + BoundsOffset) InvokeOnOutOfBounds(new Vector2i(-1, 0));
            if (Position.X > Game.Bounds.X + Game.Bounds.Width - BoundsOffset) InvokeOnOutOfBounds(new Vector2i(1, 0));
            if (Position.Y < Game.Bounds.Y + BoundsOffset) InvokeOnOutOfBounds(new Vector2i(0, -1));
            if (Position.Y > Game.Bounds.Y + Game.Bounds.Height - BoundsOffset) InvokeOnOutOfBounds(new Vector2i(0, 1));
        }
        public override void Draw()
        {
            /*
            if (Sprite != null)
            {
                Sprite.Rotation = 0;
                if (!IsSpriteFixed) Sprite.Rotation = Utils.Vector2ToAngle(new Vector2f(Velocity.X, Velocity.Y));
                Sprite.Rotation += SpriteRotation;
                Sprite.Position = new Vector2f(Position.X.ToPixels() + SpriteOffset.X, Position.Y.ToPixels() + SpriteOffset.Y);
            }
            */

            if (Sprite != null) Sprite.Rotation = 0;

            for (int i = TimelinesDrawBefore.Count - 1; i >= 0; i--)
            {
                Timeline timeline = TimelinesDrawBefore[i];
                timeline.Update(1);
            }

            if (Sprite != null)
            {
                if (!IsSpriteFixed && (Velocity.X != 0 || Velocity.Y != 0)) Sprite.Rotation = Utils.Math.Vectors.ToAngleDegrees(new Vector2f(Velocity.X, Velocity.Y));
                Sprite.Rotation += SpriteRotation;
                Sprite.Position = new Vector2f(Position.X.ToPixels() + SpriteOffset.X, Position.Y.ToPixels() + SpriteOffset.Y);

                Game.GameWindow.RenderWindow.Draw(Sprite);
            }

            for (int i = TimelinesDrawAfter.Count - 1; i >= 0; i--)
            {
                Timeline timeline = TimelinesDrawAfter[i];
                timeline.Update(1);
            }
        }
    }
}