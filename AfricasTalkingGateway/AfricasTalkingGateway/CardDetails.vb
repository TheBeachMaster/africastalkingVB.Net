Imports Newtonsoft.Json

Partial Public Class CardDetails
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
End Class
