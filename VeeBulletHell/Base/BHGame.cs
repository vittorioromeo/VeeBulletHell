#region
using System;
using System.Drawing;
using SFML.Graphics;
using SFML.Window;
using SFMLStart;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeBulletHell.Data;
using VeeBulletHell.Presets;
using Color = SFML.Graphics.Color;

#endregion
namespace VeeBulletHell.Base
{
    public class BHGame : Game
    {
        public BHGame(int mBoundsOffset = 5)
        {
            Manager = new EntityManager(this, new[] { "bullet", "player", "enemy", "character", "boss", "deadlytoplayer", "laser", "playerbullet" });
            BorderSprite = new Sprite(Assets.GetTexture("h_border"));
            Bounds = new Rectangle((32 - 32 - mBoundsOffset)*BHUtils.Unit, (16 - 16 - mBoundsOffset)*BHUtils.Unit, (384 + mBoundsOffset)*BHUtils.Unit, (448 + mBoundsOffset)*BHUtils.Unit);
            Player = BHPresetPlayers.Reimu(this);
            Player.Position = Center;
            InitializeInputs();

            //DEBUG
            Utils.Assets.SetSoundsVolume(10);
            Bind("debug", 20, Debug, null, new KeyCombination(Keyboard.Key.P));

            OnUpdate += Run;

            OnDrawBeforeCamera += () => GameWindow.RenderWindow.Clear(Color.Black);
            AddDrawAction(() => Manager.Draw());
            AddDrawAction(DrawStuff);
            OnDrawAfterDefault += () => GameWindow.RenderWindow.Draw(BorderSprite);
        }

        public EntityManager Manager { get; set; }
        public Sprite BorderSprite { get; set; }
        public Rectangle Bounds { get; set; }
        public Vector2i Center
        {
            get { return new Vector2i((32 - 32 + (384/2))*BHUtils.Unit, (16 - 16 + (448/2))*BHUtils.Unit); }
        }
        public BHEntity Player { get; set; }
        public BHStage CurrentStage { get; set; }
        public int NextX { get; set; }
        public int NextY { get; set; }
        public int Focus { get; set; }

        public void InitializeInputs()
        {
           Bind("quit", 0, () => Environment.Exit(0), null, new KeyCombination(Keyboard.Key.Escape));
           Bind("up", 0, () => { NextY = -1; }, null, new KeyCombination(Keyboard.Key.Up));
           Bind("down", 0, () => { NextY = 1; }, null, new KeyCombination(Keyboard.Key.Down));
           Bind("left", 0, () => { NextX = -1; }, null, new KeyCombination(Keyboard.Key.Left));
           Bind("right", 0, () => { NextX = 1; }, null, new KeyCombination(Keyboard.Key.Right));
           Bind("focus", 0, () => { Focus = 1; }, null, new KeyCombination(Keyboard.Key.LShift));
        }

        public void Run(float mFrameTime)
        {

            Manager.Update(mFrameTime);
            NextX = NextY = Focus = 0;

            if (CurrentStage != null) CurrentStage.Update(mFrameTime);

            GameWindow.RenderWindow.SetTitle(string.Format("FPS: {0} || {1} || Entities: {2}", (int)(60f / mFrameTime), mFrameTime, Manager.Entities.Count));
        }
        public void DrawStuff()
        {
            //WindowManager.RenderWindow.Clear(Color.Black);

            if (CurrentStage != null) CurrentStage.Draw();

            //WindowManager.RenderWindow.Draw(BorderSprite);
        }

        public void Debug() { CurrentStage = BHTestScript.TestScriptStage2(this); }
    }
}