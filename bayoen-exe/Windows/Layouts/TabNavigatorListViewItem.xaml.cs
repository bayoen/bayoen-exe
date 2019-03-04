using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace bayoen.Windows.Layouts
{
    /// <summary>
    /// TabNavigatorListItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TabNavigatorListViewItem : ListViewItem, INotifyPropertyChanged
    {

        private string _header;
        public string Header
        {
            get => this._header;
            set
            {
                if (this._header == value) return;

                this._header = value;
                this.OnPropertyChanged("Header");
            }
        }

        private string _iconCode;
        public string IconCode
        {
            get => this._iconCode;
            set
            {
                if (this._iconCode == value) return;

                this._iconCode = value;
                this.IconVisual = TryFindResource(value) as Visual;
                this.OnPropertyChanged("IconVisual");
            }
        }
        public Visual IconVisual { get; private set; }

        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;

        public TabNavigatorListViewItem()
        {
            InitializeComponent();
        }
    }
}
