using System;
using System.IO;
using System.Threading;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
//using GHI.Premium.Hardware;
//using GHI.Premium.Hardware.LowLevel;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
//using GHI.Processor;
using GHI.Pins;

namespace GR.Membres
{
    class ControleurAX12
    {
        public SerialPort m_port;
        public OutputPort m_direction;
        

        public ControleurAX12(int numSerialPort)
        {
            m_direction = new OutputPort((Cpu.Pin)EMX.IO26, false);  //IO26 si 11
            string COMPort = GT.Socket.GetSocket(numSerialPort, true, null, null).SerialPortName; //pin de la carte spider utiliser 11
            m_port = new SerialPort(COMPort, 1000000, Parity.None, 8, StopBits.One);
            m_port.ReadTimeout = 500;
            m_port.WriteTimeout = 500;
            m_port.Open();
            
        }
    }
}
