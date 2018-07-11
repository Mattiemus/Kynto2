namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Markup;
    using System.Xaml;

    using Controls;

    [XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
    public class Setter : SetterBase, ISupportInitialize
    {
        private readonly Dictionary<FrameworkElement, object> _oldValues;
        private string _propertyName;
        private IServiceProvider _serviceProvider;

        public Setter()
        {
            _oldValues = new Dictionary<FrameworkElement, object>();
        }

        public Setter(DependencyProperty property, object value)
        {
            _oldValues = new Dictionary<FrameworkElement, object>();

            Property = property;
            Value = value;
        }

        public Setter(DependencyProperty property, object value, string targetName)
        {
            _oldValues = new Dictionary<FrameworkElement, object>();

            Property = property;
            Value = value;
            TargetName = targetName;
        }

        public DependencyProperty Property { get; set; }

        public string TargetName { get; set; }

        public object Value { get; set; }

        public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
        {
            // The DependencyProperty refered to by Property may depend on the value of TargetName,
            // but we don't know that yet, so defer loading Property until EndInit().
            if (eventArgs.Member.Name == "Property")
            {
                Setter setter = (Setter)targetObject;
                setter._propertyName = (string)eventArgs.Value;
                setter._serviceProvider = eventArgs.ServiceProvider;
                eventArgs.Handled = true;
            }
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (_propertyName == null)
            {
                return;
            }

            if (TargetName == null)
            {
                Property = DependencyPropertyConverter.Resolve(_serviceProvider, _propertyName);
            }
            else
            {
                // TargetName is specified so we need to look in the containing template for the named element
                IAmbientProvider ambient = (IAmbientProvider)_serviceProvider.GetService(typeof(IAmbientProvider));
                IXamlSchemaContextProvider schema = (IXamlSchemaContextProvider)_serviceProvider.GetService(typeof(IXamlSchemaContextProvider));

                // Look up the FrameworkTemplate.Template property in the xaml schema.
                XamlType frameworkTemplateType = schema.SchemaContext.GetXamlType(typeof(FrameworkTemplate));
                XamlMember templateProperty = frameworkTemplateType.GetMember("Template");

                // Get the value of the first ambient FrameworkTemplate.Template property.
                TemplateContent templateContent = (TemplateContent)ambient.GetFirstAmbientValue(new[] { frameworkTemplateType }, templateProperty).Value;

                // Look in the template for the type of TargetName.
                Type targetType = templateContent.GetTypeForName(TargetName);

                // Finally, find the dependency property on the type.
                Property = DependencyObject.GetPropertyFromName(targetType, _propertyName);
            }
        }

        internal override void Attach(FrameworkElement frameworkElement)
        {
            object oldValue = DependencyProperty.UnsetValue;

            if (TargetName != null)
            {
                frameworkElement = (FrameworkElement)frameworkElement.FindName(TargetName);
            }

            if (!frameworkElement.IsUnset(Property))
            {
                oldValue = frameworkElement.GetValue(Property);
            }

            _oldValues.Add(frameworkElement, oldValue);

            frameworkElement.SetValue(Property, ConvertValue(Value));
        }

        internal override void Detach(FrameworkElement frameworkElement)
        {
            if (TargetName != null)
            {
                frameworkElement = (FrameworkElement)frameworkElement.FindName(TargetName);
            }

            frameworkElement.SetValue(Property, _oldValues[frameworkElement]);
            _oldValues.Remove(frameworkElement);
        }

        private object ConvertValue(object value)
        {
            if (value.GetType() == Property.PropertyType)
            {
                return value;
            }

            if (value is string && Property.PropertyType.IsEnum)
            {
                return Enum.Parse(Property.PropertyType, (string)value);
            }

            TypeConverterAttribute attribute =
                Property.PropertyType.GetCustomAttributes(typeof(TypeConverterAttribute), true)
                    .Cast<TypeConverterAttribute>()
                    .FirstOrDefault();

            if (attribute != null)
            {
                Type converterType = Type.GetType(attribute.ConverterTypeName);
                TypeConverter converter = (TypeConverter)Activator.CreateInstance(converterType);

                if (converter.CanConvertFrom(value.GetType()))
                {
                    return converter.ConvertFrom(value);
                }
            }
            else
            {
                return Convert.ChangeType(value, Property.PropertyType);
            }

            throw new NotSupportedException($"Could not convert the value '{value}' to '{Property.PropertyType.Name}");
        }
    }
}
