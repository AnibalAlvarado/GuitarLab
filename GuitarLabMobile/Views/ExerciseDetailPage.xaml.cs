using Entity.Dtos;
using GuitarLabMobile.Services;
using Microsoft.Maui.Controls;

namespace GuitarLabMobile.Views
{
    [QueryProperty(nameof(ExerciseId), "exerciseId")]
    public partial class ExerciseDetailPage : ContentPage
    {
        public int ExerciseId { get; set; }
        private readonly ApiService _apiService;

        public ExerciseDetailPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
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
            await LoadExerciseAsync();
        }

        private async Task LoadExerciseAsync()
        {
            var exercise = await _apiService.GetExerciseAsync(ExerciseId);
            if (exercise != null)
            {
                NameLabel.Text = exercise.Name;
                DescriptionLabel.Text = exercise.Description;
                DifficultyLabel.Text = exercise.DifficultyName;
                BPMLabel.Text = exercise.BPM.ToString();
                TuningLabel.Text = exercise.Tuning;
                TablatureLabel.Text = exercise.TabNotation;
            }
        }
    }
}