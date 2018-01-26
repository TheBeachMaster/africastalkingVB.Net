Imports Newtonsoft.Json

Partial Public Class BankAccount
    Public Sub New(accountNumber As String, bankCode As Integer, Optional ByVal dateOfBirth As String = Nothing, Optional ByVal accountName As String = Nothing)
        Me.AccountName = accountName
        Me.AccountNumber = accountNumber
        Me.BankCode = bankCode
        Me.DateOfBirth = dateOfBirth
    End Sub

    <JsonProperty("accountName")>
    Public Property AccountName() As String
    <JsonProperty("accountNumber")>
    Public Property AccountNumber() As String
    <JsonProperty("bankCode")>
    Public Property BankCode() As Integer
    <JsonProperty("dateOfBirth")>
    Public Property DateOfBirth() As String
End Class
