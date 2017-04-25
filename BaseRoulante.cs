using System;
using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using System.Threading;

namespace GR.BR
{
enum etatBR
{
arrive = 1, bloque, stope
};



enum sens
{
avancer = 1, reculer = -1
};
struct positionBaseRoulante
{
public int x;
public int y;
public int alpha;
};

class CBaseRoulante
{
CKangaroo m_kangaroo;
public positionBaseRoulante m_posBR;
public etatBR m_status = 0;
RelayX1 relai = new RelayX1(4);

public CBaseRoulante(int numPort)
{
m_kangaroo = new CKangaroo(numPort);
m_posBR = new positionBaseRoulante();
m_kangaroo.init();

}
public void setCouleur(Couleur c)
{
if (c == Couleur.Violet)
{
m_posBR.x = 952;
m_posBR.y = 145;
m_posBR.alpha = 90;
}
else
{
m_posBR.x = 952;
m_posBR.y = 2855;
m_posBR.alpha = -90;
}
}

public void getPosition(ref positionBaseRoulante posBR)
{
posBR = m_posBR;
}

public int getDistanceParcourue(ref int distance)
{
int erreur = 0;
int posCodeur = 0;
erreur = m_kangaroo.getPosition(mode.drive, ref posCodeur);
// PE@M je propose la condition ci-dessous pour éviter la variable alphraReel_tm1
if (!erreur.Equals(0xE3))
{
distance = (int)(posCodeur / 5.68);
}
return erreur;
}

public int getAngleTourne(ref int angle)
{
int erreur = 0;
int posCodeur = 0;
erreur = m_kangaroo.getPosition(mode.turn, ref posCodeur);
// PE@M comme ci-dessus
if (!erreur.Equals(0xE3)){
angle = (int)(posCodeur / 14.25);
}
return erreur;
}

public void stop()
{
m_status = etatBR.stope;
m_kangaroo.start(mode.drive);
}



public double recallerAngle(double alpha)
{
double beta = alpha;
if (beta <= -180)
beta += 360;
if (beta > 180)
beta -= 360;
return beta;
}


public etatBR tourner(double alphaConsigne, vitesse v, ref int alphaReel)
{

int erreur = 0;
double delta = 0;
m_status = 0;
m_kangaroo.tourner(alphaConsigne, v);
//attente d'être arrive ou bloque ou stoppe
do
{
// Thread.Sleep(1000);
erreur = getAngleTourne(ref alphaReel);
// PE@M dans le doute, 'Equals' est plus sûr qu'un '=='
if (erreur.Equals(0xE3))
{
m_status = etatBR.bloque;
relai.TurnOn();
Thread.Sleep(1000);
relai.TurnOff();
Thread.Sleep(1000);
m_kangaroo.init();
}
else
{
delta = System.Math.Abs(alphaConsigne - alphaReel);
if (delta <= 3)
{
m_status = etatBR.arrive;
Thread.Sleep(500);
getAngleTourne(ref alphaReel); //on donne à alphaReel la valeur d'angle tourné réellement mesuré
m_kangaroo.init();
}
}
} while (m_status != etatBR.arrive && m_status != etatBR.bloque && m_status != etatBR.stope);
m_posBR.alpha = m_posBR.alpha + alphaReel;
//QUOI QU'IL ARRIVE on réactualise l'angle en l'indentant de combien on vient de tourner
return m_status;
}

public etatBR allerEn(double x, double y, sens s, vitesse speedDistance = vitesse.premiere, vitesse speedRotation = vitesse.vitesseRotationMin)
{
int erreur = 0;
int dureeBlocage = 0;
double distanceConsigne = 0;
int distanceReelle = 0, distanceReelle_tm1=0, ecartDistance = 0;
double alphaConsigne = 0;
int alphaReel = 0;
double delta = 0;


alphaConsigne = System.Math.Atan2((y - m_posBR.y), (x - m_posBR.x)) * 180 / System.Math.PI - m_posBR.alpha; //angle en degre
if (s == sens.reculer)
{
alphaConsigne = (int)(alphaConsigne + 180);
}
alphaConsigne = recallerAngle(alphaConsigne);
m_status = tourner(alphaConsigne, speedRotation, ref alphaReel);
//d'abord on tourne les roues pour être face à la direction désirée puis on avance tout droit de distanceConsigne

m_status = 0;
distanceConsigne = (int)s * (int)System.Math.Sqrt(System.Math.Pow((x - m_posBR.x), 2) + System.Math.Pow((y - m_posBR.y), 2));
m_kangaroo.allerEn(distanceConsigne, speedDistance, unite.mm);
//attente d'être arrive ou bloque ou stoppe
do
{
//distanceReelle_tm1 = distanceReelle;
//ligne inutile car les deux ont été initialisées à 0 et pas modifiées depuis ?
//erreur = getDistanceParcourue(ref distanceReelle);
//ecartDistance = System.Math.Abs(distanceReelle - distanceReelle_tm1);

//calcul inutile qui ne mène à rien ! il faudrait faire :
erreur = getDistanceParcourue (ref distanceReelle);
ecartDistance = (int)System.Math.Abs(distanceReelle - distanceConsigne);
//la variable distanceReelle_tm1 en devient alors obsolète me semble-t-il
// PE@M je plussoie, ils ont fait la même chose que pour la fonction tourner ;)

if (ecartDistance == 0.0)
{
dureeBlocage++;
}
/* else
{
dureeBlocage = 0;
}*/
if (erreur.Equals(0xE3) || dureeBlocage>=5) //si on est bloqués ; le critère de blocage semble adapté
{
m_status = etatBR.bloque;
relai.TurnOn();
Thread.Sleep(1000);
relai.TurnOff();
Thread.Sleep(1000);
m_kangaroo.init();
}
else
{
//si on est arrivés
//delta = System.Math.Abs(distanceConsigne - distanceReelle); utiliser ecartDistance directement
if (ecartDistance <= 5)
{
m_status = etatBR.arrive;
Thread.Sleep(500);
//erreur = getDistanceParcourue(ref distanceReelle); inutile, on a déjà la valeur d'erreur
m_kangaroo.init();
}
//si on a pas un ecartDistance<=5 le do continue à tourner et donc on continue à avancer me semble-t-il
}
} while (m_status != etatBR.arrive && m_status != etatBR.bloque && m_status != etatBR.stope);


m_posBR.alpha = m_posBR.alpha + alphaReel;
m_posBR.x = m_posBR.x + (int)(distanceReelle * System.Math.Cos(m_posBR.alpha * System.Math.PI / 180));
m_posBR.y = m_posBR.y + (int)(distanceReelle * System.Math.Sin(m_posBR.alpha * System.Math.PI / 180));
//la position est réactualisée à la position réelle (avec la distance réelle parcourue) et l'angle avec l'angle réel parcouru, c'est bien mais il faut le faire QUOI QU'IL ARRIVE même si on est arrivés ou bloqués ou quoi que ce soit ! C'est ensuite cette position réelle qui est utilisée pour le prochain allerEn (position attribut de m_posBR) donc normalement le suivi de trajectoire prend bien en compte qu'on repart de B' et non pas de B dans le prochain calcul

return m_status;
}

/* public void tourner(int angle)
{
int posCodeur = 0;
int alpha = 0;

m_kangaroo.tourner(angle);
m_kangaroo.getPosition(mode.turn, ref posCodeur);
alpha = (int)(posCodeur / (int)unite.degre);
m_posBR.alpha = m_posBR.alpha + alpha;
}*/
}
}
