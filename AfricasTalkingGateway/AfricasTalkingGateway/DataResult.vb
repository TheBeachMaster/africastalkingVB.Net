Imports Newtonsoft.Json

Public Class DataResult
    <JsonProperty("numQueued")>
    Public Property NumQueued() As Integer

    <JsonProperty("entries")>
    Public Property Entries() As IList(Of Entry)

    <JsonProperty("totalValue")>
    Public Property TotalValue() As String

    <JsonProperty("totalTransactionFee")>
    Public Property TotalTransactionFee() As String

    Public Function ToJson() As String
        Dim result As String = JsonConvert.SerializeObject(Me)
        Return result
    End Function
End Class


