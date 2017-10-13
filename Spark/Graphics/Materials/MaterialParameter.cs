namespace Spark.Graphics.Materials
{
    using System;
    using System.Collections.Generic;
    
    public static class MaterialParameter
    {
        private static Dictionary<String, IParameterBindingProvider> _providers;
        
        static MaterialParameter()
        {
            _providers = new Dictionary<String, IParameterBindingProvider>();

            ViewProjectionMatrix = new ViewProjectionMatrixParameterBindingProvider();
            Register(ViewProjectionMatrix);
        }
        
        public static IParameterBindingProvider ViewProjectionMatrix { get; private set; }
        
        public static bool Register(IParameterBindingProvider provider)
        {
            if (provider == null)
            {
                return false;
            }

            if (_providers.ContainsKey(provider.ParameterName))
            {
                return false;
            }

            _providers.Add(provider.ParameterName, provider);
            return true;
        }
    }
}
