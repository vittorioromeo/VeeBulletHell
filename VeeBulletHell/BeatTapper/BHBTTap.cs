namespace VeeBulletHell.BeatTapper
{
    public class BHBTTap
    {
        public BHBTTap(int mCurrentFrame, string mKey)
        {
            Time = mCurrentFrame;
            Key = mKey;
        }

        public int Time { get; set; }
        public string Key { get; set; }
    }
}