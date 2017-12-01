namespace Spark.Entities
{
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;

    using Graphics;
    using Scene;

    public class World : IReadOnlyDictionary<int, Entity>
    {
        private int _currId;
        private Dictionary<int, Entity> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World()
        {
            _entities = new Dictionary<int, Entity>();
            _currId = 0;

            Scene = new Scene("World");
        }

        public event TypedEventHandler<World, EntityEventArgs> AddedEntity;

        public event TypedEventHandler<World, EntityEventArgs> RemovedEntity;

        public Scene Scene { get; }

        public Dictionary<int, Entity>.KeyCollection EntityIds => _entities.Keys;

        public Dictionary<int, Entity>.ValueCollection Entities => _entities.Values;

        IEnumerable<int> IReadOnlyDictionary<int, Entity>.Keys => _entities.Keys;

        IEnumerable<Entity> IReadOnlyDictionary<int, Entity>.Values => _entities.Values;

        public int Count => _entities.Count;

        public Entity this[int key]
        {
            get
            {
                if (_entities.TryGetValue(key, out Entity entity))
                {
                    return entity;
                }

                return null;
            }
        }
        
        public void Add(Entity entity)
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

        public bool Remove(Entity entity)
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
            
            if (!_entities.TryGetValue(entityId, out Entity entToRemove))
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
            foreach (KeyValuePair<int, Entity> kv in _entities)
            {
                NotifyEntityRemoved(kv.Value);
                kv.Value.SetWorldInfo(0, null);
            }

            _entities.Clear();
            ResetIds();
        }

        public void Update(IGameTime time)
        {
            foreach (KeyValuePair<int, Entity> kv in _entities)
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

            var newEntities = new Dictionary<int, Entity>();
            foreach (KeyValuePair<int, Entity> kv in _entities)
            {
                int oldId = kv.Value.Id;
                kv.Value.SetWorldInfo(GetNewId(), this);

                //EntityIDChangedEvent evt = new EntityIDChangedEvent(kv.Value, oldId);
                //DispatchToAll<EntityIDChangedEvent>(kv.Value, ref evt);

                newEntities.Add(kv.Value.Id, kv.Value);
            }

            _entities.Clear();
            _entities = newEntities;
        }

        bool IReadOnlyDictionary<int, Entity>.ContainsKey(int key)
        {
            return _entities.ContainsKey(key);
        }

        bool IReadOnlyDictionary<int, Entity>.TryGetValue(int key, out Entity value)
        {
            return _entities.TryGetValue(key, out value);
        }

        public Dictionary<int, Entity>.Enumerator GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, Entity>> IEnumerable<KeyValuePair<int, Entity>>.GetEnumerator()
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

        private void NotifyEntityAdded(Entity entity)
        {
            AddedEntity?.Invoke(this, new EntityEventArgs(entity, this));
        }

        private void NotifyEntityRemoved(Entity entity)
        {
            RemovedEntity?.Invoke(this, new EntityEventArgs(entity, this));
        }
    }
}
