using System;
using System.Collections.Generic;

namespace FarseerGames.FarseerPhysics.Dynamics
{
    public class ArbiterList : List<Arbiter>
    {
        private List<Arbiter> _markedForRemovalList;

        public ArbiterList()
        {
            _markedForRemovalList = new List<Arbiter>();
        }

        public void ForEachSafe(Action<Arbiter> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(this[i]);
            }
        }

        public void RemoveAllSafe(Predicate<Arbiter> match)
        {
            for (int i = 0; i < Count; i++)
            {
                if (match(this[i]))
                {
                    _markedForRemovalList.Add(this[i]);
                }
            }
            for (int j = 0; j < _markedForRemovalList.Count; j++)
            {
                Remove(_markedForRemovalList[j]);
                _markedForRemovalList[j].Reset();
            }
            _markedForRemovalList.Clear();
        }

        public void RemoveContactCountEqualsZero(Pool<Arbiter> arbiterPool)
        {
            for (int i = 0; i < Count; i++)
            {
                if (ContactCountEqualsZero(this[i]))
                {
                    _markedForRemovalList.Add(this[i]);
                }
            }
            for (int j = 0; j < _markedForRemovalList.Count; j++)
            {
                Remove(_markedForRemovalList[j]);
                arbiterPool.Release(_markedForRemovalList[j]);
            }
            _markedForRemovalList.Clear();
        }

        public void RemoveContainsDisposedBody(Pool<Arbiter> arbiterPool)
        {
            for (int i = 0; i < Count; i++)
            {
                if (ContainsDisposedBody(this[i]))
                {
                    _markedForRemovalList.Add(this[i]);
                }
            }
            for (int j = 0; j < _markedForRemovalList.Count; j++)
            {
                Remove(_markedForRemovalList[j]);
                arbiterPool.Release(_markedForRemovalList[j]);
            }
            _markedForRemovalList.Clear();
        }

        internal static bool ContactCountEqualsZero(Arbiter a)
        {
            return a.ContactCount == 0;
        }

        internal static bool ContainsDisposedBody(Arbiter a)
        {
            return a.ContainsDisposedGeom();
        }
    }
}