using System.Collections.Generic;

namespace WorldTrackerDomain.Views
{
    public class PlaceGetByIDView : DomainView
    {
        public PlaceGetByIDView()
        {
        }

        public PlaceGetByIDView(string placeID)
        {
            ID = DomainViewIDs.PlaceGetByID(placeID);
        }

        public PlaceGetByIDViewPlace Place { get; set; }
    }

    public class PlaceGetByIDViewPlace
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }
        public bool IsDeleted { get; internal set; }
    }
}
