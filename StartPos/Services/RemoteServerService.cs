using NLog;
using StartPos.Interfaces;
using System;
using System.Net.Sockets;

namespace StartPos.Services
{
    internal class RemoteServerService : IRemoteServerService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public bool IsActive(string iPAdress, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    if (!client.ConnectAsync(iPAdress, port).Wait(TimeSpan.FromSeconds(2)))
                    {
                        _logger.Error($"{nameof(IsActive)} | No response from the host:'{iPAdress}:{port}'");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, $"{nameof(IsActive)} | Error pinging host:'{iPAdress}:{port}'");
                return false;
            }
            _logger.Info($"{nameof(IsActive)} | Host is active:'{iPAdress}:{port}'");
            return true;
        }
    }
}