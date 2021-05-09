using NHX.BBS.TS.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using NHX.BBS.TS.Screen;

namespace NHX.BBS.TS
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static TelnetServer TelnetServer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            TelnetServer = new TelnetServer(IPAddress.Any, 23);
            TelnetServer.Connected += TelnetConnected;
            TelnetServer.Disconnected += TelnetDisconnected;
            TelnetServer.ErrorConnection += TelnetErrorConnection;
            TelnetServer.ErrorRecive += TelnetErrorRecive;
            TelnetServer.ErrorSend += TelnetErrorSend;
            TelnetServer.ControlChar += InputControlChar;
            TelnetServer.MessageRecived += MessageRecived;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TelnetServer.PurgeSockets();
                await Task.Delay(500, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            TelnetServer.Start();
            _logger.LogInformation("Server started at: {time}", DateTimeOffset.Now);
            await base.StartAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Server stoped at: {time}", DateTimeOffset.Now);
            TelnetServer.Stop();
            await base.StartAsync(stoppingToken);
        }

        private void TelnetConnected(NVT nvt)
        {
            _logger.LogInformation("Telnet connection established at: {time} ID: {1} IP: {2}", DateTimeOffset.Now, nvt.nvtId, nvt.remoteEndPoint);
            nvt.screen = new Welcome(nvt, TelnetServer);
            nvt.screen.Show();
        }

        private void TelnetDisconnected(EndPoint remote)
        {
            _logger.LogInformation("Telnet connection finished at: {time} IP: {1}", DateTimeOffset.Now, remote.ToString());
        }
        private void TelnetErrorConnection(EndPoint remote)
        {
            _logger.LogInformation("Telnet connection error at: {time} IP: {1}", DateTimeOffset.Now, remote.ToString());
        }
        private void TelnetErrorRecive(EndPoint remote)
        {
            _logger.LogInformation("Telnet error when recived data at: {time} IP: {1}", DateTimeOffset.Now, remote.ToString());
        }
        private void TelnetErrorSend(EndPoint remote)
        {
            _logger.LogInformation("Telnet error when send data at: {time} IP: {1}", DateTimeOffset.Now, remote.ToString());
        }
        private void InputControlChar(NVT nvt, byte[] data, int bytesRecived)
        {
            nvt.screen.HandleChar(data, bytesRecived);
        }
        private void MessageRecived(NVT nvt, string data)
        {
            nvt.screen.HandleMessage(data);
        }
    }
}
