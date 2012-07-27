#region
using System.Collections.Generic;
using System.Diagnostics;
using SFMLStart;

#endregion
namespace VeeBulletHell
{
    public class EntityManager
    {
        private bool _isSortNeeded;
        public EntityManager(Game mGame, string[] mGroups)
        {
            Debug.Assert(mGame != null);

            Game = mGame;
            Groups = mGroups;
            Entities = new List<Entity>();
            EntitiesToUpdate = new List<Entity>();
            EntitiesToDrawSorted = new List<Entity>();

            EntityDictionary = new Dictionary<string, List<Entity>>();
            foreach (string group in mGroups) EntityDictionary.Add(group, new List<Entity>());
        }

        public Dictionary<string, List<Entity>> EntityDictionary { get; set; }

        public Game Game { get; set; }
        public string[] Groups { get; set; }

        public List<Entity> Entities { get; set; }
        public List<Entity> EntitiesToUpdate { get; set; }
        public List<Entity> EntitiesToDrawSorted { get; set; }

        public void AddEntity(Entity mEntity, bool mNeedsUpdate)
        {
            Debug.Assert(mEntity != null);
            Debug.Assert(!Entities.Contains(mEntity));

            Entities.Add(mEntity);
            if (mNeedsUpdate)
            {
                Debug.Assert(!EntitiesToUpdate.Contains(mEntity));
                EntitiesToUpdate.Add(mEntity);
            }
            EntitiesToDrawSorted.Add(mEntity);

                foreach (string group in mEntity.Groups)
                {
                    Debug.Assert(!EntityDictionary[group].Contains(mEntity));
                    EntityDictionary[group].Add(mEntity);
                }
            

            _isSortNeeded = true;
        }
        public void RemoveEntity(Entity mEntity)
        {
            Debug.Assert(mEntity != null);
            Debug.Assert(Entities.Contains(mEntity));
            Debug.Assert(EntitiesToUpdate.Contains(mEntity));
            Debug.Assert(EntitiesToDrawSorted.Contains(mEntity));

            Entities.Remove(mEntity);
            EntitiesToUpdate.Remove(mEntity);
            EntitiesToDrawSorted.Remove(mEntity);

                foreach (string group in mEntity.Groups)
                {
                    Debug.Assert(EntityDictionary[group].Contains(mEntity));
                    EntityDictionary[group].Remove(mEntity);
                }
            
        }

        public virtual void Update(float mFrameTime)
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                if (Entities[i].IsInitialized == false) Entities[i].Initialize();
                Entities[i].Update(mFrameTime);
            }
        }

        public virtual void Draw()
        {

                if (_isSortNeeded) EntitiesToDrawSorted.Sort((p1, p2) => p1.DrawOrder.CompareTo(p2.DrawOrder));
                _isSortNeeded = false;
            

            for (int i = 0; i < EntitiesToDrawSorted.Count; i++) EntitiesToDrawSorted[i].Draw();
        }
    }
}