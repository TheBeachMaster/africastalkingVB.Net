Option Infer On
Option Strict Off
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Partial Public Class AfricasTalkingGateway
    Private ReadOnly _username As String
    Private ReadOnly _apikey As String
    Private ReadOnly _environment As String

    Public Sub New(ByVal username As String, ByVal apikey As String)
        _username = username
        _apikey = apikey
        _environment = GetDefaultAuthEnv(username)
    End Sub

    Private Shared Function GetDefaultAuthEnv(ByVal username As String) As String
        Dim env As String
        If username = "sandbox" Then
            env = "sandbox"
        Else
            env = "production"
        End If
        Return env
    End Function

    'Change the debug flag to true to view the full response
    Private DEBUG As Boolean = False
    Private _responseCode As Integer

    Public Function SendMessage(ByVal to_ As String, ByVal message_ As String, Optional ByVal from_ As String = Nothing, Optional ByVal bulkSMSMode_ As Integer = 1, Optional ByVal options_ As Hashtable = Nothing) As Object
        Dim data As New Hashtable()
        data("username") = _username
        data("to") = to_
        data("message") = message_

        If from_ IsNot Nothing Then
            data("from") = from_
            data("bulkSMSMode") = Convert.ToString(bulkSMSMode_)

            If options_ IsNot Nothing Then
                If options_.Contains("keyword") Then
                    data("keyword") = options_("keyword")
                End If

                If options_.Contains("linkId") Then
                    data("linkId") = options_("linkId")
                End If

                If options_.Contains("enqueue") Then
                    data("enqueue") = options_("enqueue")
                End If

                If options_.Contains("retryDurationInHours") Then
                    data("retryDurationInHours") = options_("retryDurationInHours")
                End If
            End If
        End If

        Dim response As String = SendPostRequest(data, SmsUrlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim json As String = JsonConvert.DeserializeObject(Of Object)(response)
            Dim recipients As Object = json("SMSMessageData")
            If recipients.Length > 0 Then
                Return recipients
            End If
            Throw New AfricasTalkingGatewayException(CType(json("SMSMessageData"), String))

        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function FetchMessages(ByVal lastReceivedId_ As Integer) As Object
        Dim url As String = SmsUrlString & "?username=" & _username & "&lastReceivedId=" & Convert.ToString(lastReceivedId_)
        Dim response As String = SendGetRequest(url)
        If _responseCode = CInt(HttpStatusCode.OK) Then
            Dim json As String = JsonConvert.DeserializeObject(response)
            Return json("SMSMessageData")
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function CreateSubscription(ByVal phoneNumber As String, ByVal shortCode As String, ByVal keyword As String) As Object
        If phoneNumber.Length = 0 OrElse shortCode.Length = 0 OrElse keyword.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If
        Dim data_ As New Hashtable()
        data_("username") = _username
        data_("phoneNumber") = phoneNumber
        data_("shortCode") = shortCode
        data_("keyword") = keyword
        Dim urlString As String = SubscriptionUrlString & "/create"
        Dim response As String = SendPostRequest(data_, urlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim json As String = JsonConvert.DeserializeObject(Of Object)(response)
            Return json
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function
    Public Function deleteSubscription(ByVal phoneNumber_ As String, ByVal shortCode_ As String, ByVal keyword_ As String) As Object
        If phoneNumber_.Length = 0 OrElse shortCode_.Length = 0 OrElse keyword_.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If
        Dim data_ As New Hashtable()
        data_("username") = _username
        data_("phoneNumber") = phoneNumber_
        data_("shortCode") = shortCode_
        data_("keyword") = keyword_
        Dim urlString As String = SubscriptionUrlString & "/delete"
        Dim response As String = SendPostRequest(data_, urlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim json As String = JsonConvert.DeserializeObject(Of Object)(response)
            Return json
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function
    Public Function [Call](ByVal from_ As String, ByVal to_ As String) As Object
        Dim data As New Hashtable()
        data("username") = _username
        data("from") = from_
        data("to") = to_
        Dim urlString As String = VoiceUrlString & "/call"
        Dim response As String = SendPostRequest(data, urlString)
        Dim json As String = JsonConvert.DeserializeObject(Of Object)(response)
        If CStr(json("errorMessage")) = "None" Then
            Return json("entries")
        End If
        Throw New AfricasTalkingGatewayException(CType(json("errorMessage"), String))
    End Function

    Public Function GetNumQueuedCalls(ByVal phoneNumber As String, Optional ByVal queueName As String = Nothing) As Integer
        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumbers") = phoneNumber
        If queueName IsNot Nothing Then
            data("queueName") = queueName
        End If

        Dim queuedUrl As String = VoiceUrlString & "/queueStatus"
        Try
            If _responseCode <> CInt(Math.Truncate(HttpStatusCode.Created)) Then
                Throw New AfricasTalkingGatewayException(SendPostRequest(dataMap_:=data, urlString_:=queuedUrl))
            End If
            Dim response As Object = JObject.Parse(SendPostRequest(dataMap_:=data, urlString_:=queuedUrl))
            Return response
        Catch getNumberOfQueuedCallsException As Exception
            Throw New AfricasTalkingGatewayException("An error was encountered when processing Queued Call Request: " & getNumberOfQueuedCallsException.Message)
        End Try
    End Function

    Public Sub UploadMediaFile(ByVal url_ As String)
        Dim data As New Hashtable()
        data("username") = _username
        data("url") = url_

        Dim urlString As String = VoiceUrlString & "/mediaUpload"
        Dim response As String = SendPostRequest(data, urlString)
        Dim json As String = JsonConvert.DeserializeObject(Of Object)(response)
        If CStr(json("errorMessage")) <> "None" Then
            Throw New AfricasTalkingGatewayException(CType(json("errorMessage"), String))
        End If
    End Sub

    Public Function SendAirtime(ByVal recipient As ArrayList) As Object
        Dim urlString As String = Me.AirtimeUrlString & "/send"
        Dim recipientJson As String = JsonConvert.SerializeObject(recipient)
        Dim data = New Hashtable From {{"username", _username}, {"recipients", recipientJson}}
        Try
            If _responseCode <> CInt(Math.Truncate(HttpStatusCode.Created)) Then
                Throw New AfricasTalkingGatewayException(SendPostRequest(dataMap_:=data, urlString_:=urlString))
            End If
            Dim response As Object = JObject.Parse(SendPostRequest(dataMap_:=data, urlString_:=urlString))
            Return response
        Catch sendAirtimeException As AfricasTalkingGatewayException
            Throw New AfricasTalkingGatewayException("Sending Airtime Encountered an errror: " & sendAirtimeException.Message)
        End Try
    End Function

    Public Function GetUserData() As Object
        Dim urlString As String = UserdataUrlString & "?username=" & _username
        Dim response As String = SendGetRequest(urlString)
        If _responseCode = CInt(HttpStatusCode.OK) Then
            Dim json As String = JsonConvert.DeserializeObject(Of Object)(response)
            Return json("UserData")
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Private Function SendPostRequest(ByVal dataMap_ As Hashtable, ByVal urlString_ As String) As String
        Try
            Dim dataStr As String = ""
            For Each key As String In dataMap_.Keys
                If dataStr.Length > 0 Then
                    dataStr &= "&"
                End If
                Dim value As String = DirectCast(dataMap_(key), String)
                dataStr &= HttpUtility.UrlEncode(key, Encoding.UTF8)
                dataStr &= "=" & HttpUtility.UrlEncode(value, Encoding.UTF8)
            Next key

            Dim byteArray() As Byte = Encoding.UTF8.GetBytes(dataStr)

            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf RemoteCertificateValidationCallback
            Dim webRequest As HttpWebRequest = CType(System.Net.WebRequest.Create(urlString_), HttpWebRequest)

            webRequest.Method = "POST"
            webRequest.ContentType = "application/x-www-form-urlencoded"
            webRequest.ContentLength = byteArray.Length
            webRequest.Accept = "application/json"

            webRequest.Headers.Add("apiKey", _apikey)

            Dim webpageStream As Stream = webRequest.GetRequestStream()
            webpageStream.Write(byteArray, 0, byteArray.Length)
            webpageStream.Close()

            Dim httpResponse As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
            _responseCode = CInt(httpResponse.StatusCode)
            Dim webpageReader As New StreamReader(httpResponse.GetResponseStream())
            Dim response As String = webpageReader.ReadToEnd()

            If DEBUG Then
                Console.WriteLine("Full response: " & response)
            End If

            Return response

        Catch ex As WebException
            If ex.Response Is Nothing Then
                Throw New AfricasTalkingGatewayException(ex.Message)
            End If
            Using stream = ex.Response.GetResponseStream()
                Using reader = New StreamReader(stream)
                    Dim response As String = reader.ReadToEnd()

                    If DEBUG Then
                        Console.WriteLine("Full response: " & response)
                    End If

                    Return response
                End Using
            End Using

        Catch ex As AfricasTalkingGatewayException
            Throw ex
        End Try
    End Function

    Private Function SendGetRequest(ByVal urlString As String) As String
        Try
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf RemoteCertificateValidationCallback

            Dim webRequest As HttpWebRequest = CType(System.Net.WebRequest.Create(urlString), HttpWebRequest)
            webRequest.Method = "GET"
            webRequest.Accept = "application/json"
            webRequest.Headers.Add("apiKey", _apikey)

            Dim httpResponse As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
            _responseCode = CInt(httpResponse.StatusCode)
            Dim webpageReader As New StreamReader(httpResponse.GetResponseStream())

            Dim response As String = webpageReader.ReadToEnd()

            If DEBUG Then
                Console.WriteLine("Full response: " & response)
            End If

            Return response


        Catch ex As WebException
            If ex.Response Is Nothing Then
                Throw New AfricasTalkingGatewayException(ex.Message)
            End If

            Using stream = ex.Response.GetResponseStream()
                Using reader = New StreamReader(stream)
                    Dim response As String = reader.ReadToEnd()

                    If DEBUG Then
                        Console.WriteLine("Full response: " & response)
                    End If

                    Return response
                End Using
            End Using

        Catch ex As AfricasTalkingGatewayException
            Throw ex
        End Try
    End Function

    Private Function PostAsJson(ByVal dataMap As CheckoutData, ByVal url As String) As String
        Dim client = New HttpClient()
        Dim contentPost As HttpContent = New StringContent(dataMap.ToString(), Encoding.UTF8, "application/json")
        ' Complex object
        client.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim result = client.PostAsync(url, contentPost).Result
        result.EnsureSuccessStatusCode()
        Dim stringResult = result.Content.ReadAsStringAsync().Result
        Return stringResult
    End Function

    Public Function InitiateMobilePaymentCheckout(ByVal productName As String, ByVal phoneNumber As String, ByVal currencyCode As String, ByVal amount As Integer, ByVal providerChannel As String, Optional _
                                                     ByVal metadata As Dictionary(Of String, String) = Nothing) As Object

        Try
            Return PostAsJson(New CheckoutData() With {
            .Username = _username,
            .ProductName = productName,
            .PhoneNumber = phoneNumber,
            .CurrencyCode = currencyCode,
            .Amount = amount,
            .ProviderChannel = providerChannel,
            .Metadata = metadata
        }, PaymentsUrlString)
        Catch checkoutException As Exception
            Throw New AfricasTalkingGatewayException("There was a problem processing Mobile Checkout: " & checkoutException.Message)
        End Try
    End Function

    Private Function PostB2BJson(ByVal dataMap As B2BData, ByVal url As String) As String
        Dim client = New HttpClient()
        Dim serializedB2B As HttpContent = New StringContent(dataMap.ToString(), Encoding.UTF8, "application/json")
        client.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim result = client.PostAsync(url, serializedB2B).Result
        result.EnsureSuccessStatusCode()
        Dim stringResult As String = result.Content.ReadAsStringAsync().Result
        Return stringResult

    End Function

    Public Function MobileB2B(ByVal productName As String, ByVal provider As String, ByVal transferType As String, ByVal currencyCode As String, ByVal amount As Integer, ByVal destinationChannel As String, ByVal destinationAccount As String, Optional ByVal metadata As Dictionary(Of String, String) = Nothing) As Object

        Try
            Return PostB2BJson(dataMap:=New B2BData() With {
            .Username = _username,
            .ProductName = productName,
            .Provider = provider,
            .TransferType = transferType,
            .CurrencyCode = currencyCode,
            .Amount = amount,
            .DestinationChannel = destinationChannel,
            .DestinationAccount = destinationAccount,
            .Metadata = metadata
        }, url:=PaymentsB2BUrlString)
        Catch mobileB2BException As Exception
            Throw New AfricasTalkingGatewayException("There was a problem processing Mobile B2B Request: " & mobileB2BException.Message)
        End Try
    End Function

    Public Function MobilePaymentB2CRequest(ByVal productName As String, ByVal recipients As IList(Of MobilePaymentB2CRecipient)) As Object

        Return Post(New RequestBody With {
            .ProductName = productName,
            .UserName = _username,
            .Recipients = recipients.ToList()
        }, Me.PaymentsB2CUrlString)
    End Function

    Private Function Post(ByVal body As RequestBody, ByVal url As String) As String
        Dim httpClient = New HttpClient()
        Dim httpContent As HttpContent = New StringContent(body.ToString(), Encoding.UTF8, "application/json")
        httpClient.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim result = httpClient.PostAsync(url, httpContent).Result
        result.EnsureSuccessStatusCode()
        Dim postResult As String = result.Content.ReadAsStringAsync().Result
        Return postResult

    End Function
    Private Shared Function RemoteCertificateValidationCallback(ByVal sender As Object, ByVal certificate As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal errors As Security.SslPolicyErrors) As Boolean
        Return True
    End Function
    Private ReadOnly Property ApiHost() As String
        Get
            Return (If(String.ReferenceEquals(_environment, "sandbox"), "https://api.sandbox.africastalking.com", "https://api.africastalking.com"))
        End Get
    End Property
    Private ReadOnly Property PaymentHost() As String
        Get
            Return (If(String.ReferenceEquals(_environment, "sandbox"), "https://payments.sandbox.africastalking.com", "https://payments.africastalking.com"))
        End Get

    End Property
    Private ReadOnly Property SmsUrlString() As String
        Get
            Return ApiHost & "/version1/messaging"
        End Get
    End Property
    Private ReadOnly Property VoiceUrlString() As String
        Get
            Return (If(String.ReferenceEquals(_environment, "sandbox"), "https://voice.sandbox.africastalking.com", "https://voice.africastalking.com"))
        End Get
    End Property

    Private ReadOnly Property SubscriptionUrlString() As String
        Get
            Return ApiHost & "/version1/subscription"
        End Get
    End Property

    Private ReadOnly Property UserdataUrlString() As String
        Get
            Return ApiHost & "/version1/user"
        End Get
    End Property
    Private ReadOnly Property AirtimeUrlString() As String
        Get
            Return ApiHost & "/version1/airtime"
        End Get
    End Property
    Private ReadOnly Property PaymentsUrlString() As String
        Get
            Return PaymentHost & "/mobile/checkout/request"
        End Get
    End Property
    Private ReadOnly Property PaymentsB2BUrlString() As String
        Get
            Return PaymentHost & "/mobile/b2b/request"
        End Get
    End Property
    Private ReadOnly Property PaymentsB2CUrlString() As String
        Get
            Return PaymentHost & "/mobile/b2c/request"
        End Get
    End Property

End Class

