using System.Collections.Generic;

namespace WorldTrackerDomain.Views
{
    public class PlaceGetAllView : DomainView
    {
        public PlaceGetAllView()
        {
            ID = DomainViewIDs.PlaceGetAll;
        }

        public List<PlaceGetAllViewPlace> Places { get; set; } = new List<PlaceGetAllViewPlace>();
    }

    public class PlaceGetAllViewPlace
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
