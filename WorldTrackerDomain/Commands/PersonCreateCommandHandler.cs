using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;

namespace WorldTrackerDomain.Commands
{
    public class PersonCreateCommandHandler : IRequestHandler<PersonCreateCommand>
    {
        private readonly IPersonAggregate _personAggregate;

        public PersonCreateCommandHandler(IPersonAggregate personAggregate)
        {
            _personAggregate = personAggregate;
        }

        public async Task<Unit> Handle(PersonCreateCommand request, CancellationToken cancellationToken)
        {
            await _personAggregate.Load(request.ID, cancellationToken);

            await _personAggregate.Create(request.Name, request.PictureUrl, cancellationToken);

            await _personAggregate.SaveChanges(cancellationToken);

            return Unit.Value;
        }
    }
}
