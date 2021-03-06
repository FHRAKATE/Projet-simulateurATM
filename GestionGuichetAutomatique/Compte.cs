//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GestionGuichetAutomatique
{
    using System;
    using System.Collections.Generic;
    
    public partial class Compte
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Compte()
        {
            this.Transactions = new HashSet<Transaction>();
            this.Transactions1 = new HashSet<Transaction>();
        }

        public Compte(DateTime? dateOuverture, double? soldeCompte, string codeClient, int? iDTypeCompte)
        {
            //NumeroCompte = numeroCompte;
            DateOuverture = dateOuverture;
            SoldeCompte = soldeCompte;
            CodeClient = codeClient;
            IDTypeCompte = iDTypeCompte;
        }

        public int NumeroCompte { get; set; }
        public Nullable<System.DateTime> DateOuverture { get; set; }
        public Nullable<double> SoldeCompte { get; set; }
        public string CodeClient { get; set; }
        public Nullable<int> IDTypeCompte { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual typeCompte typeCompte { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions1 { get; set; }
    }
}
