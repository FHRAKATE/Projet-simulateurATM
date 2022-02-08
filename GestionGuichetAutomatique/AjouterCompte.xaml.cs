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
    /// Logique d'interaction pour AjouterCompte.xaml
    /// </summary>
    public partial class AjouterCompte : Window
    {

        #region Mes Constantes 
        private const string TEXT_MSG_CONFIRM_ADD_CPT = "Compte créé avec succès!";
        private const string TEXT_SOLDE_VIDE = "Vous devez saisir le montant !";
        private const string TEXT_MSG_MARGE_EXISTANT = "Le Client à droit à un seul compte Marge de Crédit!";
        #endregion

        Gestion gest;
        public AjouterCompte()
        {
            InitializeComponent();
            gest = new Gestion();
            cmbCodeClt.DataContext = Singleton.Instance.myBDD.Clients.ToList();
            CmbTypeCpt.DataContext = Singleton.Instance.myBDD.typeComptes.ToList();
            dateOvCompte.SelectedDate = DateTime.Today;
        }


        private void btnAjoutCpt(object sender, RoutedEventArgs e)
        {

        }

        /*    private void btnAjoutCpt_Click(object sender, RoutedEventArgs e)
            {
                string result = gest.AjouterCompte(dateOvCompte.SelectedDate, float.Parse(txtSoldeCpt.Text), ((Client)cmbCodeClt.SelectedItem).CodeClient, ((Compte)CmbTypeCpt.SelectedItem).IDTypeCompte);

                if (string.IsNullOrEmpty(result))
                    MessageBox.Show(TEXT_MSG_CONFIRM_ADD_CPT, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

                else
                    MessageBox.Show(result);
            }*/

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            resetInfoCompte();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            new EspaceAdmin().Show();
            this.Close();

        }

        private void btnAddCpt_Click(object sender, RoutedEventArgs e)
        {
            

            if (string.IsNullOrEmpty(txtSoldeCpt.Text) ) { txtSoldeCpt.Text = "0"; }

            if(CmbTypeCpt.Text == "MARGE_CREDIT") if (gest.IsExistMarge(cmbCodeClt.SelectedItem as Client) != null) { afficheMsgErreur(TEXT_MSG_MARGE_EXISTANT); return; }
            
            float soldeCpt = gest.ConvertPoint(txtSoldeCpt.Text);

            string result = gest.AjouterCompte(dateOvCompte.SelectedDate, soldeCpt, ((Client)cmbCodeClt.SelectedItem).CodeClient.ToUpper(), ((typeCompte)CmbTypeCpt.SelectedItem).IDTypeCompte);

            if (string.IsNullOrEmpty(result))
                MessageBox.Show(TEXT_MSG_CONFIRM_ADD_CPT, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

            else
                MessageBox.Show(result);
        }

        private void txtSoldeCpt_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void resetInfoCompte()
        {
            txtSoldeCpt.Clear();
           
        }

        private void afficheMsgErreur(string msg)
        {
            MessageBox.Show(msg, "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
