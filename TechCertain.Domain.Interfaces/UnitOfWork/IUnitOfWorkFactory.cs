namespace TechCertain.Domain.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork BeginUnitOfWork();
    }
}
