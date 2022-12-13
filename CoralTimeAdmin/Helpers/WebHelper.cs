using System;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CoralTimeAdmin.Helpers {
    public class WebHelper : IWebHelper {
        #region Fields 

        private readonly HttpContextBase _httpContext;
        private readonly string[] _staticFileExtensions;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public WebHelper(HttpContextBase httpContext) {
            _httpContext = httpContext;
            _staticFileExtensions = new[] {".axd", ".ashx", ".bmp", ".css", ".gif", ".htm", ".html", ".ico", ".jpeg", ".jpg", ".js", ".png", ".rar", ".zip"};
        }

        #endregion

        #region utilities

        protected virtual bool IsRequestAvailable(HttpContextBase httpContext) {
            if (httpContext == null) {
                return false;
            }

            try {
                if (httpContext.Request == null) {
                    return false;
                }
            }
            catch (HttpException) {
                return false;
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer() {
            var referrerUrl = string.Empty;

            //URL referrer is null in some case (for example, in IE 8)
            if (IsRequestAvailable(_httpContext) && _httpContext.Request.UrlReferrer != null) {
                referrerUrl = _httpContext.Request.UrlReferrer.PathAndQuery;
            }

            return referrerUrl;
        }

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        public virtual bool IsRequestBeingRedirected {
            get {
                var response = _httpContext.Response;
                return response.IsRequestBeingRedirected;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public virtual bool IsPostBeingDone {
            get {
                if (_httpContext.Items["demos.IsPOSTBeingDone"] == null) {
                    return false;
                }

                return Convert.ToBoolean(_httpContext.Items["demos.IsPOSTBeingDone"]);
            }
            set => _httpContext.Items["demos.IsPOSTBeingDone"] = value;
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString) {
            var useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl) {
            if (!IsRequestAvailable(_httpContext)) {
                return string.Empty;
            }

            //get the host considering using SSL
            var url = GetStoreHost(useSsl).TrimEnd('/');

            //get full URL with or without query string
            url += includeQueryString ? _httpContext.Request.RawUrl : _httpContext.Request.Path;

            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store host location</returns>
        public virtual string GetStoreHost(bool useSsl) {
            var result = "";
            var httpHost = ServerVariables("HTTP_HOST");
            if (!string.IsNullOrEmpty(httpHost)) {
                result = "http://" + httpHost;
                if (!result.EndsWith("/")) {
                    result += "/";
                }
            }

            if (useSsl) {
                //Secure URL is not specified.
                //So a store owner wants it to be detected automatically.
                result = result.Replace("http:/", "https:/");
            }

            if (!result.EndsWith("/")) {
                result += "/";
            }

            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        public virtual bool IsCurrentConnectionSecured() {
            var useSsl = false;
            if (IsRequestAvailable(_httpContext)) {
                //when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true

                //1. use HTTP_CLUSTER_HTTPS?
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Use_HTTP_CLUSTER_HTTPS"]) &&
                    Convert.ToBoolean(ConfigurationManager.AppSettings["Use_HTTP_CLUSTER_HTTPS"])) {
                    useSsl = ServerVariables("HTTP_CLUSTER_HTTPS") == "on";
                }

                //2. use HTTP_X_FORWARDED_PROTO?
                else if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Use_HTTP_X_FORWARDED_PROTO"]) &&
                         Convert.ToBoolean(ConfigurationManager.AppSettings["Use_HTTP_X_FORWARDED_PROTO"])) {
                    useSsl = string.Equals(ServerVariables("HTTP_X_FORWARDED_PROTO"), "https", StringComparison.OrdinalIgnoreCase);
                }
                else {
                    useSsl = _httpContext.Request.IsSecureConnection;
                }
            }

            return useSsl;
        }

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        public virtual string ServerVariables(string name) {
            var result = string.Empty;

            try {
                if (!IsRequestAvailable(_httpContext)) {
                    return result;
                }

                //put this method is try-catch 
                //as described here http://www.nopcommerce.com/boards/t/21356/multi-store-roadmap-lets-discuss-update-done.aspx?p=6#90196
                if (_httpContext.Request.ServerVariables[name] != null) {
                    result = _httpContext.Request.ServerVariables[name];
                }
            }
            catch {
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetCurrentIpAddress() {
            if (!IsRequestAvailable(_httpContext)) {
                return string.Empty;
            }

            var result = "";
            try {
                if (_httpContext.Request.Headers != null) {
                    //The X-Forwarded-For (XFF) HTTP header field is a de facto standard
                    //for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer.
                    var forwardedHttpHeader = "X-FORWARDED-FOR";
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ForwardedHTTPheader"])) {
                        //but in some cases server use other HTTP header
                        //in these cases an administrator can specify a custom Forwarded HTTP header
                        //e.g. CF-Connecting-IP, X-FORWARDED-PROTO, etc
                        forwardedHttpHeader = ConfigurationManager.AppSettings["ForwardedHTTPheader"];
                    }

                    //it's used for identifying the originating IP address of a client connecting to a web server
                    //through an HTTP proxy or load balancer. 
                    var xff = _httpContext.Request.Headers.AllKeys
                        .Where(x => forwardedHttpHeader.Equals(x, StringComparison.InvariantCultureIgnoreCase))
                        .Select(k => _httpContext.Request.Headers[k])
                        .FirstOrDefault();

                    //if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
                    if (!string.IsNullOrEmpty(xff)) {
                        var lastIp = xff.Split(',').FirstOrDefault();
                        result = lastIp;
                    }
                }

                if (string.IsNullOrEmpty(result) && _httpContext.Request.UserHostAddress != null) {
                    result = _httpContext.Request.UserHostAddress;
                }
            }
            catch {
                return result;
            }

            //some validation
            if (result == "::1") {
                result = "127.0.0.1";
            }

            //remove port
            if (!string.IsNullOrEmpty(result)) {
                var index = result.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0) {
                    result = result.Substring(0, index);
                }
            }

            return result;
        }

        #endregion
    }
}