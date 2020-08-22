using CommandLine;
using mlops.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("set-datasource", HelpText = "Set data source")]
    public class SetDataSourceOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Option("datasource",Required =true)]
        public DataSource DataSource { get; set; }
    }
}
