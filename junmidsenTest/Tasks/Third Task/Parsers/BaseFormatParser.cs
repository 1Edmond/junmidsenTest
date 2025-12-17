using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junmidsenTest.Tasks.Third_Task.Parsers;

public abstract class BaseFormatParser
{
    public string NormalizeLogLevel(string level)
    {
        return level.ToUpper() switch
        {
            "INFORMATION" => "INFO",
            "WARNING" => "WARN",
            "ERROR" => "ERROR",
            "DEBUG" => "DEBUG",
            "INFO" => "INFO",
            "WARN" => "WARN",
            _ => level.ToUpper()
        };
    }
}