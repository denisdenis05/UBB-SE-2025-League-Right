﻿using Duo.Commands;
using Duo.Models;
using Duo.Models.Exercises;
using Duo.ViewModels.Base;
using Duo.ViewModels.ExerciseViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Duo.ViewModels.CreateExerciseViewModels
{
    class CreateFillInTheBlankExerciseViewModel : CreateExerciseViewModelBase
    {

        public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>();

        public CreateFillInTheBlankExerciseViewModel() 
        {

            Answers.Add(new Answer(""));
            AddNewAnswerCommand = new RelayCommand(AddNewAnswer);
        }

        public ICommand AddNewAnswerCommand { get; }

        public override Exercise CreateExercise(string question, Difficulty difficulty)
        {
            Debug.WriteLine("Fill in the blank!");
            Exercise newExercise = new Models.Exercises.FillInTheBlankExercise(0, question, difficulty, generateAnswerList(Answers));
            return newExercise;
        }

        public List<string> generateAnswerList(ObservableCollection<Answer> answers)
        {
            List<string> answerList = new List<string>();
            foreach (Answer answer in answers)
            {
                answerList.Add(answer.Value);
            }
            return answerList;
        }

        public void AddNewAnswer()
        {
            Answers.Add(new Answer(""));
        }

        public class Answer : ViewModelBase
        {
            private string _value;
            public string Value
            {
                get => _value;
                set
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }

            public Answer(string value)
            {
                _value = value;
            }
        }
    }
}
