namespace Data.Models.ArbitrarySql
{
    public class SqlParameter
    {
        required public string Name { get; init; }
        required public string Value { get; init; }
        public SqlParameterType Type { get; set; }
    }
}
