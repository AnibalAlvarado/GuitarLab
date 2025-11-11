using System.Collections.ObjectModel;
using Entity.Dtos;
using GuitarLabMobile.Services;
using Microsoft.Maui.Controls;

namespace GuitarLabMobile.Views
{
    [QueryProperty(nameof(LessonId), "lessonId")]
    public partial class ExercisesPage : ContentPage
    {
        public int LessonId { get; set; }
        public ObservableCollection<LessonExerciseDto> Exercises { get; set; } = new();
        private readonly ApiService _apiService;

        public ExercisesPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            ExercisesCollectionView.ItemsSource = Exercises;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            bool isAuthenticated = await _apiService.IsAuthenticatedAsync();
            if (!isAuthenticated)
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }
            await LoadExercisesAsync();
        }

        private async Task LoadExercisesAsync()
        {
            var exercises = await _apiService.GetLessonExercisesAsync(LessonId);
            if (exercises != null)
            {
                Exercises.Clear();
                foreach (var exercise in exercises)
                {
                    Exercises.Add(exercise);
                }
            }
        }

        private async void OnExerciseSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is LessonExerciseDto selectedExercise)
            {
                // Navigate to ExerciseDetailPage with exercise id
                await Shell.Current.GoToAsync($"ExerciseDetailPage?exerciseId={selectedExercise.ExerciseId}");
                ExercisesCollectionView.SelectedItem = null;
            }
        }
    }
}