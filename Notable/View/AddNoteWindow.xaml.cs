using System.Windows;
using Notable.Model; // stellt die Note-Klasse bereit

namespace Notable
{
    public partial class AddNoteWindow : Window
    {
        // Property für die neue Notiz
        public Note NewNote { get; private set; }

        public AddNoteWindow()
        {
            InitializeComponent();
        }

        private void ButtonSaveAddNote_Click(object sender, RoutedEventArgs e)
        {
            // Eingabedaten aus UI-Elementen erfassen
            string noteName = nameTextBox.Text;
            string noteContent = contentTextBox.Text;
            bool isFavorite = favCheckBox.IsChecked == true;

            // Überprüfen, ob Eingaben vorhanden sind
            if (string.IsNullOrWhiteSpace(noteName) || string.IsNullOrWhiteSpace(noteContent))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.");
                return;
            }

            // Neues Note-Objekt erstellen und speichern
            NewNote = new Note(noteName, noteContent, isFavorite);

            // Setzt DialogResult auf true, um anzuzeigen, dass Speichern erfolgreich war
            DialogResult = true;
            this.Close();
        }
    }
}
