Imports Newtonsoft.Json
Partial Public Class BankCheckout
    <JsonProperty("amount")>
    Public Property Amount() As Decimal
    <JsonProperty("bankAccount")>
    Public Property BankAccount() As BankAccount
    <JsonProperty("currencyCode")>
    Public Property CurrencyCode() As String
    <JsonProperty("metadata")>
    Public Property Metadata() As Dictionary(Of String, String)
    <JsonProperty("narration")>
    Public Property Narration() As String
    <JsonProperty("productName")>
    Public Property ProductName() As String
    <JsonProperty("username")>
    Public Property Username() As String

    Public Function ToJson() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function
End Class
