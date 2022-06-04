using System;
using System.Data;
using System.Linq;
using FS.Core.LinkTrack;
using FS.Data.Abstract;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FS.Data.Attribute;

/// <summary>
/// 存储过程执行链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LinkTrackProcParamAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var callMethod = GetCallMethodValue(args: args);
        
        // 通过入参，获取ISqlParam
        var arg = args.Arguments.FirstOrDefault(o => o is IProcParam);
        if (arg == null) throw new FarseerException($"调用ProcParam链路追踪，需要入参含有IProcParam类型");
        var procParam = (IProcParam)arg;
        
        using (FsLinkTrack.TrackDatabase(callMethod, procParam.SetMap.DbName, procParam.SetMap.TableName, CommandType.StoredProcedure, null, procParam.Param))
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