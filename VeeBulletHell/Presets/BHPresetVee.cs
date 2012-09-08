#region
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Utilities.Timelines;
using VeeBulletHell.Base;
using VeeBulletHell.Data;

#endregion

namespace VeeBulletHell.Presets
{
    public static class BHTestScript
    {
        public static BHStage TestScriptStage2(BHGame mGame)
        {
            var result = new BHStage();
            var stageTimeline = new Timeline();

            // FIRST FAIRY WAVE
            for (int i = 0; i < 200; i++)
            {
                int i1 = i;
                var position = new Vector2i();
                if (i%2 == 0) position = new Vector2i(mGame.Bounds.Right, 0);

                stageTimeline.Wait(5);
                stageTimeline.Action(() => EnemyFairy1(mGame, position, i1%2 != 0, "enemyfairy"));
            }
            // -------------------

            // DISPLAY STAGE IMAGE
            stageTimeline.Wait(150);
            stageTimeline.Action(() => BHPresetStageControl.CutIn(mGame, result, Assets.GetTexture("st01logo"), mOffset: new Vector2i(0, -50000), mLength: 200));
            // -------------------

            // FIRST BIG FAIRY
            stageTimeline.Wait(50);
            stageTimeline.Action(() => EnemyFairyBig1(mGame, new Vector2i(mGame.Center.X, 0), "enemyfairybig"));
            // -------------------

            // SECOND FAIRY WAVE
            stageTimeline.Wait(200);
            for (int i = 0; i < 50; i++)
            {
                int i1 = i;
                var position = new Vector2i();
                if (i%2 == 0) position = new Vector2i(mGame.Bounds.Right, 0);

                stageTimeline.Wait(8);
                stageTimeline.Action(() => EnemyFairy1(mGame, position, i1%2 != 0, "enemyfairy", 2));
            }
            // -------------------

            // SECOND AND THIRD BIG FAIRIES
            stageTimeline.Wait(180);
            stageTimeline.Action(() => EnemyFairyBig1(mGame, new Vector2i(mGame.Center.X - 100.ToUnits(), 0), "enemyfairybig"));
            stageTimeline.Action(() => EnemyFairyBig1(mGame, new Vector2i(mGame.Center.X + 100.ToUnits(), 0), "enemyfairybig"));
            stageTimeline.Wait(500);
            // -------------------            

            // MIDBOSS WITH 1 SPELLCARD         
            stageTimeline.Action(() => EnemyMidboss1(mGame, new Vector2i(mGame.Center.X, 0)));
            // -------------------

            result.TimelinesUpdate.Add(stageTimeline);

            return result;
        }

        public static BHEntity EnemyFairy1(BHGame mGame, Vector2i mPosition, bool mRight, string mTexture, int mMultiplier = 1)
        {
            int dir = -1;
            if (mRight) dir = 1;
            int s = 2;

            BHEntity result = BHPresetBase.Enemy(mGame, 20.ToUnits(), 2);
            result.Sprite = Assets.GetTileset(mTexture).GetSprite("s1", Assets.GetTexture("enemyfairy"));
            result.IsSpriteFixed = true;
            result.Position = mPosition;
            result.Animation = Assets.GetAnimation("enemyfairystill");

            BHPresetTimelines.Kill(result, 400);

            // MOVEMENT TIMELINES
            var timelineMovement = new Timeline();
            for (int i = 0; i < 150; i++)
            {
                int i1 = i;
                timelineMovement.AddCommand(new Do(() => result.Velocity = new Vector2i((0.1f.ToUnits()*s - (i1*-3*s))*dir, 0 + (i1*3*s))));
                if (i%3 == 0) timelineMovement.AddCommand(new Wait(1));
            }
            // -------------------

            // ATTACK TIMELINES
            var timelineAttack = new Timeline();
            timelineAttack.AddCommand(new Do(() => EnemyFairy1Attack(mGame, result.Position, new Color(135, 135, 230, 255), mMultiplier)));
            timelineAttack.AddCommand(new Wait(25));
            timelineAttack.AddCommand(new Goto(0, 8));
            // -------------------

            result.TimelinesUpdate.Add(timelineMovement);
            result.TimelinesUpdate.Add(timelineAttack);

            return result;
        }
        public static void EnemyFairy1Attack(BHGame mGame, Vector2i mPosition, Color mColor, int mMultiplier = 1)
        {
            Assets.GetSound("tan01").Play();
            for (int i = 0; i < 10*mMultiplier; i++) BHPresetBullets.RoundFull(mGame, mPosition, Utils.Random.Next(0, 360), Utils.Random.Next(2.ToUnits(), 4.ToUnits())).Sprite.Color = mColor;
        }

        public static BHEntity EnemyFairyBig1(BHGame mGame, Vector2i mPosition, string mTexture)
        {
            BHEntity result = BHPresetBase.Enemy(mGame, 30.ToUnits(), 40);

            result.Sprite = new Sprite(Assets.GetTexture(mTexture));
            result.IsSpriteFixed = true;
            result.Position = mPosition;

            BHPresetTimelines.Kill(result, 900);

            // MOVEMENT TIMELINES
            var timelineMovement = new Timeline();
            timelineMovement.Action(() => result.Velocity = new Vector2i(0, 150));
            timelineMovement.Wait(100);
            timelineMovement.Action(() => result.Velocity = new Vector2i(0, 0));
            timelineMovement.Wait(500);
            timelineMovement.Action(() => result.Velocity = new Vector2i(0, -150));
            // -------------------

            // ATTACK TIMELINES
            var timelineAttack = new Timeline();
            timelineAttack.Parameters["angle"] = 0;
            timelineAttack.AddCommand(new Do(() =>
                                             {
                                                 var angle = (int) timelineAttack.Parameters["angle"];
                                                 for (int i = 0; i < 24; i++)
                                                 {
                                                     BHPresetBullets.Glow1(mGame, result.Position, (360/24)*i + angle, 5.ToUnits());
                                                     BHPresetBullets.Glow1(mGame, result.Position, (360/24)*i - angle, 5.ToUnits());
                                                 }
                                                 timelineAttack.Parameters["angle"] = angle + 20;
                                             }));
            timelineAttack.AddCommand(new Wait(25));
            timelineAttack.AddCommand(new Goto(0, 50));

            var timelineAttack2 = new Timeline();
            timelineAttack2.AddCommand(new Do(() =>
                                              {
                                                  Assets.GetSound("tan00").Play();
                                                  for (int i = 0; i < 16; i++)
                                                  {
                                                      var bulletTimeline = new Timeline();
                                                      BHEntity bullet = BHPresetBullets.RoundSmall(mGame, result.Position, (360/16)*i, 1.ToUnits());

                                                      bulletTimeline.AddCommand(new Wait(50));
                                                      bulletTimeline.AddCommand(new Do(() =>
                                                                                       {
                                                                                           Assets.GetSound("kira01").Play();
                                                                                           var player = (BHEntity) mGame.Manager.EntityDictionary["player"][0];
                                                                                           float angle = BHUtils.GetAngleTowards(bullet, player) + Utils.Random.Next(-8, 8);
                                                                                           var newVelocity = Utils.Math.Angles.ToVectorDegrees(angle)*4.ToUnits();
                                                                                           bullet.Velocity = new Vector2i((int) newVelocity.X, (int) newVelocity.Y);
                                                                                       }));

                                                      bullet.TimelinesUpdate.Add(bulletTimeline);
                                                  }
                                              }));
            timelineAttack2.AddCommand(new Wait(50));
            timelineAttack2.AddCommand(new Goto(0, 50));
            // -------------------

            result.TimelinesUpdate.Add(timelineMovement);
            result.TimelinesUpdate.Add(timelineAttack);
            result.TimelinesUpdate.Add(timelineAttack2);

            return result;
        }

        public static BHEntity EnemyMidboss1(BHGame mGame, Vector2i mPosition)
        {
            BHEntity result = BHPresetBase.Boss(mGame, 30.ToUnits(), 100);

            result.Sprite = Assets.GetTileset("stg7enm").GetSprite("s1", Assets.GetTexture("stg7enm"));
            result.IsSpriteFixed = true;
            result.Position = mPosition;
            result.Animation = Assets.GetAnimation("stg7enmstill");

            // MOVEMENT TIMELINES
            var startTimeline = new Timeline();
            startTimeline.AddCommand(new Do(() => result.Velocity = new Vector2i(0, 0.7f.ToUnits())));
            startTimeline.AddCommand(new Wait(210));
            startTimeline.AddCommand(new Do(() => result.Velocity = new Vector2i(0, 0)));
            startTimeline.AddCommand(new Wait(25));
            startTimeline.AddCommand(new Do(() => EnemyMidboss1SpellCard1(mGame, result)));
            // -------------------

            result.TimelinesUpdate.Add(startTimeline);

            return result;
        }
        public static void EnemyMidboss1SpellCard1(BHGame mGame, BHEntity mBoss)
        {
            var spellcardTimeline = new Timeline();
            spellcardTimeline.AddCommand(new Do(() => EnemyMidboss1SpellCardAttack1(mGame, mBoss.Position)));
            spellcardTimeline.AddCommand(new Wait(145));
            spellcardTimeline.AddCommand(new Do(() => EnemyMidboss1SpellCardAttack2(mGame, mBoss.Position)));
            spellcardTimeline.AddCommand(new Wait(25));
            spellcardTimeline.AddCommand(new Do(() => EnemyMidboss1SpellCardAttack3(mGame, mBoss.Position)));
            spellcardTimeline.AddCommand(new Wait(25));
            spellcardTimeline.AddCommand(new Do(() => BHPresetTimelines.MovementLerp(mBoss,
                                                                                     new Vector2i(Utils.Random.Next(mGame.Bounds.Left + 5.ToUnits(), mGame.Bounds.Right - 5.ToUnits()), mBoss.Position.Y))));
            spellcardTimeline.AddCommand(new Goto(0));

            var continueTimeline = new Timeline();
            continueTimeline.AddCommand(new Wait(25));
            continueTimeline.AddCommand(new Do(() => BHPresetTimelines.MovementLerp(mBoss, new Vector2i(mGame.Center.X, mBoss.Position.Y))));
            continueTimeline.AddCommand(new Wait(95));
            continueTimeline.AddCommand(new Do(() => EnemyMidboss1SpellCard2(mGame, mBoss)));

            BHPresetStageControl.CutIn(mGame, mGame.CurrentStage, Assets.GetTexture("face07bs"), true, "u mad bro?",
                                       new Vector2i(600, 100), new Vector2i(-10.ToUnits(), 5.ToUnits()), 150);
            BHPresetStageControl.SpellCard(mGame, mGame.CurrentStage, mBoss, "HERE ARE SOME LASERS 4 U", 150, 1500, 10000, continueTimeline, spellcardTimeline);
        }
        public static void EnemyMidboss1SpellCardAttack1(BHGame mGame, Vector2i mPosition)
        {
            Assets.GetSound("lazer00").Play();

            for (int i = 0; i < 60; i++)
            {
                BHEntity ent = BHPresetBase.Laser(mGame,
                                                  new Sprite(Assets.GetTexture("b_laser")),
                                                  mPosition, (360/12)*i + i, 0.5f.ToUnits() + i*10, 4.ToUnits(), 30.ToUnits() + (i*0.1f.ToUnits()), true, 250);

                var laserTimeline = new Timeline();
                int i1 = i;
                laserTimeline.AddCommand(new Do(() =>
                                                {
                                                    var lineShape = (BHCSLine) ent.CollisionShape;
                                                    lineShape.Degrees += i1/20f;
                                                    lineShape.Length += 0.7f.ToUnits();
                                                }));
                laserTimeline.AddCommand(new Wait(1));
                laserTimeline.AddCommand(new Goto(0, 145));

                ent.TimelinesUpdate.Add(laserTimeline);
            }
        }
        public static void EnemyMidboss1SpellCardAttack2(BHGame mGame, Vector2i mPosition)
        {
            Assets.GetSound("lazer00").Play();

            for (int i = 1; i < 12; i++)
            {
                int nextX = mGame.Bounds.Left + (mGame.Bounds.Width/12*i);
                BHPresetBase.Laser(mGame, new Sprite(Assets.GetTexture("b_laser")), new Vector2i(nextX, 0), 90,
                                   4.ToUnits(), 5.ToUnits(), 30.ToUnits(), true, 2.5f.ToUnits());
            }
        }
        public static void EnemyMidboss1SpellCardAttack3(BHGame mGame, Vector2i mPosition)
        {
            Assets.GetSound("tan02").Play();
            for (int i = 0; i < 85; i++) BHPresetBullets.Pellet(mGame, mPosition, Utils.Random.Next(0, 360), Utils.Random.Next(0.8f.ToUnits(), 3.5f.ToUnits()));
        }

        public static void EnemyMidboss1SpellCard2(BHGame mGame, BHEntity mBoss)
        {
            var spellcardTimeline = new Timeline();
            spellcardTimeline.AddCommand(new Do(() => EnemyMidboss1SpellCardAttack3(mGame, mBoss.Position)));
            spellcardTimeline.AddCommand(new Wait(21));
            spellcardTimeline.AddCommand(new Do(() => BHPresetTimelines.MovementLerp(mBoss,
                                                                                     new Vector2i(Utils.Random.Next(mGame.Bounds.Left + 5.ToUnits(), mGame.Bounds.Right - 5.ToUnits()), mBoss.Position.Y))));
            spellcardTimeline.AddCommand(new Goto(0));

            var continueTimeline = new Timeline();
            continueTimeline.AddCommand(new Wait(25));
            continueTimeline.AddCommand(new Do(() => BHPresetTimelines.MovementLerp(mBoss, new Vector2i(mGame.Center.X, mBoss.Position.Y))));
            continueTimeline.AddCommand(new Wait(75));
            continueTimeline.AddCommand(new Do(mBoss.Destroy));
            continueTimeline.AddCommand(new Do(() => Assets.GetSound("enep01").Play()));

            BHPresetStageControl.CutIn(mGame, mGame.CurrentStage, Assets.GetTexture("face07bs"), true, "u mad bro?",
                                       new Vector2i(6.ToUnits(), 1.ToUnits()), new Vector2i(-10.ToUnits(), 5.ToUnits()), 150);
            BHPresetStageControl.SpellCard(mGame, mGame.CurrentStage, mBoss, "HERE ARE SOME BULLETS 4 U", 150, 1000, 10000, continueTimeline, spellcardTimeline);
        }
    }
}