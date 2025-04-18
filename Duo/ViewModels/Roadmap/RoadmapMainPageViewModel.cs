﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Duo;
using Duo.Commands;
using Duo.Models;
using Duo.Models.Quizzes;
using Duo.Models.Sections;
using Duo.Services;
using Duo.ViewModels.Base;
using Duo.ViewModels.Roadmap;
using Duo.Views.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Duo.ViewModels.Roadmap
{
    public class RoadmapMainPageViewModel : ViewModelBase
    {
        private RoadmapService _roadmapService;
        private Duo.Models.Roadmap.Roadmap _roadmap;

        private UserService _userService;
        private User _user;

        private BaseQuiz _selectedQuiz;

        private ObservableCollection<RoadmapSectionViewModel> _sectionViewModels;
        public ObservableCollection<RoadmapSectionViewModel> SectionViewModels
        {
            get => _sectionViewModels;
            set => SetProperty(ref _sectionViewModels, value);
        }

        public ICommand OpenQuizPreviewCommand;
        public ICommand StartQuizCommand;


        public RoadmapMainPageViewModel()
        {
            _roadmapService = (RoadmapService)App.serviceProvider.GetService(typeof(RoadmapService));
            _userService = (UserService)App.serviceProvider.GetService(typeof(UserService));

            StartQuizCommand = new RelayCommand(StartQuiz);
            OpenQuizPreviewCommand = new RelayCommandWithParameter<Tuple<int, bool>>(OpenQuizPreview);
        }

        public async Task SetupViewModel()
        {
            _roadmap = await _roadmapService.GetRoadmapById(1);
            _user = await _userService.GetByIdAsync(1);

            SectionService sectionService = (SectionService)App.serviceProvider.GetService(typeof(SectionService));
            List<Section> sections = (List<Section>)await sectionService.GetByRoadmapId(1);

            _sectionViewModels = new ObservableCollection<RoadmapSectionViewModel>();
            //sections.ForEach(section =>
            //{
            //    var sectionViewModel = (RoadmapSectionViewModel)App.serviceProvider.GetService(typeof(RoadmapSectionViewModel));
            //    sectionViewModel.SetupForSection(section.Id);
            //    _sectionViewModels.Add(sectionViewModel);
            //});

            for (int i = 1; i <= sections.Count; i += 1)
            {
                var sectionViewModel = (RoadmapSectionViewModel)App.serviceProvider.GetService(typeof(RoadmapSectionViewModel));
                if (i <= _user.NumberOfCompletedSections)
                {
                    await sectionViewModel.SetupForSection(sections[i - 1].Id, true, 0);
                }
                else if (i == _user.NumberOfCompletedSections + 1)
                {
                    await sectionViewModel.SetupForSection(sections[i - 1].Id, false, _user.NumberOfCompletedQuizzesInSection);
                }
                else
                {
                    await sectionViewModel.SetupForSection(sections[i - 1].Id, false, -1);
                }
                _sectionViewModels.Add(sectionViewModel);
            }

            OnPropertyChanged(nameof(SectionViewModels));
        }

        private async void OpenQuizPreview(Tuple<int, bool> args)
        {
            Debug.WriteLine($"Opening quiz with ID: {args.Item1}");

            var quizPreviewViewModel = (RoadmapQuizPreviewViewModel)App.serviceProvider.GetService(typeof(RoadmapQuizPreviewViewModel));
            await quizPreviewViewModel.OpenForQuiz(args.Item1, args.Item2);
        }

        // MAYBE MAKE THIS OBSOLETE
        private void StartQuiz()
        {
            Console.WriteLine($"Starting quiz with ID: {_selectedQuiz?.Id}");
            // Navigation logic goes here
        }
    }
}