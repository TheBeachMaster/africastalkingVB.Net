Imports Newtonsoft.Json

Partial Public Class OTP
    <JsonProperty("otp")>
    Public Property Otp() As String

    <JsonProperty("transactionId")>
    Public Property TransactionId() As String

    <JsonProperty("username")>
    Public Property Username() As String
End Class

