# Africa's Talking VB.NET Gateway  
___ 
Official Africa's Talking VB.NET API wrapper  
### Install 
To help decode JSON responses, you will need to add a reference in your project reference, add the  ``System.Web`` and ``System.Web.Extensions`` reference  
![alt Nuget](screenshots/reference.PNG)
### Initialization
```vb dotnet       
' Specify your Account credentials
        Dim username As String = "MyAfricasTalkingUsername"
        Dim apiKey As String = "MyAfricasTalkingAPIKey"
};
```
### [SMS ](http://docs.africastalking.com/sms/)
```vb dotnet
Public Class Sendmessage
    Public Shared Sub Main()
        ' Specify your login credentials
        Dim username As String = "MyAfricasTalkingUsername"
        Dim apiKey As String = "MyAfricasTalkingAPIKey"
        'Specify the numbers that you want to send to in a comma-separated list
        'Please ensure you include the country code (+254 for Kenya in this case)
        Dim recipients As String = "+254700YYYXXX"
        'And of course we want our recipients to know what we really do
        Dim message As String = "I'm a lumberjack and its ok, I sleep all night and I work all day"
        ' Create a new instance of our awesome gateway class
        Dim gateway As New AfricasTalkingGateway(username, apiKey)
        ' Any gateway errors will be captured by our custom Exception class below,
        ' so wrap the call in a try-catch block   
        Try
            ' Thats it, hit send and we'll take care of the rest
            Dim results As Object = gateway.sendMessage(recipients, message)
            For Each result As Object In results
                Console.Write(CStr(result("number")) & ",")
                Console.Write(CStr(result("status")) & ",") ' status is either "Success" or "error message"
                Console.Write(CStr(result("messageId")) & ",")
                Console.WriteLine(CStr(result("cost")))
            Next result
        Catch e As AfricasTalkingGatewayException
            Console.WriteLine("Encountered an error: " & e.Message)
        End Try
        Console.Read()
    End Sub
End Class
```
#### [Sending SMS Sample](http://docs.africastalking.com/sms/sending/vb)
- `send(options)`: Send a message. `options` contains:
    - `to`: A single recipient or an array of recipients. `REQUIRED`
    - `from`: Shortcode or alphanumeric ID that is registered with Africa's Talking account.
    - `message`: SMS content. `REQUIRED`
- `sendBulk(options)`: Send bulk SMS. In addition to paramaters of `send()`, we would have: 
    - `enqueue`: "[...] would like deliver as many messages to the API before waiting for an Ack from the Telcos."  
- `sendPremium(options)`: Send premium SMS. In addition to paramaters of `send()`, we would have:
    - `keyword`: Value is a premium keyword `REQUIRED`
    - `linkId`: "[...] We forward the `linkId` to your application when the user send a message to your service" `REQUIRED`
    - `retryDurationInHours`: "It specifies the number of hours your subscription message should be retried in case it's not delivered to the subscriber"   
#### [Retrieving SMS](http://docs.africastalking.com/sms/sending/vb)
> You can register a callback URL with us and we will forward any messages that are sent to your account the moment they arrive. 
> [Read more](http://docs.africastalking.com/sms/callback)
- `fetchMessages(options)`: Manually retrieve your messages.
    - `lastReceivedId`: "This is the id of the message that you last processed". Defaults to `0`. `REQUIRED`
#### [Premium Subscriptions](http://docs.africastalking.com/sms/sending/vb)
> If you have subscription products on your premium SMS short codes, you will need to configure a callback URL that we will invoke to notify you when users subscribe or unsubscribe from your products.
> [Read more](http://docs.africastalking.com/subscriptions/callback)
- `createSubscription(options)`:
  - `shortCode`: "This is a premium short code mapped to your account". `REQUIRED`
    - `keyword`: "Value is a premium keyword under the above short code and mapped to your account". `REQUIRED`
    - `phoneNumber`: "The phoneNumber to be subscribed" `REQUIRED`
- `fetchSubscription(options)`:
    - `shortCode`: "This is a premium short code mapped to your account". `REQUIRED`
    - `keyword`: "Value is a premium keyword under the above short code and mapped to your account". `REQUIRED`
    - `lastReceivedId`: "ID of the subscription you believe to be your last." Defaults to `0`
### [USSD](http://docs.africastalking.com/ussd)
> Processing USSD requests using our API is very easy once your account is set up. In particular, you will need to:
> - Register a service code with us.
> - Register a URL that we can call whenever we get a request from a client coming into our system.
> Once you register your callback URL, any requests that we receive belonging to you will trigger a callback that sends the request data to that page using HTTP POST.
> [Read more.](http://docs.africastalking.com/ussd)
___ 
## Voice
- Helpers that will construct proper `xml` to send back to Africa's Taking API when it comes `POST`ing. [Read more](http://docs.africastalking.com/voice)
    - `Say`, `Play`, `GetDigits`, `Dial`, `Record`, `Enqueue`, `Dequeue`, `Conference`, `Redirect`, `Reject`
- Initiate a call
- Fetch call queue
- Any url to ```Play``` will be cached by default.
- Remember to send back an HTTP 200.
#### [Initiate a call](http://docs.africastalking.com/voice/call)
The VBdotNet code snippet below shows how to make a call using our API
```VB dotnet
```
___ 
### Airtime
```vb dot net
Public Class Application
    Public Shared Sub Main()        
            ' Specify your login credentials            
            Dim username As String = "myAfricasTalkingUsername"
            Dim apikey As String   = "myAPIKey"         ' Specify an list to hold the airtime recipient numbers and the amount to be sent           
            Dim airtimeRecipientsList As New List(Of Hashtable)() ' Declare hashtable to hold the first recipient and amount sent to them
            ' Please ensure you include the country code for phone numbers (+254 for Kenya in this case)
            ' Specify the country currency and the amount as shown below (KES for Kenya in this case)
            Dim recipient1 As New Hashtable()
            recipient1("phoneNumber") = "+254711XXXYYY"
            recipient1("amount")      = "KES XX"
            ' Add the recipient to airtimeRecipientsList
            airtimeRecipientsList.add(recipient1)
            ' Declare hashtable to hold the second recipient
            Dim recipient2 As New Hashtable()
            recipient2("phoneNumber") = "+254733YYYZZZ"
            recipient2("amount")      = "KES YY"
            ' Add the recipient to airtimeRecipientsList
            airtimeRecipientsList.add(recipient2)
            ' Create a new instance of our awesome gateway class
            Dim gateway As New AfricasTalkingGateway(username, apikey)
            Try
             ' Hit send and we will handle the rest
                Dim response as object()= gateway.sendAirtime(airtimeRecipientsList)
                 Dim result as object
                for each result in response
                    System.Console.WriteLine("Status: " + result("status"))
                    System.Console.WriteLine("RequestID: " + result("requestId"))
                    System.Console.WriteLine("phoneNumber: " + result("phoneNumber"))
                    System.Console.WriteLine("Discount: " + result("discount"))
                    System.Console.WriteLine("amount: " + result("requestId"))
                    ' Incase the status result is Failed
                    System.Console.WriteLine("ErrorMessage: " + result("errorMessage"))
                next
        catch ex As AfricasTalkingGatewayException
            System.Console.WriteLine(ex.Message())
        End Try 
    End Sub
End Class
```     
___ 
### [ Account Balance](http://docs.africastalking.com/userdata/balance/csharp)
- `FetchAccount()`: Fetch account info; i.e. balance
```vb dotnet
Public Class Application
    Public Shared Sub Main()        
            ' Specify your login credentials            
            Dim username As String = "myAfricasTalkingUsername"
            Dim apikey As String   = "myAPIKey"         ' Create a new instance of our awesome gateway class
            Dim gateway As New AfricasTalkingGateway(username, apikey)            
            ' Wrap the function call in a Try block
            ' Any Exception will be captured by our custom AfricasTalkingException class      
            Try
                Object userData = gateway.getUserData()
                Dim balance As String = userData("balance")
                System.Console.WriteLine("Balance: " + balance)
                ' The result will have the format=> KES XXX               
                catch ex As AfricasTalkingGatewayException
                System.Console.WriteLine(ex.Message())        
        End Try 
    End Sub
End Class
```
___
### Payments
### C2B
> Mobile Consumer To Business (C2B) functionality allows your application to receive payments that are initiated by a mobile subscriber.
> This is typically achieved by disctributing a PayBill or BuyGoods number (and optionally an account number) that clients can use to make payments from their mobile devices.
> [Read more](http://docs.africastalking.com/payments/mobile-c2b)
```vb dot net
 Friend Class initiatecheckout
    Public Shared Sub Initiatecheckout()
        'Create an instance of our awesome gateway class and pass your credentials
        Dim username As String = "MyAfricasTalkingUsername"
        Dim apiKey As String = "MyAfricasTalkingAPIKey"
        ' Specify the name of your Africa's Talking payment product
        Dim productName As String = "kaATproductNamenyi"
        ' The phone number of the customer checking out
        Dim phoneNumber As String = "+254700YYYXXX"
        ' The 3-Letter ISO currency code for the checkout amount
        Dim currencyCode As String = "KES"
        ' The checkout amount
        Dim amount As Integer = 500
        ' The provider Channel - Optional
        Dim providerChannel As String = "YYYXXX"
        ' Create a new instance of our awesome gateway class
        Dim gateway As New AfricasTalkingGateway(username, apiKey)
        ' NOTE: If connecting to the sandbox, please add the sandbox flag to the constructor:
        '            ***********************************************************************************
        '                                   ****SANDBOX****            
        '            *************************************************************************************
        ' AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey, "environment");
        ' Dim gateway As New AfricasTalkingGateway(username, apiKey, "environment")
        ' Any gateway errors will be captured by our custom Exception class below,
        Try
            ' Initiate the checkout. If successful, you will get back a json response
            Dim checkoutResponse As Object = gateway.initiateMobilePaymentCheckout(productName, phoneNumber, currencyCode, amount, providerChannel)
            Console.WriteLine(checkoutResponse)
             Catch e As AfricasTalkingGatewayException
            Console.WriteLine("Encountered an error: " & e.Message)
        End Try
        Console.Read()
    End Sub
End Class
```
### Initiate checkout
- `checkout(options)`: Initiate Customer to Business (C2B) payments on a mobile subscriber's device. [More info](http://docs.africastalking.com/payments/mobile-checkout)
    - `productName`: Your Payment Product. `REQUIRED`
    - `phoneNumber`: The customer phone number (in international format; e.g. `25471xxxxxxx`). `REQUIRED`
    - `currencyCode`: 3-digit ISO format currency code (e.g `KES`, `USD`, `UGX` etc.) `REQUIRED`
    - `amount`: This is the amount. `REQUIRED`
    - `metadata`: Some optional data to associate with transaction.
```vb dot net
 It uses the same principple as to C2B
```
### B2C
- `pay(options)`:  Initiate payments to mobile subscribers from your payment wallet. [More info](http://docs.africastalking.com/payments/mobile-b2c)
    - `productName`: Your Payment Product. `REQUIRED`
    - `recipients`: A list of **up to 10** recipients. Each recipient has:
        - `phoneNumber`: The payee phone number (in international format; e.g. `25471xxxxxxx`). `REQUIRED`
        - `currencyCode`: 3-digit ISO format currency code (e.g `KES`, `USD`, `UGX` etc.) `REQUIRED`
        - `amount`: Payment amount. `REQUIRED`
        - `reason`: This field contains a string showing the purpose for the payment.
        - `metadata`: Some optional data to associate with transaction.
### B2B
 -  ``In order`` to facilitate __Mobile B2C transactions__, we have implemented a RESTFul JSON API that allows your application to request B2C Payments to a mobile subscriber's phone number. The amount specified will then be directly credited to the mobile subscriber's account. Our API allows you to initiate multiple B2C transactions in one request, all of which will be queued in our gateways for processing.
-  ``Once`` the payment provider confirms or rejects the payment request, our APIs will generate a payment notification and send it to the callback URL configured in your account. You can learn more about how to handle payment notifications [in this section](http://docs.africastalking.com/payment/notification).
- ``Please`` note that a notification will be generated regardless of whether the transaction was successful or not. 
```vb dotnet
Option Infer On
Imports System
Imports System.Collections.Generic
Public Class TestMobilePaymentB2C
    Public Shared Sub MobilePaymentB2C()
        'Specify your credentials   
        Dim username As String = "MyAfricasTalkingUsername"
        Dim apiKey As String = "MyAfricasTalkingAPIKey"
        'NOTE: If connecting to the sandbox, please use your sandbox login credentials
        'Create an instance of our awesome gateway class and pass your credentials
        Dim gateway As New AfricasTalkingGateway(username, apiKey)
        ' NOTE: If connecting to the sandbox, please add the sandbox flag to the constructor:
        '                            <summary>
        '***********************************************************************************
        '                  ****SANDBOX****
        'Dim AfricasTalkingGateway gateway As new AfricasTalkingGateway(username, apiKey,"environment");
        ' Specify the name of your Africa's Talking payment product
        Dim productName As String = "ATProductName"
        ' The 3-Letter ISO currency code for the checkout amount
        Dim currencyCode As String = "KES"
        ' Provide the details of a mobile money recipient
        Dim recipient1 As New MobilePaymentB2CRecipient("+254700YYYXXX", "KES", 10D)
        recipient1.AddMetadata("name", "Clerk")
        recipient1.AddMetadata("reason", "May Salary")
        ' You can provide up to 10 recipients at a time
        ' Dim recipient2 As New MobilePaymentB2CRecipient("+254700YYYXXX", "KES", 10D)
        'recipient2.AddMetadata("name", "Accountant")
        'recipient2.AddMetadata("reason", "May Salary")
        ' Put the recipients into an array
        Dim recipients As IList(Of MobilePaymentB2CRecipient) = New List(Of MobilePaymentB2CRecipient)()
        recipients.Add(recipient1)
        'recipients.Add(recipient2)
        Try
            Dim responses = gateway.MobilePaymentB2CRequest(productName, recipients)
            Console.WriteLine(responses)
        Catch ex As Exception
            Console.WriteLine("Received error response: " & ex.Message)
        End Try
        Console.ReadLine()
    End Sub
End Class
```
