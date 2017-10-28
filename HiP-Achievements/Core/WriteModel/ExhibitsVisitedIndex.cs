using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel
{
    public class ExhibitsVisitedIndex : IDomainIndex
    {
        private readonly Dictionary<string, List<int>> _visitedExhibits = new Dictionary<string, List<int>>();

        public void ApplyEvent(IEvent e)
        {
            switch (e)
            {
                case ActionCreated ev:
                    switch (ev.Properties)
                    {
                        case ExhibitVisitedActionArgs args:
                            if (_visitedExhibits.TryGetValue(ev.UserId, out var list))
                            {
                                list.Add(args.EntityId);
                            }
                            else
                            {
                                _visitedExhibits.Add(ev.UserId, new List<int> { args.EntityId });
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
