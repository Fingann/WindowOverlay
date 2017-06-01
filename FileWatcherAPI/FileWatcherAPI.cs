// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileWatcherAPI.cs" company="BKK AS">
//   BKK AS
// </copyright>
// <summary>
//   Defines the FileWatcherAPI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FileWatcherAPICLI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using global::FileWatcherAPI.Models;

    public delegate void SetupEventHandler(object source, Call e);
    public delegate void AlertingEventHandler(object source, Call e);

    public delegate void ConnectedEventHandler(object source, Call e);

    public delegate void HangupEventHandler(object source, Call e);

    public delegate void NoAnswerEventHandler(object source, Call e);

    public delegate void AgentPauseOnEventHandler(object source, bool e);

    public delegate void AgentPauseOffEventHandler(object source, bool e);

    public delegate void AbortedEventHandler(object source, Call e);


    public class FileWatcherAPI
    {
        DateTime LastRead = DateTime.MinValue;

        private ReverseLineReader reverseReader;

        


        private WatcherEvents callEvents; 

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the file watcher.
        /// </summary>
        public FileSystemWatcher FileWatcher
        {
            get
            {
                return this.fileWatcher;
            }

            set
            {
                this.fileWatcher = value;
            }
        }

        private ReverseLineReader ReverseReader
        {
            get
            {
                return this.reverseReader;
            }

            set
            {
                this.reverseReader = value;
            }
        }

        public WatcherEvents Events
        {
            get
            {
                return this.callEvents;
            }
            set
            {
                this.callEvents = value;
            }
        }

        /// <summary>
        /// The file watcher.
        /// </summary>
        private FileSystemWatcher fileWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcherAPI"/> class.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        public FileWatcherAPI(string filePath)
        {
            this.Events = new WatcherEvents();
            this.fileWatcher  = new FileSystemWatcher();
            this.FilePath = filePath;
            this.FileWatcher.Path = Path.GetDirectoryName(filePath);
            this.FileWatcher.Filter = Path.GetFileName(filePath);
            this.FileWatcher.Changed += this.FileChanged;
            this.ReverseReader = new ReverseLineReader(filePath, Encoding.GetEncoding("ISO-8859-1"));
        }

        /// <summary>
        /// The file changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(FilePath);
            if (lastWriteTime != this.LastRead)
            {
                var s = this.ReverseReader.FirstOrDefault();
                this.Events.ProcessLines(s);
                this.LastRead = lastWriteTime;
            }
           


        }

        /// <summary>
        /// Start the fileWatcher.
        /// </summary>
        public void Start()
        {
            this.FileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stop the fileWatcher.
        /// </summary>
        public void Stop()
        {
            this.FileWatcher.EnableRaisingEvents = false;
        }
            

    }
}
