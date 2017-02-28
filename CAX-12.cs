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
    class CAX_12
    {
        public enum AX12Mode { joint, wheel };
        public enum sens { reverse = 0, forward = 1024 };
        public enum speed { stop = 0, reverse = 1023, forward = 2047 }
        public enum Instruction : byte // instructions à envoyer dans les commandes
        {
            AX_PING = 0x01,
            AX_READ_DATA = 0x02,
            AX_WRITE_DATA = 0x03,
            AX_REG_WRITE = 0x04,
            AX_ACTION = 0x05,
            AX_RESET = 0x06,
            AX_SYNC_WRITE = 0x83,
        }
        public enum Address : byte
        {
            AX_MODEL_NUMBER_LOW = 0x00,
            AX_MODEL_NUMBER_HIGH = 0x01,
            AX_VERSION_FIRMWARE = 0x02,
            AX_ID = 0x03,
            AX_BAUD_RATE = 0x04,
            AX_RETURN_DELAY = 0x05,
            AX_CW_ANGLE_LIMIT_LOW = 0x06,
            AX_CW_ANGLE_LIMIT_HIGH = 0x07,
            AX_CCW_ANGLE_LIMIT_LOW = 0x08,
            AX_CCW_ANGLE_LIMIT_HIGH = 0x09,
            // 0x0A : Réservée
            AX_HIGH_LIMIT_TEMPERATURE = 0x0B,
            AX_LOW_LIMIT_VOLTAGE = 0x0C,
            AX_HIGH_LIMIT_VOLTAGE = 0x0D,
            AX_MAX_TORQUE_LOW = 0x0E,
            AX_MAX_TORQUE_HIGH = 0x0F,
            AX_STATUS_RETURN_LEVEL = 0x10,
            AX_ALARM_LED = 0x11,
            AX_ALARM_SHUTDOWN = 0x12,
            // 0x13 : Réservée
            AX_DOWN_CALIBRATION_LOW = 0x14,
            AX_DOWN_CALIBRATION_HIGH = 0x15,
            AX_UP_CALIBRATION_LOW = 0x16,
            AX_UP_CALIBRATION_HIGH = 0x17,
            AX_TORQUE_ENABLE = 0x18,
            AX_LED = 0x19,
            AX_CW_COMPLIANCE_MARGIN = 0x1A,
            AX_CCW_COMPLIANCE_MARGIN = 0x1B,
            AX_CW_COMPLIANCE_SLOPE = 0x1C,
            AX_CCW_COMPLIANCE_SLOPE = 0x1D,
            AX_GOAL_POSITION_LOW = 0x1E,
            AX_GOAL_POSITION_HIGH = 0x1F,
            AX_MOVING_SPEED_LOW = 020,
            AX_MOVING_SPEED_HIGH = 0x21,
            AX_TORQUE_LIMIT_LOW = 0x22,
            AX_TORQUE_LIMIT_HIGH = 0x23,
            AX_PRESENT_POSITION_LOW = 0x24,
            AX_PRESENT_POSITION_HIGH = 0x25,
            AX_PRESENT_SPEED_LOW = 0x26,
            AX_PRESENT_SPEED_HIGH = 0x27,
            AX_PRESENT_LOAD_LOW = 0x28,
            AX_PRESENT_LOAD_HIGH = 0x29,
            AX_PRESENT_VOLTAGE = 0x2A,
            AX_PRESENT_TEMPERATURE = 0x2B,
            AX_REGISTERED_INSTRUCTION = 0x2C,
            // 0x2D = Réservée
            AX_MOVING = 0x2E,
            AX_LOCK = 0x2F,
            AX_PUNCH_LOW = 0x30,
            AX_PUNCH_HIGH = 0x31,
        }

        OutputPort m_Direction;
        SerialPort m_serial;

        byte m_ID;
        AX12Mode m_mode = 0;
        int m_posCourrante = 0, m_posPrecedente = 0;
        int m_nbTour = 0;
        int m_nbImpulsion = 0;
        sens m_sens = 0;

        public CAX_12(byte id, SerialPort portserie, OutputPort direction)
        {
            m_serial = portserie;
            m_ID = id;
            m_Direction = direction;
            m_posCourrante = 0;
            m_posPrecedente = 0;
            m_nbTour = 0;

            Register U0FDR = new Register(0xE000C028, (8 << 4) | 1); // numéro du modèle
            Register PINSEL0 = new Register(0xE002C000); // version du firmware
        }

        // Méthode
        private byte calculeCRC(byte[] commande)
        {
            int taille = commande[3] + 2;
            byte crc = 0;
            for (int i = 2; i < taille + 1; i++)
            {
                crc += commande[i];
            }
            return (byte)(0xFF - crc);
        }

        public bool sendCommand(Instruction instruction, byte[] parametres)
        {
            byte[] commande = new byte[20];
            bool error = false;
            int length = 0;
            if (parametres != null)
            {
                length = parametres.Length;
            }

            commande[0] = 0xFF;
            commande[1] = 0XFF;
            commande[2] = m_ID;
            commande[3] = (byte)(length + 2);//len
            commande[4] = (byte)instruction;

            for (int i = 5; i < length + 5; i++)
            {
                commande[i] = parametres[i - 5];
            }
            commande[length + 5] = calculeCRC(commande);


            // send data
            if (m_serial.IsOpen)
            {
                // UART en transmission TX activé
                m_Direction.Write(true);
                // m_serial.DiscardInBuffer();
                // m_serial.DiscardOutBuffer();
                //Thread.Sleep(100); Pour enlever la latence
                m_serial.Write(commande, 0, length + 6);
                //wait till all is sent
                while (m_serial.BytesToWrite > 0) ;

                // UART en transmission TX desactivé
                m_Direction.Write(false);
                //Thread.Sleep(100); Pour le latence
                error = true;
            }
            return error;
            //the response is now coming back so you must read it
        }

        public int stop()
        {
            m_sens = 0;
            byte len, error = 1;
            int value = 0;

            byte[] buf = { 0x20, (byte)(value), (byte)(value >> 8) };
            sendCommand(Instruction.AX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }

        public bool move(int value)
        {
            bool erreur = false;

            if (m_mode == AX12Mode.joint)
            {
                byte[] buf = { (byte)Address.AX_GOAL_POSITION_LOW, (byte)(value), (byte)(value >> 8) };

                erreur = sendCommand(Instruction.AX_WRITE_DATA, buf);
            }
            return erreur;

        }


        public int getPosition()
        {
            int position = m_posCourrante + m_nbTour * 1023;
            return position;
        }

        public bool getReponse(out byte taille, out byte error, byte[] parametres)
        {
            byte[] reponse = new byte[20];
            bool erreur = false;
            int temp = 0;
            int nb = 0;
            do
            {
                nb++;
                temp = m_serial.Read(reponse, 0, 20);
            } while (temp == 0 && nb<10);

            if (temp > 5 && reponse[2] == m_ID)
            {

                taille = (byte)(reponse[3] - 2);
                error = reponse[4]; // 16 = CRC error
                if (error != 16)
                    erreur = true;
                if (parametres != null)
                {
                    for (int i = 0; i < taille; i++)
                    {
                        parametres[i] = reponse[5 + i];
                    }
                }

            }
            else
            {
                taille = 0;
                error = 0xff;
            }
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


        public int setMode(AX12Mode modeAX)
        {
            byte len, error = 1;

            m_mode = modeAX;
            if (m_mode == AX12Mode.joint)
            {
                int CW = 0;
                int CCW = 1023;
                byte[] limitsCW = { (byte)Address.AX_CW_ANGLE_LIMIT_LOW, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { (byte)Address.AX_CCW_ANGLE_LIMIT_LOW, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(Instruction.AX_WRITE_DATA, limitsCW);
                getReponse(out len, out error, null);
                sendCommand(Instruction.AX_WRITE_DATA, limitsCCW);
                getReponse(out len, out error, null);
            }

            else if (m_mode == AX12Mode.wheel)
            {
                int CW = 0;
                int CCW = 0;
                byte[] limitsCW = { (byte)Address.AX_CW_ANGLE_LIMIT_LOW, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { (byte)Address.AX_CCW_ANGLE_LIMIT_LOW, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(Instruction.AX_WRITE_DATA, limitsCW);
                getReponse(out len, out error, null);
                sendCommand(Instruction.AX_WRITE_DATA, limitsCCW);
                getReponse(out len, out error, null);


            }
            return (int)error;

        }

        public int setMovingSpeed(sens dir, int vitesse)
        {
            m_sens = dir;
            byte len, error = 1;
            int value = vitesse + (int)dir;

            byte[] buf = { 0x20, (byte)(value), (byte)(value >> 8) };
            sendCommand(Instruction.AX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }

        public int setTorque(int torqueValue) 
        {
            byte len, error = 1;
            
            byte[] buf = { 0x18, (byte)(torqueValue)};
            sendCommand(Instruction.AX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }

        public int setLED(int LEDValue)
        {
            byte len, error = 1;

            byte[] buf = { 0x19, (byte)(LEDValue) };
            sendCommand(Instruction.AX_WRITE_DATA, buf);
            getReponse(out len, out error, null);
            return (int)error;
        }


        public bool readPresentPosition(out int position)
        {

            bool erreur = false;
            byte len, error;
            byte[] pos = new byte[2];
            byte[] buf = { (byte)Address.AX_PRESENT_POSITION_LOW, 0x02 };
            sendCommand(Instruction.AX_READ_DATA, buf);
            if (getReponse(out len, out error, pos))
                erreur = true;
            position = pos[0] + (pos[1] << 8);
            return erreur;
        }

        public bool ReadPresentVoltage()
        {

            bool erreur = false;
            byte len, error;
            byte[] buf = { (byte)Address.AX_PRESENT_VOLTAGE, 0x02 };
            byte[] pos = new byte[2];
            sendCommand(Instruction.AX_READ_DATA, buf);
            if (getReponse(out len, out error, pos))
                erreur = true;
            m_posPrecedente = m_posCourrante;
            m_posCourrante = pos[0] + (pos[1] << 8);

            return erreur;
        }

        public bool readPresentSpeed(out int vitesse)
        {

            bool erreur = false;
            byte len, error;
            byte[] speed = new byte[2];
            byte[] buf = { (byte)Address.AX_PRESENT_SPEED_LOW, 0x02 };
            sendCommand(Instruction.AX_READ_DATA, buf);
            if (getReponse(out len, out error, speed))
                erreur = true;
            vitesse = speed[0] + (speed[1] << 8);
            return erreur;
        }

    }
}
