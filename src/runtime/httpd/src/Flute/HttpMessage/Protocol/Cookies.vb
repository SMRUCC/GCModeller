#Region "Microsoft.VisualBasic::e757b6c0a4d4fc47b0068529e63755bc, G:/GCModeller/src/runtime/httpd/src/Flute//HttpMessage/Protocol/Cookies.vb"

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

    '   Total Lines: 34
    '    Code Lines: 21
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 998 B


    '     Class Cookies
    ' 
    '         Function: CheckCookie, GetCookie, GetReader, ParseCookies, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core.Message

    Public Class Cookies

        ''' <summary>
        ''' all key names is in lower case
        ''' </summary>
        Dim cookies As Dictionary(Of String, String)

        Public Function CheckCookie(name As String) As Boolean
            Return cookies.ContainsKey(name.ToLower)
        End Function

        Public Function GetCookie(name As String) As String
            Return cookies.TryGetValue(name.ToLower)
        End Function

        Public Sub SetValue(name As String, value As String)
            cookies(name) = value
        End Sub

        Public Function GetReader() As StringReader
            Return StringReader.WrapDictionary(cookies)
        End Function

        Public Shared Function ParseCookies(cookies As String) As Cookies
            If cookies.StringEmpty Then
                Return New Cookies With {
                    .cookies = New Dictionary(Of String, String)
                }
            Else
                Dim t As String() = cookies.StringSplit("; ")
                Dim kv = t.Select(Function(ti) ti.GetTagValue("=", trim:=True)) _
                    .GroupBy(Function(ti) ti.Name.ToLower) _
                    .ToDictionary(Function(ti) ti.Key,
                                  Function(ti)
                                      Return ti.Select(Function(s) s.Value).JoinBy("; ")
                                  End Function)

                Return New Cookies With {.cookies = kv}
            End If
        End Function

        Public Overrides Function ToString() As String
            Return cookies.Keys.AsEnumerable.GetJson
        End Function

        Public Function ToJSON() As String
            Return cookies.GetJson
        End Function

    End Class
End Namespace
