namespace Cross.Helpers
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T entity);
    }
}
