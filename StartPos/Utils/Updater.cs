using AutoUpdaterDotNET;
using NLog;
using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared.Interfaces;
using StartPos.Update;
using System;
using System.Collections.Generic;
using System.Net;

namespace StartPos.Utils
{
    public class Updater : IUpdater
    {
        private readonly IFlow _flow;
        private readonly IConfig _config;

        public Updater(IFlow flow, IConfig config)
        {
            _flow = flow;
            _config = config;
        }

        public void CheckForUpdate()
        {
            if (_flow.UpdateMode != UpdaterMode.None)
            {
                UpdateActionsByFlow[_flow.UpdateMode]();
                AutoUpdater.Synchronous = true;
                AutoUpdater.Start(_config.Update.VersionInfoUrl);
            }
        }

        private Dictionary<UpdaterMode, Action> UpdateActionsByFlow => new Dictionary<UpdaterMode, Action>
        {
            { UpdaterMode.Normal, NormalUpdate },
            { UpdaterMode.Silent, SilentUpdate },
            { UpdaterMode.None, () => { } }
        };

        private void NormalUpdate()
        {
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Hours;
            AutoUpdater.RemindLaterAt = 1;
        }

        private void SilentUpdate()
        {
            if (!_config.Update.IsAutoUpdate)
                return;

            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.OpenDownloadPage = true;
            AutoUpdater.ReportErrors = false;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (args.Error != null)
            {
                logger.Error(args.Error is WebException
                    ? $"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | There is a problem reaching update server. Please check your internet connection and try again later."
                    : $"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | {args.Error.Message}");

                AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                return;
            }

            if (!args.IsUpdateAvailable)
            {
                logger.Info($"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | There is no update available");

                AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                return;
            }

            if (!RemoteFileExists(args.DownloadURL))
            {
                logger.Error($"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | Not found: {args.DownloadURL}");

                AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                return;
            }

            logger.Info($"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | There is update available");
            try
            {
                if (!AutoUpdater.DownloadUpdate(args))
                    return;

                logger.Info($"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | Starting the update...");
                Environment.Exit(0);
            }
            catch (Exception exception)
            {
                logger.Error($"{nameof(AutoUpdaterOnCheckForUpdateEvent)} | {exception.Message}");
            }
        }

        private static bool RemoteFileExists(string url)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                var response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }
    }
}