#Region "Microsoft.VisualBasic::9536ca0b5058be84ff4fa40893be1f5a, src\Flute\HttpMessage\Protocol\Cookies.vb"

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

    '   Total Lines: 56
    '    Code Lines: 41 (73.21%)
    ' Comment Lines: 3 (5.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (21.43%)
    '     File Size: 1.93 KB


    '     Class Cookies
    ' 
    '         Function: CheckCookie, GetCookie, GetReader, ParseCookies, ToJSON
    '                   ToString
    ' 
    '         Sub: SetValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core.Message

    ''' <summary>
    ''' a collection of http cookies parsed from or serialized to the
    ''' <c>Cookie</c> request header. keys are stored in lower case for
    ''' case-insensitive lookups.
    ''' </summary>
    Public Class Cookies

        ''' <summary>
        ''' all key names is in lower case
        ''' </summary>
        Dim cookies As Dictionary(Of String, String)

        ''' <summary>
        ''' check whether a cookie with the given (case-insensitive) name exists.
        ''' </summary>
        ''' <param name="name">the cookie name to look up.</param>
        ''' <returns><c>True</c> when the cookie is present.</returns>
        Public Function CheckCookie(name As String) As Boolean
            Return cookies.ContainsKey(name.ToLower)
        End Function

        ''' <summary>
        ''' get a cookie value by name (case-insensitive lookup).
        ''' </summary>
        ''' <param name="name">the cookie name.</param>
        ''' <returns>the cookie value, or empty when not present.</returns>
        Public Function GetCookie(name As String) As String
            Return cookies.TryGetValue(name.ToLower)
        End Function

        ''' <summary>
        ''' add or replace a cookie value in the collection (key stored lower case).
        ''' </summary>
        ''' <param name="name">the cookie name.</param>
        ''' <param name="value">the cookie value.</param>
        Public Sub SetValue(name As String, value As String)
            cookies(name) = value
        End Sub

        ''' <summary>
        ''' get a line-oriented reader over the name/value cookie dictionary.
        ''' </summary>
        ''' <returns>a <see cref="StringReader"/> wrapping the cookie pairs.</returns>
        Public Function GetReader() As StringReader
            Return StringReader.WrapDictionary(cookies)
        End Function

        ''' <summary>
        ''' parse a raw <c>Cookie</c> header value (semicolon separated name=value
        ''' pairs) into a <see cref="Cookies"/> collection; values sharing a name
        ''' are joined with "; ".
        ''' </summary>
        ''' <param name="cookies">the raw cookie header value, or empty for an empty collection.</param>
        ''' <returns>a populated <see cref="Cookies"/> instance.</returns>
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

        ''' <summary>
        ''' the json array of the cookie names held in this collection.
        ''' </summary>
        ''' <returns>the json string of the cookie name list.</returns>
        Public Overrides Function ToString() As String
            Return cookies.Keys.AsEnumerable.GetJson
        End Function

        ''' <summary>
        ''' serialize the cookie collection (name/value) to json.
        ''' </summary>
        ''' <returns>the json representation of the cookies.</returns>
        Public Function ToJSON() As String
            Return cookies.GetJson
        End Function

    End Class
End Namespace
