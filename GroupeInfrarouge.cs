using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Gadgeteer;

namespace GR.Vision
{
    class GroupeInfrarouge
    {
        public readonly InputPort AVD, AVG, ARD, ARG;

        public GroupeInfrarouge(int etendeur, int avd, int avg, int ard, int arg)
        {
            AVD = new InputPort(Socket.GetSocket(etendeur, true, null, null).CpuPins[avd], false, Port.ResistorMode.PullUp);
            AVG = new InputPort(Socket.GetSocket(etendeur, true, null, null).CpuPins[avg], false, Port.ResistorMode.PullUp);
            ARD = new InputPort(Socket.GetSocket(etendeur, true, null, null).CpuPins[ard], false, Port.ResistorMode.PullUp);
            ARG = new InputPort(Socket.GetSocket(etendeur, true, null, null).CpuPins[arg], false, Port.ResistorMode.PullUp);
        }
    }
}
