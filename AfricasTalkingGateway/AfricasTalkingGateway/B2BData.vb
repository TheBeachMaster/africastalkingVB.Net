Imports Newtonsoft.Json
Public Class B2BData
    <JsonProperty("username")>
    Public Property Username() As String
    <JsonProperty("productName")>
    Public Property ProductName() As String
    <JsonProperty("provider")>
    Public Property Provider() As String
    <JsonProperty("transferType")>
    Public Property TransferType() As String
    <JsonProperty("currencyCode")>
    Public Property CurrencyCode() As String
    <JsonProperty("amount")>
    Public Property Amount() As Decimal
    <JsonProperty("destinationChannel")>
    Public Property DestinationChannel() As String
    <JsonProperty("destinationAccount")>
    Public Property DestinationAccount() As String
    <JsonProperty("metadata")>
    Public Property Metadata As Dictionary(Of String, String)

    Public Function ToJson() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function
End Class

