using System.Threading.Tasks;

namespace StartPos.Interfaces
{
    public interface IBackupService
    {
        Task ObligatoryBackup();

        Task BackupPcPosDirectory();

        Task AlternativeBackup();

        Task AlternativeBackupOnlyToday();

        bool IsWriteAccessToFolder(string folderPath);
    }
}