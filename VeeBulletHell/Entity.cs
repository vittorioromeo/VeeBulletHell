#region
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SFMLStart;

#endregion
namespace VeeBulletHell
{
    public class Entity
    {
        public Entity(EntityManager mManager, int mDrawOrder = 0, params string[] mGroups)
        {
            Debug.Assert(mManager != null);
            Debug.Assert(mGroups.All(x => mManager.Groups.Contains(x)));

            Groups = new List<string>(mGroups);

            Manager = mManager;
            Manager.AddEntity(this, true);
            DrawOrder = mDrawOrder;
        }

        public EntityManager Manager { get; set; }
        public List<string> Groups { get; set; }
        public int DrawOrder { get; set; }
        public bool IsInitialized { get; set; }

        public void AddGroup(string mGroup)
        {
            Groups.Add(mGroup);
            Manager.EntityDictionary[mGroup].Add(this);
        }
        public void GroupRemove(string mGroup)
        {
            Debug.Assert(Groups.Contains(mGroup));

            Groups.Remove(mGroup);
            Manager.EntityDictionary[mGroup].Remove(this);
        }

        public virtual void Update(float mFrameTime) { }
        public virtual void Draw() { }

        public virtual void Destroy() { Manager.RemoveEntity(this); }
        public virtual void Initialize() { IsInitialized = true; }
    }
}