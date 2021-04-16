using Ionic.Zip;
using NLog;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Extesions;
using StartPos.Shared.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace StartPos.Services
{
    internal class SqlDatabaseService : ISqlDatabaseService
    {
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly string _dirBackupDay;
        private readonly ILogger _logger;
        private readonly SqlConnection _sqlConnection;
        private readonly ILogger _windowLogger;
        private bool _isIntegrityPositive;
        private bool _isSqlConnectionActive;
        private bool _isTryRepairDatabase;
        private SqlCommand _sqlCommand;
        private SqlDataReader _sqlDataReader;

        public SqlDatabaseService(IConfig config, IContext context)
        {
            _context = context;
            _config = config;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _isTryRepairDatabase = true;
            _sqlConnection = new SqlConnection(SetSqlConnection());
            _isIntegrityPositive = true;
            _dirBackupDay = DateTime.Now.GetShortDay();
        }

        public void BackupDatabase()
        {
            if (!_isSqlConnectionActive)
                return;

            var destinationDirBackup = Path.Combine(_config.ObligatoryBackup.Path, "Bazy");
            var destinationDirFileName = _context.PcPosInfo.DataBase;
            if (!_isIntegrityPositive)
                destinationDirFileName += "-uszkodzona";
            destinationDirFileName += ".bak";
            var destinationFullName = Path.Combine(destinationDirBackup, destinationDirFileName);

            if (!Directory.Exists(destinationDirBackup))
                Directory.CreateDirectory(destinationDirBackup);

            if (File.Exists(destinationFullName))
                File.Move(destinationFullName, destinationFullName + ".old");

            var query = string.Format(SqlQuerys.BackupDatabase, _context.PcPosInfo.DataBase, destinationFullName);

            try
            {
                _windowLogger.Info($"Kopia zapasowa bazy danych {_context.PcPosInfo.DataBase}...");
                _logger.Info($"{nameof(BackupDatabase)} | Database backup {_context.PcPosInfo.DataBase}...");
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(query, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();
                if (File.Exists(destinationFullName + ".old"))
                    File.Delete(destinationFullName + ".old");

                try
                {
                    var destinationFullNameZip = Path.Combine(destinationDirBackup, $"{_context.PcPosInfo.DataBase}.zip");
                    _logger.Info($"{nameof(BackupDatabase)} | Zip backup {destinationFullNameZip}...");
                    using (var zip = new ZipFile())
                    {
                        zip.UpdateFile(destinationFullName);
                        zip.Save(destinationFullNameZip);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"{nameof(BackupDatabase)} | Error while zip database backup file");
                }

                if (File.Exists(destinationFullName))
                    File.Delete(destinationFullName);
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Błąd wykonywania kopii zapasowej bazy danych. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(BackupDatabase)} | Error while backup database");
                if (File.Exists(destinationFullName + ".old"))
                    File.Move(destinationFullName + ".old", destinationFullName);
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public void BackupTable(string query)
        {
            if (!_isSqlConnectionActive || !_isIntegrityPositive)
                return;

            var destinationDirBackup =
                Path.Combine(_config.ObligatoryBackup.Path, "Pliki", _dirBackupDay, "_Sql_Tables");
            var tableName = query.Split(' ').LastOrDefault();
            var destinationDirFileName = tableName + ".xml";

            if (!Directory.Exists(destinationDirBackup))
                Directory.CreateDirectory(destinationDirBackup);

            try
            {
                _windowLogger.Info($"Kopia zapasowa tabeli {tableName} bazy danych {_context.PcPosInfo.DataBase}...");
                _logger.Info($"{nameof(BackupTable)} | Backup of the table {tableName} of the database {_context.PcPosInfo.DataBase}...");
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(query, _sqlConnection)
                {
                    CommandTimeout = 900
                };

                var da = new SqlDataAdapter(_sqlCommand);
                var ds = new DataSet();
                da.Fill(ds);
                ds.Tables[0].WriteXml(Path.Combine(destinationDirBackup, destinationDirFileName), true);

                _sqlDataReader = _sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                _windowLogger.Error(
                    $"Błąd wykonywania kopii zapasowej tabeli {tableName} bazy danych. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(BackupTable)} | Error while backup table database");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public void CheckIntegrity()
        {
            if (!_isSqlConnectionActive)
                return;

            var reportIntegrity = string.Empty;
            try
            {
                _windowLogger.Info($"Sprawdzanie integralności bazy danych {_context.PcPosInfo.DataBase}...");
                _logger.Info($"{nameof(CheckIntegrity)} | Checking the integrity of the database {_context.PcPosInfo.DataBase}...");
                _sqlConnection.InfoMessage += delegate (object sender, SqlInfoMessageEventArgs e)
                {
                    reportIntegrity += "\n" + e.Message;
                };
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(SqlQuerys.CheckIntegrityDatabase, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();
                //_windowLogger.Info($"...zakończona bez błędów.");
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Błąd wykonywania integralności bazy danych. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(CheckIntegrity)} | Error while check integrity database");
            }
            finally
            {
                _sqlConnection.Close();

                if (reportIntegrity != string.Empty)
                {
                    _windowLogger.Error("Integralność bazy danych wynik negatywny. Szczegoły w log.");
                    _logger.Error($"{nameof(CheckIntegrity)} | Integrity database - negative. {reportIntegrity}");
                    _isIntegrityPositive = false;
                    Repair();
                }
                else
                {
                    _windowLogger.Info("Integralność bazy danych wynik pozytywny.");
                    _isIntegrityPositive = true;
                }
            }
        }

        public void Defragmentation()
        {
            if (!_isSqlConnectionActive || !_isIntegrityPositive)
                return;

            try
            {
                _windowLogger.Info($"Defragmentacja bazy danych {_context.PcPosInfo.DataBase}...");
                _logger.Info($"{nameof(Defragmentation)} | Database defragmentation {_context.PcPosInfo.DataBase} ...");
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(SqlQuerys.DefragmentationDatabase, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();
                //_windowLogger.Info($"...zakończona bez błędów.");
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Błąd wykonywania defragmentacji bazy danych. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(Defragmentation)} | Error while defragmentation database");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        private void Repair()
        {
            if (!_isSqlConnectionActive || !_isTryRepairDatabase || !_config.Contractor.IsContract)
                return;

            var query = string.Format(SqlQuerys.RepairDataBase, _context.PcPosInfo.DataBase);

            try
            {
                _windowLogger.Info($"Naprawa bazy danych {_context.PcPosInfo.DataBase}...");
                _logger.Info($"{nameof(Repair)} | Database repair {_context.PcPosInfo.DataBase} ...");
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(query, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();
                //_windowLogger.Info($"...zakończona bez błędów.");
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Błąd wykonywania naprawy bazy danych. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(Repair)} | Error while repair database");
            }
            finally
            {
                _sqlConnection.Close();
            }

            _isTryRepairDatabase = false;
            CheckIntegrity();
        }

        public string DateCreation()
        {
            if (!_isSqlConnectionActive)
                return "Null";

            var valueSqlQuery = string.Empty;
            var query = string.Format(SqlQuerys.DateCreationDatabase, _context.PcPosInfo.DataBase);
            try
            {
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(query, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();

                while (_sqlDataReader.Read())
                    return _sqlDataReader["create_date"].ToString().Split(',')[0];
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(DateCreation)} | Error while sql query");
            }
            finally
            {
                _sqlConnection.Close();
            }
            _logger.Info($"{nameof(DateCreation)} | {valueSqlQuery}");
            return valueSqlQuery;
        }

        public long SizeMb()
        {
            if (!_isSqlConnectionActive)
                return -1;

            var query = string.Format(SqlQuerys.SizeDatabase, _context.PcPosInfo.DataBase);
            long databaseSize = -1;
            try
            {
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(query, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();

                while (_sqlDataReader.Read())
                    databaseSize = long.Parse(_sqlDataReader["row_size_mb"].ToString().Split(',')[0]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(SizeMb)} | Error while sql query");
            }
            finally
            {
                _sqlConnection.Close();
            }
            _logger.Info($"{nameof(SizeMb)} | {databaseSize}");
            return databaseSize;
        }

        public string Version()
        {
            if (!_isSqlConnectionActive)
                return null;
            var version = string.Empty;
            try
            {
                _sqlConnection.Open();
                _sqlCommand = new SqlCommand(SqlQuerys.VersionDatabase, _sqlConnection)
                {
                    CommandTimeout = 900
                };
                _sqlDataReader = _sqlCommand.ExecuteReader();

                while (_sqlDataReader.Read())
                    version = _sqlDataReader["sversion_name"] + " - " + _sqlDataReader["edition"];
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while sql query");
            }
            finally
            {
                _sqlConnection.Close();
            }
            _logger.Info($"{nameof(Version)} | {version}");
            return version;
        }

        private string SetSqlConnection()
        {
            var primarySqlConnect = new SqlConnectionStringBuilder(
               $"Data Source=(local)\\{_config.Database.Instance}; Initial Catalog={_context.PcPosInfo.DataBase}; Integrated Security=True");
            var secondSqlConnect = new SqlConnectionStringBuilder(
                $"Server={_config.Database.Host},{_context.PcPosInfo.Port}; Database={_context.PcPosInfo.DataBase}; User Id={_config.Database.Username};Password={_config.Database.Password}");

            primarySqlConnect.ConnectTimeout = 1;
            secondSqlConnect.ConnectTimeout = 1;

            var connection = new SqlConnection(primarySqlConnect.ConnectionString);
            var connectionAlternative = new SqlConnection(secondSqlConnect.ConnectionString);
            var exceptionConnect = string.Empty;
            try
            {
                _logger.Info($"{nameof(SetSqlConnection)} | First attempt to connect to sql: {primarySqlConnect} ");
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    _isSqlConnectionActive = true;
                    _logger.Info($"{nameof(SetSqlConnection)} | Connection correct");
                    return primarySqlConnect.ConnectionString;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, $"{nameof(Version)} | No access to the database");
            }

            try
            {
                _logger.Info($"{nameof(SetSqlConnection)} | Second attempt to connect to sql: {primarySqlConnect} ");
                _logger.Info($"{nameof(Version)} | ");
                connectionAlternative.Open();
                if (connectionAlternative.State == ConnectionState.Open)
                {
                    connectionAlternative.Close();
                    _isSqlConnectionActive = true;
                    _logger.Info($"{nameof(SetSqlConnection)} | Connection correct");
                    return secondSqlConnect.ConnectionString;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, $"{nameof(Version)} | No access to the database");
            }

            return string.Empty;
        }

        public bool IsConnectionCorrectly(string host, string instance, decimal port, string database, string password)
        {
            _logger.Info($"{nameof(IsConnectionCorrectly)} | Checking connection... Host: {host}, Instance: {instance}, Port: {port}, DataBase: {database}, Password: {password}");
            var primarySqlConnect =
                new SqlConnectionStringBuilder(
                    $"Server={host},{port}; Database={database}; User Id=sa;Password={password}");
            var secondSqlConnect =
                new SqlConnectionStringBuilder(
                    $"Data Source={host}\\{instance}; Initial Catalog={database}; Integrated Security=True");

            primarySqlConnect.ConnectTimeout = 1;
            secondSqlConnect.ConnectTimeout = 1;

            using (var connection = new SqlConnection(primarySqlConnect.ConnectionString))
            {
                using (var connectionAlternative = new SqlConnection(secondSqlConnect.ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        _logger.Info($"{nameof(IsConnectionCorrectly)} | Connection is correctly. {primarySqlConnect.ConnectionString}");
                        return true;
                    }
                    catch
                    {
                    }

                    try
                    {
                        connectionAlternative.Open();
                        _logger.Info($"{nameof(IsConnectionCorrectly)} | Connection is correctly. {secondSqlConnect.ConnectionString}");
                        return true;
                    }
                    catch
                    {
                    }
                }
                _logger.Warn($"{nameof(IsConnectionCorrectly)} | Connection is incorrectly. Host: {host}, Instance: {instance}, Port: {port}, DataBase: {database}, Password: {password}");
                return false;
            }
        }
    }
}