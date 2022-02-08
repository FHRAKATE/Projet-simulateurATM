using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Logique d'interaction pour Guichet.xaml
    /// </summary>
    public partial class Guichet : Window
    {
        public enum CategorieCompte { CHEQUE, EPARGNE, HYPOTHECAIRE, MARGE_CREDIT }

        #region Mes tables && variables 
        List<Client> users;
        List<Compte> comptes;
        List<typeCompte> typeComptes;
        List<typeCompte> types = new List<typeCompte>();
        Gestion gest = new Gestion();
        #endregion

        #region Mes Constantes 
        private const string TEXT_MSGE_ERR_RETRAIT_HYPOT = "Retrait sur compte Hypothecaire non autorisé !";
        private const string TEXT_MSGE_ERR_RETRAIT_MARGE = "Retrait sur compte Marge Crédit non autorisé !";
        private const string TEXT_MSGE_ERR_DEPOT = "Dépot vers le compte Marge Crédit non autorisé !";
        private const string TEXT_MSGE_ERR_TRANS_CHEQ = "Transfert invalide, Vous devez choisir un compte Source Chèque !";
        private const string TEXT_MSGE_ERR_PAIE_CHEQ = "Paiement invalide, Vous devez choisir un compte Chèque!";
        private const string TEXT_MSG_ATTENTION = "Attention";
        private const string TEXT_MSG_CONFIRMATION = "Confirmation";
        private const string TEXT_MNT_VIDE = "Vous devez saisir le montant !";
        private const string TEXT_MSGE_TRNS_CHEQ_INVALID = "Transfert invalide, Compte destination devrait etre différent du compte Source !";
        private const string TEXT_MSG_MONTANT_DEPASSE_1000   = "Montant saisi est invalide! Il depasse 1000 $";
        private const string TEXT_MONTANT_PAS_MULTIPLE_10 = "Montant saisi invalide ! il n'est pas multiple de 10";
        private const string TEXT_SOLDE_GUICHET_INSUFFISANT = "Nous somme désolés! le Guichet est vide!";
        private const string TEXT_MSGE_ERR_TRANS_CHEQ2 = "Transaction refusée! Compte chèque est vide!";
        #endregion
        public Guichet()
        {
            InitializeComponent();
            users = Singleton.Instance.myBDD.Clients.ToList();
            comptes = Singleton.Instance.myBDD.Comptes.ToList();
            typeComptes = Singleton.Instance.myBDD.typeComptes.ToList();
            rendreInvisibleCptDest();

            if (Application.Current.Properties["codeClient"] != null && Application.Current.Properties["numeroNIP"] != null)
            {
                string codeClient = Application.Current.Properties["codeClient"].ToString();
                int numeroNip = int.Parse(Application.Current.Properties["numeroNIP"].ToString());

                 Singleton.Instance.client = users.SingleOrDefault(u => (u.CodeClient.ToUpper()) == codeClient.ToUpper()
                 && u.NumeroNIP == numeroNip);

                foreach(Compte cpt in comptes)
                {
                    typeCompte type = typeComptes.SingleOrDefault( u =>  u.IDTypeCompte == cpt.IDTypeCompte
                                         && cpt.CodeClient.ToUpper() == Singleton.Instance.client.CodeClient.ToUpper());
                    if (type != null) types.Add(type);
                }

                CmbTypeCpt.DataContext = types.Distinct().ToList();
                CmbTypeCptDest.DataContext = types.Distinct().ToList();
                lblClient.Text = "Bienvenue " + Singleton.Instance.client.PrenomClient + " " + Singleton.Instance.client.NomClient;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties.Remove("codeClient");
            Application.Current.Properties.Remove("numeroNIP");
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
        private void txtMontant_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if ((e.Key != Key.Back) && (e.Key != Key.Tab) && (e.Key != Key.OemComma) && (e.Key != Key.OemPeriod))
                    {
                        e.Handled = true;
                        MessageBox.Show("J'accepte uniquement les chiffres, desole.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void btnSoumettre_Click(object sender, RoutedEventArgs e)
        {
            float montant = 0;
            string message = "";

            if (string.IsNullOrEmpty(txtMontant.Text)) { afficheMsgErreur(TEXT_MNT_VIDE); return; }

            montant = gest.ConvertPoint(txtMontant.Text);

            bool isDepotVersMargeCrd = CmbTypeCpt.Text == CategorieCompte.MARGE_CREDIT.ToString();
            bool isRetraitDansHypothec = CmbTypeCpt.Text == CategorieCompte.HYPOTHECAIRE.ToString();
            bool isRetraitVersMargeCrd = CmbTypeCpt.Text == CategorieCompte.MARGE_CREDIT.ToString();
            bool isTransfertInvalide = CmbTypeCpt.Text != CategorieCompte.CHEQUE.ToString();
            bool isPaiementFacture = rbPaieFact.IsChecked == true;
            bool isMontantValide = (montant <= Singleton.retraitMax);
            bool isMultiple10 = ((montant % 2) == 0);
            bool isGuichetValid = (montant < Singleton.Instance.soldeGuichet);

            Compte cpt = comptes.SingleOrDefault(u => (u.CodeClient).ToUpper() == (Singleton.Instance.client.CodeClient).ToUpper()
                         && u.NumeroCompte == int.Parse(CmbNumCpte.Text));

            if (cpt != null)
            {
                //Depot dans les Comptes cheque, epargne, hypothecaire
                if (rbDepot.IsChecked == true)
                {
                    if (isDepotVersMargeCrd) { MessageBox.Show(TEXT_MSGE_ERR_DEPOT, TEXT_MSG_ATTENTION, MessageBoxButton.OK, MessageBoxImage.Error); return; }

                    if (!string.IsNullOrEmpty(gest.Depot(cpt, montant))) { MessageBox.Show(gest.Depot(cpt, montant)); return; }
                    MessageBox.Show("Dépot de " + montant + " $ avec succès sur votre compte " + cpt.typeCompte.TypeCompte1 + " : " + CmbNumCpte.Text, TEXT_MSG_CONFIRMATION,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                    
                }

                //Retrait dans les comptes cheque, Epargne
                if (rbRetrait.IsChecked == true)
                {
                    if (isRetraitDansHypothec) {afficheMsgErreur(TEXT_MSGE_ERR_RETRAIT_HYPOT); return; }
                    if (isRetraitVersMargeCrd) {afficheMsgErreur(TEXT_MSGE_ERR_RETRAIT_MARGE); return; }

                    if (!isMontantValide) {afficheMsgErreur(TEXT_MSG_MONTANT_DEPASSE_1000); return; }
                    if (!isMultiple10) {afficheMsgErreur(TEXT_MONTANT_PAS_MULTIPLE_10); return; }
                    if (!isGuichetValid) {afficheMsgErreur(TEXT_SOLDE_GUICHET_INSUFFISANT); return; }

                    message = gest.Retrait(cpt, montant, "retrait", Singleton.Instance.client);
                    if (message == null) return;

                    try
                    {
                        Singleton.Instance.myBDD.SaveChanges();
                        MessageBox.Show(message, "Confirmation",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                
                //Paiement de facture
                if (isPaiementFacture)
                {
                    if (CmbTypeCpt.Text != CategorieCompte.CHEQUE.ToString())
                    { MessageBox.Show(TEXT_MSGE_ERR_PAIE_CHEQ, TEXT_MSG_ATTENTION, MessageBoxButton.OK, MessageBoxImage.Error); return; }

                    message = gest.PaiementFacture(cpt, montant);
                    if (message == null) return;

                    try
                    {
                        Singleton.Instance.myBDD.SaveChanges();
                        MessageBox.Show(message, "Confirmation",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

                //Transfert
                if (rbVirment.IsChecked == true)
                {
                    if (isTransfertInvalide) { afficheMsgErreur(TEXT_MSGE_ERR_TRANS_CHEQ); return; }

                    Compte cptDest = comptes.SingleOrDefault(u => (u.CodeClient).ToUpper() ==
                   (Singleton.Instance.client.CodeClient).ToUpper() && u.NumeroCompte == int.Parse(CmbNumCptDest.Text));

                    if (cptDest != null)
                    {
                        if (cpt.NumeroCompte == cptDest.NumeroCompte) { afficheMsgErreur(TEXT_MSGE_TRNS_CHEQ_INVALID); return; }
                        if (cpt.SoldeCompte <= 0 && cptDest.typeCompte.IDTypeCompte == 4) { afficheMsgErreur(TEXT_MSGE_ERR_TRANS_CHEQ2); return; }

                        message = gest.Transfert(cpt, cptDest, montant);
                        if (message == null) return;

                        try
                        {
                            Singleton.Instance.myBDD.SaveChanges();
                            MessageBox.Show(message, "Confirmation",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                }
            }
        }
        private void afficheMsgErreur(string msg)
        {
            MessageBox.Show(msg, TEXT_MSG_ATTENTION, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void CmbTypeCpt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            typeCompte typeCpt = CmbTypeCpt.SelectedItem as typeCompte;
            CmbNumCpte.DataContext = (from c in typeCpt.Comptes.ToList()
                                      where c.CodeClient.ToUpper() == Singleton.Instance.client.CodeClient.ToUpper()
                                      select c).ToList();

        }
        private void rbVirment_Click(object sender, RoutedEventArgs e)
        {
          
            if (rbVirment.IsChecked == true)
            {
                  rendreVisibleCptDest();
            }
            else
            {
                  rendreInvisibleCptDest();
            }
        }
        private void CmbCompteDest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            typeCompte typeCpt = CmbTypeCptDest.SelectedItem as typeCompte;
            CmbNumCptDest.DataContext = (from c in typeCpt.Comptes.ToList()
                                      where c.CodeClient.ToUpper() == Singleton.Instance.client.CodeClient.ToUpper()
                                      select c).ToList();
        }
        private void rendreVisibleCptDest()
        {
            grpDest.Visibility = System.Windows.Visibility.Visible;
            lblTypeDest.Visibility = System.Windows.Visibility.Visible;
            CmbTypeCptDest.Visibility = System.Windows.Visibility.Visible;
            lblCptDest1.Visibility = System.Windows.Visibility.Visible;
            CmbNumCptDest.Visibility = System.Windows.Visibility.Visible;
        }
        private void rendreInvisibleCptDest()
        {
            grpDest.Visibility = System.Windows.Visibility.Collapsed;
            lblTypeDest.Visibility = System.Windows.Visibility.Collapsed;
            CmbTypeCptDest.Visibility = System.Windows.Visibility.Collapsed;
            lblCptDest1.Visibility = System.Windows.Visibility.Collapsed;
            CmbNumCptDest.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void rbDepot_Click(object sender, RoutedEventArgs e)
        {
            rendreInvisibleCptDest();
        }
        private void rbRetrait_Click(object sender, RoutedEventArgs e)
        {
            rendreInvisibleCptDest();
        }
        private void rbPaieFact_Click(object sender, RoutedEventArgs e)
        {
            rendreInvisibleCptDest();
        }
        private void dgComptes_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void btnEtatCpt_Click(object sender, RoutedEventArgs e)
        {
            var query = from l in comptes
                        join c in typeComptes on l.IDTypeCompte equals c.IDTypeCompte
                        where l.CodeClient.ToUpper() == Singleton.Instance.client.CodeClient.ToUpper()
                        select new { l.CodeClient, l.NumeroCompte, c.TypeCompte1, l.SoldeCompte, l.DateOuverture };


            dgComptes.DataContext = query.ToList();

 
        }
    }
}
