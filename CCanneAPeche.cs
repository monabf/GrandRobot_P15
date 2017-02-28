using System;
using Microsoft.SPOT;
using System.IO.Ports;
using GT = Gadgeteer;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace GR.Membres
{
    struct configCanneAPeche
    {
        public SerialPort portSerie;
        public byte idMx64;  
        public byte idRx64;
        public OutputPort direction;
    };

    class CCanneAPeche
    {
        enum positionCaP
        {
            baissee = 462,
            rangee = 149,
            ouvrir=3800,    //mx64
            fermer=3231     //mx64
        };

        CRX_64 m_rx64;
        CRX_64 m_mx64;

        public CCanneAPeche(configCanneAPeche config)
        {
            m_rx64 = new CRX_64(config.idRx64, config.portSerie,config.direction);
           // m_rx64.setMode(MxRx64Mode.joint);
            m_mx64 = new CRX_64(config.idMx64, config.portSerie, config.direction);
            //m_mx64.setMode(MxRx64Mode.joint);
            ranger();
        }

        public void baisser()
        {
            m_rx64.move((int)positionCaP.baissee);
        }

        public void ranger()
        {
            m_mx64.move((int)positionCaP.fermer);
            m_rx64.move((int)positionCaP.rangee);
          
        }

        public void lacherPoisson()
        {
            m_rx64.move((int)positionCaP.baissee);
            Thread.Sleep(1000);
            m_mx64.move((int)positionCaP.ouvrir);
        }


    }
}