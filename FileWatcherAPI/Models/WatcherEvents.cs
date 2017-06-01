using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcherAPI.Models
{
    using System.Text.RegularExpressions;

    using FileWatcherAPICLI;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class WatcherEvents
    {

        public event SetupEventHandler Setup;

        public event AlertingEventHandler Alerting;

        public event ConnectedEventHandler Connected;

        public event HangupEventHandler Hangup;

        public event NoAnswerEventHandler NoAnswer;

        public event AgentPauseOffEventHandler PauseOff;

        public event AgentPauseOnEventHandler PauseOn;

        public event AbortedEventHandler Aborted;




        /// <summary>
        ///     Function to 
        /// eck if a string is a JSON Object
        /// </summary>
        private static readonly Func<string, bool> IsJsonObject = x =>
        {
            try
            {
                JToken.Parse(x);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        };

        /// <summary>
        ///     The process lines.
        /// </summary>
        /// <param name="lastLinesInLog">
        ///     The last lines in log.
        /// </param>
        public void ProcessLines(string lastLinesInLog)
        {
            var agentPauseOff = new Regex("AgentPauseOff");
            var agentPauseOn = new Regex("AgentPauseOn");


            if (string.IsNullOrEmpty(lastLinesInLog)) return;

            if (!IsJsonObject(lastLinesInLog))
            {
                if (agentPauseOff.IsMatch(lastLinesInLog))
                {
                    this.PauseOff(this, true);
                }

                if (agentPauseOn.IsMatch(lastLinesInLog))
                {
                    this.PauseOn(this, true);
                }

                return;
            };

            Call currentCaller;

            try
            {
                currentCaller = JsonConvert.DeserializeObject<Call>(lastLinesInLog);
            }
            catch (JsonReaderException ex)
            {
                // this.Logger.Warn("ProcessLines could not Convert logline to Json: " + lastLinesInLog, ex);
                return;
            }
            catch (ArgumentNullException ex)
            {
                // this.Logger.Warn("ProcessLines could not convert logline to Json becasue its null ", ex);
                return;
            }

            // sorting the caller 
            this.SortCaller(currentCaller);
        }

        /// <summary>
        ///     The sort caller.
        /// </summary>
        /// <param name="currentCaller">
        ///     The current caller.
        /// </param>
        private void SortCaller(Call currentCaller)
        {

            switch (currentCaller.system_call_progress)
            {
                case "SETUP":
                    this.Setup(this, currentCaller);

                    // this.Logger.Info("CALL_Progress: SETUP -> Sennding new caller via Notification:" + currentCaller);

                    //// MessengerInstance.Send(new NotificationMessage<Call>(currentCaller, "New Call detected"));
                    // var popupmessage = new DefaultPopup() { Message = currentCaller.popup_name };
                    // MessengerInstance.Send(new NotificationMessage<IPopupMessage>(popupmessage, "New Call detected"));
                    break;

                case "ALERTING":
                    this.Alerting(this, currentCaller);

                    // this.Logger.Trace("CALL_Progress: ALERTING -> " + currentCaller);
                    break;
                case "CONNECTED":
                    this.Connected(this, currentCaller);

                    // this.Logger.Trace("CALL_Progress: CONNECTED -> " + currentCaller);
                    break;
                case "HANGUP":
                    this.Hangup(this, currentCaller);

                    // this.Logger.Trace("CALL_Progress: HANGUP -> " + currentCaller);
                    break;
                case "NOANSWER":
                    this.NoAnswer(this, currentCaller);
                    break;


                case "ABORTED":
                    this.Aborted(this, currentCaller);

                    // this.Logger.Trace("CALL_Progress: NOANSWER -> " + currentCaller);
                    break;

                // case "AgentPauseOff":
                // Console.WriteLine(5);
                // break;
                // case "AgentLogoff":
                // Console.WriteLine(5);
                // break;
                default:

                    // this.Logger.Trace("DEFAULT -> Could not find a CALL_Progress" + currentCaller);
                    break;
            }
        }
    }
}
