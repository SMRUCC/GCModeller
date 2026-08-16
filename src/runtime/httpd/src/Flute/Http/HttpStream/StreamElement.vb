#Region "Microsoft.VisualBasic::95c54a79128dffdd239e152ce2fb7c76, src\Flute\Http\HttpStream\StreamElement.vb"

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


    ' Code Statistics:

    '   Total Lines: 15
    '    Code Lines: 12 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (20.00%)
    '     File Size: 497 B


    '     Class StreamElement
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Core.HttpStream

    ''' <summary>
    ''' a descriptor for a single element within a multipart/form-data stream,
    ''' recording its header metadata and the byte range it occupies.
    ''' </summary>
    Friend Class StreamElement

        ''' <summary>
        ''' the content type (mime) of this multipart element.
        ''' </summary>
        Public ContentType As String
        ''' <summary>
        ''' the form field name of this multipart element.
        ''' </summary>
        Public Name As String
        ''' <summary>
        ''' the file name of this multipart element when it is a file upload.
        ''' </summary>
        Public Filename As String
        ''' <summary>
        ''' the start byte offset of the element data within the source stream.
        ''' </summary>
        Public Start As Long
        ''' <summary>
        ''' the byte length of the element data.
        ''' </summary>
        Public Length As Long

        ''' <summary>
        ''' a human readable description of this multipart element.
        ''' </summary>
        ''' <returns>the string representation of the element metadata.</returns>
        Public Overrides Function ToString() As String
            Return "ContentType " & ContentType & ", Name " & Name & ", Filename " & Filename & ", Start " & Start.ToString() & ", Length " & Length.ToString()
        End Function
    End Class
End Namespace
