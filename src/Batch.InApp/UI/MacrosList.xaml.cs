﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xarial.CadPlus.Batch.InApp.ViewModels;

namespace Xarial.CadPlus.Batch.InApp.UI
{
    public partial class MacrosList : UserControl
    {
        public MacrosList()
        {
            InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as MacrosVM;

            if (oldVm != null) 
            {
                oldVm.AddMacros -= OnAddMacros;
            }

            var newVm = e.NewValue as MacrosVM;

            if (newVm != null)
            {
                newVm.AddMacros += OnAddMacros;
            }
        }

        private void OnAddMacros() => lstMacros.AddFilesCommand.Execute(null);
    }
}
