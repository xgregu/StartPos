namespace StartPos.Interfaces
{
    public interface IRemoteServerService
    {
        bool IsActive(string iPAdress, int port);
    }
}