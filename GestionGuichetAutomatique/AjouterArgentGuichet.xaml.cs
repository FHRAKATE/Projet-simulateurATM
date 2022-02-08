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
    /// Logique d'interaction pour AjouterArgentGuichet.xaml
    /// </summary>
    public partial class AjouterArgentGuichet : Window
    {
        private const string TEXT_SOLDE_GUICHET_ATTEINT = "Plafond d'argent autorisé est déjà atteint: 20000$ !";
        private const string TEXT_MNT_VIDE = "Vous devez saisir le montant !";
        private const string TEXT_SOLD_MAX_SOLDE = "Solde du Guichet ne peut pas dépassé le plafond : 20000$ !";
        Gestion gest;

        public AjouterArgentGuichet()
        {
            InitializeComponent();
            gest = new Gestion();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSolde.Text = Singleton.Instance.soldeGuichet.ToString();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            EspaceAdmin admin = new EspaceAdmin();
            admin.Show();
            this.Close();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            float montant = Singleton.Instance.soldeGuichet;

            if (string.IsNullOrEmpty(txtMonant.Text)) { afficheMsgErreur(TEXT_MNT_VIDE); return; }

            if (Singleton.Instance.soldeGuichet >= Singleton.plafondGuichet) { afficheMsgErreur(TEXT_SOLDE_GUICHET_ATTEINT); return; }

            montant += gest.ConvertPoint(txtMonant.Text);

            if (montant > Singleton.plafondGuichet) { afficheMsgErreur(TEXT_SOLD_MAX_SOLDE); return; }

            Singleton.Instance.soldeGuichet = montant;
            txtSolde.Text = Singleton.Instance.soldeGuichet.ToString();

            MessageBox.Show("Ajout de d'argent paier " + txtMonant.Text + "$ est effectué avec succès! ", "Confirmation",
            MessageBoxButton.OK, MessageBoxImage.Information);


        }



        private void afficheMsgErreur(string msg)
        {
            MessageBox.Show(msg, "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void txtMonant_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                //Détermine si la séquence de touches est un nombre du clavier.
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if ((e.Key != Key.Back) && (e.Key != Key.Tab) && (e.Key != Key.OemComma))
                    {
                        e.Handled = true;
                        MessageBox.Show("J'accepte uniquement les chiffres, désolé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
        }
    }
}
