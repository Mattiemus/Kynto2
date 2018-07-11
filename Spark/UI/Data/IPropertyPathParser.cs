namespace Spark.UI.Data
{
    using System.Collections.Generic;

    public interface IPropertyPathParser
    {
        IEnumerable<PropertyPathToken> Parse(object source, string path);
    }
}
