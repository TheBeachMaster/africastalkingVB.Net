Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace UnitTestAfricasTalkingGateway
    <TestClass>
    Public Class UnitTest1
        Dim username As String = "sandbox"
        Dim apikey As String = "afd635a4f295dd936312836c0b944d55f2a836e8ff2b63987da5e717cd5ff745"
        ReadOnly _gateway As New AfricasTalkingGateway(username, apikey)


        <TestMethod()>
        Public Sub TestSendMessage()
            'Tests that we can successfully Send Messages
            Dim message As String = "It works"
            Dim recipient As String = "+254724587654"
            Dim results As String = _gateway.SendMessage(recipient, message)
            ' Let's parse that result and ensure the contents does not have any Error messages 
            Dim statusMessage As Boolean = results.Contains("Success")
            Assert.IsTrue(statusMessage)
        End Sub

        <TestMethod()>
        Public Sub TestBulkSmsMode()
            Dim message As String = "This is a bulk SMS message"
            Dim recipients As String = "+254724587654,+254791854473,+254712965433"
            Dim result As String = _gateway.SendMessage(recipients, message)
            ' Let's ensure we can send bulk sms to comma separated numbers
            Dim bulkSmsStatus
        End Sub
    End Class
End Namespace

