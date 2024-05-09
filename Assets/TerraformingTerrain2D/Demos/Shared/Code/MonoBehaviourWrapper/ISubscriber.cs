
namespace DemosShared
{
    public interface ISubscriber : IUnityCallback
    {
        void Subscribe();
        void Unsubscribe();
    }
}