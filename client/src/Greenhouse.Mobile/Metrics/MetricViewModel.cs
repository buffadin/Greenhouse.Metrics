using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DIPS.Xamarin.UI.Extensions;

namespace Greenhouse.Mobile.Metrics
{
    public class MetricViewModel : INotifyPropertyChanged
    {
        private Metric _currentMetric;
        private ObservableCollection<Metric> _previousMetrics = new ObservableCollection<Metric>();
        public string Name { get; }
        public MetricType MetricType { get; }
        public string IconName { get; }

        public MetricViewModel(string name, MetricType metricType, string iconName)
        {
            Name = name;
            MetricType = metricType;
            IconName = iconName;
        }

        public Metric CurrentMetric
        {
            get => _currentMetric;
            set
            {
                PropertyChanged.RaiseWhenSet(ref _currentMetric, value);
                var previousMetrics = PreviousMetrics;
                if (_currentMetric != null)
                {
                    previousMetrics.Add(_currentMetric);
                    if (previousMetrics.Count > 20)
                    {
                        previousMetrics.Remove(PreviousMetrics.First());
                    }
                }

                PreviousMetrics = new ObservableCollection<Metric>(previousMetrics.OrderByDescending(m => m.Timestamp));
            }
        }

        public ObservableCollection<Metric> PreviousMetrics
        {
            get => _previousMetrics;
            set => PropertyChanged.RaiseWhenSet(ref _previousMetrics, value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }
}