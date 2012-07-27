#region
using System;
using System.Collections.Generic;
using System.IO;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeBulletHell.Base;

#endregion
namespace VeeBulletHell.BeatTapper
{
    public class BHBeatTapper
    {
        public BHBeatTapper(BHGame mGame)
        {
            Game = mGame;
            Reset();
        }

        public BHGame Game { get; set; }
        public bool Running { get; set; }
        public float CurrentTime { get; set; }
        public int PressDelay { get; set; }
        public List<BHBTTap> Taps { get; set; }
        public Keyboard.Key LastKey { get; set; }

        public void Reset()
        {
            CurrentTime = 0;
            Taps = new List<BHBTTap>();
        }
        public void Finish()
        {
            Running = false;

            string result = "";

            result += "Timeline timeline = new Timeline();" + Environment.NewLine;
            result += Environment.NewLine;

            foreach (BHBTTap tap in Taps)
            {
                result += string.Format("timeline.AddCommand(new Wait({0}));", tap.Time);
                result += string.Format("timeline.AddCommand(new Do( () => TapResult(\"{0}\") ));", tap.Key);
                result += Environment.NewLine;
            }

            StreamWriter streamWriter = File.CreateText(@"c:\test.txt");
            streamWriter.Write(result);
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void Update(float mFrameTime)
        {
            bool none = true;

            if (!Running) return;

            CurrentTime += mFrameTime;

            foreach (Keyboard.Key key in Enum.GetValues(typeof (Keyboard.Key)))
            {
                if (Keyboard.IsKeyPressed(key))
                {
                    if (key != LastKey && key != Keyboard.Key.P && PressDelay == 0)
                    {
                        LastKey = key;

                        if (key == Keyboard.Key.L)
                        {
                            Finish();
                            break;
                        }

                        PressDelay = 0;
                        Utils.Log(CurrentTime.ToString(), "TAP");
                        Taps.Add(new BHBTTap(Convert.ToInt32(CurrentTime), key.ToString()));
                        CurrentTime = 0;
                    }

                    none = false;
                }
            }

            if (none) LastKey = Keyboard.Key.F1;

            if (PressDelay > 0) PressDelay--;
        }
    }
}