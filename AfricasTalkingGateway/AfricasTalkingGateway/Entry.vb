Imports Newtonsoft.Json

Public Class Entry

    <JsonProperty("phoneNumber")>
    Public Property PhoneNumber() As String

    <JsonProperty("provider")>
    Public Property Provider() As String

    <JsonProperty("providerChannel")>
    Public Property ProviderChannel() As String

    <JsonProperty("transactionFee")>
    Public Property TransactionFee() As String

    <JsonProperty("status")>
    Public Property Status() As String

    <JsonProperty("value")>
    Public Property Value() As String

    <JsonProperty("transactionId")>
    Public Property TransactionId() As String
End Class


