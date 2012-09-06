#region
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeBulletHell.Base;
using VeeBulletHell.Data;

#endregion
namespace VeeBulletHell.Presets
{
    public static class BHPresetBase
    {
        public static BHEntity Bullet(BHGame mGame, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0, long mRadius = 0)
        {
            BHEntity result = new BHEntity(mGame, "deadlytoplayer", "bullet") {Position = mPosition, Velocity = BHUtils.CalculateVelocity(mAngle, mSpeed)};
            result.CollisionShape = new BHCSCircle(result, mRadius*mRadius);

            result.OnOutOfBounds += BHPresetOutOfBounds.Destroy;

            return result;
        }
        public static BHEntity Player(BHGame mGame, int mSpeedNormal, int mSpeedFocus, Animation mAnimationLeft, Animation mAnimationRight, Animation mAnimationStill)
        {
            Timeline updateTimeline = new Timeline();
            Timeline drawTimeline = new Timeline();

            Sprite hitboxSprite = new Sprite(Assets.GetTexture("p_hitbox"));

            BHEntity result = new BHEntity(mGame, "character", "player") {DrawOrder = -10000, IsSpriteFixed = true, BoundsOffset = 10.ToUnits(), Animation = mAnimationStill};
            result.CollisionShape = new BHCSPoint(result);
            result.CollisionAgainstGroups.Add("deadlytoplayer");

            result.OnOutOfBounds += BHPresetOutOfBounds.Stop;
            result.OnCollision += (entity, group) => { if (group == "deadlytoplayer") Assets.Sounds["pldead00"].Play(); };

            updateTimeline.Action(() =>
                                  {
                                      int speed = mGame.Focus == 1 ? mSpeedFocus : mSpeedNormal;

                                      if (mGame.NextX != 0 && mGame.NextY != 0) speed = (int) (speed*0.7);

                                      if (mGame.NextX > 0 && mAnimationRight != null) result.Animation = mAnimationRight;
                                      else if (mGame.NextX < 0 && mAnimationLeft != null) result.Animation = mAnimationLeft;
                                      else if (mAnimationStill != null) result.Animation = mAnimationStill;

                                      result.Velocity = new Vector2i(mGame.NextX*speed, mGame.NextY*speed);

                                      hitboxSprite.Rotation++;
                                  });
            updateTimeline.Wait();
            updateTimeline.Goto();

            drawTimeline.Action(() =>
                                {
                                    if (mGame.Focus == 0) return;
                                    hitboxSprite.Origin = new Vector2f(32, 32);
                                    hitboxSprite.Position = new Vector2f(result.Position.X.ToPixels(), result.Position.Y.ToPixels());
                                    mGame.GameWindow.RenderWindow.Draw(hitboxSprite);
                                });
            drawTimeline.Wait();
            drawTimeline.Goto();

            result.TimelinesUpdate.Add(updateTimeline);
            result.TimelinesDrawAfter.Add(drawTimeline);

            return result;
        }
        public static BHEntity Laser(BHGame mGame, Sprite mSprite, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0,
                                     long mRadius = 0, int mLength = 0, bool mGrow = false, int mGrowSpeed = 0, bool mPrimed = false, int mPrimeTime = 50, int mPrimeSurviveTime = 50)
        {
            Timeline primeTimeline = new Timeline();
            Timeline growTimeline = new Timeline();
            Timeline drawTimeline = new Timeline();

            long radius = (mRadius/2)*(mRadius/2);

            BHEntity result = new BHEntity(mGame, "laser") {Position = mPosition, Velocity = BHUtils.CalculateVelocity(mAngle, mSpeed)};
            if (!mPrimed) result.AddGroup("deadlytoplayer");

            result.CollisionShape = mGrow ? new BHCSLine(result, mAngle, 0, radius) : new BHCSLine(result, mAngle, mLength, radius);
            result.BoundsOffset = -1000.ToUnits();

            result.Sprite = mSprite;
            result.Sprite.Rotation = mAngle;
            result.Sprite.Transform.Scale(1, result.Sprite.Texture.Size.Y);
            result.Sprite.Origin = new Vector2f(0, result.Sprite.Origin.Y);
            //result.Sprite.BlendMode = BlendMode.Add;
            result.IsSpriteFixed = true;

            float height = (mRadius*2).ToPixels();

            if (mPrimed) height = 1;
            // if (mPrimed) result.Sprite.BlendMode = BlendMode.Alpha;

            primeTimeline.Wait(mPrimeTime);
            primeTimeline.Action(() =>
                                 {
                                     result.AddGroup("deadlytoplayer");
                                     height = (mRadius*2).ToPixels();
                                     // result.Sprite.BlendMode = BlendMode.Add;
                                 });
            primeTimeline.Wait(mPrimeSurviveTime);
            primeTimeline.Action(result.Destroy);

            growTimeline.Action(() =>
                                {
                                    BHCSLine lineShape = (BHCSLine) result.CollisionShape;
                                    if (lineShape.Length < mLength)
                                    {
                                        lineShape.Length += mGrowSpeed;
                                        result.Velocity = new Vector2i(0, 0);
                                    }
                                    else
                                    {
                                        result.Velocity = BHUtils.CalculateVelocity(mAngle, mSpeed);
                                        growTimeline.Finished = true;
                                    }
                                });
            growTimeline.Wait();
            growTimeline.Goto();

            drawTimeline.Action(() =>
                                {
                                    BHCSLine lineShape = (BHCSLine) result.CollisionShape;
                                    result.Sprite.Rotation = lineShape.Degrees;
                                    result.Sprite.Transform.Scale(lineShape.Length.ToPixels(), height);
                                    result.Sprite.Origin = new Vector2f(0, result.Sprite.Origin.Y);
                                });
            drawTimeline.Wait();
            drawTimeline.Goto();

            if (mPrimed) result.TimelinesUpdate.Add(primeTimeline);
            result.TimelinesUpdate.Add(growTimeline);
            result.TimelinesDrawBefore.Add(drawTimeline);

            result.OnOutOfBounds += BHPresetOutOfBounds.Destroy;

            return result;
        }
        public static BHEntity Enemy(BHGame mGame, int mRadius, int mHealth)
        {
            Timeline deathTimeline = new Timeline();

            BHEntity result = new BHEntity(mGame, "enemy", "character");
            result.CollisionShape = new BHCSCircle(result, mRadius*mRadius);
            result.Parameters["health"] = mHealth;

            deathTimeline.Action(() =>
                                 {
                                     if ((int) result.Parameters["health"] <= 0)
                                     {
                                         result.Destroy();
                                         Assets.Sounds["se_enep00"].Play();
                                     }
                                 });
            deathTimeline.Wait();
            deathTimeline.Goto();

            result.TimelinesUpdate.Add(deathTimeline);

            return result;
        }
        public static BHEntity Boss(BHGame mGame, int mRadius, int mHealth)
        {
            Timeline deathTimeline = new Timeline();

            BHEntity result = new BHEntity(mGame, "boss", "enemy", "character");
            result.CollisionShape = new BHCSCircle(result, mRadius*mRadius);
            result.Parameters["health"] = mHealth;

            deathTimeline.Action(() => { if ((int) result.Parameters["health"] <= 0) {} });
            deathTimeline.Wait();
            deathTimeline.Goto();

            result.TimelinesUpdate.Add(deathTimeline);

            return result;
        }

        public static BHEntity Polygon(BHGame mGame, List<Vector2i> mVertices)
        {
            Timeline drawTimeline = new Timeline();

            BHEntity result = new BHEntity(mGame, "deadlytoplayer") {Position = mGame.Center};
            result.CollisionShape = new BHCSPolygon(result, mVertices.ToArray());

            drawTimeline.Action(() =>
                                {
                                    BHCSPolygon shape = (BHCSPolygon) result.CollisionShape;
                                    //foreach (Vector2i vertex in shape.Vertices) mGame.WindowManager.RenderWindow.Draw(new CircleShape(new Vector2f(vertex.X.ToPixels(), vertex.Y.ToPixels()), 2, Color.Red));
                                });
            drawTimeline.Wait();
            drawTimeline.Goto();

            result.TimelinesDrawBefore.Add(drawTimeline);

            return result;
        }
        public static BHEntity PlayerBullet(BHGame mGame, Vector2i mPosition = default(Vector2i), float mAngle = 0, int mSpeed = 0)
        {
            BHEntity result = new BHEntity(mGame, "playerbullet") {Position = mPosition, Velocity = BHUtils.CalculateVelocity(mAngle, mSpeed)};
            result.CollisionShape = new BHCSPoint(result);
            result.CollisionAgainstGroups.Add("enemy");

            result.OnCollision += (entity, group) =>
                                  {
                                      if (group == "enemy")
                                      {
                                          entity.Sprite.Color = new Color((byte)Utils.Random.Next(0, 255), (byte)Utils.Random.Next(0, 255), (byte)Utils.Random.Next(0, 255));
                                          entity.Parameters["health"] = (int) entity.Parameters["health"] - 1;
                                          result.Destroy();
                                          Assets.Sounds["se_damage00"].Play();
                                      }
                                  };

            result.OnOutOfBounds += BHPresetOutOfBounds.Destroy;

            return result;
        }
    }
}