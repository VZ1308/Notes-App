using Notable.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Notable
{
    public class NoteManager
    {
        // Bietet die notwendige Funktionalität, um sicherzustellen, dass die Benutzeroberfläche immer aktuell bleibt, wenn Änderungen an der Sammlung vorgenommen werden
        public ObservableCollection<Note> AllNotes { get; private set; } // bedeutet, dass die Eigenschaft nur innerhalb der Klasse gesetzt, aber von außen gelesen werden kann
        public ObservableCollection<Note> FavoriteNotes { get; private set; }

        // Konstruktor
        public NoteManager()
        {
            AllNotes = new ObservableCollection<Note>(); // Initialisiert die Sammlung für alle Notizen
            FavoriteNotes = new ObservableCollection<Note>(); // Initialisiert die Sammlung für Favoriten
        }

        // Notiz hinzufügen
        public void AddNote(Note note)
        {
            AllNotes.Add(note);
            if (note.IsFavorite)
            {
                FavoriteNotes.Add(note);
            }
        }

        // Notiz entfernen
        public void RemoveNote(Note note)
        {
            AllNotes.Remove(note);
            FavoriteNotes.Remove(note);
        }

        // Notiz als Favorit markieren
        public void MarkAsFavorite(Note note)
        {
            if (!FavoriteNotes.Contains(note))
            {
                note.IsFavorite = true;
                FavoriteNotes.Add(note);
            }
            else
            {
                MessageBox.Show("Die Notiz befindet sich bereits in der Favoritenliste.");
            }
        }

        // Filtert die Liste der Notizen und gibt nur die Notizen zurück, die den Suchbegriff im Namen oder im Inhalt enthalten
        public IEnumerable<Note> SearchNotes(string searchTerm)
        {
            return AllNotes.Where(n => n.NoteName.Contains(searchTerm) || n.NoteContent.Contains(searchTerm));
        }

        // Notizen speichern
        public void SaveNotesToFile(string filePath)
        {
            if (AllNotes.Count == 0)
                throw new InvalidOperationException("Keine Notizen zum Speichern.");

            StringBuilder sb = new StringBuilder();
            foreach (var note in AllNotes)
            {
                sb.AppendLine($"{note.NoteName}: {note.NoteContent}");
            }

            File.AppendAllText(filePath, sb.ToString());
        }

        // Gibt alle Notizen zurück
        public IEnumerable<Note> GetAllNotes() => AllNotes;

        // Gibt die Favoriten zurück
        public IEnumerable<Note> GetFavoriteNotes() => FavoriteNotes;
       

        // Toggle für Favoritenstatus
        public void ToggleFavorite(Note note)
        {
            // Suche nach der Notiz in der AllNotes-Sammlung
            var existingNote = AllNotes.FirstOrDefault(n => n.NoteName == note.NoteName && n.NoteContent == note.NoteContent);
            if (existingNote != null)
            {
                //Umkehren des Favoritenstatus
                existingNote.IsFavorite = !existingNote.IsFavorite;
                if (existingNote.IsFavorite)
                {
                    //Überprüfen, ob die Notiz bereits in der FavoriteNotes-Sammlung vorhanden ist
                    if (!FavoriteNotes.Contains(existingNote))
                    {
                        FavoriteNotes.Add(existingNote);
                    }
                }
                else
                {
                    FavoriteNotes.Remove(existingNote);
                }
            }
        }
    }
}
