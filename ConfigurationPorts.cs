
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
        public int Plateforme;
        public int AX12;
        public CPince.configPince pince;
        public CBras.configBras bras;
        public CReservoir.configReservoir reservoir;
        public byte IdFunnyBras;
        public int InfrarougeAVD;
        public int InfrarougeAVG;
        public int InfrarougeARD;
        public int InfrarougeARG;
        public int CapteurUltrason;
    }

}
