using L046_Labb3_Code_Along.Command;
using L046_Labb3_Code_Along.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace L046_Labb3_Code_Along.ViewModels
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public DelegateCommand SubmitAnswerCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        
        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {

            this._mainWindowViewModel = mainWindowViewModel;
            SubmitAnswerCommand = new DelegateCommand(SubmitAnswer);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private int _questionNumber = 0;
        public int QuestionNumber
        {
            get => _questionNumber;
            set
            {
                _questionNumber = value;
                RaisePropertyChanged();

            }
        }

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                RaisePropertyChanged();

            }
        }

        public int _playerPoints = 0;

        public int PlayerPoints
        {
            get => _playerPoints;
            set
            {
                if (_playerPoints != value)
                {
                    _playerPoints = value;
                    RaisePropertyChanged(nameof(PlayerPoints));
                }
            }
        }

        private DispatcherTimer _timer;

        private int _timeRemaining;
        public int TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining != value)
                {
                    _timeRemaining = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void RestartTimer()
        {

            _timer?.Stop();

            if (ActivePack != null)
                TimeRemaining = ActivePack.TimeLimitInSeconds;
            else
                TimeRemaining = 10;

            _timer?.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining--;
            }
            else
            {
                _timer.Stop();
                System.Windows.Forms.MessageBox.Show("Time’s up!");
                ProgressToNextQuestion();
            }
        }

        public void RestartGame()
        {
            if (ActivePack == null || ActivePack.Questions == null || ActivePack.Questions.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Please add at least 1 Question pack with at least 1 question");
                return;
            }

            PlayerPoints = 0;
            QuestionNumber = 1;

            CurrentQuestion = ActivePack.Questions[0];
            RestartTimer();
        }

        public void StartGame()
        {

            if (ActivePack == null)
            {
                System.Windows.Forms.MessageBox.Show("Please add atleast 1 Question pack with atleast 1 question");
                return;
            }
            else
            {
                ProgressToNextQuestion();
            }
        }


        private void ProgressToNextQuestion()
        {
            if (QuestionNumber <= ActivePack.Questions.Count - 1)
            {
                QuestionNumber++;
                CurrentQuestion = ActivePack.Questions[QuestionNumber - 1];
            }
            RestartTimer();
        }

        private void SubmitAnswer(object? answer)
        {
           
            var questions = ActivePack.Questions;

            if (answer == CurrentQuestion.CorrectAnswer)
            {
                PlayerPoints++;
                System.Windows.Forms.MessageBox.Show($"Correct!");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Incorrect");
            }


            if (QuestionNumber == ActivePack.Questions.Count)
            {
                System.Windows.Forms.MessageBox.Show($"You got {PlayerPoints.ToString()} Questions Correct! ");


                var result = System.Windows.Forms.MessageBox.Show("Would you like to restart the game?",
                                             "Game Over",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    RestartTimer();
                    RestartGame();
                }

                return;

            }

            ProgressToNextQuestion();
        }
    }
}
