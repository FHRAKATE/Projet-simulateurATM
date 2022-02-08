using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace GestionGuichetAutomatique
{
    class Gestion
    {
        #region Mes Constantes 
         private const string TEXT_MSG_ATTENTION = "Attention";
         private const string TEXT_MSG_MARGE_INEXISTANT = "Solde insuffisant, Pas de Compte Marge de Crédit! Transaction refusée!";
        #endregion

        public void AjouterTransaction(DateTime? dateTransaction, double? montantTransaction, int? compteSource, int? compteDestination, int? codeTypeTransaction)
        {
            Transaction newTrans = new Transaction(dateTransaction, montantTransaction, compteSource, compteDestination, codeTypeTransaction);
            Singleton.Instance.myBDD.Transactions.Add(newTrans);
        }
        public string Retrait(Compte cpt, float montant, string oper, Client clt)
        {
            if (cpt.SoldeCompte >= montant)
            {
                cpt.SoldeCompte -= montant;
                Singleton.Instance.soldeGuichet -= montant;
                AjouterTransaction(DateTime.Today, Math.Round(montant,2), cpt.NumeroCompte, null, 2);

                if(oper == "retrait")
                 return "Retrait de " + montant+ " effectué avec succès";
                else
                    return "Prélèvement du montant " + montant + " effectué avec succès!";
            }
            else
            {
                if (IsExistMarge(clt) == null) { afficheMsgErreur(TEXT_MSG_MARGE_INEXISTANT); return null; }
                Compte cptMarge = IsExistMarge(clt);
                float soldeMarge = montant - (float)cpt.SoldeCompte;
                float soldeCompte = (float)cpt.SoldeCompte;

                cpt.SoldeCompte = 0;
                cptMarge.SoldeCompte += soldeMarge;

                if (soldeCompte > 0) AjouterTransaction(DateTime.Today, Math.Round(soldeCompte,2), cpt.NumeroCompte, null, 2);
                AjouterTransaction(DateTime.Today, soldeMarge, cptMarge.NumeroCompte, null, 2);

                return "Solde insuffisant! Montant de " + Math.Round(soldeMarge,2) + " $ puiser dans la Marge de Crédit ";

            }
 
        }
        public Compte IsExistMarge(Client clt)
        {
            Compte cpt;
            try
            {
                return cpt = (Compte)(from c in clt.Comptes where c.IDTypeCompte == 4 select c).First();
            }
            catch
            {
                return cpt = null;
            }
        }
        public string PaiementFacture(Compte cpt, float montant)
        {
            float resteFrais;

            if (cpt.SoldeCompte >= montant + Singleton.fraisFacture)
            {
                cpt.SoldeCompte -= montant;
                cpt.SoldeCompte -= Singleton.fraisFacture;
                AjouterTransaction(DateTime.Today, montant, cpt.NumeroCompte, null, 4);
                AjouterTransaction(DateTime.Today, Singleton.fraisFacture, cpt.NumeroCompte, null, 4);
                return "Paiement de Facture : " + montant + " $ à partir du Compte Chèque : " + cpt.NumeroCompte.ToString();
            }
            else
            {
               if (IsExistMarge(Singleton.Instance.client) == null) { afficheMsgErreur(TEXT_MSG_MARGE_INEXISTANT); return null; }
                Compte cptMarge = IsExistMarge(Singleton.Instance.client);
                float soldeMarge = montant - (float)cpt.SoldeCompte;
                float soldeCpt = (float)cpt.SoldeCompte;

                if(cpt.SoldeCompte < montant)
                     cpt.SoldeCompte = 0;
                else
                     cpt.SoldeCompte -= montant;

                if (soldeCpt > montant)
                {
                    AjouterTransaction(DateTime.Today, montant, cpt.NumeroCompte, null, 4);
                    resteFrais = Singleton.fraisFacture - (soldeCpt - montant);
                    AjouterTransaction(DateTime.Today, (soldeCpt - montant), cpt.NumeroCompte, null, 4);
                    cptMarge.SoldeCompte += resteFrais;
                    AjouterTransaction(DateTime.Today, resteFrais, cptMarge.NumeroCompte, null, 4);
                    return "Solde insuffisant! Montant de " + resteFrais + " puiser dans la Marge de Crédit";
                   
                }
                else if (soldeCpt == montant)
                {
                    cptMarge.SoldeCompte += Singleton.fraisFacture;
                    AjouterTransaction(DateTime.Today, soldeCpt, cpt.NumeroCompte, null, 4);
                    AjouterTransaction(DateTime.Today, Singleton.fraisFacture, cptMarge.NumeroCompte, null, 4);
                    return "Solde insuffisant! Montant de " + Singleton.fraisFacture + " $ puiser dans la Marge de Crédit";           
                }
                else 
                {
                    cptMarge.SoldeCompte += (soldeMarge + Singleton.fraisFacture);
                    if (soldeCpt > 0) AjouterTransaction(DateTime.Today, soldeCpt, cpt.NumeroCompte, null, 4);
                    AjouterTransaction(DateTime.Today, soldeMarge, cptMarge.NumeroCompte, null, 4);
                    AjouterTransaction(DateTime.Today, Singleton.fraisFacture, cptMarge.NumeroCompte, null, 4);
                    return "Solde insuffisant! Montant de " + (soldeMarge + Singleton.fraisFacture) + " $ puiser dans la Marge de Crédit";
                }

            }

        }
        public string Depot(Compte cptSrc, float montant)
        {
            cptSrc.SoldeCompte += montant;
            AjouterTransaction(DateTime.Today, montant, cptSrc.NumeroCompte, null, 1);
            try
            {
                Singleton.Instance.myBDD.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public string Transfert(Compte cptSrc, Compte cptDest, float montant)
        {

            if (cptSrc.SoldeCompte >= montant)
            {
                cptSrc.SoldeCompte -= montant;
                if (cptDest.typeCompte.IDTypeCompte == 4)
                    cptDest.SoldeCompte -= montant;
                else
                    cptDest.SoldeCompte += montant;

                AjouterTransaction(DateTime.Today, montant, cptSrc.NumeroCompte, cptDest.NumeroCompte, 3);
                return messageTransferValid(montant, cptSrc.typeCompte.TypeCompte1, cptDest.typeCompte.TypeCompte1);
            }
            else
            {
                if (IsExistMarge(Singleton.Instance.client) == null) { afficheMsgErreur(TEXT_MSG_MARGE_INEXISTANT); return null; }
              
                Compte cptMarge = IsExistMarge(Singleton.Instance.client);

                float soldeMarge = montant - (float)cptSrc.SoldeCompte;
                float soldeCompte = (float)cptSrc.SoldeCompte;
                cptSrc.SoldeCompte = 0;
                cptMarge.SoldeCompte += soldeMarge;

                if (cptDest.typeCompte.IDTypeCompte == 4)
                    cptDest.SoldeCompte -= (soldeMarge + soldeCompte);
                else
                    cptDest.SoldeCompte += (soldeMarge + soldeCompte);

                if(soldeCompte > 0)
                AjouterTransaction(DateTime.Today, soldeCompte, cptSrc.NumeroCompte, cptDest.NumeroCompte, 3);
                AjouterTransaction(DateTime.Today, soldeMarge, cptMarge.NumeroCompte, cptDest.NumeroCompte, 3);

                return "Solde insuffisant! Montant de " + soldeMarge + " $ puiser dans la Marge de Crédit";
            }
        }
        public string AjouterClient(string codeClient, decimal numeroNIP, string nomClient, string prenomClient, string telephone, string courriel, bool statusClient)
        {
            Client newClt = new Client(codeClient, numeroNIP, nomClient, prenomClient, telephone, courriel, statusClient);
            Singleton.Instance.myBDD.Clients.Add(newClt);

            try
            {
                Singleton.Instance.myBDD.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public string AjouterCompte(DateTime? dateOvCompte, double? soldeCompte, string codeClient, int? iDTypeCompte)
        {
            Compte newCpt = new Compte(dateOvCompte, soldeCompte, codeClient, iDTypeCompte);
            Singleton.Instance.myBDD.Comptes.Add(newCpt);

            try
            {
                Singleton.Instance.myBDD.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public void PayerInteret(Compte cpt)
        {
                if (cpt.IDTypeCompte == 2)
                {
                    cpt.SoldeCompte += Math.Round((double)(cpt.SoldeCompte * Singleton.interetEpargne),2);
                   // AjouterTransaction(DateTime.Today, Math.Round((double)(cpt.SoldeCompte * Singleton.interetEpargne), 2), cpt.NumeroCompte, null, 1);

                }
        }
        public string AugmentMargeCrd()
        {

            foreach (Compte cpt in Singleton.Instance.myBDD.Comptes.ToList())
            {
                if (cpt.IDTypeCompte == 4)
                {
                    cpt.SoldeCompte += Math.Round((double)(cpt.SoldeCompte * Singleton.interetMarge));
                  //  AjouterTransaction(DateTime.Today, Math.Round((double)(cpt.SoldeCompte * Singleton.interetMarge), 2), cpt.NumeroCompte, null, 1);
                }
            }

            try
            {
                Singleton.Instance.myBDD.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public float ConvertPoint(String sNumber)
        { //Gestion de la de conversion des points décimaux

            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            sNumber = sNumber.Replace(",", separator).Replace(".", separator);

            return float.Parse(sNumber);
        }
        public string InputTelephone(string telephone)
        {
            Regex regex = new Regex(@"^\((\d{3})\)(\d{3})\-(\d{4})$");
            if (regex.IsMatch(telephone))
            {
                return telephone;
            }

            return null;

        }
        public string InputEmail(string email)
        {
            Regex regex = new Regex("^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+[.])+[a-z]{2,5}$");
            bool isValidEmail = regex.IsMatch(email);

            if (isValidEmail) { return email; }
            return null;
        }
        private void afficheMsgErreur(string msg)
        {
            MessageBox.Show(msg, TEXT_MSG_ATTENTION, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private string messageTransferValid(float montant, string CptSource, string CptDest)
            {
                string msg = "Transfert de " + montant + " $ de votre compte " + CptSource + " vers compte " + CptDest;
                return msg;
            }

    }
}
