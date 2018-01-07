namespace Spark.Scripting.Lua
{
    using System;

    using MoonSharp.Interpreter;

    public sealed class TestScript
    {
        public static void Test()
        {
            string script = @"
		        -- defines a factorial function
		        function fact (n)
			        if (n == 0) then
				        return 1
			        else
				        return n*fact(n - 1)
			        end
		        end

	            testing(fact)";

            var test = new Script(CoreModules.None);
            test.Globals["testing"] = (Func<Closure, int>)Testing;

            DynValue res = test.DoString(script);
        }

        public static int Testing(Closure func)
        {
            var test = func.Call(5);
            return 5;
        }
    }
}
