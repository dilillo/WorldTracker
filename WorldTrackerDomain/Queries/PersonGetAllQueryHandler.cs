using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Projectors;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetAllQueryHandler : IRequestHandler<PersonGetAllQuery, PersonGetAllView>
    {
        private readonly IDomainViewRepository _domainViewRepository;
        private readonly IDomainEventRepository _domainEventRepository;
        private readonly IPersonGetAllViewProjector _personGetAllViewProjector;

        public PersonGetAllQueryHandler(IDomainViewRepository domainViewRepository, IDomainEventRepository domainEventRepository, IPersonGetAllViewProjector personGetAllViewProjector)
        {
            _domainViewRepository = domainViewRepository;
            _personGetAllViewProjector = personGetAllViewProjector;
            _domainEventRepository = domainEventRepository;
        }

        public async Task<PersonGetAllView> Handle(PersonGetAllQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PersonGetAll, cancellationToken) as PersonGetAllView ?? new PersonGetAllView();

            var unappliedEvents = await _domainEventRepository.GetMoreRecentThan(data.AggregateVersions, cancellationToken);

            if (unappliedEvents.Count > 0)
            {
                data = _personGetAllViewProjector.Predict(data, unappliedEvents.ToArray());
            }

            return data;
        }
    }
}
