using CustomIdentity.Models;
using System.Collections.Generic;

public class Film
{
    public int Id { get; set; }
    public string Tytul { get; set; }
    public string Opis { get; set; }
    public string Miniaturka { get; set; }

    public string LinkDoFilmu { get; set; }

    public List<Ocena> Oceny { get; set; } = new List<Ocena>();

    public bool IsPremium { get; set; }
}
