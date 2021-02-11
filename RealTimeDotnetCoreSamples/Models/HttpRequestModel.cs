using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;

namespace RealTimeDotnetCoreSamples.Models
{
    public class HttpRequestModel : Validation
    {
        public E_HttpClient ClientName { get; set; }
        public string MethodName { get; set; }
        public System.Net.Http.HttpContent HttpContent { get; set; }
        public System.Net.Http.HttpMethod HttpMethod { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> QueryString { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (this.ClientName == E_HttpClient.Undefined)
                yield return new ValidationResult($"{nameof(this.ClientName)} is undefined.", new string[] { nameof(this.ClientName) });
            if (string.IsNullOrEmpty(this.MethodName))
                yield return new ValidationResult($"{nameof(this.MethodName)} is null or empty.", new string[] { nameof(this.MethodName) });
            if (this.HttpMethod == null)
                yield return new ValidationResult($"{nameof(this.HttpMethod)} is null.", new string[] { nameof(this.HttpMethod) });
        }
    }
}
