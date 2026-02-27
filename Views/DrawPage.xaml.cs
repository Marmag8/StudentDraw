using StudentDraw.Models;
using StudentDraw.Services;

namespace StudentDraw.Views
{
    public partial class DrawPage : ContentPage
    {
        Dictionary<string, List<Student>> students = new();

        public DrawPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            students = Utils.LoadFromFile();

            List<string> classNames = new List<string> { "Wszystkie Klasy" };
            classNames.AddRange(students.Keys);

            string? selectedClass = ClassPicker.SelectedItem?.ToString();

            ClassPicker.ItemsSource = classNames;

            if (selectedClass != null && classNames.Contains(selectedClass))
            {
                ClassPicker.SelectedItem = selectedClass;
            }
            else
            {
                ClassPicker.SelectedIndex = 0;
            }
        }

        private void Draw(object sender, EventArgs e)
        {
            List<Student> drawPool = new List<Student>();

            if (ClassPicker.SelectedIndex == 0)
            {
                foreach (List<Student> classStudents in students.Values)
                {
                    drawPool.AddRange(classStudents);
                }
            }
            else
            {
                string selectedClass = ClassPicker.SelectedItem.ToString() ?? "";
                if (students.ContainsKey(selectedClass))
                {
                    drawPool.AddRange(students[selectedClass]);
                }
            }
            if (drawPool.Count == 0)
            {
                Result.Text = "Brak uczniów do losowania.";
                return;
            }

            Random rand = new Random();
            int index = rand.Next(drawPool.Count);
            Student selectedStudent = drawPool[index];
            Result.Text = $"Wylosowany uczeń: {selectedStudent.DisplayName}";
        }
    }
}
