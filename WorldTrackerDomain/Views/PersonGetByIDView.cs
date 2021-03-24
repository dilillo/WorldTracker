using System.Collections.Generic;

namespace WorldTrackerDomain.Views
{
    public class PersonGetByIDView : DomainView
    {
        public PersonGetByIDView()
        {
        }

        public PersonGetByIDView(string personID)
        {
            ID = DomainViewIDs.PersonGetByID(personID);
        }

        public PersonGetByIDViewPerson Person { get; set; }
    }

    public class PersonGetByIDViewPerson
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }

        public int Visits { get; set; }
    }
}
