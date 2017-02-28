using System;
using Microsoft.SPOT;

namespace GR
{
  class CBras
  {
      struct configPince
      {
          public byte idAx12BrasSupport;
          public byte idAx12BrasModule;
      }

      enum positionPince
      {
        haut = ,
        bas = ,
        ouverte = ,
        fermee = ,
      }

      public CBras(ControleurAX12 controleur, configPince config)
      {
          m_ax12BrasSupport = new CAX_12(config.idAx12BrasSupport, controleur.m_port, controleur.m_direction);
          m_ax12BrasModule = new CAX_12(config.idAx12BrasModule, controleur.m_port, controleur.m_direction);
         // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
          //position initiale
          // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);

      }

      public void attraper(Couleur equipe)
      {
          m_ax12BrasSupport.move((int) positionBras.bas);
          m_ax12BrasModule.move((int) positionBras.fermee);
          m_ax12BrasSupport.move((int) positionBras.haut);
      }


      public void lacher(Couleur equipe)
      {
          m_ax12BrasModule.move((int) positionBras.ouverte);
      }


      public void poserCylindre(Couleur equipe)
      {
          m_ax12BrasSupport.move((int) positionBras.bas);
      }



}
