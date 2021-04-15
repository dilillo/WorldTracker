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
        private readonly IPersonGetAllViewProjector _personGetAllViewProjector;

        public PersonGetAllQueryHandler(IDomainViewRepository domainViewRepository, IPersonGetAllViewProjector personGetAllViewProjector)
        {
            _domainViewRepository = domainViewRepository;
            _personGetAllViewProjector = personGetAllViewProjector;
        }

        public async Task<PersonGetAllView> Handle(PersonGetAllQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PersonGetAll, cancellationToken) as PersonGetAllView ?? new PersonGetAllView();

            if (request.PendingDomainEvents?.Length > 0)
            {
                data = _personGetAllViewProjector.Predict(data, request.PendingDomainEvents);
            }

            return data;
        }
    }
}
