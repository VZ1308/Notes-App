using System.Runtime.ConstrainedExecution;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Notable
{
    public partial class AddNoteWindow : Window
    {
        public string NoteName { get; private set; }
        public string NoteContent { get; private set; }
        public bool IsFavorite { get; private set; }
        public AddNoteWindow()
        {
            InitializeComponent(); //automatisch generierte Methode, die die UI-Elemente basierend auf der XAML-Datei erstellt und initialisiert
        }

        private void ButtonSaveAddNote_Click(object sender, RoutedEventArgs e)
        {
            NoteName = nameTextBox.Text;
            NoteContent = contentTextBox.Text;
            IsFavorite = favCheckBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(NoteName) || string.IsNullOrWhiteSpace(NoteContent))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.");
                return;
            }

            DialogResult = true; // gibt an, dass das Fenster erfolgreich geschlossen wird und eine gültige Notiz erstellt wurde
            this.Close();
        }
    }
}
