
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
        public int BaseRoulante;
        public int AX12;
        public int MX64;
        public byte IdPince;
        public byte IdParasol;
        public configCanneAPeche ConfigCanne;
        public int InfrarougeAVD;
        public int InfrarougeAVG;
        public int InfrarougeARD;
        public int InfrarougeARG;
        public int CapteurUltrason;
        public int TelemetreLaser;
        public int SlotCanne;
    }
}