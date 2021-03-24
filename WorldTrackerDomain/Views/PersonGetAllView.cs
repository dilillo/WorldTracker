using System.Collections.Generic;

namespace WorldTrackerDomain.Views
{
    public class PersonGetAllView : DomainView
    {
        public PersonGetAllView()
        {
            ID = DomainViewIDs.PersonGetAll;
        }

        public List<PersonGetAllViewPerson> People { get; set; } = new List<PersonGetAllViewPerson>();
    }

    public class PersonGetAllViewPerson
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
