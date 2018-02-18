# Official Africa's Talking VB.NET API wrapper  

The Africa's Talking VB.NET API wrapper provides convenient access to the Africa's Talking API from applications written in VB.NET. With support for .NET Standard 2.0. 

## Documentation 
Take a look at the [API docs here](http://docs.africastalking.com/) for more information. 

## Installation Options
1. #### Using Visual Studio IDE 

   #### Search for the latest version of `AfricasTalking.NET.VB` from the Nuget package manager window


2. ####  Using .NET CLI 

+ From the _command prompt/powershell window_ opened in your project directory, key in the following and press *Enter*. 
```powershell 
 dotnet add package AfricasTalking.NET.VB --version 2.1.1
```
> Ensure you have the latest version of the package. Visit [Nuget](https://www.nuget.org/packages/AfricasTalking.NET.VB/) for more info on the latest release of this package. 

3. #### Using Nuget Package Manger Console 

+ On your Nuget package manager console,key in the following and press *Enter* 
```powershell 
Install-Package AfricasTalking.NET.VB -Version 2.1.1
```
> Ensure you have the latest version of the package. Visit [Nuget](https://www.nuget.org/packages/AfricasTalking.NET.VB/) for more info on the latest release of this package


## Usage 

+ To use this package ensure you add the following `Imports` statement to your project file: 
```vb 
 Imports AfricasTalkingGateway
```

The package needs to be configured with your Africa's Talking username and API key (which you can get from the dashboard). 

```vb  
 ' Substitute the values with your credentials
 Dim username As String = "sandbox" 
 Dim apikey As String = "apikey"
 ReadOnly _gateway As New AfricasTalkingGateway(username,apikey)

```

> Always ensure to register a callback URL with our services from the dashboard for you to receive various notifications posted by our API. 
> Ensure that your callback url responds with an `OK` or `200` status on message receipt.

### SMS 

#### [Sending SMS](http://docs.africastalking.com/sms/sending) 

- `SendMessage(to,message,sender,bulkSmsMode,options)` :  The following arguments are supplied to facilitate sending of messages via our APIs  

    - `to` : The recipient(s) expecting the message 
    - `message` : The SMS body. 
    - `sender` :  (`Optional`) The Short-code or Alphanumeric ID that is associated with an Africa's Talking account.  
    - `bulkSmsMode` (`Optional`) : This parameter will be used by the Mobile Service Provider to determine who gets  billed for a message sent using a Mobile-Terminated Short-code. Must be set to  *1*  for Bulk SMS. .
    - `options` :   (`Optional`). Passed as _key-value_ pairs 
        -   `enque` : This parameter is used for Bulk SMS clients that would like deliver as many messages to the API before waiting for an Ack from the Telcos. If enabled, the API will store the messages in its databases and send them out asynchronously after responding to the request 
        -   `keyword` : This parameter is used for premium services. It is essential for subscription premium services.
        -   `linkId` : This parameter is used for premium services to send OnDemand messages. We forward the linkId to your application when the user send a message to your service. (Essential for premium subscription services) 
        -   `retryDurationInHours` : This parameter is used for premium messages. It specifies the number of hours your subscription message should be retried in case it's not delivered to the subscriber. (Essential for premium subscription services)

```vb 
  
Dim message As String = "My message"
Dim recipient As String = "+25NNNNNNNN"
Dim results As String = _gateway.SendMessage(recipient, message)  

```

```vb 

Dim message As String = "This is a bulk SMS message"
Dim recipients As String = "+254714587654,+254791854473,+254712965433"
Dim result As String = _gateway.SendMessage(recipients, message) 

```

#### [Retrieving SMS](http://docs.africastalking.com/sms/fetchmessages)

- `FetchMessages(lastReceivedId)` : Manually retrieve your messages.

    - `lastReceivedId` : This is the id of the message that you last processed. If this is your first call, pass in 0. `REQUIRED`


#### [Premium Subscriptions](http://docs.africastalking.com/subscriptions/create)

- `CreateSubscription(phoneNumber,shortCode,keyWord,checkoutToken)`:

    - `shortCode` : This is a premium short code mapped to your account `REQUIRED`
    - `keyWord` : Value is a premium keyword under the above short code and mapped to your account. `REQUIRED`
    - `phoneNumber`: The phoneNumber to be subscribed `REQUIRED`
    - `checkoutToken` :  This is a token used to validate the subscription request  `REQUIRED` 

     > If you have subscription products on your premium SMS short codes, you will need to configure a callback URL that we will send message payload to notifying you when users subscribe or unsubscribe from your products (currently supported on Safaricom).Visit [this link](http://docs.africastalking.com/subscriptions/callback) to learn more on how to setup a subscription callback  



### [Airtime](http://docs.africastalking.com/airtime/sending) 

- `SendAirtime(recipients)`: 
    - `recipients`: Contains JSON objects containing the following keys
        - `phoneNumber`: Recipient of airtime
        - `amount`: Amount sent `>= 10 && <= 10K` with currency e.g `KES 100`

```vb 
  
Dim airtimeRecipientsList As New ArrayList()
Dim recipient1 As New Hashtable()
recipient1("phoneNumber") = "+25NNNNNNN"
recipient1("amount") = "KES 250"
Dim recipient2 As New Hashtable()
recipient2("phoneNumber") = "+25NNNNNNN"
recipient2("amount") = "KES 3000"
airtimeRecipientsList.Add(recipient1)
airtimeRecipientsList.Add(recipient2)
Dim airtimeTransact As String = _gateway.SendAirtime(airtimeRecipientsList)  

```

### [Payments](http://docs.africastalking.com/payments)

> Mobile Consumer To Business (C2B) functionality allows your application to receive payments that are initiated by a mobile subscriber.
> This is typically achieved by disctributing a PayBill or BuyGoods number (and optionally an account number) that clients can use to make payments from their mobile devices.
> [Read more](http://docs.africastalking.com/payments/mobile-c2b)

#### [Checkout](http://docs.africastalking.com/payments/mobile-checkout)

- `InitiateMobilePaymentCheckout(productName,phoneNumber,currencyCode,amount,providerChannel,metadata)` :  Initiate Customer to Business (C2B) payments on a mobile subscriber's device. [More info](http://docs.africastalking.com/payments/mobile-checkout)

    - `productName`: Your Payment Product. `REQUIRED`

    - `phoneNumber`: The customer phone number (in international format; e.g. `+25471xxxxxxx`). `REQUIRED`

    - `currencyCode`: 3-digit ISO format currency code (e.g `KES`, `USD`, `UGX` etc.) `REQUIRED`

    - `amount`: This is the amount. `REQUIRED`

    - `providerChannel`: Default provider channel details.For example `MPESA` or `Athena` for sandbox.  
      - **(Sandbox) :**  Note that for sandbox you'll have to manually create a channel that will be associated with `Athena`. This is the channel name that you will pass as an argument during a checkout.  Example after creating a channel you will have `Athena:channel_name` , pass `channel_name ` as the  _providerChannel_ . This is a mandatory field for `Sandbox`

        ​

    - `metadata` : This value contains a map of any metadata that you would like us to associate with this request. You can use this field to send data that will map notifications to checkout requests, since we will include it when we send notifications once the checkout is complete. `(Optional)`

```vb 
 
Dim productName As String = "coolproduct"
Dim phoneNumber As String = "+254724587654"
Dim amount As Decimal = 30853
Dim channel As String = "mychannel"
Dim currency As String = "KES"
Dim metadata As Dictionary(Of String, String) = New Dictionary(Of String, String) From
            {
                {"reason", "test reason"}
            }
Dim mobileChckoutResult As String = _gateway.InitiateMobilePaymentCheckout(productName, phoneNumber, currency, amount, channel, metadata)  

```

#### [B2C](http://docs.africastalking.com/payments/mobile-b2c)


- `MobilePaymentB2CRequest(productName,recepients)`:  Initiate payments to mobile subscribers from your payment wallet. [More info](http://docs.africastalking.com/payments/mobile-b2c)

    - `productName`: Your Payment Product. `REQUIRED`

    - `recipients`: A list of **up to 10** recipients info and metadata. Each recipient has:

        - `phoneNumber`: The payee phone number (in international format; e.g. `+25471xxxxxxx`). `REQUIRED`
        - `currencyCode`: 3-digit ISO format currency code (e.g `KES`, `USD`, `UGX` etc.) `REQUIRED`
        - `amount`: Payment amount. `REQUIRED`
        - `metadata`: Some optional data to associate with transaction. 


```vb
            Dim productName As String = "awesomeproduct"
            Dim currencyCode As String = "KES"
            Dim rec1Num As String = "+254723881465"
            Dim rec2Num As String = "+254724587654"
            Dim rec1Name As String = "T'Challa"
            Dim rec2Name As String = "Shuri"
            Dim rec1Amount As Decimal = 15320
            Dim rec2Amount As Decimal = 33500

            Dim rec1 As MobilePaymentB2CRecipient = New MobilePaymentB2CRecipient(rec1Name, rec1Num, currencyCode, rec1Amount)
            Dim rec2 As MobilePaymentB2CRecipient = New MobilePaymentB2CRecipient(rec2Name, rec2Num, currencyCode, rec2Amount)
            rec2.AddMetadata("Reason", "Awesome Tech")
            rec1.AddMetadata("Reason", "Saving the Kingdom")
            Dim recList As IList(Of MobilePaymentB2CRecipient) = New List(Of MobilePaymentB2CRecipient) From
                    {
                    rec1, rec2
                    }
            Dim mobileB2Ctransaction As String = _gateway.MobilePaymentB2CRequest(productName, recList)
```




#### [B2B](http://docs.africastalking.com/payments/mobile-b2b)


- `MobileB2B(productName,provider,transferType,currencyCode,transferAmount,destinationChannel,destinationAccount,metadata)` :  Mobile Business To Business (B2B) APIs allow you to initiate payments TO businesses eg banks FROM your payment wallet. [More info](http://docs.africastalking.com/payments/mobile-b2b) 

    - `productName` :  Your Payment Product as setup on your account. `REQUIRED`   
    - `provider` :  This contains the payment provider that is facilitating this transaction.`REQUIRED` 
       Supported providers at the moment are:  

    ​         

    ```vb
    Dim provider As String = "Athena";
    // or 
    Dim provider As String = "MPESA";
    ```

    ​

    - `transferType`: This is the transfer type for the transaction: `REQUIRED`  

      ```vb
      Dim transferType  As String = "BusinessToBusinessTransfer";
      ' Transfer type can be any of the following: 
        '    BusinessBuyGoods
        '    BusinessPayBill
        '    DisburseFundsToBusiness
        '    BusinessToBusinessTransfer
                    
      ```

      ​

    - `currencyCode`: 3-digit ISO format currency code (e.g `KES`, `USD`, `UGX` etc.) `REQUIRED`

    - `destinationChannel`: This value contains the name or number of the channel that will receive payment by the provider. `REQUIRED`

    - `destinationAccount`: This value contains the account name used by the business to receive money on the provided destinationChannel. `REQUIRED`

    - `transferAmount`: Payment amount. `REQUIRED`

    - `metadata`: Some optional data to associate with transaction. `REQUIRED`   

```vb 

Dim productName As String = "awesomeproduct"
Dim currencyCode As String = "KES"
Dim amount As Decimal = 15
Dim provider As String = "Athena"
Dim destinationChannel As String = "mychannel"
Dim destinationAccount As String = "coolproduct"
Dim metadataDetails As Dictionary(Of String, String) = New Dictionary(Of String, String) From
                {
                    {"Shop Name", "Katanga Hardware"},
                    {"Reason", "Secret Purchase"}
                }
Dim transferType As String = "BusinessToBusinessTransfer"
Dim b2BResult As String = _gateway.MobileB2B(productName, provider, transferType, currencyCode, amount, destinationChannel, destinationAccount, metadataDetails)

```

#### [Banking - Checkout](http://docs.africastalking.com/bank/checkout)


- `BankCheckout(productName,bankAccount,currencyCode,amount,narration,metadata)` : Bank Account checkout APIs allow your application to collect money into your Payment Wallet by initiating an OTP-validated transaction that deducts money from a customer's bank account. These APIs are currently only available in Nigeria.
    - `productName` :  This value identifies the Africa's Talking Payment Product that should be used to initiate this transaction. `REQUIRED`   
    - `bankAccount` :  This is a complex type whose structure is described below. It contains the details of the bank account to be charged in this transaction.  
        - `accountName` :  The name of the bank account. `Optional`
        - `accountNumber` : The account number. `REQUIRED` 
        - `bankCode` :  A 6-Digit Integer Code for the bank that we allocate. `REQURED`. See this [link](http://docs.africastalking.com/bank/checkout) for more details
        - `dateOfBirth` : Date of birth of the account owner. `Optional`/`Required` - for Zenith Bank NG.

    - `currencyCode` : This is the 3-digit ISO format currency code for the value of this transaction (e.g NGN, USD, KES etc). 
    - `amount` : This is the amount (in the provided currency) that the mobile subscriber is expected to confirm. 
    - `narration` : A short description of the transaction that can be displayed on the client's statement. 
    - `metadata` : This value contains a map of any metadata that you would like us to associate with this request. You can use this field to send data that will map notifications to checkout requests, since we will include it when we send notifications once the checkout is complete.


```vb 

Dim productName As String = "coolproduct"
Dim accountName As String = "Fela Kuti"
Dim accountNumber As String = "1234567890"
Dim bankCode As Integer = 234001
Dim currencyCode As String = "NGN"
Dim amount As Decimal = 1000.5D
Dim dob As String = "2017-11-22"
Dim metadata As Dictionary(Of String, String) = New Dictionary(Of String, String) From
                    {
                    {"Reason", "Buy Vega Records"}
                    }
Dim narration As String = "We're buying something cool"
Dim receBank As BankAccount = New BankAccount(accountNumber, bankCode, dob, accountName)
Dim res As String = _gateway.BankCheckout(productName, receBank, currencyCode, amount, narration, metadata)

```



#### [Banking - Transfer](http://docs.africastalking.com/bank/transfer)


- `BankTransfer(productName,recipients)` :  Our API allows you to initiate multiple transactions in one request, all of which will be queued in our gateways for processing.Once the payment provider confirms or rejects the payment request, our APIs will generate a payment notification and send it to the callback URL configured in your account.  [More info](http://docs.africastalking.com/bank/transfer) 

    - `productName` :  This value identifies the Africa's Talking Payment Product that should be used to initiate this transaction. `REQUIRED`   
    - `recipients` :  This contains a list of Recipient elements, each of which corresponds to a B2C Transaction request. The format for                           each of these recipients is described in the table below. `REQUIRED`.
        - `bankAccount` : This is a complex type whose structure is described below. It contains the details of the bank account to be charged in this transaction.  
            - `accountName` :  The name of the bank account. `Optional`
            - `accountNumber` : The account number. `REQUIRED` 
            - `bankCode` :  A 6-Digit Integer Code for the bank that we allocate. `REQURED`. See this [link](http://docs.africastalking.com/bank/transfer) for more details
            - `dateOfBirth` : Date of birth of the account owner. `Optional`/`Required` - for Zenith Bank NG. 

        - `currencyCode` : This is the 3-digit ISO format currency code for the value of this transaction (e.g NGN, USD, KES etc). 
        - `amount` : This is the amount (in the provided currency) that the mobile subscriber is expected to confirm. 
        - `narration` : A short description of the transaction that can be displayed on the client's statement. 
        - `metadata` : This value contains a map of any metadata that you would like us to associate with this request. You can use this field to send data that will map notifications to checkout requests, since we will include it when we send notifications once the checkout is complete.

 ```vb 
    
Dim productname As String = "coolproduct"
Dim currencyCode As String = "NGN"
Dim recipient1AccountName As String = "Alyssa Hacker"
Dim recipient1AccountNumber As String = "1234567890"
Dim recipient1BankCode As Integer = 234001
Dim recipient1Amount As Decimal = 1500.5D
Dim recipient1Narration As String = "December Bonus"
Dim recipient2AccountName As String = "Ben BitDiddle"
Dim recipient2AccountNumber As String = "234567891"
Dim recipient2BankCode As Integer = 234004
Dim recipient2Amount As Decimal = 1500.5D
Dim recipient2Narration As String = "February Bonus"
Dim recipient1Account As BankAccount = New BankAccount(recipient1AccountNumber, recipient1BankCode, recipient1AccountName)
Dim recipientBody As BankTransferRecipients = New BankTransferRecipients(recipient1Amount, recipient1Account, currencyCode, recipient1Narration)
            recipientBody.AddMetadata("Funky", "Metadata")
Dim recipient2BankAccount As BankAccount = New BankAccount(recipient2AccountNumber, recipient2BankCode, recipient2AccountName)
Dim recipient2Body As BankTransferRecipients = New BankTransferRecipients(recipient2Amount, recipient2BankAccount, currencyCode, recipient2Narration)
            recipient2Body.AddMetadata("Another", "Funky Metadata")
Dim recipients As IList(Of BankTransferRecipients) = New List(Of BankTransferRecipients) From
                {
                    recipient2Body, recipientBody
                }
Dim bankTransferResults As String = _gateway.BankTransfer(productname, recipients)

 ```




#### OTP Validation [Banking](http://docs.africastalking.com/bank/validate) and [Card](http://docs.africastalking.com/card/validate) 
> Checkout Validation APIs allow your application to validate bank/card charge requests that deduct money from a customer's bank account.

 > Card Validation

- `OtpValidateCard(transactionId, otp)` :  Payment Card Checkout Validation APIs allow your application to validate card charge requests that deduct money from a customer's Debit or Credit Card. [More info](http://docs.africastalking.com/card/validate) 

    - `transactionId` :This value identifies the transaction that your application wants to validate. This value is contained in the response  to the charge request. `REQUIRED`   
    - `otp` :  This contains the One Time Password that the card issuer sent to the client that owns the payment card. `REQUIRED`.  

 > Bank Validation

- `OtpValidateBank(transactionId, otp)`: Bank Account checkout Validation APIs allow your application to validate bank charge requests that deduct money from a customer's bank account. [More info](http://docs.africastalking.com/bank/validate).
     - `transactionId` :This value identifies the transaction that your application wants to validate. This value is contained in the response  to the charge request. `REQUIRED`   
     - `otp` :  This contains the One Time Password that the card issuer sent to the client that owns the payment card. `REQUIRED`.  




#### [Card Checkout](http://docs.africastalking.com/card/checkout)


- `CardCheckout(productName,paymentCard,currencyCode,amount,narration,metadata)` : Payment Card Checkout APIs allow your application to collect money into your Payment Wallet by initiating transactions that deduct money from a customer's Debit or Credit Card. [More info]((http://docs.africastalking.com/card/checkout) 
- `productName` :  This value identifies the Africa's Talking Payment Product that should be used to initiate this transaction. `REQUIRED`   
    - `paymentCard` :  TThis is a complex type whose structure is described below. It contains the details of the Payment Card to be charged in this transaction. Please note that you can EITHER provider this or provider a checkoutToken if you have one.  `REQUIRED`.
        - `number` : The payment card number. `REQUIRED`
        - `countryCode` :  The 2-Digit countryCode where the card was issued (only NG is supported).. `REQUIRED`
        - `cvvNumber` :  The 3 or 4 digit Card Verification Value. `REQUIRED`
        - `expiryMonth` : The expiration month on the card (e.g 1, 5, 12). `REQUIRED`  
        - `expiryYear` :  The expiration year on the card (e.g 2019) `Optional`
        - `authToken` : The card's ATM PIN. `REQUIRED` 

    - `currencyCode` : This is the 3-digit ISO format currency code for the value of this transaction (e.g NGN, USD, KES etc). 
    - `amount` : This is the amount (in the provided currency) that the mobile subscriber is expected to confirm. 
    - `narration` : A short description of the transaction that can be displayed on the client's statement. 
    - `metadata` : This value contains a map of any metadata that you would like us to associate with this request. You can use this field to send data that will map notifications to checkout requests, since we will include it when we send notifications once the checkout is complete.

```vb 
 
  
Const productName As String = "awesomeproduct"
Const currencyCode As String = "NGN"
Const amount As Decimal = 7500.5D
Const narration As String = "Buy Aluku Records"
Dim metadata As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
                    {"Parent Company", "Offering Records"},
                    {"C.E.O", "Boddhi Satva"}
                    }
Const cardCvv As Short = 123
Const cardNum As String = "123456789012345"
Const countryCode As String = "NG"
Const cardPin As String = "1234"
Const validTillMonth As Integer = 9
Const validTillYear As Integer = 2019
Dim cardDetails As PaymentCard = New PaymentCard(cardPin, countryCode, cardCvv, validTillMonth, validTillYear, cardNum)
Dim checkoutRes As String = _gateway.CardCheckout(productName, cardDetails, currencyCode, amount, narration, metadata)   

```

