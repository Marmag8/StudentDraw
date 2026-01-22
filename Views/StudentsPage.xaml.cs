using StudentDraw.Services;
using StudentDraw.Models;

namespace StudentDraw.Views;

public partial class StudentsPage : ContentPage
{
	public StudentsPage()
	{
		InitializeComponent();

		Dictionary<string, List<Student>> students = Utils.LoadFromFile();
    }
}