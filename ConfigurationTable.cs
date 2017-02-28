namespace GR
{
    /// <summary>
    /// Dimensions de la table de jeu et positionnement des objets
    /// </summary>
    class ConfigurationTable
    {
        public readonly Vecteur2 Taille = new Vecteur2(2000, 3000);
        public readonly ObjetTable[] Cabines = new ObjetTable[2];
        public readonly ObjetTable[] CabinesAdversaire = new ObjetTable[2];
        public readonly ObjetTable[] Coquillages = new ObjetTable[5];
        public readonly ObjetTable[] CoquillagesAdversaire = new ObjetTable[5];
        public readonly ObjetTable[] CoquillagesNeutres = new ObjetTable[6];
        public readonly ObjetTable[] Barres = new ObjetTable[5];
        public readonly ObjetTable[] Dunes = new ObjetTable[5];
        public readonly ObjetTable Aquarium;
        public readonly ObjetTable AquariumAdversaire;
        public readonly ObjetTable Filet;
        public readonly ObjetTable FiletAdversaire;
        public readonly ObjetTable Serviette;
        public readonly ObjetTable ServietteAdversaire;

        public readonly int NumDisposition;

        /// <summary>
        /// Initialise la configuration à partir de valeurs prédéfinies
        /// </summary>
        /// <param name="equipe">Couleur de l'équipe</param>
        /// <param name="disposition">Numero de la disposition des coquillages</param>
        public ConfigurationTable(Couleur equipe, int disposition)
        {
            var serviettes = new ObjetTable[2];
            var aquariums = new ObjetTable[5];
            var filets = new ObjetTable[2];
            var cabines = new ObjetTable[4];
            var coquillages = new ObjetTable[16];

            NumDisposition = disposition;

            // initialisation de la position de chaque objet

            serviettes[0] = new ObjetTable(new Vecteur3(850, 150, 0), Couleur.Violet);
            serviettes[1] = new ObjetTable(new Vecteur3(850, 2850, 0), Couleur.Vert);

            aquariums[0] = new ObjetTable(new Vecteur3(2000, 498, 0), Couleur.Violet);
            aquariums[1] = new ObjetTable(new Vecteur3(2000, 2502, 0), Couleur.Vert);

            filets[0] = new ObjetTable(new Vecteur3(2000, 928, 0), Couleur.Violet);
            filets[1] = new ObjetTable(new Vecteur3(2000, 2072, 0), Couleur.Vert);

            cabines[0] = new ObjetTable(new Vecteur3(0, 300, 0), Couleur.Violet);
            cabines[1] = new ObjetTable(new Vecteur3(0, 600, 0), Couleur.Violet);
            cabines[2] = new ObjetTable(new Vecteur3(0, 2400, 0), Couleur.Vert);
            cabines[3] = new ObjetTable(new Vecteur3(0, 2700, 0), Couleur.Vert);

            switch (disposition)
            {
                case 1:
                    coquillages[0] = new ObjetTable(new Vecteur3(1250, 200, 0), Couleur.Neutre);
                    coquillages[1] = new ObjetTable(new Vecteur3(1550, 200, 0), Couleur.Neutre);
                    coquillages[2] = new ObjetTable(new Vecteur3(1810, 75, 44), Couleur.Violet);  
                    coquillages[3] = new ObjetTable(new Vecteur3(1925, 75, 66), Couleur.Violet);  
                    coquillages[4] = new ObjetTable(new Vecteur3(1925, 200, 44), Couleur.Violet);  
                    coquillages[5] = new ObjetTable(new Vecteur3(1450, 900, 0), Couleur.Violet);
                    coquillages[6] = new ObjetTable(new Vecteur3(1650, 1200, 0), Couleur.Vert);
                    coquillages[7] = new ObjetTable(new Vecteur3(1550, 1500, 0), Couleur.Neutre);
                    coquillages[8] = new ObjetTable(new Vecteur3(1850, 1500, 0), Couleur.Neutre);
                    coquillages[9] = new ObjetTable(new Vecteur3(1650, 1800, 0), Couleur.Violet);
                    coquillages[10] = new ObjetTable(new Vecteur3(1450, 2100, 0), Couleur.Vert);
                    coquillages[11] = new ObjetTable(new Vecteur3(1250, 2800, 0), Couleur.Neutre);
                    coquillages[12] = new ObjetTable(new Vecteur3(1550, 2800, 0), Couleur.Neutre);
                    coquillages[13] = new ObjetTable(new Vecteur3(1925, 2800, 44), Couleur.Vert);
                    coquillages[14] = new ObjetTable(new Vecteur3(1810, 2925, 44), Couleur.Vert);
                    coquillages[15] = new ObjetTable(new Vecteur3(1925, 2925, 66), Couleur.Vert); 
                    break;
                case 2:
                    coquillages[0] = new ObjetTable(new Vecteur3(1250, 200, 0), Couleur.Violet);
                    coquillages[1] = new ObjetTable(new Vecteur3(1550, 200, 0), Couleur.Neutre);
                    coquillages[2] = new ObjetTable(new Vecteur3(1810, 75, 44), Couleur.Violet);
                    coquillages[3] = new ObjetTable(new Vecteur3(1925, 75, 66), Couleur.Neutre);
                    coquillages[4] = new ObjetTable(new Vecteur3(1925, 200, 44), Couleur.Violet);
                    coquillages[5] = new ObjetTable(new Vecteur3(1450, 900, 0), Couleur.Violet);
                    coquillages[6] = new ObjetTable(new Vecteur3(1650, 1200, 0), Couleur.Violet);
                    coquillages[7] = new ObjetTable(new Vecteur3(1550, 1500, 0), Couleur.Neutre);
                    coquillages[8] = new ObjetTable(new Vecteur3(1850, 1500, 0), Couleur.Neutre);
                    coquillages[9] = new ObjetTable(new Vecteur3(1650, 1800, 0), Couleur.Vert);
                    coquillages[10] = new ObjetTable(new Vecteur3(1450, 2100, 0), Couleur.Vert);
                    coquillages[11] = new ObjetTable(new Vecteur3(1250, 2800, 0), Couleur.Vert);
                    coquillages[12] = new ObjetTable(new Vecteur3(1550, 2800, 0), Couleur.Neutre);
                    coquillages[13] = new ObjetTable(new Vecteur3(1925, 2800, 44), Couleur.Vert);
                    coquillages[14] = new ObjetTable(new Vecteur3(1810, 2925, 44), Couleur.Vert);
                    coquillages[15] = new ObjetTable(new Vecteur3(1925, 2925, 66), Couleur.Neutre);  
                    break;
                case 3:
                    coquillages[0] = new ObjetTable(new Vecteur3(1250, 200, 0), Couleur.Violet);
                    coquillages[1] = new ObjetTable(new Vecteur3(1550, 200, 0), Couleur.Neutre);
                    coquillages[2] = new ObjetTable(new Vecteur3(1810, 75, 44), Couleur.Violet);
                    coquillages[3] = new ObjetTable(new Vecteur3(1925, 75, 66), Couleur.Neutre);
                    coquillages[4] = new ObjetTable(new Vecteur3(1925, 200, 44), Couleur.Violet);
                    coquillages[5] = new ObjetTable(new Vecteur3(1250, 600, 0), Couleur.Violet);
                    coquillages[6] = new ObjetTable(new Vecteur3(1550, 600, 0), Couleur.Neutre);
                    coquillages[7] = new ObjetTable(new Vecteur3(1650, 1200, 0), Couleur.Violet);
                    coquillages[8] = new ObjetTable(new Vecteur3(1650, 1800, 0), Couleur.Vert);
                    coquillages[9] = new ObjetTable(new Vecteur3(1250, 2400, 0), Couleur.Vert);
                    coquillages[10] = new ObjetTable(new Vecteur3(1550, 2400, 0), Couleur.Neutre);
                    coquillages[11] = new ObjetTable(new Vecteur3(1250, 2800, 0), Couleur.Vert);
                    coquillages[12] = new ObjetTable(new Vecteur3(1550, 2800, 0), Couleur.Neutre);
                    coquillages[13] = new ObjetTable(new Vecteur3(1925, 2800, 44), Couleur.Vert);
                    coquillages[14] = new ObjetTable(new Vecteur3(1810, 2925, 44), Couleur.Vert);
                    coquillages[15] = new ObjetTable(new Vecteur3(1925, 2925, 66), Couleur.Neutre);  
                    break;
                case 4:
                    coquillages[0] = new ObjetTable(new Vecteur3(1250, 200, 0), Couleur.Violet);
                    coquillages[1] = new ObjetTable(new Vecteur3(1550, 200, 0), Couleur.Violet);
                    coquillages[2] = new ObjetTable(new Vecteur3(1810, 75, 44), Couleur.Vert);
                    coquillages[3] = new ObjetTable(new Vecteur3(1925, 75, 66), Couleur.Neutre);
                    coquillages[4] = new ObjetTable(new Vecteur3(1925, 200, 44), Couleur.Vert);
                    coquillages[5] = new ObjetTable(new Vecteur3(1250, 600, 0), Couleur.Violet);
                    coquillages[6] = new ObjetTable(new Vecteur3(1550, 600, 0), Couleur.Neutre);
                    coquillages[7] = new ObjetTable(new Vecteur3(1650, 1200, 0), Couleur.Neutre);
                    coquillages[8] = new ObjetTable(new Vecteur3(1650, 1800, 0), Couleur.Neutre);
                    coquillages[9] = new ObjetTable(new Vecteur3(1250, 2400, 0), Couleur.Vert);
                    coquillages[10] = new ObjetTable(new Vecteur3(1550, 2400, 0), Couleur.Neutre);
                    coquillages[11] = new ObjetTable(new Vecteur3(1250, 2800, 0), Couleur.Vert);
                    coquillages[12] = new ObjetTable(new Vecteur3(1550, 2800, 0), Couleur.Vert);
                    coquillages[13] = new ObjetTable(new Vecteur3(1925, 2800, 44), Couleur.Violet);
                    coquillages[14] = new ObjetTable(new Vecteur3(1810, 2925, 44), Couleur.Violet);
                    coquillages[15] = new ObjetTable(new Vecteur3(1925, 2925, 66), Couleur.Neutre); 
                    break;
                case 5:
                    coquillages[0] = new ObjetTable(new Vecteur3(1250, 200, 0), Couleur.Violet);
                    coquillages[1] = new ObjetTable(new Vecteur3(1550, 200, 0), Couleur.Violet);
                    coquillages[2] = new ObjetTable(new Vecteur3(1810, 75, 44), Couleur.Violet);
                    coquillages[3] = new ObjetTable(new Vecteur3(1925, 75, 66), Couleur.Neutre);
                    coquillages[4] = new ObjetTable(new Vecteur3(1925, 200, 44), Couleur.Neutre);
                    coquillages[5] = new ObjetTable(new Vecteur3(1250, 600, 0), Couleur.Violet);
                    coquillages[6] = new ObjetTable(new Vecteur3(1550, 600, 0), Couleur.Vert);
                    coquillages[7] = new ObjetTable(new Vecteur3(1850, 600, 0), Couleur.Neutre);
                    coquillages[8] = new ObjetTable(new Vecteur3(1250, 2400, 0), Couleur.Vert);
                    coquillages[9] = new ObjetTable(new Vecteur3(1550, 2400, 0), Couleur.Neutre);
                    coquillages[10] = new ObjetTable(new Vecteur3(1850, 2400, 0), Couleur.Vert);
                    coquillages[11] = new ObjetTable(new Vecteur3(1250, 2800, 0), Couleur.Violet);
                    coquillages[12] = new ObjetTable(new Vecteur3(1550, 2800, 0), Couleur.Neutre);
                    coquillages[13] = new ObjetTable(new Vecteur3(1925, 2800, 44), Couleur.Vert);
                    coquillages[14] = new ObjetTable(new Vecteur3(1810, 2925, 44), Couleur.Vert);
                    coquillages[15] = new ObjetTable(new Vecteur3(1925, 2925, 66), Couleur.Neutre); 
                    break;
            }

            Barres[0] = new ObjetTable(new Vecteur3(200, 800, 0), Couleur.Neutre);
            Barres[1] = new ObjetTable(new Vecteur3(750, 900, 0), Couleur.Neutre);
            Barres[2] = new ObjetTable(new Vecteur3(1350, 1500, 0), Couleur.Neutre);
            Barres[3] = new ObjetTable(new Vecteur3(750, 2100, 350), Couleur.Neutre);
            Barres[4] = new ObjetTable(new Vecteur3(200, 2200, 350), Couleur.Neutre);

            Dunes[0] = new ObjetTable(new Vecteur3(900, 650, 0), Couleur.Neutre);
            Dunes[1] = new ObjetTable(new Vecteur3(58, 880, 0), Couleur.Neutre);
            Dunes[2] = new ObjetTable(new Vecteur3(0, 1500, 0), Couleur.Neutre);
            Dunes[3] = new ObjetTable(new Vecteur3(58, 2120, 0), Couleur.Neutre);
            Dunes[4] = new ObjetTable(new Vecteur3(900, 2350, 0), Couleur.Neutre);

            // initialisation de la position des objets en fonction de l'équipe

            switch (equipe)
            {
                case Couleur.Violet:
                    Serviette = serviettes[0];
                    ServietteAdversaire = serviettes[1];

                    Aquarium = aquariums[0];
                    AquariumAdversaire = aquariums[1];

                    Filet = filets[0];
                    FiletAdversaire = filets[1];

                    TrierParCouleur(cabines, CabinesAdversaire, Cabines, null);
                    TrierParCouleur(coquillages, CoquillagesAdversaire, Coquillages, CoquillagesNeutres);
                    break;
                case Couleur.Vert:
                    Serviette = serviettes[1];
                    ServietteAdversaire = serviettes[0];

                    Aquarium = aquariums[1];
                    AquariumAdversaire = aquariums[0];

                    Filet = filets[1];
                    FiletAdversaire = filets[0];

                    TrierParCouleur(cabines, Cabines, CabinesAdversaire, null);
                    TrierParCouleur(coquillages, Coquillages, CoquillagesAdversaire, CoquillagesNeutres);
                    break;
            }
        }

        private static void TrierParCouleur(ObjetTable[] objets, ObjetTable[] vert, ObjetTable[] violet, ObjetTable[] neutre)
        {
            int idxVert = 0, idxViolet = 0, idxNeutre = 0;

            for(int i = 0; i < objets.Length; i++)
                switch (objets[i].Couleur)
                {
                    case Couleur.Vert:
                        vert[idxVert++] = objets[i];
                        break;
                    case Couleur.Violet:
                        violet[idxViolet++] = objets[i];
                        break;
                    case Couleur.Neutre:
                        neutre[idxNeutre++] = objets[i];
                        break;
                }
        }
    }
}
