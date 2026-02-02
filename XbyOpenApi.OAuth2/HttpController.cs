using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XbyOpenApi.OAuth2
{
  /// <summary>
  /// Internal controller to execute Http requests.
  /// </summary>
  internal class HttpController
  {
    /// <summary>
    /// Invoke a url and return the reponse from the server.
    /// </summary>
    /// <param name="url">Invoke this URL</param>
    /// <param name="method">Request method  (e.g. GET or POST).</param>
    /// <param name="initRequest">Do further request initialization in this callback, e.g. set Authorization headers</param>
    /// <exception cref="HttpException">If an exception occurs internally</exception>
    /// <returns>Contains the server response: Result code of the request (success or error) and 
    /// either the response text or a error message</returns>
    public static async Task<HttpResponse> ExecuteRequest(string url, HttpMethod method, Action<HttpRequestMessage>? initRequest = null)
    {
      try
      {
        HttpClient client = new HttpClient();

        // Create a request for the URL.
        HttpRequestMessage request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);

        request.Method = method;

        // Invoke the callback for further initialization (e.g. setting headers)
        if (initRequest != null)
        {
          initRequest(request);
        }


        // Get the response.
        HttpResponseMessage response = await client.SendAsync(request);

        string responseFromServer = await response.Content.ReadAsStringAsync();
        HttpStatusCode responseCode = response.StatusCode;

        response.Dispose();

        request.Dispose();
        client.Dispose();

        if (response.IsSuccessStatusCode == true)
        {
          return HttpResponse.CreateSuccess(response.StatusCode, responseFromServer);
        }
        else
        {
          return HttpResponse.CreateError(response.StatusCode, responseFromServer);
        }
      }
      catch (Exception ex)
      {
        throw new HttpException("Error invoking url " + url, ex);
      }
    }

    #region Inner classes
    /// <summary>
    /// This exception is thrown whenever an error with Http requests occurs
    /// </summary>
    public class HttpException : Exception
    {
      /// <summary>
      /// No message constructor
      /// </summary>
      public HttpException()
      {
      }

      /// <summary>
      /// Create exception with a message text
      /// </summary>
      /// <param name="message">Error message</param>
      public HttpException(string message) : base(message)
      {
      }

      /// <summary>
      /// Create exception with a message text and an inner exception
      /// </summary>
      /// <param name="message">Error message</param>
      /// <param name="innerException">Inner exception</param>
      public HttpException(string message, Exception innerException) : base(message, innerException)
      {
      }
    }

    /// <summary>
    /// Wraps the result of a Http call: contains the response data, and also a error message of the request has a "error" status code
    /// </summary>
    public class HttpResponse
    {
      #region Construktor
      /// <summary>
      /// Konstruktor, dem Service-Zustand und die Service-Antwortdaten (im Falle von Zustand=Error vermutlich nur Dummydaten)
      /// übergeben werden.
      /// 
      /// Besser die "Create"-Methoden verwenden!
      /// </summary>
      /// <param name="isOk">Service-Antwort "Erfolg" oder "Fehler" </param>
      /// <param name="_statusCode">Bei Fehlern: StatusCode der HTTP-Antwort</param>
      /// <param name="_data">Dies sind die eigentlichen Daten, die vom Service kamen. Im Fehlerfall: Dummydaten</param>
      public HttpResponse(bool isOk, HttpStatusCode _statusCode, string _data)
      {
        this.IsOK = isOk;
        this.Data = _data;
        this.StatusCode = _statusCode;
      }
      #endregion

      #region Static Factory-Methoden
      /// <summary>
      /// Creates a Response object for a successful respose: a statuscode (probably "200") and the response data are set.
      /// </summary>
      /// <param name="_statusCode">Status code of the request</param>
      /// <param name="_data">Response content</param>
      /// <returns>Response object</returns>
      public static HttpResponse CreateSuccess(HttpStatusCode _statusCode, string _data)
      {
        return new HttpResponse(true, _statusCode, _data);
      }

      /// <summary>
      /// Creates a Response object for a "error response: an error status code and a error message are set.
      /// </summary>
      /// <param name="_statusCode">Status code of the request</param>
      /// <param name="_strErrorMessage">Errormessage from the response (if any)</param>
      /// <returns>Response object</returns>
      public static HttpResponse CreateError(HttpStatusCode _statusCode, string _strErrorMessage)
      {
        HttpResponse response = new HttpResponse(false, _statusCode, string.Empty);
        response.ErrorMessage = _strErrorMessage;
        return response;
      }
      #endregion

      #region Properties
      /// <summary>
      /// Was the request succesful or did raise an error?
      /// </summary>
      public bool IsOK
      {
        get;
        private set;
      }

      /// <summary>
      /// If <see cref="IsOK"/> = true: response from the server.
      /// In case of an error it is null.
      /// </summary>
      public string? Data
      {
        get;
        private set;
      }


      /// <summary>
      /// Status code of the HTTP response.
      /// </summary>
      public HttpStatusCode StatusCode
      {
        get;
        private set;
      }

      /// <summary>
      /// If <see cref="IsOK"/> = false: Error message returned from server.
      /// Could be empty.
      /// </summary>
      public string? ErrorMessage
      {
        get;
        private set;
      }
      #endregion

      #region Overrides
      /// <summary>
      /// ToString for logging: prints state and data + error message.
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
        string toString = (this.IsOK == true ? "[OK] " : "[ERROR] " + this.StatusCode + " ");
        //"Data" could be NULL:
        if (string.IsNullOrEmpty(this.Data) == false)
        {
          toString = toString + " " + this.Data;
        }

        //ErrorMessage: append only if not empty:
        if (string.IsNullOrEmpty(this.ErrorMessage) == false)
        {
          toString = toString + " " + this.ErrorMessage;
        }

        return toString;
      }
      #endregion
    }
    #endregion
  }


}