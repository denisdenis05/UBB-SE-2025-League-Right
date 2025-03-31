﻿using Azure;
using Duo.Models.Exercises;
using Duo.Models.Quizzes;
using Duo.Services;
using Duo.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Duo.ViewModels
{
    public class QuizExamViewModel : ViewModelBase
    {
        private readonly ExerciseService _exerciseService;
        private readonly QuizService _quizService;
        private int _quizId;
        private int _examId;
        private Quiz _currentQuiz;
        private Exam _currentExam;
        private List<Exercise> _exercises;
        private Exercise _currentExercise;
        private int _currentExerciseIndex;
        private bool? _validatedCurrent = null;

        public int QuizId
        {
            get => _quizId;
            set
            {
                _quizId = value;
                _examId = -1;

                LoadQuiz();
                OnPropertyChanged(nameof(QuizId));
                LoadExercises();
            }
        }

        public int ExamId
        {
            get => _examId;
            set
            {
                _examId = value;
                _quizId = -1;

                LoadExam();
                OnPropertyChanged(nameof(ExamId));
                LoadExercises();
            }
        }

        public List<Exercise> Exercises
        {
            get => _exercises;
            private set
            {
                _exercises = value;
                OnPropertyChanged(nameof(Exercises));
            }
        }

        public Exercise CurrentExercise
        {
            get => _currentExercise;
            set
            {
                _currentExercise = value;
                OnPropertyChanged(nameof(CurrentExercise));
            }
        }

        public Quiz CurrentQuiz
        {
            get => _currentQuiz;
            set
            {
                _currentQuiz = value;
                OnPropertyChanged(nameof(CurrentQuiz));
            }
        }

        public Exam CurrentExam
        {
            get => _currentExam;
            set
            {
                _currentExam = value;
                OnPropertyChanged(nameof(CurrentExam));
            }
        }

        public int CurrentExerciseIndex
        {
            get => _currentExerciseIndex;
            set
            {
                _currentExerciseIndex = value;
                OnPropertyChanged(nameof(CurrentExerciseIndex));
            }
        }

        public bool? ValidatedCurrent
        {
            get => _validatedCurrent;
            set
            {
                _validatedCurrent = value;
                OnPropertyChanged(nameof(ValidatedCurrent));
            }
        }

        public QuizExamViewModel(ExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public QuizExamViewModel()
        {
            try
            {
                _exerciseService = (ExerciseService)App.serviceProvider.GetService(typeof(ExerciseService));
                _quizService = (QuizService)App.serviceProvider.GetService(typeof(QuizService));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task LoadQuiz()
        {
            try
            {
                CurrentQuiz = await _quizService.GetQuizById(_quizId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Quiz: {ex.Message}");
            }
        }

        public async Task LoadExam()
        {
            try
            {
                CurrentExam = await _quizService.GetExamById(_quizId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Quiz: {ex.Message}");
            }
        }

        public async Task LoadExercises()
        {
            try
            {
                if (QuizId != -1)
                {
                    Exercises = await _exerciseService.GetAllExercisesFromQuiz(QuizId);
                }
                else
                {
                    Exercises = await _exerciseService.GetAllExercisesFromExam(ExamId);
                }

                CurrentExerciseIndex = 0;
                CurrentExercise = Exercises[CurrentExerciseIndex];
                ValidatedCurrent = null;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading exercises: {ex.Message}");
                Exercises = new List<Exercise>();
            }
        }

        private void UpdateQuiz(bool? isExerciseValid)
        {
            if (isExerciseValid == true)
            {
                CurrentQuiz.IncrementCorrectAnswers();
                CurrentQuiz.IncrementNumberOfAnswersGiven();
            }
            else
                CurrentQuiz.IncrementNumberOfAnswersGiven();
        }

        private void UpdateExam(bool? isExerciseValid)
        {
            if (isExerciseValid == true)
            {
                CurrentExam.IncrementCorrectAnswers();
                CurrentExam.IncrementNumberOfAnswersGiven();
            }
            else
                CurrentExam.IncrementNumberOfAnswersGiven();
        }

        public bool? ValidateCurrentExercise(object responses)
        {
            if (ValidatedCurrent is not null)
                return ValidatedCurrent;

            bool isValid = false;

            if (CurrentExercise is AssociationExercise associationExercise)
            {
                isValid = associationExercise.ValidateAnswer((List<(string, string)>)responses);
            }
            else if (CurrentExercise is FillInTheBlankExercise fillInTheBlanksExercise)
            {
                isValid = fillInTheBlanksExercise.ValidateAnswer((List<string>)responses);
            }
            else if (CurrentExercise is MultipleChoiceExercise multipleChoiceExercise)
            {
                isValid = multipleChoiceExercise.ValidateAnswer((List<string>)responses);
            }
            ValidatedCurrent = isValid;
            if (QuizId != -1)
                UpdateQuiz(ValidatedCurrent);
            else
                UpdateExam(ValidatedCurrent);

            return isValid;
        }

        public bool LoadNext()
        {
            if (ValidatedCurrent == null)
                return false;

            CurrentExerciseIndex += 1;
            if (Exercises.Count <= CurrentExerciseIndex)
                CurrentExercise = null;
            else
                CurrentExercise = Exercises[CurrentExerciseIndex];

            return true;
        }

        public float GetPercentageDone()
        {
            return CurrentQuiz.ExerciseList.Count / CurrentQuiz.GetNumberOfAnswersGiven();
        }

        public float GetPercentageCorrect()
        {
            return CurrentQuiz.ExerciseList.Count / CurrentQuiz.GetNumberOfCorrectAnswers();
        }

        public string GetCurrentExerciseCorrectAnswer()
        {
            string correctAnswers = "";

            if (CurrentExercise is AssociationExercise associationExercise)
            {
                for (int i = 0; i < associationExercise.FirstAnswersList.Count; i++) {
                    correctAnswers += associationExercise.FirstAnswersList[i] + " - " + associationExercise.SecondAnswersList[i] + "\n";
                 }
            }
            else if (CurrentExercise is FillInTheBlankExercise fillInTheBlanksExercise)
            {
                foreach(var answer in fillInTheBlanksExercise.PossibleCorrectAnswers)
                    correctAnswers += answer + "\n";

            }
            else if (CurrentExercise is MultipleChoiceExercise multipleChoiceExercise)
            {
                correctAnswers += multipleChoiceExercise.Choices.Select(choice => choice.IsCorrect == true).First();
            }

            return correctAnswers;
        }
    }
}
