using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetAllQueryHandler : IRequestHandler<PersonGetAllQuery, PersonGetAllView>
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PersonGetAllQueryHandler(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task<PersonGetAllView> Handle(PersonGetAllQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PersonGetAll, cancellationToken);

            return data as PersonGetAllView ?? new PersonGetAllView();
        }
    }
}
