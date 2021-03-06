using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YourList.Exceptions;
using YourList.Internal;

namespace YourList.ReverseEngineering
{
    internal class YoutubeHttpClient
    {
        private readonly HttpClient _innerHttpClient;

        public YoutubeHttpClient(HttpClient innerHttpClient)
        {
            _innerHttpClient = innerHttpClient;
        }

        private void CheckResponse(HttpResponseMessage response)
        {
            // Some pages redirect to https://www.google.com/sorry instead of return 429
            if (response.RequestMessage.RequestUri.Host.EndsWith(".google.com", StringComparison.OrdinalIgnoreCase) &&
                response.RequestMessage.RequestUri.LocalPath.StartsWith("/sorry/", StringComparison.OrdinalIgnoreCase))
                throw RequestLimitExceededException.FailedHttpRequest(response);

            var statusCode = (int) response.StatusCode;

            if (statusCode >= 500)
                throw TransientFailureException.FailedHttpRequest(response);

            if (statusCode == 429)
                throw RequestLimitExceededException.FailedHttpRequest(response);

            if (statusCode >= 400)
                throw FatalFailureException.FailedHttpRequest(response);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            HttpCompletionOption completion = HttpCompletionOption.ResponseHeadersRead)
        {
            return await _innerHttpClient.SendAsync(request, completion);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
        }

        public async Task<HttpResponseMessage> HeadAsync(string requestUri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
            return await SendAsync(request);
        }

        public async Task<string> GetStringAsync(string requestUri, bool ensureSuccess = true)
        {
            using var response = await GetAsync(requestUri);

            if (ensureSuccess)
                CheckResponse(response);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Stream> GetStreamAsync(string requestUri,
            long? from = null, long? to = null, bool ensureSuccess = true)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Range = new RangeHeaderValue(from, to);

            var response = await SendAsync(request);

            if (ensureSuccess)
                CheckResponse(response);

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<long?> TryGetContentLengthAsync(string requestUri, bool ensureSuccess = true)
        {
            using var response = await HeadAsync(requestUri);

            if (ensureSuccess)
                CheckResponse(response);

            return response.Content.Headers.ContentLength;
        }

        public SegmentedHttpStream CreateSegmentedStream(string url, long length, long segmentSize)
        {
            return new SegmentedHttpStream(this, url, length, segmentSize);
        }
    }
}