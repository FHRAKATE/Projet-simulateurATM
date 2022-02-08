using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GestionGuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour EspaceAdmin.xaml
    /// </summary>
    /// 
    
    public partial class EspaceAdmin : Window
    {

        private const string TEXT_PAIE_INTERET = "Paiement de 1% d'Intérêt aux comptes Epargnes effectué avec succès !";
        private const string TEXT_AUGMT_MARGE = "Augumentation de 5% des comptes Marges de crédit effectués avec succès !";

        Gestion gest;

        public EspaceAdmin()
        {
            InitializeComponent();
            gest = new Gestion();
        }

        private void TransLst_Click(object sender, RoutedEventArgs e)
        {
            HistoriqueTransaction listTrans = new HistoriqueTransaction();
            listTrans.Show();
            this.Close();
        }

        private void addClt_Click(object sender, RoutedEventArgs e)
        {
            AjouterClient ajoutClt = new AjouterClient();
            ajoutClt.Show();
            this.Close();
        }

        private void addCompte_Click(object sender, RoutedEventArgs e)
        {
            AjouterCompte ajoutCpt = new AjouterCompte();
            ajoutCpt.Show();
            this.Close();
        }

        private void prelevMontant_Click(object sender, RoutedEventArgs e)
        {
            PreleverMontantHypothec ajoutCpt = new PreleverMontantHypothec();
            ajoutCpt.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {    
            string message = "Désirez-vous réellement Fermer le Guichet ? ";
            string titre = "Attention";

            MessageBoxResult result = MessageBox.Show(message, titre, MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void btnPayerInteret_Click(object sender, RoutedEventArgs e)
        {
            foreach (Compte cpt in Singleton.Instance.myBDD.Comptes.ToList())
            {
                gest.PayerInteret(cpt);
            }
            try
            {
                Singleton.Instance.myBDD.SaveChanges();
                MessageBox.Show(TEXT_PAIE_INTERET, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnAugmentMarge_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(gest.AugmentMargeCrd()))
                MessageBox.Show(TEXT_AUGMT_MARGE, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show(gest.AugmentMargeCrd());
        }

        private void btnAjoutArgent_Click(object sender, RoutedEventArgs e)
        {
            AjouterArgentGuichet ajoutArgent = new AjouterArgentGuichet();
            ajoutArgent.Show();
            this.Close();
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            MainWindow con = new MainWindow();
            con.Show();
            this.Close();
        }
    }
}
