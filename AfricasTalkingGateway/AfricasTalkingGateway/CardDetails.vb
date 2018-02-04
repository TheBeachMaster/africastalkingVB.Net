Imports Newtonsoft.Json

Partial Public Class CardDetails
    'Public Sub New(amount As Decimal, currencyCode As String, narration As String, paymentCard As PaymentCard, productName As String, username As String, Optional _
    '                  ByVal checkoutToken As String = Nothing, Optional ByVal metadata As Dictionary(Of String, String) = Nothing)
    '    Me.Amount = amount
    '    Me.CheckoutToken = checkoutToken
    '    Me.CurrencyCode = currencyCode
    '    Me.Metadata = metadata
    '    Me.Narration = narration
    '    Me.PaymentCard = paymentCard
    '    Me.ProductName = productName
    '    Me.Username = username
    'End Sub

    <JsonProperty("amount")>
    Public Property Amount() As Decimal

    <JsonProperty("checkoutToken")>
    Public Property CheckoutToken() As String

    <JsonProperty("currencyCode")>
    Public Property CurrencyCode() As String

    <JsonProperty("metadata")>
    Public Property Metadata() As Dictionary(Of String, String)

    <JsonProperty("narration")>
    Public Property Narration() As String

    <JsonProperty("paymentCard")>
    Public Property PaymentCard() As PaymentCard

    <JsonProperty("productName")>
    Public Property ProductName() As String

    <JsonProperty("username")>
    Public Property Username() As String

    Public Function ToJson() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function
End Class
