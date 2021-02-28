﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xarial.XCad.Documents;
using Xarial.XToolkit.Wpf.Extensions;

namespace Xarial.CadPlus.Batch.Extensions.ViewModels
{
    public class DocumentVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public IXDocument Document { get; }

        private bool m_IsChecked;

        public bool IsChecked 
        {
            get => m_IsChecked;
            set 
            {
                m_IsChecked = value;
                this.NotifyChanged();
            }
        }

        public DocumentVM(IXDocument doc) 
        {
            Document = doc;
            IsChecked = true;
        }
    }

    public class ReferenceVM : DocumentVM
    {
        public ObservableCollection<DocumentVM> Drawings { get; }

        private object m_Lock = new object();

        public ReferenceVM(IXDocument doc)  : base(doc)
        {
            Drawings = new ObservableCollection<DocumentVM>();
            BindingOperations.EnableCollectionSynchronization(Drawings, m_Lock);
        }
    }
}
