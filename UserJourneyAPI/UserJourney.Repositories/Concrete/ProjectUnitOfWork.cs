namespace UserJourney.Repositories.Concrete
{
    using UserJourney.Repositories.Contracts;
    using UserJourney.Repositories.EF;

    public class ProjectUnitOfWork : UnitOfWork, IProjectUnitOfWork
    {
        public ProjectUnitOfWork(UserJourneyContext context) : base(context)
        {

        }
    }
}
