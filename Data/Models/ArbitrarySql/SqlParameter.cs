﻿namespace Data.Models.ArbitrarySql
{
    public class SqlParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public SqlParameterType Type { get; set; }
    }
}