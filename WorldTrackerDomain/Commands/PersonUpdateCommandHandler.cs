﻿using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Projectors;

namespace WorldTrackerDomain.Commands
{
    public class PersonUpdateCommandHandler : IRequestHandler<PersonUpdateCommand, DomainEvent[]>
    {
        private readonly IPersonAggregate _personAggregate;
        private readonly IPersonGetByIDViewProjector _personGetByIDViewProjector;

        public PersonUpdateCommandHandler(IPersonAggregate personAggregate, IPersonGetByIDViewProjector personGetByIDViewProjector)
        {
            _personAggregate = personAggregate;
            _personGetByIDViewProjector = personGetByIDViewProjector;
        }

        public async Task<DomainEvent[]> Handle(PersonUpdateCommand request, CancellationToken cancellationToken)
        {
            await _personAggregate.Load(request.ID, cancellationToken);

            await _personAggregate.Update(request.Name, cancellationToken);

            var events = await _personAggregate.SaveChanges(cancellationToken);

            await _personGetByIDViewProjector.Project(events, cancellationToken);

            return events;
        }
    }
}
