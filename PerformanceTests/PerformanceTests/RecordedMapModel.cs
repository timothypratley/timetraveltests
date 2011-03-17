using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Linq;

namespace PerformanceTests
{
    /// <summary>
    /// Objects stored in this Map need to be immutable to ensure they are not modified externally,
    /// so that all changes are recorded in the history.
    /// History is stored in an ordered map of version to state,
    /// where the state is a map of keys to values.
    /// </summary>
    public class RecordedMapModel<TKeyType, TValueType> : IEnumerable<TValueType>
    {
        private FSharpMap<int, FSharpMap<TKeyType, TValueType>> _history
            = new FSharpMap<int, FSharpMap<TKeyType, TValueType>>(
                Enumerable.Empty<Tuple<int,FSharpMap<TKeyType,TValueType>>>());
        private FSharpMap<TKeyType, TValueType> _currentMap
            = new FSharpMap<TKeyType,TValueType>(Enumerable.Empty<Tuple<TKeyType,TValueType>>());

        public int CurrentVersion { get; private set; }
        public RecordedMapModel()
        {
            CurrentVersion = -1;
            Clear();
        }

        public FSharpMap<TKeyType, TValueType> Map
        {
            get { return _currentMap; }
            set
            {
                _currentMap = value;
                _history = _history.Add(++CurrentVersion, value);
            }
        }

        public RecordedMapModel(IEnumerable<KeyValuePair<TKeyType, TValueType>> keyValuePairs) : this()
        {
            Contract.Requires<ArgumentNullException>(keyValuePairs != null);

            foreach (var e in keyValuePairs)
                Map = Map.Add(e.Key, e.Value);
        }

        public void Set(TKeyType key, TValueType value)
        {
            Map = Map.Add(key, value);
        }

        public void Remove(TKeyType key)
        {
            Map = Map.Remove(key);
        }

        public TValueType Get(TKeyType id)
        {
            var option = Map.TryFind(id);
            return option == FSharpOption<TValueType>.None ? default(TValueType) : option.Value;
        }

        public IEnumerable<TValueType> Values { get { return Map.Select(x=>x.Value); } }

        public int Count { get { return Map.Count; } }

        public void Clear()
        {
            Map = new FSharpMap<TKeyType, TValueType>(Enumerable.Empty<Tuple<TKeyType, TValueType>>());
        }

        public bool ContainsKey(TKeyType key)
        {
            return Map.ContainsKey(key);
        }

        #region IEnumerable<TValueType> Members

        public IEnumerator<TValueType> GetEnumerator()
        {
            return Map.Select(x=>x.Value).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Map.Select(x=>x.Value).GetEnumerator();
        }

        #endregion
    }
}
