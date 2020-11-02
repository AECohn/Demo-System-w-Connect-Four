using Crestron.SimplSharp;                              // For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.Diagnostics;
using Crestron.SimplSharpPro.DM.Streaming;
using Crestron.SimplSharpPro.UI;
using System;
using System.Timers;

namespace _4_Series_Processor_Template
{
    public class ControlSystem : CrestronControlSystem
    {
        public static Tsw1060 Touchpanel;
        private Timer ClockTimer = new System.Timers.Timer();
        private Timer Win_Flash = new System.Timers.Timer();
        private Calculator MyCalc = new Calculator();
        private int Calc_Mode;
        private int NumA, NumB;
        private bool Flash_Color;

        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        ///
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        ///
        /// You cannot send / receive data in the constructor
        /// </summary>
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                Touchpanel = new Tsw1060(04, this);
                Touchpanel.Register();
                Touchpanel.SigChange += Touchpanel_SigChange;
                Touchpanel.ButtonStateChange += Touchpanel_ButtonStateChange;

                ClockTimer.AutoReset = true;
                ClockTimer.Interval = 1000;
                ClockTimer.Elapsed += ClockTimer_Elapsed;
                Win_Flash.AutoReset = true;
                Win_Flash.Interval = 350;
                Win_Flash.Elapsed += Win_Flash_Elapsed;

                
                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);
                CrestronEnvironment.GetLocalTime();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        private void Win_Flash_Elapsed(object sender, ElapsedEventArgs e)
        {
            Flash_Color = !Flash_Color;
            foreach (UInt16 item in ConnectFour.Winning_Positions)
            {
                if (Flash_Color)
                {
                    Touchpanel.UShortInput[item].UShortValue = 0;
                }
                else
                {
                    if (ConnectFour.is_red)
                    {
                        Touchpanel.UShortInput[item].UShortValue = 2;
                    }
                    else
                    {
                        Touchpanel.UShortInput[item].UShortValue = 1;
                    }
                }
            }
        }

        private void ClockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Touchpanel.StringInput[1].StringValue = System.DateTime.Now.ToString();
        }

        private void Touchpanel_ButtonStateChange(GenericBase device, ButtonEventArgs args)  //Physical buttons on the TSW1060
        {
            switch (args.Button.State)
            {
                case eButtonState.Pressed:
                    switch (args.Button.Name)
                    {
                        case eButtonName.Power:
                            break;

                        case eButtonName.Home:
                            Touchpanel.BooleanInput[13].BoolValue = true;
                            break;

                        case eButtonName.Lights:

                            break;

                        case eButtonName.Up:
                            ClockTimer.Start();
                            break;

                        case eButtonName.Down:
                            ClockTimer.Stop();
                            break;
                    }
                    break;
            }
        }

        private void Clear_TP()
        {
            //string value;
            for (int i = 1; i < 8; i++)
            {
                for (int x = 0; x < 6; x++)
                {
                    Touchpanel.UShortInput[Convert.ToUInt16($"{i}{x}")].UShortValue = 0;
                }
            }
            Touchpanel.StringInput[10].StringValue = "Yellow to Play";
            Win_Flash.Stop();
        }

        private static void Output_Board()
        {
            for (int i = 5; i >= 0; i--)
            {
                String temp = $"{ConnectFour.board_tracker[i, 0]}{ConnectFour.board_tracker[i, 1]}{ConnectFour.board_tracker[i, 2]}{ConnectFour.board_tracker[i, 3]}{ConnectFour.board_tracker[i, 4]}{ConnectFour.board_tracker[i, 5]}{ConnectFour.board_tracker[i, 6]}";

                CrestronConsole.PrintLine(temp);
            }
        }

        private void Column_Updater(ushort Column_Num)
        {
            if (ConnectFour.Winner == false)
            {
                ushort join = Convert.ToUInt16((Column_Num + 1) * 10);
                ushort row = 0;

                for (ushort i = join; i <= join + 5; i++)
                {
                    Touchpanel.UShortInput[i].UShortValue = ConnectFour.board_tracker[row, Column_Num];
                    row++;
                }
            }
        }

        private void Touchpanel_SigChange(BasicTriList currentDevice, SigEventArgs args) //Interaction with the TSW1060 Touch Screen
        {
            switch (args.Sig.Type)
            {
                case eSigType.Bool:

                    switch (args.Sig.BoolValue)
                    {
                        case true:

                            switch (args.Sig.Number)
                            {
                                case 10:
                                    for (ushort i = 10; i <= 13; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    break;

                                case 11:
                                    for (ushort i = 10; i <= 13; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;                                    //Interlocked Pages
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    break;

                                case 12:
                                    for (ushort i = 10; i <= 13; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    break;

                                case 20:
                                    for (ushort i = 20; i <= 23; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    Calc_Mode = 1; //add
                                    Calc_Logic();
                                    break;

                                case 21:
                                    for (ushort i = 20; i <= 23; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    Calc_Mode = 2; //subtract
                                    Calc_Logic();
                                    break;

                                case 22:
                                    for (ushort i = 20; i <= 23; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    Calc_Mode = 3; //divide
                                    Calc_Logic();
                                    break;

                                case 23:
                                    for (ushort i = 20; i <= 23; i++)
                                    {
                                        currentDevice.BooleanInput[i].BoolValue = false;
                                    }
                                    currentDevice.BooleanInput[args.Sig.Number].BoolValue = true;
                                    Calc_Mode = 4; //multiply
                                    Calc_Logic();
                                    break;

                                case 50:
                                    ConnectFour.column_counter(0);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 51:
                                    ConnectFour.column_counter(1);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 52:
                                    ConnectFour.column_counter(2);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 53:
                                    ConnectFour.column_counter(3);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 54:
                                    //example
                                    ConnectFour.column_counter(4);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 55:
                                    ConnectFour.column_counter(5);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 56:
                                    ConnectFour.column_counter(6);

                                    currentDevice.StringInput[10].StringValue = ConnectFour.Determine_Winner();
                                    Column_Updater(Convert.ToUInt16(args.Sig.Number - 50));
                                    if (ConnectFour.Winner)
                                    {
                                        Win_Flash.Start();
                                    }

                                    break;

                                case 57:
                                    CrestronConsole.PrintLine("Board Cleared");
                                    Clear_TP();
                                    ConnectFour.Clear_Board();
                                    break;
                            }
                            break;

                        case false:

                            switch (args.Sig.Number)
                            {
                                case 1:
                                    break;

                                case 2:
                                    break;

                                case 3:
                                    break;
                            }
                            break;
                    }

                    break;

                case eSigType.UShort:
                    switch (args.Sig.Number)
                    {
                        case 1:
                            ushort X_FB = currentDevice.UShortOutput[args.Sig.Number].UShortValue;
                            currentDevice.UShortInput[args.Sig.Number].UShortValue = X_FB;
                            currentDevice.UShortInput[3].UShortValue = (ushort)CrestronEnvironment.ScaleWithLimits(X_FB, 65535, 0, 100, 0);
                            break;

                        case 2:
                            currentDevice.UShortInput[args.Sig.Number].UShortValue = currentDevice.UShortOutput[args.Sig.Number].UShortValue;
                            break;

                        case 3:
                            break;

                        case 10:
                            NumA = CrestronEnvironment.ScaleWithLimits(currentDevice.UShortOutput[args.Sig.Number].UShortValue, 65535, 0, 10000000, 0);
                            currentDevice.UShortInput[args.Sig.Number].UShortValue = currentDevice.UShortOutput[args.Sig.Number].UShortValue;
                            currentDevice.StringInput[args.Sig.Number].StringValue = NumA.ToString();
                            Calc_Logic();
                            break;

                        case 11:
                            NumB = CrestronEnvironment.ScaleWithLimits(currentDevice.UShortOutput[args.Sig.Number].UShortValue, 65535, 0, 10000000, 0);
                            currentDevice.UShortInput[args.Sig.Number].UShortValue = currentDevice.UShortOutput[args.Sig.Number].UShortValue;
                            currentDevice.StringInput[args.Sig.Number].StringValue = NumB.ToString();
                            Calc_Logic();
                            break;
                    }
                    break;

                case eSigType.String:

                    switch (args.Sig.Number)
                    {
                        case 1:
                            break;

                        case 2:
                            break;

                        case 3:
                            break;
                    }
                    break;
            }
        }

        private void Calc_Logic()
        {
            switch (Calc_Mode)
            {
                case 1: //add
                    {
                        Touchpanel.StringInput[2].StringValue = MyCalc.Add(NumA, NumB).ToString();
                        break;
                    }

                case 2: //subtract
                    {
                        Touchpanel.StringInput[2].StringValue = MyCalc.Subtract(NumA, NumB).ToString();
                        break;
                    }

                case 3: //divide
                    {
                        Touchpanel.StringInput[2].StringValue = MyCalc.Divide(NumA, NumB).ToString();
                        break;
                    }

                case 4: //multiply
                    {
                        Touchpanel.StringInput[2].StringValue = MyCalc.Multiply(NumA, NumB).ToString();
                        break;
                    }
            }
        }

        /// <summary>
        /// InitializeSystem - this method gets called after the constructor
        /// has finished.
        ///
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        ///
        /// Please be aware that InitializeSystem needs to exit quickly also;
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {
                ClockTimer.Start();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down.
        /// Use these events to close / re-open sockets, etc.
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values
        /// such as whether it's a Link Up or Link Down event. It will also indicate
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        private void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for.
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;

                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        private void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    ClockTimer.Stop();
                    break;

                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    ClockTimer.Start();
                    break;

                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads.
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    ClockTimer.Dispose();
                    break;
            }
        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        private void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;

                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;

                case (eSystemEventType.Rebooting):
                    //The system is rebooting.
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }
        }
    }
}