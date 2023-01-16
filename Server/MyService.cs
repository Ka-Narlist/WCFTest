using System.Runtime.Serialization;

namespace Server
{
    public class MyService : IMyService
    {
        public string Test(int i)
        {
            return i.ToString();
        }
    }
}
