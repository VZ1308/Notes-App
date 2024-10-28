using Notable.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace Notable
{
    public class NoteManager
    {
        // bietet die notwendige Funktionalität, um sicherzustellen, dass Benutzeroberfläche immer aktuell bleibt, wenn Änderungen an der Sammlung vorgenommen werden
        public ObservableCollection<Note> AllNotes { get; private set; } // bedeutet, dass die Eigenschaft nur innerhalb der Klasse gesetzt, aber von außen gelesen werden kann
        public ObservableCollection<Note> FavoriteNotes { get; private set; }

        // Konstruktor
        public NoteManager()
        {
            AllNotes = new ObservableCollection<Note>(); // Initialisiert die Sammlung für alle Notizen
            FavoriteNotes = new ObservableCollection<Note>();
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
            FavoriteNotes.Remove(note); // Automatisch aus Favoriten entfernen, wenn vorhanden
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
    }
}
