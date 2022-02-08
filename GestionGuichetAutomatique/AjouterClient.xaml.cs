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
    /// Logique d'interaction pour AjouterClient.xaml
    /// </summary>
    public partial class AjouterClient : Window
    {
        #region Mes Constantes 
        private const string TEXT_NSG_CONFIRM_ADD_CLT = "Client créé avec succès!";
        private const string TEXT_MSG_INFO_CLT_INVALID = "Veuillez saisir tous les informations du client!";
        private const string TEXT_MSG_email_INVALID = "L'email saisie n'est pas valide. Faites attention au format (example@example.ex)";
        private const string TEXT_MSG_TEL_INVALID = "Le Téléphone saisie n'est pas valide. Faites attention au format ((XXX)XXX-XXXX)";
        private const string TEXT_MSG_NIP_INVALID = "NIP Invalide! Veuillez saisir quatre chiffre!";

       
        #endregion

        #region Mes Variables 
        Gestion gest;
        #endregion

        public AjouterClient()
        {
            InitializeComponent();
            gest = new Gestion();
        
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {

                new EspaceAdmin().Show();
                this.Close();

           
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)


        {
            if(string.IsNullOrEmpty(txtCodeClt.Text) || string.IsNullOrEmpty(txtNIP.Text) || 
               string.IsNullOrEmpty(txtNomClt.Text) ||  string.IsNullOrEmpty(txtPrenomClt.Text) ||
               string.IsNullOrEmpty(txtTelephone.Text) || string.IsNullOrEmpty(txtCourriel.Text)) 
              { afficheMsgErreur(TEXT_MSG_INFO_CLT_INVALID); return; }


            string telephone = gest.InputTelephone((txtTelephone.Text).ToString()); if(telephone == null)
            { afficheMsgErreur(TEXT_MSG_TEL_INVALID); txtTelephone.Focus(); txtTelephone.Text = String.Empty; return; }

            string email = gest.InputEmail((txtCourriel.Text).ToString()); if (email == null)
            { afficheMsgErreur(TEXT_MSG_email_INVALID); txtCourriel.Focus(); txtCourriel.Text = String.Empty; return; }
            
            int taille = (txtNIP.Text).Length;
            if(taille < 4 || taille > 4) { afficheMsgErreur(TEXT_MSG_NIP_INVALID); return; }
         

            string result =  gest.AjouterClient(txtCodeClt.Text.ToUpper(), decimal.Parse(txtNIP.Text), txtNomClt.Text.ToUpper(), txtPrenomClt.Text.ToUpper(), telephone, email, Boolean.Parse(CmbStatus.Text));

            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show(TEXT_NSG_CONFIRM_ADD_CLT, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                resetInfoClient();
            }
            else
                MessageBox.Show(result);

        }


        private void afficheMsgErreur(string msg)
        {
            MessageBox.Show(msg, "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void txtNIP_PreviewKeyDown(object sender, KeyEventArgs e)
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


        private void resetInfoClient()
        {
            txtNomClt.Clear();
            txtPrenomClt.Clear();
            txtCourriel.Clear();
            txtCodeClt.Clear();
            txtNIP.Clear();
            txtTelephone.Clear();
        }

        private void btnInitialiser_Click(object sender, RoutedEventArgs e)
        {
            resetInfoClient();
        }
    }
}
