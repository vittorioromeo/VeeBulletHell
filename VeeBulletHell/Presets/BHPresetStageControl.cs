#region
using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SFMLStart;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Utilities.Timelines;
using VeeBulletHell.Base;

#endregion
namespace VeeBulletHell.Presets
{
    public static class BHPresetStageControl
    {
        public static void CutIn(BHGame mGame, BHStage mStage, Texture mTexture, bool mText = false, string mBackgroundText = "spell attack",
                                 Vector2i mVelocity = default(Vector2i), Vector2i mOffset = default(Vector2i), int mLength = 100)
        {
            Timeline cutInTimeline = new Timeline();

            List<Text> texts = new List<Text>();
            List<Action> drawEvents = new List<Action>();

            if (mText)
            {
                for (int iX = -2; iX < 3; iX++)
                {
                    for (int i = 0; i < 35; i++)
                    {
                        Text temp = new Text(mBackgroundText, Font.DefaultFont)
                                    {
                                        Position = new Vector2f(300 + (iX*84), (i*35) - 200),
                                        Color = Color.White,
                                        CharacterSize = 15,
                                        Rotation = 25
                                    };

                        texts.Add(temp);

                        Action drawEvent = () => mGame.GameWindow.RenderWindow.Draw(temp);
                        drawEvents.Add(drawEvent);
                        mGame.AddDrawAction(drawEvent);
                    }
                }
            }

            Sprite cutInSprite = new Sprite(mTexture)
                                 {
                                     Color = new Color(255, 255, 255, 0)
                                 };

            BHEntity cutInEntity = new BHEntity(mGame)
                                   {
                                       DrawOrder = 100000,
                                       Sprite = cutInSprite,
                                       Position = mGame.Center + mOffset
                                   };

            BHPresetTimelines.Fade(cutInEntity, 0, true, 16);
            BHPresetTimelines.Fade(cutInEntity, mLength, false, 16);

            cutInTimeline.Action(() => { foreach (Text text in texts) text.Position -= new Vector2f(3, 1); });
            cutInTimeline.Action(() => cutInEntity.Position += mVelocity);
            cutInTimeline.Wait();
            cutInTimeline.Goto(mTimes: 200);
            cutInTimeline.Action(() =>
                                 {
                                     foreach (Action drawEvent in drawEvents) mGame.RemoveDrawAction(drawEvent);
                                     cutInEntity.Destroy();
                                 });

            mStage.TimelinesUpdate.Add(cutInTimeline);
        }
        public static void SpellCard(BHGame mGame, BHStage mStage, BHEntity mBoss, string mSpellCardName, int mHealth, int mTime, int mScore, Timeline mOnEnd, params Timeline[] mTimelines)
        {
            Timeline spellCardTimeline = new Timeline();

            mBoss.Parameters["health"] = mHealth;

            List<Action> drawEvents = new List<Action>();

            Text spellCardText = new Text(mSpellCardName, Font.DefaultFont)
                                 {
                                     Position = new Vector2f(32, 16),
                                     Color = Color.White,
                                     CharacterSize = 15,
                                 };
            Text mTimeText = new Text(mTime.ToString(), Font.DefaultFont)
                             {
                                 Position = new Vector2f(384, 16),
                                 Color = Color.White,
                                 CharacterSize = 15
                             };

            Action spellCardTextDrawEvent = () => mGame.GameWindow.RenderWindow.Draw(spellCardText);
            drawEvents.Add(spellCardTextDrawEvent);
            mGame.AddDrawAction(spellCardTextDrawEvent);

            Action timeTextDrawEvent = () => mGame.GameWindow.RenderWindow.Draw(mTimeText);
            drawEvents.Add(timeTextDrawEvent);
            mGame.AddDrawAction(timeTextDrawEvent);

            mStage.TimelinesUpdate.AddRange(mTimelines);

            Assets.Sounds["cat00"].Play();

            spellCardTimeline.Action(() =>
                                     {
                                         mTime--;
                                         mTimeText.DisplayedString = mTime.ToString();
                                     });
            spellCardTimeline.Wait();
            spellCardTimeline.AddCommand(new GotoConditional(() => mTime < 1 || (int) mBoss.Parameters["health"] < 1, 0, -1));
            spellCardTimeline.Action(() =>
                                     {
                                         foreach (Action drawEvent in drawEvents) mGame.RemoveDrawAction(drawEvent);
                                         foreach (Timeline timeline in mTimelines) timeline.Finished = true;
                                         ClearBullets(mGame, mStage);
                                         if (mOnEnd != null) mStage.TimelinesUpdate.Add(mOnEnd);
                                     });

            mStage.TimelinesUpdate.Add(spellCardTimeline);
        }

        public static void ClearBullets(BHGame mGame, BHStage mStage)
        {
            for (int i = mGame.Manager.EntityDictionary["deadlytoplayer"].Count - 1; i > 0; i--)
            {
                Entity entity = mGame.Manager.EntityDictionary["deadlytoplayer"][i];
                entity.Destroy();
            }
        }
    }
}