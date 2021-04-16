using Unity;

namespace StartPos.Shared.Interfaces
{
    public interface IConfigInitializer
    {
        void Initialize(IUnityContainer container);
    }
}