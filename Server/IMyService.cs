using CoreWCF;

namespace Server
{
    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        string Test(int i);
    }
}
