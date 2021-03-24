using MediatR;
using System;

namespace WorldTrackerDomain.Commands
{
    public class VisitCreateCommand : IRequest
    {
        public string PersonID { get; set; }

        public string PlaceID { get; set; }
    }
}
