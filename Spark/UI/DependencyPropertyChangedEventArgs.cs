namespace Spark.UI
{
    using System;

    public delegate void DependencyPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs e);

    public struct DependencyPropertyChangedEventArgs
    {
        public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object NewValue { get; }

        public object OldValue { get; }

        public DependencyProperty Property { get; }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (obj is DependencyPropertyChangedEventArgs args)
            {
                return Equals(args);
            }

            return false;
        }

        public bool Equals(DependencyPropertyChangedEventArgs args)
        {
            return Property == args.Property &&
                   NewValue == args.NewValue &&
                   OldValue == args.OldValue;
        }

        public static bool operator !=(DependencyPropertyChangedEventArgs left, DependencyPropertyChangedEventArgs right)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(DependencyPropertyChangedEventArgs left, DependencyPropertyChangedEventArgs right)
        {
            throw new NotImplementedException();
        }
    }
}
