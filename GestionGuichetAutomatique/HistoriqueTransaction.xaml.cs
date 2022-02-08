using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour HistoriqueTransaction.xaml
    /// </summary>
    public partial class HistoriqueTransaction : Window
    {
        #region Mes tables && variables 
        List<Client> users;
        List<Compte> comptes;
        List<typeCompte> typeComptes;
        List<Transaction> transactions;
        List<TypeTransaction> typeTransactions;
        #endregion

        private const string TEXT_SELECT_CMBO = "Vous devez selectionner un numéro de compte !";
        private const string TEXT_TRANS_VIDE = "Nouveau compte pas transactions crées !";

        public HistoriqueTransaction()
        {
            InitializeComponent();
            users = Singleton.Instance.myBDD.Clients.ToList();
            comptes = Singleton.Instance.myBDD.Comptes.ToList();
            typeComptes = Singleton.Instance.myBDD.typeComptes.ToList();
            transactions = Singleton.Instance.myBDD.Transactions.ToList();
            typeTransactions = Singleton.Instance.myBDD.TypeTransactions.ToList();
            CmbClient.DataContext = users;
            CmbTypeCompte.DataContext = typeComptes;
        }

        private void btnAfficher_Click(object sender, RoutedEventArgs e)
        {
                             var queryTrans = from t in transactions
                                               join c in comptes on t.CompteSource equals c.NumeroCompte
                                               join l in typeTransactions on t.CodeTypeTransaction equals l.CodeTypeTransaction
                                               join p in typeComptes on c.IDTypeCompte equals p.IDTypeCompte
                                               where c.IDTypeCompte == ((typeCompte)CmbTypeCompte.SelectedItem).IDTypeCompte
                                               && c.CodeClient.ToUpper() == ((Client)CmbClient.SelectedItem).CodeClient.ToUpper()
                                               orderby t.DateTransaction descending
                              select new
                             {
                                 t.NumroTransaction,
                                 t.MontantTransaction,
                                 t.CompteSource,
                                 t.CompteDestination,
                                 l.TypeTransaction1,
                                 t.DateTransaction,
                             };

            if(queryTrans.ToList().Count == 0)
            {
                dgTransactions.DataContext = null;
                afficheMsgWarning(TEXT_TRANS_VIDE); return;
                
            }
            else
            dgTransactions.DataContext = queryTrans.ToList();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new EspaceAdmin().Show();
            this.Close();
        }
        private void afficheMsgWarning(string msg)
        {
            MessageBox.Show(msg, "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void CmbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
              Client clt = CmbClient.SelectedItem as Client;
              var query  = (from c in typeComptes.ToList()
                            join p in clt.Comptes on c.IDTypeCompte equals p.IDTypeCompte
                             where p.CodeClient.ToUpper() == ((Client)CmbClient.SelectedItem).CodeClient.ToUpper()
                             select c).ToList();

             CmbTypeCompte.DataContext = query.Distinct().ToList();

        } 
    }
}
