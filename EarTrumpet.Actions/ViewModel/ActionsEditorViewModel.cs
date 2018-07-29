﻿using EarTrumpet.DataModel.Storage;
using EarTrumpet.UI.Helpers;
using EarTrumpet.UI.ViewModels;
using EarTrumpet_Actions.DataModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace EarTrumpet_Actions.ViewModel
{
    public class ActionsEditorViewModel : INotifyPropertyChanged, IWindowHostedViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning disable CS0067
        public event Action Close;
#pragma warning restore CS0067
        public event Action<object> HostDialog;

        public string Title => "Actions & hotkeys";

        public EarTrumpetActionViewModel SelectedAction
        {
            get => _selectedAction;
            set
            {
                if (value != _selectedAction)
                {
                    if (_selectedAction != null)
                    {
                        _selectedAction.IsExpanded = false;
                    }
                    _selectedAction = value;
                    if (_selectedAction != null)
                    {
                        _selectedAction.IsExpanded = true;
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAction)));
                }
            }
        }
        
        public ObservableCollection<EarTrumpetActionViewModel> Actions { get; }

        public ICommand New { get; }
        public ICommand RemoveItem { get; }
        public ICommand OpenItem { get; }


        private EarTrumpetActionViewModel _selectedAction;
        private ISettingsBag _settings = StorageFactory.GetSettings("Eartrumpet.Actions");
        private ICommand _openDialog;

        public ActionsEditorViewModel()
        {
            New = new RelayCommand(() =>
            {
                var vm = new EarTrumpetActionViewModel(this, new EarTrumpetAction { DisplayName = "New Action" });
                vm.Remove = RemoveItem;
                vm.Open = OpenItem;
                vm.OpenDialog = _openDialog;
                Actions.Add(vm);
                SelectedAction = vm;
            });

            RemoveItem = new RelayCommand(() =>
            {
                Actions.Remove(SelectedAction);
                SelectedAction = null;
            });

            OpenItem = new RelayCommand(() =>
            {
                _openDialog.Execute(SelectedAction);
            });

            _openDialog = new RelayCommand<object>((o) =>
            {
                HostDialog(o);
            });

            Actions = new ObservableCollection<EarTrumpetActionViewModel>(Addon.Current.Manager.Actions.Select(a => new EarTrumpetActionViewModel(this, a)));
            foreach (var action in Actions)
            {
                action.Remove = RemoveItem;
                action.Open = OpenItem;
                action.OpenDialog = new RelayCommand<object>((o) => _openDialog.Execute(o));
            }
        }

        public void OnClosing()
        {
            throw new NotImplementedException();
        }

        public void OnPreviewKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}