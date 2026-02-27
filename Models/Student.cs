namespace StudentDraw.Models
{
    public class Student
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }

        private string _classSymbol;
        public string ClassSymbol
        {
            get { return _classSymbol; }
            set { _classSymbol = value; }
        }

        private bool _isPresent;
        public bool IsPresent
        {
            get { return _isPresent; }
            set { _isPresent = value; }
        }

        public string DisplayName => $"{Surname} {Name}";

        public Student(string name, string surname, string classSymbol)
        {
            _name = name;
            _surname = surname;
            _classSymbol = classSymbol;
            _isPresent = true;
        }

        public override string ToString()
        {
            return $"{Name},{Surname},{ClassSymbol},{IsPresent}";
        }

        public static Student FromString(string data)
        {
            string[] parts = data.Split(',');
            if (parts.Length < 3)
            {
                throw new ArgumentException("Nieprawidłowy format danych");
            }
            var student = new Student(parts[0], parts[1], parts[2]);
            if (parts.Length >= 4 && bool.TryParse(parts[3], out bool isPresent))
            {
                student.IsPresent = isPresent;
            }
            return student;
        }
    }
}
