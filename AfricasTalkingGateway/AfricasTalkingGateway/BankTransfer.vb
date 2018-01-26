Imports Newtonsoft.Json

Partial Public Class BankTransfer
    <JsonProperty("productName")>
    Public Property ProductName() As String

    <JsonProperty("recipients")>
    Public Property Recipients() As List(Of BankTransferRecipients)

    <JsonProperty("username")>
    Public Property Username() As String
End Class