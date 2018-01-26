
Imports Newtonsoft.Json
''' This contains a list of Recipient elements.
''' 
Partial Public Class BankTransferRecipients
    Public Sub New(amount As Decimal, bankAccount As BankAccount, currencyCode As String, narration As String)
        Me.Amount = amount
        Me.BankAccount = bankAccount
        Me.CurrencyCode = currencyCode
        Metadata = New Dictionary(Of String, String)()
        Me.Narration = narration
    End Sub

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

    Public Sub AddMetadata(key As String, value As String)
        Metadata.Add(key, value)
    End Sub
End Class