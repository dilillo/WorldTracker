using MediatR;
using System.Collections.Generic;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetAllQuery : IRequest<PersonGetAllView>
    {
    }
}
