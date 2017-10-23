using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class AllItemsResult<T>
    {
        public int Total { get; set; }

        public IReadOnlyCollection<T> Items { get; set; }
    }
}
