using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Auth
{
    internal class MQSigner : IServiceSigner
    {
        #region Immutable Properties

        private static readonly Regex            CompressWhitespaceRegex = new Regex(pattern: "\\s+");
        private const           SigningAlgorithm SignerAlgorithm         = SigningAlgorithm.HmacSHA1;

        #endregion

        #region Public Signing Methods

        public void Sign
        (
            IRequest request,
            string   accessKeyId,
            string   secretAccessKey,
            string   stsToken
        )
        {
            var signingRequest = SignRequest(request: request, secretAccessKey: secretAccessKey);
            var signingResult  = new StringBuilder();
            signingResult.AppendFormat(format: "{0} {1}:{2}",
                                       arg0: Constants.MQ_AUTHORIZATION_HEADER_PREFIX,
                                       arg1: accessKeyId,
                                       arg2: signingRequest);
            request.Headers[key: Constants.AuthorizationHeader] = signingResult.ToString();

            if (!string.IsNullOrEmpty(value: stsToken)) request.Headers[key: Constants.SecurityToken] = stsToken;
        }

        public string SignRequest(IRequest request, string secretAccessKey)
        {
            InitializeHeaders(headers: request.Headers);

            var parametersToCanonicalize = GetParametersToCanonicalize(request: request);
            var canonicalParameters      = CanonicalizeQueryParameters(parameters: parametersToCanonicalize);
            var canonicalResource        = CanonicalizeResource(canonicalQueryString: canonicalParameters, resourcePath: request.ResourcePath);
            var canonicalHeaders         = CanonoicalizeHeaders(headers: request.Headers);

            var canonicalRequest = CanonicalizeRequest(httpMethod: request.HttpMethod,
                                                       headers: request.Headers,
                                                       canonicalHeaders: canonicalHeaders,
                                                       canonicalResource: canonicalResource);

            return ComputeSignature(secretAccessKey: secretAccessKey, canonicalRequest: canonicalRequest);
        }

        #endregion

        #region Public Signing Helpers

        private static void InitializeHeaders(IDictionary<string, string> headers)
        {
            // clean up any prior signature in the headers if resigning
            headers.Remove(key: Constants.AuthorizationHeader);
        }

        public static string ComputeSignature(string secretAccessKey, string canonicalRequest) => ComputeKeyedHash(algorithm: SignerAlgorithm, key: secretAccessKey, data: canonicalRequest);

        public static string ComputeKeyedHash(SigningAlgorithm algorithm, string key, string data) => CryptoUtilFactory.CryptoInstance.HMACSign(data: data, key: key, algorithmName: algorithm);

        #endregion

        #region Private Signing Helpers

        protected static string CanonoicalizeHeaders(IDictionary<string, string> headers)
        {
            var headersToCanonicalize = new SortedDictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers.Where(predicate: header =>
                                                         header.Key.ToLowerInvariant().StartsWith(value: Constants.X_MQ_HEADER_PREFIX)))
                    headersToCanonicalize.Add(key: header.Key.ToLowerInvariant(), value: header.Value);
            }

            return CanonicalizeHeaders(sortedHeaders: headersToCanonicalize);
        }

        protected static string CanonicalizeResource(string canonicalQueryString, string resourcePath)
        {
            var canonicalResource = new StringBuilder();
            canonicalResource.Append(value: CanonicalizeResourcePath(resourcePath: resourcePath));
            if (canonicalQueryString != string.Empty) canonicalResource.AppendFormat(format: "?{0}", arg0: canonicalQueryString);
            return canonicalResource.ToString();
        }

        protected static string CanonicalizeResourcePath(string resourcePath)
        {
            if (string.IsNullOrEmpty(value: resourcePath) || resourcePath == "/") return "/";

            var pathSegments      = resourcePath.Split(separator: new[] { '/' }, options: StringSplitOptions.RemoveEmptyEntries);
            var canonicalizedPath = new StringBuilder();
            foreach (var segment in pathSegments) canonicalizedPath.AppendFormat(format: "/{0}", arg0: segment);

            if (resourcePath.EndsWith(value: "/", comparisonType: StringComparison.Ordinal)) canonicalizedPath.Append(value: "/");

            return canonicalizedPath.ToString();
        }

        protected static string CanonicalizeRequest
        (
            string                      httpMethod,
            IDictionary<string, string> headers,
            string                      canonicalHeaders,
            string                      canonicalResource
        )
        {
            var canonicalRequest = new StringBuilder();
            canonicalRequest.AppendFormat(format: "{0}\n", arg0: httpMethod);

            var contentMD5                                                       = string.Empty;
            if (headers.ContainsKey(key: Constants.ContentMD5Header)) contentMD5 = headers[key: Constants.ContentMD5Header];
            canonicalRequest.AppendFormat(format: "{0}\n", arg0: contentMD5);

            var contentType                                                        = string.Empty;
            if (headers.ContainsKey(key: Constants.ContentTypeHeader)) contentType = headers[key: Constants.ContentTypeHeader];
            canonicalRequest.AppendFormat(format: "{0}\n", arg0: contentType);

            canonicalRequest.AppendFormat(format: "{0}\n", arg0: headers[key: Constants.DateHeader]);
            canonicalRequest.Append(value: canonicalHeaders);
            canonicalRequest.Append(value: canonicalResource);

            return canonicalRequest.ToString();
        }

        protected static string CanonicalizeHeaders(ICollection<KeyValuePair<string, string>> sortedHeaders)
        {
            if (sortedHeaders == null || sortedHeaders.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            foreach (var entry in sortedHeaders)
            {
                builder.Append(value: entry.Key.ToLower(culture: CultureInfo.InvariantCulture));
                builder.Append(value: ":");
                builder.Append(value: CompressSpaces(data: entry.Value));
                builder.Append(value: "\n");
            }

            return builder.ToString();
        }

        protected static IDictionary<string, string> GetParametersToCanonicalize(IRequest request)
        {
            var parametersToCanonicalize = new Dictionary<string, string>();

            if (request.SubResources != null && request.SubResources.Count > 0)
            {
                foreach (var subResource in request.SubResources) parametersToCanonicalize.Add(key: subResource.Key, value: subResource.Value);
            }

            if (request.Parameters != null && request.Parameters.Count > 0)
            {
                foreach (var queryParameter in request.Parameters.Where(predicate: queryParameter => queryParameter.Value != null)) parametersToCanonicalize.Add(key: queryParameter.Key, value: queryParameter.Value);
            }

            return parametersToCanonicalize;
        }

        protected static string CanonicalizeQueryParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0) return string.Empty;

            var canonicalQueryString = new StringBuilder();
            var queryParams          = new SortedDictionary<string, string>(dictionary: parameters, comparer: StringComparer.Ordinal);
            foreach (var p in queryParams)
            {
                if (canonicalQueryString.Length > 0) canonicalQueryString.Append(value: "&");
                if (string.IsNullOrEmpty(value: p.Value))
                    canonicalQueryString.AppendFormat(format: "{0}=", arg0: p.Key);
                else
                    canonicalQueryString.AppendFormat(format: "{0}={1}", arg0: p.Key, arg1: p.Value);
            }

            return canonicalQueryString.ToString();
        }

        private static string CompressSpaces(string data)
        {
            if (data == null || !data.Contains(value: " ")) return data;

            var compressed = CompressWhitespaceRegex.Replace(input: data, replacement: " ");
            return compressed;
        }

        #endregion
    }
}