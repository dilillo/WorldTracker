using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Projectors;

namespace WorldTrackerDomain.Commands
{
    public class PersonCreateCommandHandler : IRequestHandler<PersonCreateCommand, DomainEvent[]>
    {
        private readonly IPersonAggregate _personAggregate;
        private readonly IPersonGetByIDViewProjector _personGetByIDViewProjector;

        public PersonCreateCommandHandler(IPersonAggregate personAggregate, IPersonGetByIDViewProjector personGetByIDViewProjector)
        {
            _personAggregate = personAggregate;
            _personGetByIDViewProjector = personGetByIDViewProjector;
        }

        public async Task<DomainEvent[]> Handle(PersonCreateCommand request, CancellationToken cancellationToken)
        {
            await _personAggregate.Load(request.ID, cancellationToken);

            await _personAggregate.Create(request.Name, request.PictureUrl, cancellationToken);

            var events = await _personAggregate.SaveChanges(cancellationToken);

            await _personGetByIDViewProjector.Project(events, cancellationToken);

            return events;
        }
    }
}
