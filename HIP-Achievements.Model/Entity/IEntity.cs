namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
