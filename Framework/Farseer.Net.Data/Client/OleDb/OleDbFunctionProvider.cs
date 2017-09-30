using Farseer.Net.Data.Infrastructure;

namespace Farseer.Net.Data.Client.OleDb
{
    public class OleDbFunctionProvider : AbsFunctionProvider
    {
        public override string CharIndex(string fieldName, string paramName, bool isNot) => $"INSTR({fieldName},{paramName}) {(isNot ? "<=" : ">")} 0";
    }
}