using System;
using System.Data;
using System.Linq;
using FS.Cache;
using FS.Core.Abstract.Data;
using FS.Core.LinkTrack;
using FS.Data.Abstract;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FS.Data.Attribute;

/// <summary>
/// Sql执行链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LinkTrackSqlParamAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var callMethod = GetCallMethodValue(args: args);

        // 通过入参，获取ISqlParam
        var arg = args.Arguments.FirstOrDefault(o => o is ISqlParam);
        if (arg == null) throw new FarseerException($"调用SqlParam链路追踪，需要入参含有ISqlParam类型");
        var sqlParam = (ISqlParam)arg;

        using (FsLinkTrack.TrackDatabase(method: callMethod, sqlParam.SetMap.DbName, sqlParam.SetMap.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param))
        {
            args.Proceed();
        }
    }
    
    private string GetCallMethodValue(MethodInterceptionArgs args)
    {
        string callMethod     = null;
        var    parameterInfos = args.Method.GetParameters();
        for (int i = 0; i < parameterInfos.Length; i++)
        {
            if (parameterInfos[i].ParameterType == typeof(string) && parameterInfos[i].Name == "callMethod")
            {
                callMethod = args.Arguments[i].ToString();
            }
        }
        return callMethod;
    }
}