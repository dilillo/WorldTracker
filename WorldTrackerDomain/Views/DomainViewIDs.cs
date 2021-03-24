using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTrackerDomain.Views
{
    public static class DomainViewIDs
    {
        public static string PersonGetAll => nameof(PersonGetAll);

        public static string PersonGetByID(string personID) => nameof(PersonGetByID) + $"({personID})";

        public static string PlaceGetAll => nameof(PlaceGetAll);

        public static string PlaceGetByID(string placeID) => nameof(PlaceGetByID) + $"({placeID})";

        public static string Summary => nameof(Summary);
    }
}
