Imports Newtonsoft.Json

Partial Public Class PaymentCard

    Public Sub New(authToken As String, countryCode As String, cvvNumber As Short, expiryMonth As Integer, expiryYear As Integer, number As String)
        Me.AuthToken = authToken
        Me.CountryCode = countryCode
        Me.CvvNumber = cvvNumber
        Me.ExpiryMonth = expiryMonth
        Me.ExpiryYear = expiryYear
        Me.Number = number
    End Sub
    <JsonProperty("authToken")>
    Public Property AuthToken() As String
    <JsonProperty("countryCode")>
    Public Property CountryCode() As String
    <JsonProperty("cvvNumber")>
    Public Property CvvNumber() As Short
    <JsonProperty("expiryMonth")>
    Public Property ExpiryMonth() As Integer
    <JsonProperty("expiryYear")>
    Public Property ExpiryYear() As Integer
    <JsonProperty("number")>
    Public Property Number() As String
End Class
