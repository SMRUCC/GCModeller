#Region "Microsoft.VisualBasic::7ef6fedd58e1693b8b075db8463186df, ..\httpd\WebCloud\SMRUCC.WebCloud.QRCode\test\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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

