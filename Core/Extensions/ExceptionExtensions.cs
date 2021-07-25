using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Core.Util;

namespace bEmu.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetValidatedExceptionString(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            var details = ex.Message + Environment.NewLine + ex.StackTrace;

            if (details.Length >= 512) 
                details = details.Substring(0, 512);

            return details;
        }
    }
}