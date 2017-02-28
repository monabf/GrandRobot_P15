
using GR.Membres;
namespace GR
{
    /// <summary>
    /// Configuration des ports d'un robot
    /// </summary>

    class ConfigurationPorts
    {
        public int IO;
        public int Jack;
        public int DetecteurIR;
        public int Plateforme;
        public int AX12;
        public configPince IdPince;
        public configBras IdBras;
        public byte IdPoussoir;
        public byte IdFunnyBras;
        public byte IdRotateur;
        public int InfrarougeAVD;
        public int InfrarougeAVG;
        public int InfrarougeARD;
        public int InfrarougeARG;
        public int CapteurUltrason;
    }

}
