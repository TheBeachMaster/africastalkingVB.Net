
Imports Newtonsoft.Json

Public Class CheckoutData
    <JsonProperty("username")>
    Public Property Username() As String
    <JsonProperty("productName")>
    Public Property ProductName() As String
    <JsonProperty("phoneNumber")>
    Public Property PhoneNumber() As String
    <JsonProperty("currencyCode")>
    Public Property CurrencyCode() As String
    <JsonProperty("amount")>
    Public Property Amount() As Decimal
    <JsonProperty("providerChannel")>
    Public Property ProviderChannel() As String
    <JsonProperty("metadata")>
    Public Property Metadata() As Dictionary(Of String, String)
    Public Function ToJson() As String
        Dim checkoutObject As String = JsonConvert.SerializeObject(Me)
        Return checkoutObject
    End Function
End Class
