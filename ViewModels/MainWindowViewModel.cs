using L046_Labb3_Code_Along.Command;
using L046_Labb3_Code_Along.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace L046_Labb3_Code_Along.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; private set; } = new();

        private QuestionPackViewModel _activePack;
        public DelegateCommand SetFullScreenCommand { get; }
        public QuestionPackViewModel ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged();
                PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
                ConfigurationViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
            }
        }

        public DelegateCommand OpenPlayerViewCommand { get; }

        public DelegateCommand OpenConfigViewCommand { get; }

        private Visibility _visabilityPlayViewVisible = Visibility.Collapsed;

        private Visibility _visabilityConfigViewVisible = Visibility.Visible;
        
        public Visibility PlayViewVisible {
            get => _visabilityPlayViewVisible;
            set
            {
                _visabilityPlayViewVisible = value;
                RaisePropertyChanged(nameof(PlayViewVisible));
            }
        }

        public Visibility ConfigViewVisible
        {
            get => _visabilityConfigViewVisible;
            set
            {
                _visabilityConfigViewVisible = value;
                RaisePropertyChanged(nameof(ConfigViewVisible));
            }
        }
        public PlayerViewModel? PlayerViewModel { get; }
        public ConfigurationViewModel? ConfigurationViewModel { get; }
        
        
        public void LoadQuestions(string filePath)
        {
            
            if (!File.Exists(filePath))
            {
                var pack = new QuestionPack("Default Pack");

                if (ActivePack == null)
                {
                    ActivePack = new QuestionPackViewModel(pack);
                    Packs.Add(ActivePack);
                }

                return;
            }
            
            string json = File.ReadAllText(filePath);
            Packs = JsonSerializer.Deserialize<ObservableCollection<QuestionPackViewModel>>(json);

            ActivePack = Packs?.FirstOrDefault();
        }
        public MainWindowViewModel()
        {
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);
         
            OpenPlayerViewCommand = new DelegateCommand(OpenPlayerView);
            OpenConfigViewCommand = new DelegateCommand(OpenConfigView);
            SetFullScreenCommand = new DelegateCommand(setFullScreen);
            
            LoadQuestions("questions.json");
         }
        public void setFullScreen(object? parameter)
        {

            if (Application.Current.MainWindow != null)
            {
                var window = Application.Current.MainWindow;

                if (window.WindowState == WindowState.Maximized && window.WindowStyle == WindowStyle.None)
                {
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    window.WindowState = WindowState.Normal;
                }
                else
                {
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                }
            }
        }
        public void OpenPlayerView(object? obj)
        {
            ConfigViewVisible = Visibility.Collapsed;
            PlayViewVisible = Visibility.Visible;

            PlayerViewModel.RestartGame();
        }

        public void OpenConfigView(object? obj)
        {
            PlayViewVisible = Visibility.Collapsed;
            ConfigViewVisible = Visibility.Visible;
        }
    }
}
