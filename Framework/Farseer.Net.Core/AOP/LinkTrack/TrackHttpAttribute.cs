using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.Http;
using FS.Core.LinkTrack;
using FS.Extends;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// HTTP埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
public class TrackHttpAttribute : MethodInterceptionAspect
{
    private string _method;
    private string _urlParamName;
    private string _headerDataParamName;
    private string _requestBodyParamName;

    /// <summary>
    /// HTTP埋点链路追踪
    /// </summary>
    /// <param name="method">GET or POST or PUT or DELETE</param>
    /// <param name="urlParamName">方法入参中Url参数名称</param>
    /// <param name="headerDataParamName">方法入参中Header参数名称</param>
    /// <param name="requestBodyParamName">方法入参中requestBody参数名称</param>
    public TrackHttpAttribute(string method, string urlParamName, string headerDataParamName, string requestBodyParamName)
    {
        this._method               = method;
        this._urlParamName         = urlParamName;
        this._headerDataParamName  = headerDataParamName;
        this._requestBodyParamName = requestBodyParamName;
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var url         = args.GetParamValue<string>(_urlParamName);
        var headerData  = args.GetParamValue<IDictionary<string, string>>(_headerDataParamName);
        var requestBody = args.GetParamValue<string>(_requestBodyParamName);

        using (var trackEnd = FsLinkTrack.TrackHttp(url, _method, headerData, requestBody))
        {
            await args.ProceedAsync();
            var httpResponseResult = (HttpResponseResult)args.ReturnValue;

            trackEnd.SetHttpResponseBody(httpResponseResult);
        }
    }
}