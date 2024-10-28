using Notable;
using System.Collections.Generic; // Für die List<T> -Verwendung
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Net.WebRequestMethods;

namespace NotesApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Event für GotFocus, um den Platzhaltertext zu entfernen
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Suche nach Notizen...")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black; // Setze Textfarbe auf schwarz
            }
        }

        // Event für LostFocus, um den Platzhaltertext zurückzusetzen
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Suche nach Notizen...";
                textBox.Foreground = Brushes.Gray; // Setze Textfarbe auf grau für den Platzhalter
            }
        }

        // Such-Logik
        private void searchNote_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob das Textfeld nicht leer ist
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text) && searchTextBox.Text != "Suche nach Notizen...")
            {
                string searchTerm = searchTextBox.Text.ToLower();

                // Liste für gefundene Notizen
                List<string> foundNotes = new List<string>();

                // Durchlaufe alle Notizen und filtere sie
                foreach (var item in allNotesListBox.Items)
                {
                    if (item is ListBoxItem listItem)
                    {
                        string note = listItem.Content.ToString();

                        // Überprüfen, ob die Notiz den Suchbegriff enthält (Teilwortsuche)
                        if (note.ToLower().Contains(searchTerm))
                        {
                            foundNotes.Add(note);
                        }
                    }
                }

                // Überprüfen, ob gefundene Notizen vorhanden sind
                if (foundNotes.Count > 0)
                {
                    // Fasse die gefundenen Notizen in einer einzigen Nachricht zusammen
                    string message = "Gefundene Notizen:\n" + string.Join("\n", foundNotes);
                    MessageBox.Show(message, "Suchergebnisse");
                }
                else
                {
                    MessageBox.Show("Keine Notizen gefunden.", "Suchergebnisse");
                }
            }
            else
            {
                MessageBox.Show("Bitte geben Sie einen Suchbegriff ein.", "Warnung");
            }
        }

        // Methode zum Hinzufügen einer Notiz
        private void addNote_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen einer neuen Instanz des AddNoteWindow
            AddNoteWindow addNoteWindow = new AddNoteWindow();

            // Fenster anzeigen und auf die Eingaben warten
            bool? result = addNoteWindow.ShowDialog();

            // Prüfen, ob der Benutzer auf "Speichern" geklickt hat
            if (result == true)
            {
                // Eine neue Notiz zur ListBox hinzufügen
                string newNote = addNoteWindow.NoteName + ": " + addNoteWindow.NoteContent;

                // Statt eines ListBoxItem wird der Text direkt hinzugefügt
                allNotesListBox.Items.Add(new ListBoxItem { Content = newNote });

                // Falls die Notiz als Favorit markiert ist, zur Favoriten-Liste hinzufügen
                if (addNoteWindow.IsFavorite)
                {
                    favoriteNotesListBox.Items.Add(new ListBoxItem { Content = newNote });
                }

                MessageBox.Show("Notiz wurde erfolgreich hinzugefügt.");
            }
        }

        // Methode zum Entfernen einer Notiz
        private void removeNote_Click(object sender, RoutedEventArgs e)
        {
            // Prüfen, ob eine Notiz ausgewählt ist
            if (allNotesListBox.SelectedItem != null)
            {
                // Das ausgewählte ListBoxItem in einen string umwandeln
                string selectedNote = (allNotesListBox.SelectedItem as ListBoxItem).Content.ToString();

                // Notiz aus "Alle Notizen"-Liste entfernen
                allNotesListBox.Items.Remove(allNotesListBox.SelectedItem);

                // Überprüfen, ob die Notiz auch in der Favoritenliste vorhanden ist
                // und sie aus der Favoritenliste entfernen, wenn vorhanden
                for (int i = favoriteNotesListBox.Items.Count - 1; i >= 0; i--)
                {
                    // Vergleiche den Inhalt der Favoritenliste mit der ausgewählten Notiz
                    if ((favoriteNotesListBox.Items[i] as ListBoxItem).Content.ToString() == selectedNote)
                    {
                        favoriteNotesListBox.Items.RemoveAt(i);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Notiz zum Entfernen aus.");
            }
        }

        // Methode zum Markieren einer Notiz als Favorit
        private void favNote_Click(object sender, RoutedEventArgs e)
        {
            // Prüfen, ob eine Notiz ausgewählt ist
            if (allNotesListBox.SelectedItem != null)
            {
                // Extrahiere den Text der ausgewählten Notiz
                string selectedNote = (allNotesListBox.SelectedItem as ListBoxItem).Content.ToString();

                // Überprüfen, ob diese Notiz bereits in den Favoriten vorhanden ist
                bool alreadyFavorited = false;
                foreach (var item in favoriteNotesListBox.Items)
                {
                    if ((item as ListBoxItem).Content.ToString() == selectedNote)
                    {
                        alreadyFavorited = true;
                        break;
                    }
                }

                // Wenn die Notiz noch nicht in den Favoriten ist, füge sie hinzu
                if (!alreadyFavorited)
                {
                    // Erstelle ein neues ListBoxItem, um die Notiz zu den Favoriten hinzuzufügen
                    favoriteNotesListBox.Items.Add(new ListBoxItem { Content = selectedNote });
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

        // Methode zum Speichern der Notizen in einer Datei
        private void SaveNotizenToFile()
        {
            // Erstellen eines StringBuilders
            StringBuilder sb = new StringBuilder();

            // Überprüfen, ob die ListBox Elemente enthält
            if (allNotesListBox.Items.Count == 0)
            {
                MessageBox.Show("Die Liste enthält keine Notizen zum Speichern.");
                return; // Abbrechen
            }

            // Notiz aus der ListBox hinzufügen
            foreach (var notiz in allNotesListBox.Items)
            {
                sb.AppendLine(((ListBoxItem)notiz).Content.ToString());
            }

            string filePath = Path.Combine(Environment.CurrentDirectory, "C:\\Users\\051505\\source\\repos\\To do List\\Notizen.txt");

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true)) // true = anhängen
                {
                    sw.Write(sb.ToString());
                }
                MessageBox.Show("Notizen erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        
    }

        // Event-Handler für den Menüpunkt "Speichern"
        private void MenuItemSpeichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveNotizenToFile();
                MessageBox.Show("Notizen erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        }

        // Event-Handler für den Menüpunkt "Löschen"
        private void MenuItemLöschen_Click(object sender, RoutedEventArgs e)
        {
            allNotesListBox.Items.Clear();
            MessageBox.Show("Notizen wurden gelöscht.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Event-Handler für den Menüpunkt "Hilfe"
        private void MenuItemHilfe_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Willkommen in der Notizen-App! Hier finden Sie eine Übersicht der Hauptfunktionen und Anweisungen, wie Sie die App verwenden können.\r\n\r\nFunktionen\r\nNotizen hinzufügen\r\n\r\nGeben Sie Ihre Notizen in das Eingabefeld oder die ListBox ein und drücken Sie Enter, um neue Einträge hinzuzufügen.\r\nNotizen durchsuchen\r\n\r\nGeben Sie ein Stichwort oder einen Suchbegriff in das Suchfeld ein und klicken Sie auf Suchen.\r\nNotizen speichern\r\n\r\nUm Ihre Notizen in einer Textdatei zu speichern, " +
                $"klicken Sie im Menü auf Datei speichern.\r\nAlle Notizen in der Liste werden in eine Datei mit dem Namen Notizen.txt gespeichert, die Sie jederzeit öffnen und einsehen können.\r\nNotizen löschen\r\n\r\nUm alle Notizen auf einmal zu löschen, klicken Sie im Menü auf Löschen.\r\nDies entfernt alle Einträge in der ListBox. Achtung: Die Notizen werden dauerhaft gelöscht und können nicht wiederhergestellt werden.\r\nHilfe\r\n\r\nFalls Sie weitere Informationen benötigen, klicken Sie im Menü auf Hilfe. Hier finden Sie immer eine Anleitung zur Nutzung der App.\r\nHinweise\r\nDateiformat: Die Notizen werden als einfache Textdatei (Notizen.txt) gespeichert und können mit jedem Texteditor geöffnet werden.\r\nSpeichern vor dem Schließen: Denken Sie daran, Ihre Notizen regelmäßig zu speichern, bevor Sie die App schließen, um Datenverlust zu vermeiden.");
        }
    }
}
