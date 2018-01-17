
Option Strict Off
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class AfricasTalkingGateway
    Private ReadOnly _username As String
    Private ReadOnly _apikey As String
    Private ReadOnly _environment As String

    Private Sub New(username As String, apikey As String)
        _username = username
        _apikey = apikey
    End Sub

    Public Sub New(username As String, apikey As String, Optional ByVal environment As String = "production")
        Me.New(username, apikey)
        If username = "sandbox" Then
            _environment = "sandbox"
        Else
            _environment = environment
        End If
    End Sub

    'Change the debug flag to true to view the full response
    Private DEBUG As Boolean = False
    Private _responseCode As Integer


    Public Function SendMessage([to] As String, message As String, Optional ByVal recipient As String = Nothing, Optional ByVal bulkSmsMode As Integer = 1, Optional ByVal options As Hashtable = Nothing) As String
        Dim data As New Hashtable()
        data("username") = _username
        data("to") = [to]
        data("message") = message

        If recipient IsNot Nothing Then
            data("from") = recipient
            data("bulkSMSMode") = Convert.ToString(bulkSmsMode)

            If options IsNot Nothing Then
                If options.Contains("keyword") Then
                    data("keyword") = options("keyword")
                End If

                If options.Contains("linkId") Then
                    data("linkId") = options("linkId")
                End If

                If options.Contains("enqueue") Then
                    data("enqueue") = options("enqueue")
                End If

                If options.Contains("retryDurationInHours") Then
                    data("retryDurationInHours") = options("retryDurationInHours")
                End If
            End If
        End If

        Dim response As String = SendPostRequest(data, SmsUrlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim json As String = JsonConvert.DeserializeObject(Of String)(response)
            Dim recipients As String = json
            If recipients.Length > 0 Then
                Return recipients
            End If
            Throw New AfricasTalkingGatewayException("An error ocurred during this process")

        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function FetchMessages(ByVal lastReceivedId As Integer) As String
        Dim url As String = SmsUrlString & "?username=" & _username & "&lastReceivedId=" & Convert.ToString(lastReceivedId)
        Dim response As String = SendGetRequest(url)
        If _responseCode = CInt(HttpStatusCode.OK) Then
            Dim json As String = JsonConvert.DeserializeObject(response)
            Return json("SMSMessageData")
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function CreateSubscription(phoneNumber As String, shortCode As String, keyword As String) As String
        If phoneNumber.Length = 0 OrElse shortCode.Length = 0 OrElse keyword.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If
        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumber") = phoneNumber
        data("shortCode") = shortCode
        data("keyword") = keyword
        Dim urlString As String = SubscriptionUrlString & "/create"
        Dim response As String = SendPostRequest(data, urlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim json As String = JsonConvert.DeserializeObject(Of String)(response)
            Return json
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function DeleteSubscription(phoneNumber As String, shortCode As String, keyword As String) As String
        If phoneNumber.Length = 0 OrElse shortCode.Length = 0 OrElse keyword.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If
        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumber") = phoneNumber
        data("shortCode") = shortCode
        data("keyword") = keyword
        Dim urlString As String = SubscriptionUrlString & "/delete"
        Dim response As String = SendPostRequest(data, urlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim json As String = JsonConvert.DeserializeObject(Of String)(response)
            Return json
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function [Call](caller As String, [recipients] As String) As String
        Dim data As New Hashtable()
        data("username") = _username
        data("from") = caller
        data("to") = [recipients]
        Dim urlString As String = VoiceUrlString & "/call"
        Dim response As String = SendPostRequest(data, urlString)
        Dim json As String = JsonConvert.DeserializeObject(Of String)(response)
        If CStr(json("errorMessage")) = "None" Then
            Return json("entries")
        End If
        Throw New AfricasTalkingGatewayException(CType(json("errorMessage"), String))
    End Function

    Public Function GetNumQueuedCalls(phoneNumber As String, Optional ByVal queueName As String = Nothing) As Integer
        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumbers") = phoneNumber
        If queueName IsNot Nothing Then
            data("queueName") = queueName
        End If

        Dim queuedUrl As String = VoiceUrlString & "/queueStatus"
        Try
            If _responseCode <> CInt(Math.Truncate(HttpStatusCode.Created)) Then
                Throw New AfricasTalkingGatewayException(SendPostRequest(dataMap:=data, urlString:=queuedUrl))
            End If
            Dim response As String = JObject.Parse(SendPostRequest(dataMap:=data, urlString:=queuedUrl))
            Return response
        Catch getNumberOfQueuedCallsException As Exception
            Throw New AfricasTalkingGatewayException("An error was encountered when processing Queued Call Request: " & getNumberOfQueuedCallsException.Message)
        End Try
    End Function

    Public Sub UploadMediaFile(ByVal url As String)
        Dim data As New Hashtable()
        data("username") = _username
        data("url") = url

        Dim urlString As String = VoiceUrlString & "/mediaUpload"
        Dim response As String = SendPostRequest(data, urlString)
        Dim json As String = JsonConvert.DeserializeObject(Of String)(response)
        If CStr(json("errorMessage")) <> "None" Then
            Throw New AfricasTalkingGatewayException(CType(json("errorMessage"), String))
        End If
    End Sub

    Public Function SendAirtime(recipient As ArrayList) As String
        Dim urlString As String = AirtimeUrlString & "/send"
        Dim recipientJson As String = JsonConvert.SerializeObject(recipient)
        Dim data = New Hashtable From {{"username", _username}, {"recipients", recipientJson}}
        Try
            If _responseCode <> CInt(Math.Truncate(HttpStatusCode.Created)) Then
                Throw New AfricasTalkingGatewayException(SendPostRequest(dataMap:=data, urlString:=urlString))
            End If
            Dim response As String = JObject.Parse(SendPostRequest(dataMap:=data, urlString:=urlString))
            Return response
        Catch sendAirtimeException As AfricasTalkingGatewayException
            Throw New AfricasTalkingGatewayException("Sending Airtime Encountered an errror: " & sendAirtimeException.Message)
        End Try
    End Function

    Public Function GetUserData() As String
        Dim urlString As String = UserdataUrlString & "?username=" & _username
        Dim response As String = SendGetRequest(urlString)
        If _responseCode = (HttpStatusCode.OK) Then
            Dim json As String = JsonConvert.DeserializeObject(Of String)(response)
            Return json("UserData")
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Private Function SendPostRequest(dataMap As Hashtable, urlString As String) As String
        Try
            Dim dataStr As String = ""
            For Each key As String In dataMap.Keys
                If dataStr.Length > 0 Then
                    dataStr &= "&"
                End If
                Dim value As String = DirectCast(dataMap(key), String)
                dataStr &= HttpUtility.UrlEncode(key, Encoding.UTF8)
                dataStr &= "=" & HttpUtility.UrlEncode(value, Encoding.UTF8)
            Next key

            Dim byteArray() As Byte = Encoding.UTF8.GetBytes(dataStr)

            ServicePointManager.ServerCertificateValidationCallback = AddressOf RemoteCertificateValidationCallback
            Dim webRequest As HttpWebRequest = CType(Net.WebRequest.Create(urlString), HttpWebRequest)

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
            Dim jsonOutput As String
            jsonOutput = JsonConvert.SerializeObject(response)

            Return jsonOutput

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
            Throw
        End Try
    End Function

    Private Function SendGetRequest(urlString As String) As String
        Try
            ServicePointManager.ServerCertificateValidationCallback = AddressOf RemoteCertificateValidationCallback

            Dim webRequest As HttpWebRequest = CType(Net.WebRequest.Create(urlString), HttpWebRequest)
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

            Dim jsonOutput As String
            jsonOutput = JsonConvert.SerializeObject(response)

            Return jsonOutput


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
            Throw
        End Try
    End Function

    Private Function PostAsJson(dataMap As CheckoutData, url As String) As String
        Dim client = New HttpClient()
        Dim contentPost As HttpContent = New StringContent(dataMap.ToString(), Encoding.UTF8, "application/json")
        ' Complex object
        client.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim result = client.PostAsync(url, contentPost).Result
        result.EnsureSuccessStatusCode()
        Dim stringResult = result.Content.ReadAsStringAsync().Result
        Dim jsonOutput As String
        jsonOutput = JsonConvert.SerializeObject(stringResult)
        Return jsonOutput
    End Function

    Public Function InitiateMobilePaymentCheckout(productName As String, phoneNumber As String, currencyCode As String, amount As Integer, providerChannel As String, Optional _
                                                     ByVal metadata As Dictionary(Of String, String) = Nothing) As String

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

    Private Function PostB2BJson(dataMap As B2BData, url As String) As String
        Dim client = New HttpClient()
        Dim serializedB2B As HttpContent = New StringContent(dataMap.ToString(), Encoding.UTF8, "application/json")
        client.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim result = client.PostAsync(url, serializedB2B).Result
        result.EnsureSuccessStatusCode()
        Dim stringResult As String = result.Content.ReadAsStringAsync().Result
        Dim jsonOutput As String
        jsonOutput = JsonConvert.SerializeObject(stringResult)
        Return jsonOutput

    End Function

    Public Function MobileB2B(productName As String, provider As String, transferType As String, currencyCode As String, amount As Integer, destinationChannel As String, destinationAccount As String, Optional ByVal metadata As Dictionary(Of String, String) = Nothing) As String

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

    Public Function MobilePaymentB2CRequest(productName As String, recipients As IList(Of MobilePaymentB2CRecipient)) As String

        Return Post(New RequestBody With {
            .ProductName = productName,
            .UserName = _username,
            .Recipients = recipients.ToList()
        }, PaymentsB2CUrlString)
    End Function

    Private Function Post(body As RequestBody, url As String) As String
        Dim httpClient = New HttpClient()
        Dim httpContent As HttpContent = New StringContent(body.ToString(), Encoding.UTF8, "application/json")
        httpClient.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim result = httpClient.PostAsync(url, httpContent).Result
        result.EnsureSuccessStatusCode()
        Dim postResult As String = result.Content.ReadAsStringAsync().Result

        Dim jsonOutput As String
        jsonOutput = JsonConvert.SerializeObject(postResult)
        Return jsonOutput

    End Function

    Private Shared Function RemoteCertificateValidationCallback(sender As Object, certificate As System.Security.Cryptography.X509Certificates.X509Certificate, chain As System.Security.Cryptography.X509Certificates.X509Chain, errors As Security.SslPolicyErrors) As Boolean
        Return True
    End Function

    Private ReadOnly Property ApiHost() As String
        Get
            Return (If(ReferenceEquals(_environment, "sandbox"), "https://api.sandbox.africastalking.com", "https://api.africastalking.com"))
        End Get
    End Property

    Private ReadOnly Property PaymentHost() As String
        Get
            Return (If(ReferenceEquals(_environment, "sandbox"), "https://payments.sandbox.africastalking.com", "https://payments.africastalking.com"))
        End Get

    End Property

    Private ReadOnly Property SmsUrlString() As String
        Get
            Return ApiHost & "/version1/messaging"
        End Get
    End Property

    Private ReadOnly Property VoiceUrlString() As String
        Get
            Return (If(ReferenceEquals(_environment, "sandbox"), "https://voice.sandbox.africastalking.com", "https://voice.africastalking.com"))
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
