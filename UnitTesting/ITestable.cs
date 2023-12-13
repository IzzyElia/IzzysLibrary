using System.Linq;
using System.Collections.Generic;
using System;

namespace Izzy.UnitTesting
{
    public interface ITestable
	{
		TestResult[] RunTests();
	}
}