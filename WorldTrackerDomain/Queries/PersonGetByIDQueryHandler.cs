using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetByIDQueryHandler : IRequestHandler<PersonGetByIDQuery, PersonGetByIDView>
    {
        private readonly IDomainViewReaderRepository _domainViewRepository;

        public PersonGetByIDQueryHandler(IDomainViewReaderRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task<PersonGetByIDView> Handle(PersonGetByIDQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PersonGetByID(request.ID), cancellationToken);

            return data as PersonGetByIDView ?? new PersonGetByIDView(request.ID);
        }
    }
}
