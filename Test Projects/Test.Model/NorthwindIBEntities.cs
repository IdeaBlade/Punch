namespace Test.Model
{
    public partial class NorthwindIBEntities
    {
        public NorthwindIBEntities(bool shouldConnect = true, string compositionContextName = null)
            : base(shouldConnect, compositionContextName: compositionContextName)
        {
        }
    }
}