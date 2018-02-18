Imports Newtonsoft.Json

Public Class MobilePaymentB2CRecipient
    <JsonProperty("name")>
    Public Property Name() As String

    <JsonProperty("phoneNumber")>
    Public Property PhoneNumber() As String

    <JsonProperty("currencyCode")>
    Public Property CurrencyCode() As String

    <JsonProperty("amount")>
    Public Property Amount() As Decimal

    <JsonProperty("metadata")>
    Public Property Metadata As Dictionary(Of String, String)

    Public Sub New(name As String, phoneNumber As String, currencyCode As String, amount As Decimal)
        Me.Name = name
        Me.PhoneNumber = phoneNumber
        Me.CurrencyCode = currencyCode
        Me.Amount = amount
        Metadata = New Dictionary(Of String, String)()
    End Sub

    Public Sub AddMetadata(ByVal key As String, ByVal value As String)
        Metadata.Add(key, value)
    End Sub

    Public Function ToJson() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function

End Class



