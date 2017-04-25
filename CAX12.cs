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
using GHI.Processor;

namespace PR
{
    enum AX12Mode { joint, wheel };

    enum speed { stop = 0, reverse = 1023, forward = 2047 }

    enum Instruction : byte
    {
        AX_PING = 0x01,
        AX_READ_DATA = 0x02,
        AX_WRITE_DATA = 0x03,
        AX_REG_WRITE = 0x04,
        AX_ACTION = 0x05,
        AX_RESET = 0x06,
        AX_SYNC_WRITE = 0x83,
    }

    enum Address : byte
    {
        AX_ID = 0x03,
        AX_CW_LIMIT = 0x06,
        AX_CCW_LIMIT = 0x08,
        AX_LED = 0x19,
    }

    class CAX12
    {
        OutputPort m_Direction;
        SerialPort m_serial;
        private byte[] m_commande = new byte[20];
        byte m_ID = 0;
        AX12Mode m_mode = 0;
        uint m_posCourrante = 0, m_posPrecedente = 0;
        int m_nbTour = 0;
        speed m_speed = speed.stop;


        public CAX12(byte ID, SerialPort portserie, OutputPort direction)
        {
            m_serial = portserie;
            m_ID = ID;
            m_Direction = direction;


            Register U0FDR = new Register(0xE000C028, (8 << 4) | 1);
            // U0FDR.Write((8 << 4) | 1);//fix the fractional divider register
            Register PINSEL0 = new Register(0xE002C000);

        }


        public int setMode(AX12Mode modeAX)
        {
            byte len, error = 1;
            byte outID;
            m_mode = modeAX;
            if (m_mode == AX12Mode.joint)
            {
                int CW = 0;
                int CCW = 1023;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCW);
                getReponse(out outID, out len, out error, null);
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCCW);
                getReponse(out outID, out len, out error, null);
            }

            else if (m_mode == AX12Mode.wheel)
            {
                int CW = 0;
                int CCW = 0;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCW);
                getReponse(out outID, out len, out error, null);
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCCW);
                getReponse(out outID, out len, out error, null);

            }
            return (int)error;

        }

        private byte calculeCRC()
        {
            int taille = m_commande[3] + 2;
            byte crc = 0;
            for (int i = 2; i < taille + 1; i++)
            {
                crc += m_commande[i];
            }
            return (byte)(0xFF - crc);
        }



        public bool sendCommand(byte ID, Instruction instruction, byte[] parametres)
        {
            bool error = false;
            int length = 0;
            if (parametres != null)
            {
                length = parametres.Length;
            }

            m_commande[0] = 0xFF;
            m_commande[1] = 0XFF;
            m_commande[2] = ID;
            m_commande[3] = (byte)(length + 2);//len
            m_commande[4] = (byte)instruction;

            for (int i = 5; i < length + 5; i++)
            {
                m_commande[i] = parametres[i - 5];
            }
            m_commande[length + 5] = calculeCRC();


            // send data
            if (m_serial.IsOpen)
            {
                // UART en transmission TX activé
                m_Direction.Write(true);
                //   m_serial.DiscardInBuffer();
                //m_serial.DiscardOutBuffer();
                Thread.Sleep(100);
                m_serial.Write(m_commande, 0, length + 6);
                //wait till all is sent
                while (m_serial.BytesToWrite > 0) ;

                // UART en transmission TX desactivé
                m_Direction.Write(false);
                // Thread.Sleep(100);
                error = true;
            }
            return error;
            //the response is now coming back so you must read it
        }


        public bool getReponse(out byte ID, out byte taille, out byte error, byte[] parametres)
        {
            bool erreur = false;
            for (int i = 0; i < m_commande.Length; i++)
                m_commande[i] = 0;
            int temp = 0;
            int nbByte = m_serial.BytesToRead;
            do
            {
                temp = m_serial.Read(m_commande, 0, 20);
            } while (temp == 0);

            if (temp < 5)
            {
                ID = 0;
                taille = 0;
                error = 0xff;
            }
            else
            {

                ID = m_commande[2];
                taille = (byte)(m_commande[3] - 2);
                error = m_commande[4]; // 16 = CRC error
                if (error != 16)
                    erreur = true;
                if (parametres != null)
                {
                    for (int i = 0; i < taille; i++)
                    {
                        parametres[i] = m_commande[5 + i];
                    }
                }
            }
            return erreur;
        }

        public bool move(int value)
        {
            byte len, error = 1;
            byte outID;
            bool erreur = false;

            if (m_mode == AX12Mode.joint)
            {
                byte[] buf = { 0x1E, (byte)(value), (byte)(value >> 8) };

                erreur = sendCommand(m_ID, Instruction.AX_WRITE_DATA, buf);
                getReponse(out outID, out len, out error, null);

            }
            return erreur;

        }



        public int setMovingSpeed(speed vitesse)
        {
            m_speed = vitesse;
            byte len, error = 1;
            int value = (int)vitesse;
            if (m_mode == AX12Mode.wheel)
            {
                byte[] buf = { 0x20, (byte)(value), (byte)(value >> 8) };
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, buf);
                Thread.Sleep(100);
                getReponse(out m_ID, out len, out error, null);
            }

            return (int)error;
        }

        public bool getPosition(out uint position)
        {
            bool erreur = false;
            byte len, error;
            byte[] pos = new byte[2];
            byte[] buf = { 0x24, 0x02 };
            sendCommand(m_ID, Instruction.AX_READ_DATA, buf);
            Thread.Sleep(100);
            m_posPrecedente = m_posCourrante;
            if (getReponse(out m_ID, out len, out error, pos))
                erreur = true;
            m_posCourrante = (uint)pos[0] + (uint)(pos[1] << 8);

            if (m_mode == AX12Mode.wheel)
            {
                if (m_speed == speed.reverse && m_posCourrante < m_posPrecedente && m_posCourrante >= 0 && m_posPrecedente <= 1023)
                    m_nbTour--;
                if (m_speed == speed.forward && m_posCourrante > m_posPrecedente && m_posCourrante <= 1023 && m_posPrecedente >= 0)
                    m_nbTour++;
            }
            position = m_posCourrante;
            return erreur;
        }
    }

}
