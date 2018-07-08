namespace Spark.UI
{
    using System;
    using System.Linq;

    public class Style
    {
        private SetterCollection _setters;
        private TriggerCollection _triggers;

        public Style()
        {
        }

        public Style(Type targetType)
        {
            TargetType = targetType;
        }

        public Style(Type targetType, Style basedOn)
        {
            TargetType = targetType;
            BasedOn = basedOn;

            if (basedOn != null)
            {
                foreach (Setter setter in basedOn.Setters)
                {
                    Setters.Add(setter);
                }

                foreach (Trigger trigger in basedOn.Triggers)
                {
                    Triggers.Add(trigger);
                }
            }
        }
        
        public Type TargetType { get; set; }

        public SetterCollection Setters
        {
            get
            {
                if (_setters == null)
                {
                    _setters = new SetterCollection();
                }

                return _setters;
            }
            internal set
            {
                _setters = value;
            }
        }

        public TriggerCollection Triggers
        {
            get
            {
                if (_triggers == null)
                {
                    _triggers = new TriggerCollection();
                }

                return _triggers;
            }
            internal set
            {
                _triggers = value;
            }
        }

        public Style BasedOn { get; internal set; }

        public bool OverridesDefaultStyle
        {
            get
            {
                Setter setter = Setters.FirstOrDefault(s => s.Property.Name == "OverridesDefaultStyle");
                if (setter != null)
                {
                    return (bool)setter.Value;
                }

                return false;
            }
        }
    }
}
