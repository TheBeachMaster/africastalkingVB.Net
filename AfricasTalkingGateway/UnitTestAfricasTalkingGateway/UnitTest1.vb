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
        Public Sub TestBulkSms()
            Dim message As String = "This is a bulk SMS message"
            Dim recipients As String = "+254714587654,+254791854473,+254712965433"
            Dim result As String = _gateway.SendMessage(recipients, message)
            ' Let's ensure we can send bulk sms to comma separated numbers
            Dim bulkSmsStatus As Boolean = result.Contains("Success")
            Assert.IsTrue(bulkSmsStatus)
        End Sub

        <TestMethod()>
        Public Sub TestFetchMessages()
            Dim receivedId As Integer = 0
            Dim result As String = _gateway.FetchMessages(receivedId)
            ' Fetch messages should contain IDs or SMSMessageData (if empty)
            Dim messageIds As Boolean = result.Contains("SMSMessageData")
            Assert.IsTrue(messageIds)
        End Sub

        <TestMethod()>
        Public Sub TestTokenCreation()
            Dim phoneNumber As String = "+254724587654"
            Dim tokenResult As String = _gateway.CreateCheckoutToken(phoneNumber)
            ' Expect a success message 
            Dim tokenStatus As Boolean = tokenResult.Contains("Success")
            Assert.IsTrue(tokenStatus)
        End Sub

        <TestMethod()>
        Public Sub TestCreateSubscription()
            Dim phoneNumber As String = "+254724587654"
            Dim shortcode As String = "44005"
            Dim keyword As String = "coolguy"
            Dim token As String = "CkTkn_860f9ee9-fb1e-4322-b1f4-7e0c1d00b12c"
            Dim result As String = _gateway.CreateSubscription(phoneNumber, shortcode, keyword, token)
            Dim resStatus As Boolean = result.Contains("Success")
            Assert.IsTrue(resStatus)
        End Sub

        <TestMethod()>
        Public Sub TestAirtimeService() ' WIP
            Dim airtimeData As New ArrayList
            airtimeData.Add("'phoneNumber': '+254714587654','amount':'KES 250'")
            airtimeData.Add("'phoneNumber':'+254791854473','amount':'KES 200'")
            airtimeData.Add("'phoneNumber':'+254712965433','amount':'KES 100'")
            Dim airtimeTransact As String = _gateway.SendAirtime(airtimeData)
            Dim status As Boolean = airtimeTransact.Contains("Sent")
            Assert.IsTrue(status)
        End Sub

        <TestMethod()>
        Public Sub TestCallService()
            Dim caller As String = "+254724587654"
            'Dim recipients As String = "+254714587654,+254791854473,+254712965433"
            Dim recipients As String = "+254714587654"
            Dim callResult As String = _gateway.Call(caller, recipients)
            Dim callStatus As Boolean = callResult.Contains("Queued")
            Assert.IsTrue(callStatus)
        End Sub

        <TestMethod()>
        Public Sub TestCallMultipleNumbers()
            Dim caller As String = "+254724587654"
            Dim recipients As String = "+254714587654,+254791854473,+254712965433"
            Dim callResult As String = _gateway.Call(caller, recipients)
            Dim callStatus As Boolean = callResult.Contains("Queued")
            Assert.IsTrue(callStatus)
        End Sub
    End Class
End Namespace

