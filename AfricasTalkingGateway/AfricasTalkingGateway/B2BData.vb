Imports Newtonsoft.Json
Public Class B2BData
    Public Property Username() As String
    Public Property ProductName() As String
    Public Property Provider() As String
    Public Property TransferType() As String
    Public Property CurrencyCode() As String
    Public Property Amount() As Decimal
    Public Property DestinationChannel() As String
    Public Property DestinationAccount() As String
    Public Property Metadata As Dictionary(Of String, String)

    Public Overrides Function ToString() As String
        Dim json As String = JsonConvert.SerializeObject(Me)
        Return json
    End Function
End Class

