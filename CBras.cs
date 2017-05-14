using System;
using Microsoft.SPOT;
using GR.Membres;
using System.Threading;


namespace GR
{
  class CBras
  {
      public struct configBras
      {
          public byte idAx12BrasSupport;
          public byte idAx12BrasModule;
      }

      enum positionBras
      {
        haut = 500,
        bas = 833,
        intrmediaire = 766,
        ouverte = 540,
        fermee = 455
      };

        CAX12 m_ax12BrasSupport;
        CAX12 m_ax12BrasModule;

      public CBras(ControleurAX12 controleur, configBras config)
      {
          Debug.Print(""+controleur.m_port);
          m_ax12BrasSupport = new CAX12(config.idAx12BrasSupport, controleur.m_port, controleur.m_direction);
          Debug.Print("bras support actif");
          m_ax12BrasModule = new CAX12(config.idAx12BrasModule, controleur.m_port, controleur.m_direction);
          Debug.Print("bras module actif");
         // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
          //position initiale
          // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);

      }

      public void descendre(Couleur equipe)
      {
     
          m_ax12BrasSupport.move((int) positionBras.bas);
          Thread.Sleep(1000);
       }
      public void semidescendre(Couleur equipe)
      {
          m_ax12BrasModule.move((int)positionBras.fermee);
          Thread.Sleep(1000);
          m_ax12BrasSupport.move((int)positionBras.intrmediaire);
          Thread.Sleep(1000);
          m_ax12BrasModule.move((int)positionBras.ouverte);
          Thread.Sleep(1000);
      }


      public void lacher(Couleur equipe)
      {
          m_ax12BrasModule.move((int) positionBras.ouverte);
          Thread.Sleep(1000);
      }


      public void Monter(Couleur equipe)
      {
          m_ax12BrasModule.move((int)positionBras.fermee);
          Thread.Sleep(1000);
          m_ax12BrasSupport.move((int)positionBras.haut);
          Thread.Sleep(1000);
      }
  }
}
