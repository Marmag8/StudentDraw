using System.Collections.ObjectModel;
using StudentDraw.Models;
using StudentDraw.Services;

namespace StudentDraw.Views;

public partial class StudentsPage : ContentPage
{
    private readonly Dictionary<string, List<Student>> students;

    private ObservableCollection<StudentGroup> _groupedStudents = new();
    public ObservableCollection<StudentGroup> GroupedStudents
    {
        get => _groupedStudents;
        set
        {
            if (_groupedStudents != value)
            {
                _groupedStudents = value;
                OnPropertyChanged();
            }
        }
    }

    public StudentsPage()
    {
        InitializeComponent();

        BindingContext = this;

        students = Utils.LoadFromFile();
        BuildGroupedCollection(students);
    }

    private void BuildGroupedCollection(Dictionary<string, List<Student>> source)
    {
        ObservableCollection<StudentGroup> newCollection = [];

        foreach (KeyValuePair<string, List<Student>> kvp in source.OrderBy(k => k.Key))
        {
            string classSymbol = kvp.Key;
            
            IEnumerable<StudentEntry> entries = kvp.Value
                .OrderBy(s => s.Surname)
                .ThenBy(s => s.Name)
                .Select(s => new StudentEntry(classSymbol, s.Name, s.Surname));

            newCollection.Add(new StudentGroup(classSymbol, entries));
        }

        GroupedStudents = newCollection;
    }

    private async void AddStudentToClass(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.CommandParameter is not string classSymbol || string.IsNullOrWhiteSpace(classSymbol))
        {
            return;
        }

        if (!students.ContainsKey(classSymbol))
        {
            await DisplayAlert("B³¹d", $"Nie znaleziono klasy: {classSymbol}", "OK");
            return;
        }

        string name = await DisplayPromptAsync(
            "Nowy uczeñ",
            $"Podaj imiê i nazwisko (klasa: {classSymbol})",
            "OK",
            "Anuluj",
            placeholder: "np. Jan Kowalski");

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (name.Contains(','))
        {
            await DisplayAlert("B³¹d", "Nie u¿ywaj przecinków. Wpisz: Imiê Nazwisko.", "OK");
            return;
        }

        string[] parts = name
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length < 2)
        {
            await DisplayAlert("B³¹d", "Podaj imiê i nazwisko oddzielone spacj¹.", "OK");
            return;
        }

        string firstName = parts[0];
        string surname = string.Join(' ', parts.Skip(1));

        students[classSymbol].Add(new Student(firstName, surname, classSymbol));
        Utils.SaveToFile(students);

        BuildGroupedCollection(students);
    }

    private async void AddClass(object sender, EventArgs e)
    {
        var classSymbol = await DisplayPromptAsync("Nowa klasa", "Podaj symbol klasy (np. 1A, 2B)", "OK", "Anuluj");
        if (string.IsNullOrWhiteSpace(classSymbol))
        {
            return;
        }

        classSymbol = classSymbol.Trim();

        if (students.ContainsKey(classSymbol))
        {
            await DisplayAlert("B³¹d", "Taka klasa ju¿ istnieje.", "OK");
            return;
        }

        students.Add(classSymbol, new List<Student>());
        Utils.SaveToFile(students);

        BuildGroupedCollection(students);
    }

    private async void DeleteClass(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.CommandParameter is not string classSymbol || string.IsNullOrWhiteSpace(classSymbol))
        {
            return;
        }

        if (!students.ContainsKey(classSymbol))
        {
            await DisplayAlert("B³¹d", $"Nie znaleziono klasy: {classSymbol}", "OK");
            return;
        }

        students.Remove(classSymbol);
        Utils.SaveToFile(students);

        BuildGroupedCollection(students);
    }

    private async void DeleteStudent(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }
        if (button.CommandParameter is not StudentEntry entry)
        {
            return;
        }
        if (!students.ContainsKey(entry.ClassSymbol))
        {
            await DisplayAlert("B³¹d", $"Nie znaleziono klasy: {entry.ClassSymbol}", "OK");
            return;
        }
        Student? studentToRemove = students[entry.ClassSymbol]
            .FirstOrDefault(s => s.Name == entry.Name && s.Surname == entry.Surname);
        if (studentToRemove == null)
        {
            await DisplayAlert("B³¹d", $"Nie znaleziono ucznia: {entry.DisplayName} w klasie {entry.ClassSymbol}", "OK");
            return;
        }
        students[entry.ClassSymbol].Remove(studentToRemove);
        Utils.SaveToFile(students);
        BuildGroupedCollection(students);
    }

    public class StudentEntry
    {
        public string ClassSymbol { get; }
        public string Name { get; }
        public string Surname { get; }
        public string DisplayName => $"{Surname} {Name}";

        public StudentEntry(string classSymbol, string name, string surname)
        {
            ClassSymbol = classSymbol;
            Name = name;
            Surname = surname;
        }
    }

    public class StudentGroup : ObservableCollection<StudentEntry>
    {
        public string ClassSymbol { get; }

        public StudentGroup(string classSymbol, IEnumerable<StudentEntry> entries) : base(entries)
        {
            ClassSymbol = classSymbol;
        }
    }
}