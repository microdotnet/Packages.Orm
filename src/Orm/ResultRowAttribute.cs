using System;

namespace MicroDotNet.Packages.Orm
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ResultRowAttribute : Attribute
    {
    }
}