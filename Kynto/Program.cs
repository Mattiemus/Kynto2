namespace Kynto
{
    /// <summary>
    /// Application entry point class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">Application arguments</param>
        public static void Main(string[] args)
        {
            using (KyntoWindow window = new KyntoWindow())
            {
                window.Run();
            }
        }
    }
}
