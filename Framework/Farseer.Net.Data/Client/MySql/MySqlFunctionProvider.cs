namespace FS.Data.Client.MySql
{
    public class MySqlFunctionProvider : AbsFunctionProvider
    {
        public override string CharIndex(string  fieldName, string paramName, bool isNot) => $"POSITION({paramName} IN {fieldName}) {(isNot ? "<=" : ">")} 0";
        public override string StartsWith(string fieldName, string paramName, bool isNot) => $"POSITION({paramName} IN {fieldName}) {(isNot ? ">" : "=")} 1";
    }
}