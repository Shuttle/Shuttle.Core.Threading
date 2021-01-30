namespace Shuttle.Core.Threading
{
    public interface ICancellationTokenSource
    {
        System.Threading.CancellationTokenSource Get();
        void Renew();
    }
}