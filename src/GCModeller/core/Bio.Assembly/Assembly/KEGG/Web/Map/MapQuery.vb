#Region "Microsoft.VisualBasic::2a78c6813d3c4c7859618ae7bf057f8b, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\MapQuery.vb"

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

'   Total Lines: 52
'    Code Lines: 44
' Comment Lines: 0
'   Blank Lines: 8
'     File Size: 2.04 KB


'     Class MapQuery
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: createUrl, getID, ParseHtml
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Net.Http
Imports PathwayEntry = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' web query module for the kegg pathway map
    ''' </summary>
    Public Class MapQuery : Inherits WebQueryModule(Of PathwayEntry)

        Public ReadOnly Property FileSystem As IFileSystemEnvironment
            Get
                Return cache
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            Call MyBase.New(
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(cacheFs As IFileSystemEnvironment,
                Optional interval% = -1,
                Optional offline As Boolean = False)

            Call MyBase.New(cacheFs, interval, offline)
        End Sub

        Friend Shared Function getID(entry As PathwayEntry) As String
            If entry.entry.name.IsPattern("\d+") Then
                Return "map" & entry.EntryId
            Else
                Dim s As String = entry.entry.text
                s = r.Match(s, "\[PATH:.+?\]", RegexICSng).Value
                s = s.GetStackValue("[", "]").Split(":"c).Last
                Return s
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function doParseUrl(context As PathwayEntry) As String
            Return $"https://www.kegg.jp/pathway/{getID(context)}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function doParseObject(html As String, schema As Type) As Object
            Return ParseHtmlExtensions.ParseHTML(html, fs:=cache)
        End Function

        Protected Overrides Function doParseGuid(context As PathwayEntry) As String
            If TypeOf cache Is IFileSystemEnvironment Then
                Dim md5 As String = context.EntryId.MD5
                Dim prefix As String = "/.cache/" & md5.Substring(7, 2) & "/" & md5

                Return prefix
            Else
                Return context.EntryId.MD5
            End If
        End Function
    End Class
End Namespace
