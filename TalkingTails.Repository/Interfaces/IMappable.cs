using System.Linq.Expressions;

namespace TalkingTails.Repository.Interfaces
{
    public interface IMappable<TSource>
    {
        static abstract Dictionary<string, Expression<Func<TSource, object>>> Mappings { get; }
    }
}