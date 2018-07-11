namespace Spark.UI
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    // TODO: [TypeConverterAttribute(typeof(TemplateKeyConverter))]
    public abstract class TemplateKey : ResourceKey, ISupportInitialize
    {
        protected TemplateKey(TemplateType templateType)
        {
        }

        protected TemplateKey(TemplateType templateType, object dataType)
        {
            DataType = dataType;
        }
        
        public override Assembly Assembly
        {
            get { throw new NotImplementedException(); }
        }

        public object DataType { get; set; }

        public override bool Equals(object o)
        {
            TemplateKey other = o as TemplateKey;
            if (other != null)
            {
                return other.GetType() == GetType() && other.DataType == DataType;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ ((DataType != null) ? DataType.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return DataType.ToString();
        }

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        protected enum TemplateType
        {
            DataTemplate,
            TableTemplate,
        }
    }
}
