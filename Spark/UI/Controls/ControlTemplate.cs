namespace Spark.UI.Controls
{
    using System;
    using System.Windows.Markup;

    public class ControlTemplate : FrameworkTemplate
    {
        public ControlTemplate()
        {
            Triggers = new TriggerCollection();
        }

        [Ambient]
        public Type TargetType { get; set; }

        public TriggerCollection Triggers { get; }

        internal override FrameworkElement CreateVisualTree(DependencyObject parent)
        {
            FrameworkElement result = base.CreateVisualTree(parent);
            foreach (TriggerBase trigger in Triggers)
            {
                trigger.Attach(result, parent);
            }

            return result;
        }
    }
}
