using System;
namespace Izzy.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        public TestAttribute()
        {
        }
    }
}