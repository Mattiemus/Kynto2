namespace Spark.Engine
{
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;

    using Graphics;
    using Scene;

    public class World : IReadOnlyDictionary<int, IEntity>
    {
        private int _currId;
        private Dictionary<int, IEntity> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World()
        {
            _entities = new Dictionary<int, IEntity>();
            _currId = 0;

            Scene = new Scene("World");
        }
        
        public Scene Scene { get; }

        public Dictionary<int, IEntity>.KeyCollection EntityIds => _entities.Keys;

        public Dictionary<int, IEntity>.ValueCollection Entities => _entities.Values;

        IEnumerable<int> IReadOnlyDictionary<int, IEntity>.Keys => _entities.Keys;

        IEnumerable<IEntity> IReadOnlyDictionary<int, IEntity>.Values => _entities.Values;

        public int Count => _entities.Count;

        public IEntity this[int key]
        {
            get
            {
                if (_entities.TryGetValue(key, out IEntity entity))
                {
                    return entity;
                }

                return null;
            }
        }
        
        public void Add(IEntity entity)
        {
            if (entity == null || entity.World != null)
            {
                return;
            }

            entity.SetWorldInfo(GetNewId(), this);
            _entities.Add(entity.Id, entity);

            NotifyEntityAdded(entity);
        }

        public bool Contains(int entityId)
        {
            return _entities.ContainsKey(entityId);
        }

        public bool Remove(IEntity entity)
        {
            if (entity == null || entity.World != this)
            {
                return false;
            }

            NotifyEntityRemoved(entity);

            _entities.Remove(entity.Id);
            entity.SetWorldInfo(0, null);

            if (_entities.Count == 0)
            {
                ResetIds();
            }

            return true;
        }

        public bool Remove(int entityId)
        {
            if (entityId <= 0)
            {
                return false;
            }
            
            if (!_entities.TryGetValue(entityId, out IEntity entToRemove))
            {
                return false;
            }

            NotifyEntityRemoved(entToRemove);

            _entities.Remove(entityId);
            entToRemove.SetWorldInfo(0, null);

            if (_entities.Count == 0)
            {
                ResetIds();
            }

            return true;
        }

        public void Clear()
        {
            foreach (var kv in _entities)
            {
                NotifyEntityRemoved(kv.Value);
                kv.Value.SetWorldInfo(0, null);
            }

            _entities.Clear();
            ResetIds();
        }

        public void Update(IGameTime time)
        {
            foreach (var kv in _entities)
            {
                kv.Value.Update(time);
            }

            Scene.Update(time);
        }

        public void ProcessVisibleSet(IRenderer renderer)
        {
            ProcessVisibleSet(renderer, false);
        }

        public void ProcessVisibleSet(IRenderer renderer, bool skipCullCheck)
        {
            Scene.ProcessVisibleSet(renderer, skipCullCheck);
        }

        public void RenumberEntities()
        {
            ResetIds();

            var newEntities = new Dictionary<int, IEntity>();
            foreach (var kv in _entities)
            {
                int oldId = kv.Value.Id;
                kv.Value.SetWorldInfo(GetNewId(), this);

                NotifyEntityIdChanged(kv.Value, oldId, kv.Value.Id);
                
                newEntities.Add(kv.Value.Id, kv.Value);
            }

            _entities.Clear();
            _entities = newEntities;
        }

        bool IReadOnlyDictionary<int, IEntity>.ContainsKey(int key)
        {
            return _entities.ContainsKey(key);
        }

        bool IReadOnlyDictionary<int, IEntity>.TryGetValue(int key, out IEntity value)
        {
            return _entities.TryGetValue(key, out value);
        }

        public Dictionary<int, IEntity>.Enumerator GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, IEntity>> IEnumerable<KeyValuePair<int, IEntity>>.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        private int GetNewId()
        {
            return Interlocked.Increment(ref _currId);
        }

        private void ResetIds()
        {
            Interlocked.Exchange(ref _currId, 0);
        }

        private void NotifyEntityAdded(IEntity entity)
        {
            // TODO
        }

        private void NotifyEntityRemoved(IEntity entity)
        {
            // TODO
        }

        private void NotifyEntityIdChanged(IEntity entity, int oldId, int newId)
        {
            // TODO
        }
    }
}
