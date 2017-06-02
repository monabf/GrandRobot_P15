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
        intrmediaire = 760,
        ouverte = 550,
        fermee = 455
      };

        CAX12 m_ax12BrasSupport;
        CAX12 m_ax12BrasModule;

      public CBras(ControleurAX12 controleur, configBras config)
      {
          m_ax12BrasSupport = new CAX12(config.idAx12BrasSupport, controleur.m_port, controleur.m_direction);
          m_ax12BrasModule = new CAX12(config.idAx12BrasModule, controleur.m_port, controleur.m_direction);
          m_ax12BrasModule.setMode(AX12Mode.joint);
          m_ax12BrasSupport.setMode(AX12Mode.joint);
      }

      public void killBras()
      {
          m_ax12BrasSupport.setMode(AX12Mode.wheel);
          m_ax12BrasModule.setMode(AX12Mode.wheel);
      }

      public void Descendre(Couleur equipe)
      {
          m_ax12BrasSupport.move((int) positionBras.bas);
          Thread.Sleep(100);
       }
      public void SemiDescendre(Couleur equipe)
      {
          m_ax12BrasModule.move((int)positionBras.fermee);
          Thread.Sleep(50);
          m_ax12BrasSupport.move((int)positionBras.intrmediaire);
          Thread.Sleep(200);
          m_ax12BrasModule.move((int)positionBras.ouverte);
          Thread.Sleep(50);
      }


      public void Lacher(Couleur equipe)
      {
          m_ax12BrasModule.move((int) positionBras.ouverte);
          Thread.Sleep(150);
      }


      public void Monter(Couleur equipe)
      {
          m_ax12BrasModule.move((int)positionBras.fermee);
          Thread.Sleep(50);
          m_ax12BrasSupport.move((int)positionBras.haut);
          Thread.Sleep(1000);
      }
  }
}
