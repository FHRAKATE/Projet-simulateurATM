using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionGuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Client> users;
        //Calcule le nombre essaie(Max 3 essaie)
        int compteur;

        #region Mes Constantes 
        private const string TEXT_MESSAGE_COMPTE_BLOQUE = "Votre compte est bloqué! Merci de Contacter votre banque!";
        private const string TEXT_MESSAGE_SAISIR_USER_PWD = "Vous devez saisir votre user et mot de passe";
        private const string TEXT_MESSAGE_PWD_INCORRECT = "Mot de passe incorrect";
        private const string TEXT_MESSAGE_USER_PWD_INCORRECT = "Non d'utilisateur et mot de passe incorrect!";
        private const string TEXT_ATTENTION = "Attention";
        private const string USER_NAME_ADMIN = "ADMIN";
        private const int NIP_ADMIN = 0000;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            users = Singleton.Instance.myBDD.Clients.ToList();
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            string user = txtNom.Text;
            string modPass = txtMotPass.Password;

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(modPass))
            {
                // connexion Admin
                if (isValidAdmin(user, modPass)) return;


                // connexion Client
                Client clientValid = users.SingleOrDefault(u => (u.CodeClient).ToUpper() == user.ToUpper()
                && u.NumeroNIP == int.Parse(modPass) && u.StatusClient == false);

                if (isValidClient(clientValid))
                {
                    //User & Mot de passe correctes
                    compteur = 0;
                    Application.Current.Properties["codeClient"] = clientValid.CodeClient;
                    Application.Current.Properties["numeroNIP"] = clientValid.NumeroNIP;
                    txtNom.Text = "";
                    txtMotPass.Password = "";
                    //SessionClient maSession = new SessionClient();
                    Guichet maSession = new Guichet();
                    maSession.Show();
                    this.Close(); 
                }
            }
            else
                //User & Mot de passe vide
                AfficherMessage(TEXT_MESSAGE_SAISIR_USER_PWD, TEXT_ATTENTION, MessageBoxImage.Information);
        }

        private bool isValidAdmin(string user, string modPass)
        {
            bool isvalidUser = user.ToUpper().Equals(USER_NAME_ADMIN);
            bool isvalidPwd = int.Parse(modPass) == NIP_ADMIN;
            if (isvalidUser && isvalidPwd)
            { 
                Application.Current.Properties["codeAdmin"] = user;
                Application.Current.Properties["numeroNIP"] = modPass;
                txtNom.Text = "";
                txtMotPass.Password = "";
                // SessionAdmin maSession = new SessionAdmin();
                EspaceAdmin frClt = new EspaceAdmin();
                frClt.Show();
                this.Close();
                return true;
            }

            return false;
        }
       
        private bool isValidClient(Client client)
        {
            if (client != null) return true;

            Client clientExist = users.SingleOrDefault(u => (u.CodeClient).ToUpper() == txtNom.Text.ToUpper());

            if (clientExist == null)

                AfficherMessage(TEXT_MESSAGE_USER_PWD_INCORRECT, TEXT_ATTENTION, MessageBoxImage.Warning);
            else
            {
                if (clientExist.StatusClient == false)
                {
                    //Mot de passe incorrect
                    AfficherMessage(TEXT_MESSAGE_PWD_INCORRECT, TEXT_ATTENTION, MessageBoxImage.Warning);
                    txtMotPass.Focus();
                    compteur++;
                    CompteBloquerManyEssaie(clientExist);
                }
                else
                {
                    AfficherMessage(TEXT_MESSAGE_COMPTE_BLOQUE, TEXT_ATTENTION, MessageBoxImage.Error);
                    txtNom.Text = string.Empty;
                }
            }
            return false;
        }

        private void CompteBloquerManyEssaie(Client clt)
        {
            if (compteur > 3 && clt.StatusClient == false)
            {
                clt.StatusClient = true;
                try
                {
                    Singleton.Instance.myBDD.SaveChanges();
                    AfficherMessage(TEXT_MESSAGE_COMPTE_BLOQUE, TEXT_ATTENTION, MessageBoxImage.Error);
                    txtNom.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AfficherMessage(string chaine, string typeMessage, MessageBoxImage ImageMessage)
        {
            MessageBox.Show(chaine, typeMessage, MessageBoxButton.OK, ImageMessage);
            txtMotPass.Password = string.Empty;
            //txtNom.Text = string.Empty;
            txtNom.SelectAll();
            txtNom.Focus();

        }
       
        private void txtMotPass_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if ((e.Key != Key.Back) && (e.Key != Key.Tab))
                    {
                        e.Handled = true;
                        MessageBox.Show("J'accepte uniquement les chiffres, desole.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

  

    }
}