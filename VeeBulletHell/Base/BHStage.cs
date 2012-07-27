#region
using System.Collections.Generic;
using SFMLStart.Utilities;

#endregion
namespace VeeBulletHell.Base
{
    public class BHStage
    {
        public BHStage()
        {
            TimelinesUpdate = new List<Timeline>();
            TimelinesDraw = new List<Timeline>();
        }

        public List<Timeline> TimelinesUpdate { get; set; }
        public List<Timeline> TimelinesDraw { get; set; }

        public void Update(float mFrameTime) { for (int i = TimelinesUpdate.Count - 1; i >= 0; i--) TimelinesUpdate[i].Update(mFrameTime); }
        public void Draw() { for (int i = TimelinesDraw.Count - 1; i >= 0; i--) TimelinesDraw[i].Update(1); }
    }
}