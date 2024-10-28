using Notable.Model;

namespace Notable.Controller
{
    /// <summary>
    /// Die NotesController-Klasse verwaltet eine Liste von Notizen.
    /// Sie ermöglicht das Hinzufügen, Entfernen, Suchen und Verwalten von Favoriten.
    /// </summary>
    public class NotesController
    {
        // Eine private Liste von Notizen, die in dieser Klasse verwaltet wird
        private List<Note> notes = new List<Note>();

        public IEnumerable<Note> GetAllNotes() => notes; // ist ein Interface, das nur die Fähigkeit zum Iterieren über eine Sammlung bietet, ohne Details über deren Implementierung preiszugeben
        // Datenkapselung, wird verwendet wenn man nichts über Datenstruktur wissen möchte und nur lesen will
        public void AddNote(Note note)
        {
            notes.Add(note);
        }

        public void RemoveNote(Note note)
        {
            notes.Remove(note);
        }

        // Filtert die Liste der Notizen und gibt nur die Notizen zurück, die den Suchbegriff im Namen oder im Inhalt enthalten
        public IEnumerable<Note> SearchNotes(string searchTerm)
        {
            return notes.Where(n => n.NoteName.Contains(searchTerm) || n.NoteContent.Contains(searchTerm));
        }

        public void ToggleFavorite(Note note)
        {
            var existingNote = notes.FirstOrDefault(n => n.NoteName == note.NoteName && n.NoteContent == note.NoteContent);
            if (existingNote != null)
                existingNote.IsFavorite = !existingNote.IsFavorite;
        }

        public IEnumerable<Note> GetFavoriteNotes()
        {
            return notes.Where(n => n.IsFavorite);
        }
    }
}