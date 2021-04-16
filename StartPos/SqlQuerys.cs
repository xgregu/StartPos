namespace StartPos
{
    public static class SqlQuerys
    {
        public const string SelectOperatorTable = "Select * from Operator";
        public const string SelectProfileConfigTable = "Select * from ProfileConfig";
        public const string SelectCustomerCardFormatTable = "Select * from CustomerCardFormat";
        public const string SelectPosDocCounterTable = "Select * from PosDocCounter";

        public const string DateCreationDatabase = @"SELECT      create_date
                                                     FROM        sys.databases
                                                     WHERE       name = '{0}'";

        public const string SizeDatabase =
            @"SELECT row_size_mb = CAST(SUM(CASE WHEN type_desc = 'ROWS' THEN size END) * 8. / 1024 AS DECIMAL(8,2))
                                            FROM sys.master_files WITH(NOWAIT)
                                            WHERE database_id = DB_ID()-- for current db
                                            GROUP BY database_id";

        public const string VersionDatabase =
            @"SELECT sversion_name = SUBSTRING(v.ver, 0, CHARINDEX('-', v.ver)), edition = SERVERPROPERTY('Edition')
                                                FROM (SELECT ver = @@VERSION) v";

        public const string BackupDatabase = "Backup database {0} to disk='{1}'";

        public const string DefragmentationDatabase =
            "DECLARE @TableName VARCHAR(255) DECLARE @sql NVARCHAR(500) DECLARE @fillfactor INT SET @fillfactor = 90 DECLARE TableCursor CURSOR FOR SELECT OBJECT_SCHEMA_NAME([object_id])+'.'+name AS TableName FROM sys.tables OPEN TableCursor FETCH NEXT FROM TableCursor INTO @TableName WHILE @@FETCH_STATUS = 0 BEGIN SET @sql = 'ALTER INDEX ALL ON ' + @TableName + ' REBuild WITH (FILLFACTOR = ' + CONVERT(VARCHAR(3),@fillfactor) + ')' EXEC (@sql) FETCH NEXT FROM TableCursor INTO @TableName END CLOSE TableCursor DEALLOCATE TableCursor";

        public const string CheckIntegrityDatabase = "DBCC CHECKDB WITH NO_INFOMSGS, ALL_ERRORMSGS";

        public const string RepairDataBase = @"dbcc checkdb({0}) with no_infomsgs
                                                        Alter database {0} set SINGLE_USER
                                                        dbcc checkdb({0}, REPAIR_REBUILD)
                                                        dbcc checkdb({0}, REPAIR_ALLOW_DATA_LOSS)
                                                        ALTER database {0} set MULTI_USER";
    }
}