namespace WorldTrackerDomain.Views
{
    public class SummaryView : DomainView
    {
        public SummaryView()
        {
            ID = DomainViewIDs.Summary;
        }

        public int People { get; set; }

        public int Places { get; set; }

        public int Visits { get; set; }
    }
}
