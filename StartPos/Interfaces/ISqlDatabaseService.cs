namespace StartPos.Services
{
    internal interface ISqlDatabaseService
    {
        void BackupDatabase();

        void BackupTable(string query);

        void CheckIntegrity();

        string DateCreation();

        void Defragmentation();

        bool IsConnectionCorrectly(string host, string instance, decimal port, string database, string password);

        long SizeMb();

        string Version();
    }
}