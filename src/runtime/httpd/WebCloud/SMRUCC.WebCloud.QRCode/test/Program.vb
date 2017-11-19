Imports System.Collections.Generic
Imports System.ComponentModel
Imports SMRUCC.WebCloud.QRCode

Class Program
    Public Shared Sub Main(args As String())
        Dim code As QRCodeCreator = Nothing

        code = New QRCodeCreator("01234567")
        code.Save("Small Numeric.png", 4)

        code = New QRCodeCreator("SMALL TEXT")
        code.Save("Small Text.png", 4)

        code = New QRCodeCreator("12345678912345678912345679")
        code.Save("Longer Numeric.png", 4)

        code = New QRCodeCreator("MORE TEXT THAT IS LONGER AND NEEDS A BIGGER CODE")
        code.Save("Longer Text.png", 4)

        code = New QRCodeCreator("Bytes needed.")
        code.Save("Small Bytes.png", 4)

        code = New QRCodeCreator("This is a longer message that will take a bigger QR code to fit.  The small ones just won't be big enough.  Lets see what happens.")
        code.Save("Longer Bytes.png", 4)
    End Sub
End Class
