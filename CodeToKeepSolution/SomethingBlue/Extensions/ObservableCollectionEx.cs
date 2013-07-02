using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SomethingBlue.Extensions
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        bool _suspendCollectionChangeNotification;

        public ObservableCollectionEx(List<T> list)
            : base(list)
        {
            _suspendCollectionChangeNotification = false;
        }

        public ObservableCollectionEx()
        {
            _suspendCollectionChangeNotification = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suspendCollectionChangeNotification == false)
                base.OnCollectionChanged(e);
        }

        public void SuspendCollectionChangeNotification()
        {
            _suspendCollectionChangeNotification = true;
        }

        public void ResumeCollectionChangeNotification()
        {
            _suspendCollectionChangeNotification = false;
        }

        public void AddRange(IEnumerable<T> items)
        {
            SuspendCollectionChangeNotification();
            try
            {
                foreach (var i in items)
                    base.InsertItem(Count, i);
            }
            finally
            {
                ResumeCollectionChangeNotification();
                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                OnCollectionChanged(arg);
            }
        }

        public void RemoveAll(Func<T, bool> condition)
        {
            for (int i = Count - 1; i >= 0; i--)
                if (condition(this[i]))
                    RemoveAt(i);
        }

    }
}