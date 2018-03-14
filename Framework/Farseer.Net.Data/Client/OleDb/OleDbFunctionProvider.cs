using FS.Data.Infrastructure;

namespace FS.Data.Client.OleDb
{
    public class OleDbFunctionProvider : AbsFunctionProvider
    {
        public override string CharIndex(string fieldName, string paramName, bool isNot) => $"INSTR({fieldName},{paramName}) {(isNot ? "<=" : ">")} 0";
    }
}