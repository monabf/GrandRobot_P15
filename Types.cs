using System;

namespace GR
{
    enum Couleur { Null = 0, Bleu, Jaune, Vert, Violet, Neutre }

    struct Vecteur2
    {
        public readonly int X;
        public readonly int Y;

        public Vecteur2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double Distance(Vecteur2 v)
        {
            return Math.Sqrt(Math.Pow(X - v.X, 2) + Math.Pow(Y - v.Y, 2));
        }
    }

    struct Vecteur3
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Vecteur3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Distance(Vecteur3 v)
        {
            return Math.Sqrt(Math.Pow(X - v.X, 2) + Math.Pow(Y - v.Y, 2) + Math.Pow(Z - v.Z, 2));
        }
    }

    class ObjetTable
    {
        public readonly Vecteur3 Position;
        public readonly Couleur Couleur;

        public ObjetTable(Vecteur3 position, Couleur couleur)
        {
            Position = position;
            Couleur = couleur;
        }
    }
}
