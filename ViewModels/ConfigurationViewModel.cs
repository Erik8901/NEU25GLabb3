using L046_Labb3_Code_Along.Command;
using L046_Labb3_Code_Along.Models;
using L046_Labb3_Code_Along.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace L046_Labb3_Code_Along.ViewModels;

class ConfigurationViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;

    public DelegateCommand AddQuestionCommand { get; }

    public DelegateCommand RemoveQuestionCommand { get; }

    public DelegateCommand OpenPackOptionsCommand { get; }
    public DelegateCommand SetSelecetedQuestionPackAsActiveCommand { get; }
    public DelegateCommand RemoveSelectedQuestionPackCommand { get; }
    public DelegateCommand CreateNewQuestionPackCommand {  get; }

    private Visibility _visibilityPackOptions = Visibility.Collapsed;

    public ICommand OpenPackOptionsPopUpCommand { get; }

    private Question _activeQuestion;

    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

    public Question ActiveQuestion
    {
        get => _activeQuestion;
        set
        {
            _activeQuestion = value;
            RaisePropertyChanged();
        }
    }

    private int _timeLimit = 10;
    private QuestionPackViewModel NewPack;

    public int TimeLimit
    {
        get => _timeLimit;
        set
        {
            {
                _timeLimit = ActivePack.TimeLimitInSeconds;
                RaisePropertyChanged();
            }
        }
    }

    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        this._mainWindowViewModel = mainWindowViewModel;
        AddQuestionCommand = new DelegateCommand(AddQuestion);
        RemoveQuestionCommand = new DelegateCommand(RemoveQuestion);
        OpenPackOptionsCommand = new DelegateCommand(OpenDialogButton_Click);
        OpenPackOptionsPopUpCommand = new DelegateCommand(OpenPackOptions);
        CreateNewQuestionPackCommand = new DelegateCommand(CreateNewQuestionPack);
        SetSelecetedQuestionPackAsActiveCommand = new DelegateCommand(SetSelecetedQuestionPackAsActive);
        RemoveSelectedQuestionPackCommand = new DelegateCommand(RemoveSelecetedQuestionPackAsActive);
        _mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        
    }
    private void RemoveSelecetedQuestionPackAsActive(object? obj)
    {
        var questionToRemove = ActivePack;
        _mainWindowViewModel.Packs.Remove(questionToRemove);
        SaveQuestions("questions.json");
        _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs?.FirstOrDefault();
    }

    private void SetSelecetedQuestionPackAsActive(object? obj)
    {
        _mainWindowViewModel.ActivePack = (QuestionPackViewModel)obj;
    }


    public Visibility VisibilityPackOptions
    {
        get => _visibilityPackOptions;
        set
        {
            _visibilityPackOptions = value;
            RaisePropertyChanged(nameof(VisibilityPackOptions));
        }
    }

    private void CreateNewQuestionPack(object? obj)
    {
        var pack = new QuestionPack("New Default Pack");
        _mainWindowViewModel.ActivePack = new QuestionPackViewModel(pack);
        _mainWindowViewModel.Packs.Add(ActivePack);
    }

    private void OpenPackOptions(object? obj)
    {
        var window = new PackOptionsPopUp
        {
            DataContext = this
        };
        window.ShowDialog();
    }

    public void OpenDialogButton_Click(object? obj)
    {
        VisibilityPackOptions = Visibility.Visible;
        MessageBox.Show(VisibilityPackOptions.ToString());
    }
    private void RemoveQuestion(object? obj)
    {
        var questionToRemove = ActivePack.Questions.FirstOrDefault(q =>
             q.Query == ActiveQuestion.Query &&
             q.CorrectAnswer == ActiveQuestion.CorrectAnswer &&
             q.IncorrectAnswers[0] == ActiveQuestion.IncorrectAnswers[0] &&
             q.IncorrectAnswers[1] == ActiveQuestion.IncorrectAnswers[1] &&
             q.IncorrectAnswers[2] == ActiveQuestion.IncorrectAnswers[2]
            );

        if (questionToRemove != null)
        {
            ActivePack.Questions.Remove(questionToRemove);
            SaveQuestions("questions.json");
        }

    }

    private void AddQuestion(object? obj)
    {
      ActivePack.Questions.Add(new Question($"New Question", "CorrectAnswer", "InCorrectAnswer1", "InCorrectAnswer2", "InCorrectAnswer3"));
    }



    public void SaveQuestions(string filePath)
    {
        if (ActivePack == null) return;

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_mainWindowViewModel.Packs, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving questions: {ex.Message}");
        }
    }
}

