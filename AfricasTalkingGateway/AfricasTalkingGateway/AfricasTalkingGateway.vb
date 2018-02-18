
Option Strict Off

Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
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

    Public Sub New()
    End Sub

    'Change the debug flag to true to view the full response
    Private DEBUG As Boolean = False
    Private _responseCode As Integer


    Public Function SendMessage([to] As String, message As String, Optional ByVal sender As String = Nothing, Optional ByVal bulkSmsMode As Integer = 1, Optional ByVal options As Hashtable = Nothing) As String

        Dim numbers() As String = [to].Split(separator:={","c}, options:=StringSplitOptions.RemoveEmptyEntries)
        Dim isValidphoneNumber = IsPhoneNumber(numbers)
        If [to].Length = 0 OrElse message.Length = 0 OrElse Not isValidphoneNumber Then
            Throw New AfricasTalkingGatewayException("The message is either empty or phone number(s) is not valid")
        Else
            Dim data As New Hashtable()
            data("username") = _username
            data("to") = [to]
            data("message") = message

            If sender IsNot Nothing Then
                data("from") = sender
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
        End If
    End Function


    Public Function FetchMessages(ByVal lastReceivedId As Integer) As String
        Dim url As String = SmsUrlString & "?username=" & _username & "&lastReceivedId=" & Convert.ToString(lastReceivedId)
        Dim response As String = SendGetRequest(url)
        If _responseCode = CInt(HttpStatusCode.OK) Then
            Dim json As String = JsonConvert.DeserializeObject(response)
            Return json
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function CreateSubscription(phoneNumber As String, shortCode As String, keyword As String, checkoutToken As String) As String
        If phoneNumber.Length = 0 OrElse shortCode.Length = 0 OrElse keyword.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If
        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumber") = phoneNumber
        data("shortCode") = shortCode
        data("keyword") = keyword
        data("checkoutToken") = checkoutToken
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
        Return json
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

    Public Function SendAirtime(recipients As ArrayList) As String
        Dim urlString As String = AirtimeUrlString & "/send"
        Dim recipientsData As String = JsonConvert.SerializeObject(recipients)
        Dim data As New Hashtable()
        data("username") = _username
        data("recipients") = recipientsData
        Dim response As String = SendPostRequest(data, urlString)
        If _responseCode = CInt(HttpStatusCode.Created) Then
            Dim jsonResponse As String = JsonConvert.DeserializeObject(Of String)(response)
            Return jsonResponse
        End If
        Throw New AfricasTalkingGatewayException(response)
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

    Public Function CreateCheckoutToken(phoneNumber As String) As String
        Dim status = Regex.Match(phoneNumber, "^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d{5}$").Success
        If Not status Then
            Throw New AfricasTalkingGatewayException("The phone number supplied is not valid")
        Else
            Try
                Dim payload = New Hashtable()
                payload("phoneNumber") = phoneNumber
                Dim response = SendPostRequest(payload, TokenCreateUrl)
                ' Dim tokenRes As String = JObject.Parse(response)
                Return response
            Catch e As AfricasTalkingGatewayException
                Throw New AfricasTalkingGatewayException("An error ocurred while creating this token: " & e.Message)
            End Try
        End If
    End Function

    Public Function InitiateUssdPushRequest(phoneNumber As String, prompt As String, checkoutToken As String) As String
        Dim numbers() As String = phoneNumber.Split(separator:={","c}, options:=StringSplitOptions.RemoveEmptyEntries)
        If Not IsValidToken(checkoutToken) OrElse prompt.Length = 0 OrElse Not IsPhoneNumber(numbers) Then
            Throw New AfricasTalkingGatewayException("One or some of the arguments supplied are invalid.")
        End If

        Try
            Dim data = New Hashtable()
            data("username") = _username
            data("phoneNumber") = phoneNumber
            data("menu") = prompt
            data("checkoutToken") = checkoutToken
            Dim apiPath = UssdPushUrl
            Dim response = SendPostRequest(data, apiPath)
            Dim res As Object = JObject.Parse(response)
            Return res
        Catch e As Exception
            Throw New AfricasTalkingGatewayException(e.Message & e.StackTrace)
        End Try
    End Function

    ' Payments Section

    Public Function InitiateMobilePaymentCheckout(productName As String, phoneNumber As String, currencyCode As String, amount As Decimal, providerChannel As String, Optional _
                                                     ByVal metadata As Dictionary(Of String, String) = Nothing) As String

        Dim symbol As String = Nothing
        Dim status = Regex.Match(phoneNumber, "^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d{5}$").Success
        If Not IsValidProductName(productName) OrElse Not status OrElse Not IsValidCurrency(currencyCode, symbol) OrElse providerChannel.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Missing or malformed arguments :  invalid currency symbol or phonenumber or product name")
        End If
        Try
            Dim c2BTransaction As String = New CheckoutData() With {
                                 .Username = _username,
                                 .ProductName = productName,
                                 .PhoneNumber = phoneNumber,
                                 .CurrencyCode = currencyCode,
                                 .Amount = amount,
                                 .ProviderChannel = providerChannel,
                                 .Metadata = metadata
                                 }.ToJson()
            Dim c2BResponse = TransactionHandler(c2BTransaction, PaymentsUrlString)
            Return c2BResponse
        Catch checkoutException As Exception
            Throw New AfricasTalkingGatewayException("There was a problem processing Mobile Checkout: " & checkoutException.Message)
        End Try
    End Function

    Public Function MobileB2B(productName As String, provider As String, transferType As String, currencyCode As String, amount As Decimal, destinationChannel As String, destinationAccount As String, Optional ByVal metadata As Dictionary(Of String, String) = Nothing) As String
        Dim cSym As String = Nothing
        If Not IsValidProductName(productName) OrElse provider.Length = 0 OrElse transferType.Length = 0 OrElse Not IsValidCurrency(currencyCode, cSym) OrElse destinationAccount.Length = 0 OrElse destinationChannel.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Invalid arguments")
        End If
        ' TBD Set provider as Athena if username is sandbox
        Try
            Dim b2BTrasaction As String = New B2BData() With {
                .Username = _username,
                .ProductName = productName,
                .Provider = provider,
                .TransferType = transferType,
                .CurrencyCode = currencyCode,
                .Amount = amount,
                .DestinationAccount = destinationAccount,
                .DestinationChannel = destinationChannel,
                .Metadata = metadata
                }.ToJson()
            Dim response = TransactionHandler(b2BTrasaction, PaymentsB2BUrlString)
            Return response
        Catch e As Exception
            Throw New AfricasTalkingGatewayException(message:="An error occurred during B2B Request Processing: " & e.Message)
        End Try
    End Function

    Public Function MobilePaymentB2CRequest(productName As String, recipients As IList(Of MobilePaymentB2CRecipient)) As String
        If Not IsValidProductName(productName) Then
            Throw New AfricasTalkingGatewayException("Malformed product name")
        End If

        Dim requestBody = New RequestBody With {
                .ProductName = productName,
                .UserName = _username,
                .Recipients = recipients.ToList()
                }
        Dim mobileB2CTransaction As String = requestBody.ToJson()
        Dim response = TransactionHandler(mobileB2CTransaction, PaymentsB2CUrlString)
        Return response

    End Function

    Public Function BankTransfer(productName As String, recipients As IEnumerable(Of BankTransferRecipients)) As String
        If Not IsValidProductName(productName) Then
            Throw New AfricasTalkingGatewayException("Not a valid product name")
        End If

        Dim transferDetails = New BankTransfer() With {
                .Recipients = recipients.ToList(),
                .ProductName = productName,
                .Username = _username
                }

        Dim bankTransferTransaction As String = transferDetails.ToJson()

        Try
            Dim bankTransferRes = TransactionHandler(bankTransferTransaction, BankTransferUrl)
            Return bankTransferRes
        Catch exception As Exception
            Throw New AfricasTalkingGatewayException(exception.Message)
        End Try
    End Function

    Public Function BankCheckout(productName As String, bankAccount As BankAccount, currencyCode As String, amount As Decimal, narration As String, Optional ByVal metadata As Dictionary(Of String, String) = Nothing) As String
        Dim curSym As String = Nothing
        If IsValidProductName(productName) <> 0 AndAlso IsValidCurrency(currencyCode, curSym) AndAlso narration.Length <> 0 Then
            Dim bankCheckoutData = New BankCheckout() With {
                    .Username = _username,
                    .ProductName = productName,
                    .CurrencyCode = currencyCode,
                    .Amount = amount,
                    .Narration = narration,
                    .BankAccount = bankAccount
                    }
            If metadata IsNot Nothing Then
                bankCheckoutData.Metadata = metadata
            End If

            Dim banckCheckoutTransaction As String = bankCheckoutData.ToJson()

            Try
                Dim response = TransactionHandler(banckCheckoutTransaction, BankCheckoutUrl)
                Return response
            Catch exception As Exception
                Throw New AfricasTalkingGatewayException(exception)
            End Try
        Else
            Throw New AfricasTalkingGatewayException("Invalid arguments")
        End If
    End Function

    Public Function OtpValidateCard(transactionId As String, otp As String) As String
        If transactionId.Length < 32 OrElse otp.Length < 3 OrElse Not IsValidTransactionId(transactionId) Then
            Throw New AfricasTalkingGatewayException("Incorrect Transaction ID or invalid OTP length(Less than 3 characters or digits)")
        End If

        Dim cardOtp = New OTP With {
                .Otp = otp,
                .TransactionId = transactionId,
                .Username = _username
                }

        Try
            Dim cardOtpResult = ProcessOtp(cardOtp, CardOtpValidationUrl)
            Return cardOtpResult
        Catch exception As Exception
            Throw New AfricasTalkingGatewayException(message:="An error ocuured during OTP Card Validation: " & exception.Message)
        End Try
    End Function

    Public Function OtpValidateBank(transactionId As String, otp As String) As String
        Dim otpValidate = New OTP With {
                .Username = _username,
                .TransactionId = transactionId,
                .Otp = otp
                }
        Try
            Dim bankOtpResult = ProcessOtp(otpValidate, BankOtpValidationUrl)
            Return bankOtpResult
        Catch exception As Exception
            Throw New AfricasTalkingGatewayException(exception)
        End Try
    End Function

    Public Function CardCheckout(productName As String, paymentCard As PaymentCard, currencyCode As String, amount As Decimal, narration As String, Optional ByVal metadata As Dictionary(Of String, String) = Nothing, Optional ByVal checkoutToken As String = Nothing) As String
        Dim curSym As String = Nothing
        If productName.Length <> 0 AndAlso IsValidCurrency(currencyCode, curSym) AndAlso narration.Length <> 0 Then

            Dim checkoutDetails = New CardDetails With {
                    .Username = _username,
                    .ProductName = productName,
                    .CurrencyCode = currencyCode,
                    .PaymentCard = paymentCard,
                    .Amount = amount,
                    .Narration = narration
                    }
            If metadata IsNot Nothing Then
                checkoutDetails.Metadata = metadata
            End If

            If checkoutToken IsNot Nothing Then
                checkoutDetails.CheckoutToken = checkoutToken
            End If

            Dim cardCheckoutTransaction As String = checkoutDetails.ToJson()

            Try
                Dim response = TransactionHandler(cardCheckoutTransaction, CardCheckoutUrl)
                Return response
            Catch exception As Exception
                Throw New AfricasTalkingGatewayException(exception)
            End Try
        Else
            Throw New AfricasTalkingGatewayException("Invalid arguments")
        End If
    End Function

    ' End Payments Section

    ' Regex and Validators

    Private Shared Function IsPhoneNumber(number() As String) As Boolean
        Dim valid As Boolean = True
        For Each num As String In number
            Dim status = Regex.Match(num, "^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d{5,}$").Success
            valid = valid And status
        Next num
        Return valid
    End Function

    Private Shared Function IsValidTransactionId(transactionId As String) As Boolean
        Return Regex.Match(transactionId, "^ATPid_.*$").Success AndAlso transactionId.Length > 7
    End Function

    Private Shared Function IsValidToken(token As String) As Boolean
        Return Regex.Match(token, "^CkTkn_.*$").Success AndAlso token.Length > 7
    End Function

    Private Shared Function IsValidProductName(productName As String) As Boolean
        Return productName.Length > 0
    End Function

    Private Shared Function IsValidCurrency(isoCurrency As String, ByRef symbol As String) As Boolean
        symbol = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(Function(c) Not c.IsNeutralCulture).Select(Function(culture)
                                                                                                                        Try
                                                                                                                            Return New RegionInfo(culture.LCID)
                                                                                                                        Catch
                                                                                                                            Return Nothing
                                                                                                                        End Try
                                                                                                                    End Function).Where(Function(ri) ri IsNot Nothing AndAlso ri.ISOCurrencySymbol = isoCurrency).Select(Function(ri) ri.CurrencySymbol).FirstOrDefault()
        Return symbol IsNot Nothing
    End Function

    ' End Regex and Validators

    ' Processes

    Private Function TransactionHandler(transactionType As String, transactionUrl As String)
        Dim transactionClient = New HttpClient
        Dim transactionContent As HttpContent = New StringContent(transactionType, Encoding.UTF8, "application/json")
        transactionClient.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim transactionResult = transactionClient.PostAsync(transactionUrl, transactionContent).Result
        transactionResult.EnsureSuccessStatusCode()
        Dim transactionHandlerResults = transactionResult.Content.ReadAsStringAsync().Result
        Return transactionHandlerResults
    End Function


    Private Function ProcessOtp(otp As OTP, otpUrl As String) As String
        Dim otpClient = New HttpClient()
        Dim otpPayload As String = otp.ToJson()
        Dim otpContent As HttpContent = New StringContent(otpPayload, Encoding.UTF8, "application/json")
        otpClient.DefaultRequestHeaders.Add("apiKey", _apikey)
        Dim otpResult = otpClient.PostAsync(otpUrl, otpContent).Result
        otpResult.EnsureSuccessStatusCode()
        Dim otpRes = otpResult.Content.ReadAsStringAsync().Result
        Return otpRes
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
            ' Debug Only
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

    Private ReadOnly Property CardOtpValidationUrl() As String
        Get
            Return PaymentHost & "/card/checkout/validate"
        End Get
    End Property

    Private ReadOnly Property CardCheckoutUrl() As String
        Get
            Return PaymentHost & "/card/checkout/charge"
        End Get
    End Property

    Private ReadOnly Property BankCheckoutUrl() As String
        Get
            Return PaymentHost & "/bank/checkout/charge"
        End Get
    End Property

    Private ReadOnly Property BankOtpValidationUrl() As String
        Get
            Return PaymentHost & "/bank/checkout/validate"
        End Get
    End Property

    Private ReadOnly Property BankTransferUrl() As String
        Get
            Return PaymentHost & "/bank/transfer"
        End Get
    End Property

    Private ReadOnly Property TokenCreateUrl() As String
        Get
            Return ApiHost & "/checkout/token/create"
        End Get
    End Property

    Private ReadOnly Property UssdPushUrl() As String
        Get
            Return ApiHost & "/ussd/push/request"
        End Get
    End Property

End Class
