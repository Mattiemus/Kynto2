namespace Spark.UI
{
    using System;
    using System.Collections.Generic;

    public class DependencyObjectType
    {
        private static Dictionary<Type, DependencyObjectType> _typeMap = new Dictionary<Type, DependencyObjectType>();
        private static int _currentId;

        private DependencyObjectType(int id, Type systemType)
        {
            Id = id;
            SystemType = systemType;
        }

        public DependencyObjectType BaseType
        {
            get { return FromSystemType(SystemType.BaseType); }
        }

        public int Id { get; }

        public string Name => SystemType.Name;

        public Type SystemType { get; }

        public static DependencyObjectType FromSystemType(Type systemType)
        {
            if (_typeMap.ContainsKey(systemType))
            {
                return _typeMap[systemType];
            }

            DependencyObjectType dot = new DependencyObjectType(_currentId++, systemType);
            _typeMap[systemType] = dot;

            return dot;
        }

        public bool IsInstanceOfType(DependencyObject dependencyObject)
        {
            return SystemType.IsInstanceOfType(dependencyObject);
        }

        public bool IsSubclassOf(DependencyObjectType dependencyObjectType)
        {
            return SystemType.IsSubclassOf(dependencyObjectType.SystemType);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
