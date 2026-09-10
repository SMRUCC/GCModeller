#Region "Microsoft.VisualBasic::77930cdbfb80d17affe8e028618d77fb, src\Flute\Http\HttpStream\HttpMultipart.vb"

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

    '   Total Lines: 266
    '    Code Lines: 208 (78.20%)
    ' Comment Lines: 22 (8.27%)
    '    - Xml Docs: 31.82%
    ' 
    '   Blank Lines: 36 (13.53%)
    '     File Size: 9.56 KB


    '     Class HttpMultipart
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CompareBytes, GetContentDispositionAttribute, GetContentDispositionAttributeWithEncoding, MoveToNextBoundary, ReadBoundary
    '                   ReadHeaders, ReadLine, ReadNextElement, StripPath
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

Namespace Core.HttpStream

    ''' <summary>
    ''' Stream-based multipart handling.
    '''
    ''' In this incarnation deals with an HttpInputStream as we are now using
    ''' IntPtr-based streams instead of byte [].   In the future, we will also
    ''' send uploads above a certain threshold into the disk (to implement
    ''' limit-less HttpInputFiles). 
    ''' </summary>
    Public Class HttpMultipart

        Dim data As Stream
        Dim boundary As String
        Dim boundary_bytes As Byte()
        Dim buffer As Byte()
        Dim at_eof As Boolean
        Dim encoding As Encoding
        Dim sb As StringBuilder

        ' See RFC 2046 
        ' In the case of multipart entities, in which one or more different
        ' sets of data are combined in a single body, a "multipart" media type
        ' field must appear in the entity's header.  The body must then contain
        ' one or more body parts, each preceded by a boundary delimiter line,
        ' and the last one followed by a closing boundary delimiter line.
        ' After its boundary delimiter line, each body part then consists of a
        ' header area, a blank line, and a body area.  Thus a body part is
        ' similar to an RFC 822 message in syntax, but different in meaning.

        Public Sub New(data As Stream, b As String, encoding As Encoding)
            Me.data = data
            boundary = b
            boundary_bytes = encoding.GetBytes(b)
            ' the buffer must hold the boundary plus the trailing CRLF (or the
            ' closing ``--``), otherwise MoveToNextBoundary can never validate
            ' the delimiter line.
            buffer = New Byte(boundary_bytes.Length + 2) {}
            ' CRLF or '--'
            Me.encoding = encoding
            sb = New StringBuilder()
        End Sub

        Private Function ReadLine() As String
            ' CRLF or LF are ok as line endings.
            Dim got_cr As Boolean = False
            Dim b As Integer = 0

            sb.Length = 0

            While True
                b = data.ReadByte()
                If b = -1 Then
                    Return Nothing
                End If

                If b = ASCII.Byte.LF Then
                    Exit While
                End If
                got_cr = (b = ASCII.Byte.CR)
                sb.Append(ChrW(b))
            End While

            If got_cr Then
                sb.Length -= 1
            End If

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' find the position of a content-disposition attribute (for example
        ''' ``name=`` or ``filename=``), making sure the match is a real attribute
        ''' (preceded by a space or ``;``) so that ``name=`` does not match inside
        ''' ``filename=``.
        ''' </summary>
        Private Shared Function findAttribute(l As String, name As String) As Integer
            Dim token As String = name & "="
            Dim from As Integer = 0

            While from < l.Length
                Dim idx As Integer = l.IndexOf(token, from, StringComparison.OrdinalIgnoreCase)
                If idx < 0 Then
                    Return -1
                End If
                If idx = 0 OrElse l(idx - 1) = " "c OrElse l(idx - 1) = ";"c Then
                    Return idx
                End If
                from = idx + 1
            End While

            Return -1
        End Function

        ''' <summary>
        ''' read an attribute value that may be either quoted (``name="value"``)
        ''' or a bare token (``name=value``), as produced by the .NET HttpClient
        ''' multipart writer.
        ''' </summary>
        Private Shared Function readAttributeValue(l As String, idx As Integer, name As String) As String
            Dim begin As Integer = idx + name.Length + 1

            If begin >= l.Length Then
                Return Nothing
            End If

            If l(begin) = """"c Then
                Dim [end] As Integer = l.IndexOf(""""c, begin + 1)
                If [end] < 0 Then
                    Return Nothing
                End If
                If begin + 1 = [end] Then
                    Return ""
                End If
                Return l.Substring(begin + 1, [end] - begin - 1)
            Else
                Dim [end] As Integer = l.IndexOf(";"c, begin)
                If [end] < 0 Then
                    [end] = l.Length
                End If
                Return l.Substring(begin, [end] - begin).Trim()
            End If
        End Function

        Private Shared Function GetContentDispositionAttribute(l As String, name As String) As String
            Dim idx As Integer = findAttribute(l, name)
            If idx < 0 Then
                Return Nothing
            End If
            Return readAttributeValue(l, idx, name)
        End Function

        Private Function GetContentDispositionAttributeWithEncoding(l As String, name As String) As String
            Dim value As String = GetContentDispositionAttribute(l, name)
            If value Is Nothing Then
                Return Nothing
            End If

            Dim source As Byte() = New Byte(value.Length - 1) {}
            For i As Integer = value.Length - 1 To 0 Step -1
                source(i) = CByte(AscW(value(i)))
            Next

            Return encoding.GetString(source)
        End Function

        Private Function ReadBoundary() As Boolean
            Try
                Dim line As String = ReadLine()
                While line = ""
                    line = ReadLine()
                End While
                If line(0) <> "-"c OrElse line(1) <> "-"c Then
                    Return False
                End If

                If Not StrUtils.EndsWith(line, boundary, False) Then
                    Return True
                End If
            Catch
            End Try

            Return False
        End Function

        Private Function ReadHeaders() As String
            Dim s As String = ReadLine()
            If s = "" Then
                Return Nothing
            End If

            Return s
        End Function

        Private Function CompareBytes(orig As Byte(), other As Byte()) As Boolean
            For i As Integer = orig.Length - 1 To 0 Step -1
                If orig(i) <> other(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Function MoveToNextBoundary() As Long
            Dim retval As Long = 0
            Dim got_cr As Boolean = False

            Dim state As Integer = 0
            Dim c As Integer = data.ReadByte()
            While True
                If c = -1 Then
                    Return -1
                End If

                If state = 0 AndAlso c = ASCII.Byte.LF Then
                    retval = data.Position - 1
                    If got_cr Then
                        retval -= 1
                    End If
                    state = 1
                    c = data.ReadByte()
                ElseIf state = 0 Then
                    got_cr = (c = ASCII.Byte.CR)
                    c = data.ReadByte()
                ElseIf state = 1 AndAlso c = ASCII.Byte.Hyphen Then
                    c = data.ReadByte()
                    If c = -1 Then
                        Return -1
                    End If

                    If c <> ASCII.Byte.Hyphen Then
                        state = 0
                        got_cr = False
                        ' no ReadByte() here
                        Continue While
                    End If

                    Dim nread As Integer = data.Read(buffer, 0, buffer.Length)
                    Dim bl As Integer = buffer.Length
                    If nread <> bl Then
                        Return -1
                    End If

                    If Not CompareBytes(boundary_bytes, buffer) Then
                        Call $"multipart debug: boundary mismatch, buffer='{encoding.GetString(buffer)}' expected='{boundary}'".warning()
                        state = 0
                        data.Position = retval + 2
                        If got_cr Then
                            data.Position += 1
                            got_cr = False
                        End If
                        c = data.ReadByte()
                        Continue While
                    End If

                    If buffer(bl - 2) = ASCII.Byte.Hyphen AndAlso buffer(bl - 1) = ASCII.Byte.Hyphen Then
                        at_eof = True
                    ElseIf buffer(bl - 2) <> ASCII.Byte.CR OrElse buffer(bl - 1) <> ASCII.Byte.LF Then
                        state = 0
                        data.Position = retval + 2
                        If got_cr Then
                            data.Position += 1
                            got_cr = False
                        End If
                        c = data.ReadByte()
                        Continue While
                    End If
                    data.Position = retval + 2
                    If got_cr Then
                        data.Position += 1
                    End If
                    Exit While
                Else
                    ' state == 1
                    ' no ReadByte() here
                    state = 0
                End If
            End While

            Return retval
        End Function

        ''' <summary>
        ''' read the next multipart body part from the stream and return its
        ''' <see cref="StreamElement"/> descriptor (name, filename, content type,
        ''' bounds and length). returns <c>Nothing</c> when the stream has ended.
        ''' </summary>
        ''' <returns>the next <see cref="StreamElement"/>, or <c>Nothing</c> at end of stream.</returns>
        Friend Function ReadNextElement() As StreamElement
            If at_eof OrElse ReadBoundary() Then
                Call $"multipart debug: end of stream (at_eof={at_eof})".info()
                Return Nothing
            End If

            Dim elem As New StreamElement()
            Dim header As New Value(Of String)
            While (header = ReadHeaders()) IsNot Nothing
                If StrUtils.StartsWith(header.Value, "Content-Disposition:", True) Then
                    elem.Name = GetContentDispositionAttribute(header.Value, "name")
                    elem.Filename = StripPath(GetContentDispositionAttributeWithEncoding(header.Value, "filename"))
                ElseIf StrUtils.StartsWith(header.Value, "Content-Type:", True) Then
                    elem.ContentType = header.Value.Substring("Content-Type:".Length).Trim()
                End If
            End While

            Call $"multipart debug: element name='{elem.Name}', filename='{elem.Filename}'".info()

            Dim start As Long = data.Position
            elem.Start = start
            Dim pos As Long = MoveToNextBoundary()
            If pos = -1 Then
                Call "multipart debug: next boundary was not found".warning()
                Return Nothing
            End If

            elem.Length = pos - start
            Return elem
        End Function

        Private Shared Function StripPath(path As String) As String
            If path Is Nothing OrElse path.Length = 0 Then
                Return path
            End If

            If path.IndexOf(":\") <> 1 AndAlso Not path.StartsWith("\\") Then
                Return path
            End If
            Return path.Substring(path.LastIndexOf("\"c) + 1)
        End Function
    End Class
End Namespace
