﻿namespace LMS.Core.Clients
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using Abp;
    using Abp.Dependency;
    using Json;
    using Newtonsoft.Json;
    using RestSharp;
    using Serilog;

    public class RestClientBase : RestClient, ITransientDependency
    {
        public RestClientBase()
        {
            AddHandlers();
        }

        public RestClientBase(Uri baseUrl) : base(baseUrl)
        {
            AddHandlers();

        }

        public RestClientBase(string baseUrl) : base(baseUrl)
        {
            AddHandlers();
        }

        protected void AddHandlers()
        {           
            AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            AddHandler("application/x-www-form-urlencoded", NewtonsoftJsonSerializer.Default);

        }

        private readonly ILogger _logger = Log.ForContext<RestClientBase>();

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = base.Execute<T>(request);
            stopWatch.Stop();

            LogRequest(request, response, stopWatch.ElapsedMilliseconds);

            TimeoutCheck(request, response);

            return response;
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = base.Execute(request);
            stopWatch.Stop();

            LogRequest(request, response, stopWatch.ElapsedMilliseconds);

            TimeoutCheck(request, response);

            return response;
        }

        public virtual T Get<T>(IRestRequest request)
            where T : new()
        {
            var response = Execute<T>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            LogError(BaseUrl, request, response);
            return default(T);
        }

        private void LogError(Uri baseUrl, IRestRequest request, IRestResponse response)
        {
            //Get the values of the parameters passed to the API
            string parameters = string.Join(", ", request.Parameters.Select(x => x.Name.ToString() + "=" + (x.Value ?? "NULL")).ToArray());

            //Set up the information message with the URL, the status code, and the parameters.
            string info = "Request to " + baseUrl.AbsoluteUri + request.Resource + " failed with status code " + response.StatusCode + ", parameters: "
                + parameters + ", and content: " + response.Content;

            //Acquire the actual exception
            Exception ex;
            if (response.ErrorException != null)
            {
                ex = response.ErrorException;
            }
            else
            {
                ex = new Exception(info);
                info = string.Empty;
            }

            //Log the exception and info message
            _logger.Error("Request to {AbsoluteUri} failed with status code {StatusCode}, parameters: {@Parameters}, and content: {Content}", baseUrl.AbsoluteUri,request.Resource,response.StatusCode,parameters,response.Content);
            _logger.Debug(ex, ex.Message);
            throw new AbpException(info, ex);
        }

        private void LogRequest(IRestRequest request, IRestResponse response, long durationMs)
        {
            var requestToLog = new
            {
                resource = request.Resource,
                // Parameters are custom anonymous objects in order to have the parameter type as a nice string
                // otherwise it will just show the enum value
                parameters = request.Parameters.Select(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
                // ToString() here to have the method as a nice string otherwise it will just show the enum value
                method = request.Method.ToString(),
                // This will generate the actual Uri used in the request
                uri = BuildUri(request)
            };

            var responseToLog = new
            {
                statusCode = response.StatusCode,
                content = response.Content,
                headers = response.Headers,
                // The Uri that actually responded (could be different from the requestUri if a redirection occurred)
                responseUri = response.ResponseUri,
                errorMessage = response.ErrorMessage
            };

            _logger.Information("Request completed in {Duration} ms, Request: ({Method}) - ({Url}) - {StatusCode}", durationMs, request.Method, BuildUri(request), response.StatusCode.ToString());
            _logger.Debug("Request completed in {durationMs} ms, Request: {@Request}, Response: {@Response}", durationMs,requestToLog, responseToLog);
        }

        private void TimeoutCheck(IRestRequest request, IRestResponse response)
        {
            if (response.StatusCode == 0)
            {
                LogError(BaseUrl, request, response);
            }
        }
    }
}