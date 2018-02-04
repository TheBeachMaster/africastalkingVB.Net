Imports Newtonsoft.Json

Partial Public Class BankTransfer
    <JsonProperty("productName")>
    Public Property ProductName() As String

    <JsonProperty("recipients")>
    Public Property Recipients() As List(Of BankTransferRecipients)

    <JsonProperty("username")>
    Public Property Username() As String

    Public Function ToJson() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function
End Class