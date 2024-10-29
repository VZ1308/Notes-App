namespace Notable.Model
{
    public class Note
    {
        // Private Attribute
        private string _noteName;
        private string _noteContent;
        private bool _isFavorite;

        // Öffentliche Eigenschaften/Propertys
        public string NoteName
        {
            get => _noteName; 
            set => _noteName = value; 
        }

        public string NoteContent
        {
            get => _noteContent; 
            set => _noteContent = value;  
        }

        public bool IsFavorite
        {
            get => _isFavorite;  
            set => _isFavorite = value;  
        }

        // Konstruktor
        public Note(string noteName, string noteContent, bool isFavorite)
        {
            _noteName = noteName;
            _noteContent = noteContent;
            _isFavorite = isFavorite;
        }

        public override string ToString()
        {
            return $"{NoteName}: {NoteContent}";
        }
    }
}
