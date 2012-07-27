#region
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeBulletHell.Base;

#endregion
namespace VeeBulletHell.Presets
{
    public static class BHPresetTimelines
    {
        public static void Fade(BHEntity mEntity, int mDelay = 0, bool mFadeIn = true, int mSpeedMultiplier = 3)
        {
            int value = mSpeedMultiplier;
            if (!mFadeIn) value *= -1;

            Timeline fadeTimeline = new Timeline();
            fadeTimeline.Wait(mDelay);
            fadeTimeline.Action(() =>
                                {
                                    byte r = mEntity.Sprite.Color.R;
                                    byte g = mEntity.Sprite.Color.G;
                                    byte b = mEntity.Sprite.Color.B;

                                    if (mEntity.Sprite.Color.A + value > 255) mEntity.Sprite.Color = new Color(r, g, b, 255);
                                    else if (mEntity.Sprite.Color.A + value < 0) mEntity.Sprite.Color = new Color(r, g, b, 0);
                                    else mEntity.Sprite.Color = new Color(r, g, b, (byte) (mEntity.Sprite.Color.A + value));
                                }
                );
            fadeTimeline.Wait();
            fadeTimeline.Goto(1, 255/mSpeedMultiplier);

            mEntity.TimelinesUpdate.Add(fadeTimeline);
        }
        public static void Kill(BHEntity mEntity, int mFrames)
        {
            Timeline timelineKill = new Timeline();
            timelineKill.Wait(mFrames);
            timelineKill.Action(mEntity.Destroy);
            mEntity.TimelinesUpdate.Add(timelineKill);
        }
        public static void MovementLerp(BHEntity mEntity, Vector2i mPosition, int mSteps = 100)
        {
            Vector2i startPosition = mEntity.Position;
            Vector2i endPosition = mPosition;

            Timeline timelineLerp = new Timeline();

            for (int i = 0; i < mSteps; i++)
            {
                float value = i/100f;

                timelineLerp.Action(() => mEntity.Position = Utils.Math.Vectors.Lerp(startPosition, endPosition, value)); // LAgs with 10000 test it out
                timelineLerp.Wait();
            }

            mEntity.TimelinesUpdate.Add(timelineLerp);
        }
    }
}