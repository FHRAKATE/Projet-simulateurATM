using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Logique d'interaction pour PreleverMontantHypothec.xaml
    /// </summary>
    public partial class PreleverMontantHypothec : Window
    {
        Gestion gest;
        List<Client> userHypothec;
        List<typeCompte> typeComptes;
        List<Client> users;
        ObservableCollection<Compte> ListCompte =
       new ObservableCollection<Compte>(Singleton.Instance.myBDD.Comptes.ToList());

        private const string TEXT_MSG_MARGE_INEXISTANT = "Transaction refusée! Solde insuffisant et Vous n'avez pas le Compte Marge de Crédit! ";
        private const string TEXT_MNT_VIDE = "Vous devez saisir le montant !";
        public PreleverMontantHypothec()
        {
            InitializeComponent();
            gest = new Gestion();
            userHypothec = new List<Client>();
            users = Singleton.Instance.myBDD.Clients.ToList();
            typeComptes = Singleton.Instance.myBDD.typeComptes.ToList();


            foreach (Client clt in users)
            {
                foreach(Compte cpt in clt.Comptes)
                {
                    if (cpt.IDTypeCompte == 3) userHypothec.Add(clt);
                }
            }

            cmbClient.DataContext = userHypothec.Distinct().ToList();

        }

        private void CmbT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Client client = cmbClient.SelectedItem as Client;
            cmbNumCpt.DataContext = (from c in client.Comptes.ToList()
                                      where c.CodeClient.ToUpper() == ((Client)cmbClient.SelectedItem).CodeClient.ToUpper()
                                      && c.IDTypeCompte == 3 select c).ToList();
        }

        private void cmbNumCpt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Compte cpt = cmbNumCpt.SelectedItem as Compte;
            txtSoldeCpt.Text = cpt.SoldeCompte.ToString();

           
                Client client = cmbClient.SelectedItem as Client;
                var query = from l in client.Comptes.ToList()
                            join c in typeComptes on l.IDTypeCompte equals c.IDTypeCompte
                            where l.CodeClient.ToUpper() == ((Client)cmbClient.SelectedItem).CodeClient.ToUpper()
                            && l.IDTypeCompte == 3 || l.IDTypeCompte == 4
                            select new { l.CodeClient, l.NumeroCompte, l.SoldeCompte, l.DateOuverture, c.TypeCompte1 };
                dgListHypothec.DataContext = query.ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new EspaceAdmin().Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnPreleve_Click(object sender, RoutedEventArgs e)
        {
            string message;

            if (string.IsNullOrEmpty(txtMontant.Text)) { afficheMsgErreur(TEXT_MNT_VIDE); return; }

            Compte cpt = cmbNumCpt.SelectedItem as Compte;
            float montant = gest.ConvertPoint(txtMontant.Text);

            if (gest.IsExistMarge(cmbClient.SelectedItem as Client) == null) { afficheMsgErreur(TEXT_MSG_MARGE_INEXISTANT); return; }

            message = gest.Retrait(cpt, montant,"Prelever", cmbClient.SelectedItem as Client);
            if (message == null) return;

            try
            {
                Singleton.Instance.myBDD.SaveChanges();
                txtSoldeCpt.Text = cpt.SoldeCompte.ToString();
                afficherDataGrid();
                MessageBox.Show(message, "Confirmation",
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void afficheMsgErreur(string msg)
        {
            MessageBox.Show(msg, "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void afficherDataGrid()
        {
            Client client = cmbClient.SelectedItem as Client;
            var query = from l in client.Comptes.ToList()
                        join c in typeComptes on l.IDTypeCompte equals c.IDTypeCompte
                        where l.CodeClient.ToUpper() == ((Client)cmbClient.SelectedItem).CodeClient.ToUpper()
                        && l.IDTypeCompte == 3 || l.IDTypeCompte == 4
                        select new { l.CodeClient, l.NumeroCompte, l.SoldeCompte, l.DateOuverture, c.TypeCompte1 };


            dgListHypothec.DataContext = query.ToList();
        }

        private void dgListHypothec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gdCompt = (DataGrid)sender;
            DataRowView selectedRow = gdCompt.SelectedItem as DataRowView;
            if (selectedRow!= null)
            {
                txtSoldeCpt.Text = selectedRow["SoldeCompte"].ToString();
                cmbNumCpt.Text = selectedRow["NumeroCompte"].ToString();
            }
        
        }

        private void dgListHypothec_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid gdCompt = (DataGrid)sender;
            DataRowView selectedRow = gdCompt.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                txtSoldeCpt.Text = selectedRow["SoldeCompte"].ToString();
                cmbNumCpt.Text = selectedRow["NumeroCompte"].ToString();
            }
        }

        private void dgListHypothec_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid gdCompt = (DataGrid)sender;
            DataRowView selectedRow = gdCompt.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                txtSoldeCpt.Text = selectedRow["SoldeCompte"].ToString();
                cmbNumCpt.Text = selectedRow["NumeroCompte"].ToString();
            }
        }

        private void txtMontant_PreviewKeyDown(object sender, KeyEventArgs e)
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
