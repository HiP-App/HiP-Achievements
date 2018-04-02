using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel
{
    public class EntityIndex : IDomainIndex
    {
        private readonly Dictionary<ResourceType, EntityTypeInfo> _types = new Dictionary<ResourceType, EntityTypeInfo>();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Gets a new, never-used-before ID for a new entity of the specified type.
        /// </summary>
        public int NextId(ResourceType entityType)
        {
            lock (_lockObject)
            {
                var info = GetOrCreateEntityTypeInfo(entityType);
                return ++info.MaximumId;
            }
        }

        /// <summary>
        /// Get UserId of an entity owner
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Owner(ResourceType entityType, int id)
        {
            var info = GetOrCreateEntityTypeInfo(entityType);

            if (info.Entities.TryGetValue(id, out var entity))
                return entity.UserId;

            return null;
        }

        public IReadOnlyCollection<int> AllIds(ResourceType entityType)
        {
            lock (_lockObject)
            {
                var info = GetOrCreateEntityTypeInfo(entityType);
                return info.Entities.Select(kvp => kvp.Key).ToList();
            }
        }

        public int Id(ResourceType entityType, IIdentity userId)
        {
            var info = GetOrCreateEntityTypeInfo(entityType);

            return info.Entities.FirstOrDefault(x => x.Value.UserId == userId.GetUserIdentity()).Key;
        }
        /// <summary>
        /// Determines whether an entity with the specified type and ID exists.
        /// </summary>
        public bool Exists(ResourceType entityType, int id)
        {
            lock (_lockObject)
            {
                var info = GetOrCreateEntityTypeInfo(entityType);
                return info.Entities.ContainsKey(id);
            }
        }

        public void ApplyEvent(IEvent e)
        {
            if (e is BaseEvent baseEvent)
            {
                ResourceType type = baseEvent.GetEntityType();
                if (type.BaseResourceType == ResourceTypes.Achievement) type = ResourceTypes.Achievement;
                else if (type.BaseResourceType == ResourceTypes.Action) type = ResourceTypes.Action;

                switch (e)
                {
                    case CreatedEvent ev:
                        lock (_lockObject)
                        {
                            var user = ev.UserId;
                            var info = GetOrCreateEntityTypeInfo(type);
                            info.MaximumId = Math.Max(info.MaximumId, ev.Id);
                            info.Entities.Add(ev.Id, new EntityInfo { UserId = user });
                        }
                        break;
                    case PropertyChangedEvent ev:
                        lock (_lockObject)
                        {
                            var owner = ev.UserId;
                            var info = GetOrCreateEntityTypeInfo(type);
                            if (info.Entities.All(x => x.Value.UserId != owner))
                            {
                                info.MaximumId = Math.Max(info.MaximumId, ev.Id);
                                info.Entities.Add(ev.Id, new EntityInfo { UserId = owner });
                            }
                        }
                        break;

                    case DeletedEvent ev:
                        lock (_lockObject)
                        {
                            var info = GetOrCreateEntityTypeInfo(type);
                            info.Entities.Remove(ev.Id);
                        }
                        break;
                }
            }
        }

        private EntityTypeInfo GetOrCreateEntityTypeInfo(ResourceType entityType)
        {
            if (_types.TryGetValue(entityType, out var info))
                return info;

            return _types[entityType] = new EntityTypeInfo();
        }

        class EntityTypeInfo
        {
            /// <summary>
            /// The largest ID ever assigned to an entity of the type.
            /// </summary>
            public int MaximumId { get; set; } = -1;

            /// <summary>
            /// Stores only the most basic information about all entities of the type.
            /// It is assumed that this easily fits in RAM.
            /// </summary>
            public Dictionary<int, EntityInfo> Entities { get; } = new Dictionary<int, EntityInfo>();
        }

        class EntityInfo
        {
            /// <summary>
            /// Owner of the entity
            /// </summary>
            public string UserId { get; set; }
        }
    }
}
