#nullable enable

namespace GestionMaintenance
{
    /// <summary>
    /// Classe abstraite : informations communes à toute personne.
    /// </summary>
    public abstract class Personne : IEntite
    {
        public int Id { get; protected set; }

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public string NomComplet => $"{Prenom} {Nom}";

        protected Personne(
            int id,
            string nom,
            string prenom,
            string email,
            string telephone)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }
    }
}