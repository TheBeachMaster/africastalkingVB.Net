Imports Newtonsoft.Json

Public Class RequestBody
    Public Sub New()
        Recipients = New List(Of MobilePaymentB2CRecipient)()
    End Sub
    <JsonProperty("username")>
    Public Property UserName() As String

    <JsonProperty("productName")>
    Public Property ProductName() As String

    <JsonProperty("recipients")>
    Public Property Recipients() As List(Of MobilePaymentB2CRecipient)

    Public Function ToJson() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function
End Class


