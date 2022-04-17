using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using EnsureThat.Enforcers;

namespace peter_ficsit_apitool
{
    public static class StringArgExtensions
    {
        public static string IsFilePath(this StringArg _, string value, string paramName = null)
        {
            if (File.Exists(value))
            {
                return value;
            }
            else
            {
                throw Ensure.ExceptionFactory.ArgumentException("File doesn't exist! " + value, paramName);
            }

            
        }
    }

}
