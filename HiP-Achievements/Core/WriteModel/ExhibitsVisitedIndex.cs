using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel
{
    public class ExhibitsVisitedIndex : IDomainIndex
    {
        private readonly Dictionary<string, IList<int>> _visitedExhibits = new Dictionary<string, IList<int>>();

        public void ApplyEvent(IEvent e)
        {
            switch (e)
            {
                case CreatedEvent ev:
                    var resourceType = ev.GetEntityType();
                    switch (resourceType)
                    {
                        case ResourceType _ when resourceType == ResourceTypes.ExhibitVisitedAction:
                            if (!_visitedExhibits.ContainsKey(ev.UserId))
                            {
                                _visitedExhibits.Add(ev.UserId, new List<int>());
                            }

                            break;
                    }
                    break;

                case PropertyChangedEvent ev:
                    resourceType = ev.GetEntityType();
                    switch (resourceType)
                    {
                        case ResourceType _ when resourceType == ResourceTypes.ExhibitVisitedAction:
                            if (ev.PropertyName == nameof(ExhibitVisitedActionArgs.EntityId) && _visitedExhibits.TryGetValue(ev.UserId, out var list))
                            {
                                list.Add((int)ev.Value);
                            }
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Determines if a user has visisted a specified exhibit
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="exhibitId">Id of the exhibit</param>
        /// <returns></returns>
        public bool Exists(string userId, int exhibitId)
        {
            if (_visitedExhibits.TryGetValue(userId, out var list))
            {
                return list.Contains(exhibitId);
            }

            return false;
        }
    }
}
