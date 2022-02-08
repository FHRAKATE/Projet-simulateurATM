using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionGuichetAutomatique
{

    public class Singleton
    {
        private static Singleton instance = null;

        public guichetAutomatiqueEntities myBDD = new guichetAutomatiqueEntities();

        public Client client;

        public const float fraisFacture = 1.25f;
        public const float plafondGuichet = 20000;
        public float soldeGuichet = 10000;
        public const float retraitMax = 1000;
        public const double interetEpargne = 0.01;
        public const double interetMarge = 0.05;

        private Singleton(){ }
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }

    }
}
