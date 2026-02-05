namespace StudentDraw.Models
{
    internal class Student
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

        public string DisplayName => $"{Surname} {Name}";

        public Student(string name, string surname, string classSymbol)
        {
            _name = name;
            _surname = surname;
            _classSymbol = classSymbol;
        }

        public override string ToString()
        {
            return $"{Name},{Surname},{ClassSymbol}";
        }

        public static Student FromString(string data)
        {
            string[] parts = data.Split(',');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Nieprawidłowy format danych");
            }
            return new Student(parts[0], parts[1], parts[2]);
        }
    }
}
