using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetByIDQueryHandler : IRequestHandler<PersonGetByIDQuery, PersonGetByIDQueryResult>
    {
        private readonly IDomainViewRepository _domainViewRepository;
        private readonly IDomainEventRepository _domainEventRepository;

        public PersonGetByIDQueryHandler(IDomainViewRepository domainViewRepository, IDomainEventRepository domainEventRepository)
        {
            _domainViewRepository = domainViewRepository;
            _domainEventRepository = domainEventRepository;
        }

        public async Task<PersonGetByIDQueryResult> Handle(PersonGetByIDQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PersonGetByID(request.ID), cancellationToken);
            var events = await _domainEventRepository.GetByAggregateID(request.ID, cancellationToken);

            return new PersonGetByIDQueryResult
            {
                PersonGetByIDView = data as PersonGetByIDView ?? new PersonGetByIDView(request.ID),
                Events = events
            };
        }
    }
}
