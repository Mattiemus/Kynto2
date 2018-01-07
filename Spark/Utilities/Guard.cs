namespace Spark.Utilities
{
    using System;
    
    public static class Guard
    {
        static Guard()
        {
            Against = new GuardInstance();
        }

        public static IGuardInstance Against { get; }

        private class GuardInstance : IGuardInstance
        {
            public void NullArgument(object value, string argumentName)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(argumentName);
                }
            }

            public void NullOrEmptyArgument(string value, string argumentName)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(argumentName);
                }
            }
        }
    }
}
