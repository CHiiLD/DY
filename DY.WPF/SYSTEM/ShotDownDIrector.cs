using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF.SYSTEM
{
    public class ShotDownDirector : IDisposable
    {
        private static ShotDownDirector THIS;
        List<IDisposable> IDisposables;

        private ShotDownDirector()
        {
            IDisposables = new List<IDisposable>();
        }

        ~ShotDownDirector()
        {
            Dispose();
        }

        public static ShotDownDirector GetInstance()
        {
            if (THIS == null)
                THIS = new ShotDownDirector();
            return THIS;
        }

        public bool AddIDisposable(IDisposable item)
        {
            IEnumerable<IDisposable> find_ret =
                from Disposable in IDisposables
                where Disposable.GetHashCode() == item.GetHashCode()
                select Disposable;
            bool hasItem = find_ret.Count() != 0;
            if (!hasItem)
                IDisposables.Add(item);
            return !hasItem;
        }

        public bool RemoveIDisposable(IDisposable item)
        {
            IEnumerable<IDisposable> find_ret =
                from Disposable in IDisposables
                where Disposable.GetHashCode() == item.GetHashCode()
                select Disposable;
            bool hasItem = find_ret.Count() != 0;
            if (hasItem)
                IDisposables.Remove(find_ret.ElementAt(0));
            return hasItem;
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in IDisposables)
                disposable.Dispose();
            IDisposables.Clear();
            THIS = null;
            GC.SuppressFinalize(this);
        }
    }
}
