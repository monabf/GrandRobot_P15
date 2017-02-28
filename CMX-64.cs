using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using GHI.Pins;
using System.IO.Ports;
using Microsoft.SPOT.Hardware;
using GHI.Processor;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace GR.Membres
{   
    class CMX_64
    {
        public enum MX64Mode { joint, wheel, Multi_Turn };
        public enum sens { reverse = 0, forward = 1024 };
        public enum speed { stop = 0, reverse = 1023, forward = 2047 }
        public enum Instruction : byte // instructions à envoyer dans les commandes
        {
            MX_PING = 0x01,
            MX_READ_DATA = 0x02,
            MX_WRITE_DATA = 0x03,
            MX_REG_WRITE = 0x04,
            MX_ACTION = 0x05,
            MX_RESET = 0x06,
            MX_SYNC_WRITE = 0x83,
        }
        public enum Address : byte
        {
            MX_MODEL_NUMBER_LOW = 0x00,
            MX_MODEL_NUMBER_HIGH = 0x01,
            MX_VERSION_FIRMWARE = 0x02,
            MX_ID = 0x03,
            MX_BAUD_RATE = 0x04,
            MX_RETURN_DELAY = 0x05,
            MX_CW_ANGLE_LIMIT_LOW = 0x06,
            MX_CW_ANGLE_LIMIT_HIGH = 0x07,
            MX_CCW_ANGLE_LIMIT_LOW = 0x08,
            MX_CCW_ANGLE_LIMIT_HIGH = 0x09,
            // 0x0A : Réservée
            MX_HIGH_LIMIT_TEMPERATURE = 0x0B,
            MX_LOW_LIMIT_VOLTAGE = 0x0C,
            MX_HIGH_LIMIT_VOLTAGE = 0x0D,
            MX_MAX_TORQUE_LOW = 0x0E,
            MX_MAX_TORQUE_HIGH = 0x0F,
            MX_STATUS_RETURN_LEVEL = 0x10,
            MX_ALARM_LED = 0x11,
            MX_ALARM_SHUTDOWN = 0x12,
            // 0x13 : Réservée
            MX_MULTI_TURN_OFFSET_LOW = 0x14,
            MX_MULTI_TURN_OFFSET_HIGH = 0x15,
            MX_RESOLUTION_DIVIDER = 0x16,
            MX_UP_CALIBRATION_HIGH = 0x17,
            MX_TORQUE_ENABLE = 0x18,
            MX_LED = 0x19,
            MX_DERIVATE_GAIN = 0x1A,
            MX_INTEGRAL_GAIN = 0x1B,
            MX_PROPORTIONAL_GAIN = 0x1C,
            // 0x1D : Réservée
            MX_GOAL_POSITION_LOW = 0x1E,
            MX_GOAL_POSITION_HIGH = 0x1F,
            MX_MOVING_SPEED_LOW = 020,
            MX_MOVING_SPEED_HIGH = 0x21,
            MX_TORQUE_LIMIT_LOW = 0x22,
            MX_TORQUE_LIMIT_HIGH = 0x23,
            MX_PRESENT_POSITION_LOW = 0x24,
            MX_PRESENT_POSITION_HIGH = 0x25,
            MX_PRESENT_SPEED_LOW = 0x26,
            MX_PRESENT_SPEED_HIGH = 0x27,
            MX_PRESENT_LOAD_LOW = 0x28,
            MX_PRESENT_LOAD_HIGH = 0x29,
            MX_PRESENT_VOLTAGE = 0x2A,
            MX_PRESENT_TEMPERATURE = 0x2B,
            MX_REGISTERED_INSTRUCTION = 0x2C,
            // 0x2D = Réservée
            MX_MOVING = 0x2E,
            MX_LOCK = 0x2F,
            MX_PUNCH_LOW = 0x30,
            MX_PUNCH_HIGH = 0x31,
            MX_CURRENT_LOW = 0x44,
            MX_CURRENT_HIGH = 0x45,
            MX_TORQUE_CONTROL_MODE = 0x46,
            MX_GOAL_TORQUE_LOW = 0x47,
            MX_GOAL_TORQUE_HIGH = 0x48,
            MX_GOAL_ACCELERATION = 0x49,
        } 

        OutputPort m_Direction;
        SerialPort m_serial;

        private byte[] m_commande = new byte[20];
        byte m_ID = 0;
        MX64Mode m_mode = 0;
        int m_posCourrante = 0, m_posPrecedente = 0, m_nbTour = 0, m_nbImpulsion = 0;
        sens m_sens = 0;
        speed m_speed = speed.stop;

        public CMX_64(byte ID, SerialPort portserie, OutputPort direction)
        {
            m_serial = portserie;
            m_ID = ID;
            m_Direction = direction;
            m_posCourrante = 0;
            m_posPrecedente = 0;
            m_nbTour = 0;

            
            /*Fonctionne sans le model number et la version du firmware
            Model Number
            version AX-12 : 12
            Register U0FDR = new Register(0xE000C028, (8<<4) | 1);
            version MX-64 : 310
            Register U0FDR = new Register(0xE0136028, (8<<4) | 1);

            Version of Firmware
            version AX-12 : 24
            Register PINSEL0 = new Register(0xE002C000);
            version MX-64 : 36
            Register PINSEL0 = new Register(0xE003C000);*/
        }

        // Méthode
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

        public bool sendCommand(Instruction instruction, byte[] parametres)
        {
            bool error = false;
            int length = 0;

            if (parametres != null)
            {
                length = parametres.Length;
            }

            m_commande[0] = 0xFF;
            m_commande[1] = 0XFF;
            m_commande[2] = m_ID;
            m_commande[3] = (byte)(length + 2);
            m_commande[4] = (byte)instruction;

            for (int i = 5; i < length + 5; i++)
            {
                m_commande[i] = parametres[i - 5];
            }
            m_commande[length + 5] = calculeCRC();


            //Envoie de la commande 
            if (m_serial.IsOpen)
            {
                // UART en transmission TX activé
                m_Direction.Write(true);
                //m_serial.DiscardInBuffer();
                //m_serial.DiscardOutBuffer();
                Thread.Sleep(100);
                m_serial.Write(m_commande, 0, length + 6);
                while (m_serial.BytesToWrite > 0) ;
                // UART en transmission TX desactivé
                m_Direction.Write(false);
                Thread.Sleep(100);
                error = true;
            }
            return error;
        }

        public bool move(int value)
        {
            byte len, error = 1;
            bool erreur = false;

            if (m_mode == MX64Mode.joint)
            {
                byte[] buf = { 0x1E, (byte)(value), (byte)(value >> 8) };

                erreur = sendCommand(Instruction.MX_WRITE_DATA, buf);
                getReponse(out len, out error, null);

            }
            return erreur;
        }

        public int stop()
        {
            m_sens = 0;
            byte len, error = 1;
            int value = 0;

            byte[] buf = { 0x20, (byte)(value), (byte)(value >> 8) };
            sendCommand(Instruction.MX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }


        public int setMode(MX64Mode modeMX)
        {
            byte len, error = 1;
            m_mode = modeMX;

            if (m_mode == MX64Mode.joint)
            {
                int CW = 0;
                int CCW = 4095;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(Instruction.MX_WRITE_DATA, limitsCW);
                getReponse(out len, out error, null);
                sendCommand(Instruction.MX_WRITE_DATA, limitsCCW);
                getReponse(out len, out error, null);
            }

            else if (m_mode == MX64Mode.wheel)
            {
                int CW = 0;
                int CCW = 0;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(Instruction.MX_WRITE_DATA, limitsCW);
                getReponse(out len, out error, null);
                sendCommand(Instruction.MX_WRITE_DATA, limitsCCW);
                getReponse(out len, out error, null);

            }
            else if (m_mode == MX64Mode.Multi_Turn)
            {
                int CW = 4095;
                int CCW = 4095;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(Instruction.MX_WRITE_DATA, limitsCW);
                getReponse(out len, out error, null);
                sendCommand(Instruction.MX_WRITE_DATA, limitsCCW);
                getReponse(out len, out error, null);
            }
            return (int)error;
        }

        public int setMovingSpeed(speed vitesse)
        {
            m_speed = vitesse;
            byte len, error = 1;
            int value = (int)vitesse;

            if (m_mode == MX64Mode.wheel)
            {
                byte[] buf = { 0x20, (byte)(value), (byte)(value >> 8) };
                sendCommand(Instruction.MX_WRITE_DATA, buf);
                Thread.Sleep(100);
                getReponse(out len, out error, null);
            }
            return (int)error;
        }

        public int setTorque(int torqueValue)
        {
            byte len, error = 1;

            byte[] buf = { 0x18, (byte)(torqueValue) };
            sendCommand(Instruction.MX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }

        public int setLED(int LEDValue)
        {
            byte len, error = 1;

            byte[] buf = { 0x19, (byte)(LEDValue) };
            sendCommand(Instruction.MX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }


        public bool getReponse(out byte taille, out byte error, byte[] parametres)
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
                 m_ID = 0;
                 taille = 0;
                 error = 0xff;
             }
             else
             {

                 m_ID = m_commande[2];
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

        public bool getPosition(out int position)
        {
            bool erreur = false;
            byte len, error;
            byte[] pos = new byte[2];
            byte[] buf = { 0x24, 0x02 };
            sendCommand(Instruction.MX_READ_DATA, buf);
            Thread.Sleep(100);
            m_posPrecedente = m_posCourrante;
            if (getReponse(out len, out error, pos))
                erreur = true;
            m_posCourrante = pos[0] + (pos[1] << 8);

            if (m_mode == MX64Mode.wheel)
            {
                if (m_speed == speed.reverse && m_posCourrante < m_posPrecedente && m_posCourrante >= 0 && m_posPrecedente <= 1023)
                    m_nbTour--;
                if (m_speed == speed.forward && m_posCourrante > m_posPrecedente && m_posCourrante <= 1023 && m_posPrecedente >= 0)
                    m_nbTour++;
            }
            position = m_posCourrante;
            return erreur;
        }

        public bool getNbTour(out int nbTour, out int nbImpulsion)
        {

            bool erreur = false;
            // readPresentPosition
            if (m_posCourrante > 1000 && m_posCourrante < 1023)
            {
                Thread.Sleep(200);
            }
            else
            {
                int diff = m_posCourrante - m_posPrecedente;
                if (System.Math.Abs(diff) >= 512)  //abs(posCourante-posPrecedente)>512
                {

                    if (m_sens == sens.reverse)
                        m_nbTour--;
                    if (m_sens == sens.forward)
                        m_nbTour++;
                }
            }

            nbTour = m_nbTour;
            nbImpulsion = m_posCourrante;
            m_nbImpulsion = m_posCourrante;
            return erreur;
        }


        public bool readPresentPosition(out int position)
        {

            bool erreur = false;
            byte len, error;
            byte[] pos = new byte[2];
            byte[] buf = { (byte)Address.MX_PRESENT_POSITION_LOW, 0x02 };
            sendCommand(Instruction.MX_READ_DATA, buf);
            if (getReponse(out len, out error, pos))
                erreur = true;
            position = pos[0] + (pos[1] << 8);
            return erreur;
        }

        public bool readPresentSpeed(out int vitesse)
        {

            bool erreur = false;
            byte len, error;
            byte[] speed = new byte[2];
            byte[] buf = { (byte)Address.MX_PRESENT_SPEED_LOW, 0x02 };
            sendCommand(Instruction.MX_READ_DATA, buf);
            if (getReponse(out len, out error, speed))
                erreur = true;
            vitesse = speed[0] + (speed[1] << 8);
            return erreur;
        }

        public bool ReadPresentVoltage()
        {

            bool erreur = false;
            byte len, error;
            byte[] buf = { (byte)Address.MX_PRESENT_VOLTAGE, 0x02 };
            byte[] pos = new byte[2];
            sendCommand(Instruction.MX_READ_DATA, buf);
            if (getReponse(out len, out error, pos))
                erreur = true;
            m_posPrecedente = m_posCourrante;
            m_posCourrante = pos[0] + (pos[1] << 8);

            return erreur;
        }

    }
}
