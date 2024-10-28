using Notable;
using Notable.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NotesApp
{
    /// <summary>
    /// Die MainWindow-Klasse ist das Hauptfenster der Notizen-App,
    /// die für die Benutzeroberfläche und die Interaktion mit dem Benutzer verantwortlich ist.
    /// </summary>
    public partial class MainWindow : Window
    {
        private NoteManager _noteManager;

        public MainWindow()
        {
            InitializeComponent();
            _noteManager = new NoteManager();

            // Bindet die ObservableCollection an die ListBoxes, sodass sie automatisch aktualisiert werden,
            // wenn Notizen hinzugefügt oder entfernt werden
            allNotesListBox.ItemsSource = _noteManager.AllNotes;
            favoriteNotesListBox.ItemsSource = _noteManager.FavoriteNotes;
        }
        // Event für GotFocus, um den Platzhaltertext zu entfernen
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == "Suche nach Notizen...")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        // Event für LostFocus, um den Platzhaltertext zurückzusetzen
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Suche nach Notizen...";
                textBox.Foreground = Brushes.Gray;
            }
        }


        // Such-Logik
        private void searchNote_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchTextBox.Text?.Trim(); // holt Suchbegriff und entfernt Leerzeichen
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Suche nach Notizen...")
            {
                MessageBox.Show("Bitte geben Sie einen Suchbegriff ein.", "Warnung");
                return;
            }
            // Sucht Notizen, die den Suchbegriff enthalten, unabhängig von der Groß/Kleinschreibung
            var foundNotes = _noteManager.AllNotes.Where(note =>
                note.NoteName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || // prüft jede Notiz in AllNotes
                note.NoteContent.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList(); // konvertiert die gefilterte IEnumerable zurück in eine Liste, die in foundNotes gespeichert wird

            if (foundNotes.Any())
            {
                string message = "Gefundene Notizen:\n" + string.Join("\n", foundNotes.Select(n => $"{n.NoteName}: {n.NoteContent}"));
                // LINQ-Abfrage durchläuft jedes Element (n) in foundNotes und erstellt einen String für jede Notiz
                MessageBox.Show(message, "Suchergebnisse");
            }
            else
            {
                MessageBox.Show("Keine Notizen gefunden.", "Suchergebnisse");
            }
        }

        // Methode zum Hinzufügen einer Notiz
        private void addNote_Click(object sender, RoutedEventArgs e)
        {
            AddNoteWindow addNoteWindow = new AddNoteWindow(); // erstellt neues AddNoteWindow Fensrer
            bool? result = addNoteWindow.ShowDialog(); // blockieren die Interaktion mit anderen Fenstern der Anwendung, bis das Dialogfeld geschlossen wird
            // wenn Notiz erfolgreich hinzugefügt wurde, wird sie zum NoteManager hinzugefügt
            if (result == true && addNoteWindow.NewNote != null)
            {
                _noteManager.AddNote(addNoteWindow.NewNote);
                MessageBox.Show("Notiz wurde erfolgreich hinzugefügt.");
            }
        }

        // Methode zum Entfernen einer Notiz
        private void removeNote_Click(object sender, RoutedEventArgs e)
        {
            if (allNotesListBox.SelectedItem is Note selectedNote) // is wird verwendet, um zu prüfen, ob das SelectedItem vom Typ Note ist
            {
                _noteManager.RemoveNote(selectedNote);
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Notiz zum Entfernen aus.");
            }
        }

        // Methode zum Markieren einer Notiz als Favorit
        private void favNote_Click(object sender, RoutedEventArgs e)
        {
            if (allNotesListBox.SelectedItem is Note selectedNote)
            {
                if (!selectedNote.IsFavorite)
                {
                    _noteManager.MarkAsFavorite(selectedNote);
                    MessageBox.Show("Notiz als Favorit markiert.");
                }
                else
                {
                    MessageBox.Show("Diese Notiz ist bereits in den Favoriten.");
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Notiz zum Hinzufügen zu den Favoriten aus.");
            }
        }

        // Methode zum Speichern der Notizen
       private void MenuItemSpeichern_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Notizen.txt");

            try
            {
                _noteManager.SaveNotesToFile(filePath);
                MessageBox.Show("Notizen erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        }

        // Menü-Events zum Löschen
        
        private void MenuItemLöschen_Click(object sender, RoutedEventArgs e)
        {
            _noteManager.AllNotes.Clear();
            _noteManager.FavoriteNotes.Clear();
            MessageBox.Show("Notizen wurden gelöscht.");
        }

        private void MenuItemHilfe_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Willkommen in der Notizen - App! Hier finden Sie eine Übersicht der Hauptfunktionen und Anweisungen, wie Sie die App verwenden können.\r\n\r\nFunktionen\r\nNotizen hinzufügen\r\n\r\nGeben Sie Ihre Notizen in das Eingabefeld oder die ListBox ein und drücken Sie Enter, um neue Einträge hinzuzufügen.\r\nNotizen durchsuchen\r\n\r\nGeben Sie ein Stichwort oder einen Suchbegriff in das Suchfeld ein und klicken Sie auf Suchen.\r\nNotizen speichern\r\n\r\nUm Ihre Notizen in einer Textdatei zu speichern, " +
                $"klicken Sie im Menü auf Datei speichern.\r\nAlle Notizen in der Liste werden in eine Datei mit dem Namen Notizen.txt gespeichert, die Sie jederzeit öffnen und einsehen können.\r\nNotizen löschen\r\n\r\nUm alle Notizen auf einmal zu löschen, klicken Sie im Menü auf Löschen.\r\nDies entfernt alle Einträge in der ListBox. Achtung: Die Notizen werden dauerhaft gelöscht und können nicht wiederhergestellt werden.\r\nHilfe\r\n\r\nFalls Sie weitere Informationen benötigen, klicken Sie im Menü auf Hilfe. Hier finden Sie immer eine Anleitung zur Nutzung der App.\r\nHinweise\r\nDateiformat: Die Notizen werden als einfache Textdatei (Notizen.txt) gespeichert und können mit jedem Texteditor geöffnet werden.\r\nSpeichern vor dem Schließen: Denken Sie daran, Ihre Notizen regelmäßig zu speichern, bevor Sie die App schließen, um Datenverlust zu vermeiden.");
        }
    }
}
