
<Serializable>
Friend Class AfricasTalkingGatewayException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal ex As Exception)
        MyBase.New(ex.Message, ex)

    End Sub
End Class

