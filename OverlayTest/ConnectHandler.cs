using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlayTest
{
    public delegate void ProgramRunningEventHandler(object source, bool e);
    public delegate void PointerChangedEventHandler(object source, IntPtr e);

    public class ConnectHandler
     {
         public ConnectHandler()
         {
             
         }
         public event ProgramRunningEventHandler RunningEvent;

         public event PointerChangedEventHandler PointerEvent;
        private IntPtr _pointer;
         private bool _programIsRunning;


        public bool ProgramIsRunning
         {
             get { return _programIsRunning; }
            set
            {
                if (value != _programIsRunning)
                {
                    _programIsRunning = value;
                    RunningEvent?.Invoke(this, _programIsRunning);

                }
                
            }
         }

         

         public IntPtr Pointer
        {
            get { return _pointer; }
             set
             {
                 if (value != _pointer)
                 {
                     _pointer = value;
                     PointerEvent?.Invoke(this, _pointer);
                 }
                 
             }
        }

       
    }
}

