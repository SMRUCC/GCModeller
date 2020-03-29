#Region "Microsoft.VisualBasic::b85883f305921f67d98c42e6b83c2f92, markdown2pdf\PdfConvert\Arguments\Arguments.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class PdfOutput
    ' 
    '         Properties: OutputCallback, OutputFilePath, OutputStream
    ' 
    '         Function: ToString
    ' 
    '     Class PdfConvertEnvironment
    ' 
    '         Properties: Debug, TempFolderPath, Timeout, WkHtmlToPdfPath
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Arguments

    Public Class PdfOutput

        Public Property OutputFilePath As String
        Public Property OutputStream As Stream
        Public Property OutputCallback As Action(Of PDFContent, Byte())

        Public Overrides Function ToString() As String
            Return OutputFilePath
        End Function

    End Class

    Public Class PdfConvertEnvironment

        Public Property TempFolderPath As String
        Public Property WkHtmlToPdfPath As String
        Public Property Timeout As Integer
        Public Property Debug As Boolean

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
