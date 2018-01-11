Public Class AfricasTalkingGateway
    Private _username As String
    Private _apikey As String
    Private _environment As String

    Public Sub New(ByVal username As String, ByVal apikey As String)
        _username = username
        _apikey = apikey
        _environment = GetDefaultAuthEnv(username)
    End Sub

    Private Shared Function GetDefaultAuthEnv(ByVal username As String) As String
        Dim env As String
        If username = "sandbox" Then
            env = "sandbox"
        Else
            env = "production"
        End If
        Return env
    End Function
End Class
