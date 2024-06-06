#Region "Microsoft.VisualBasic::30ba65853c742c3f4507040785ad398d, graphquery\kegg\src\kegg_api\Html\WebQuery.vb"

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

    '   Total Lines: 82
    '    Code Lines: 66 (80.49%)
    ' Comment Lines: 3 (3.66%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (15.85%)
    '     File Size: 3.06 KB


    '     Class WebQuery
    ' 
    '         Properties: FileSystem
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: doParseGuid, doParseObject, doParseUrl, getID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports entry = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
Imports r = System.Text.RegularExpressions.Regex

Namespace Html

    ''' <summary>
    ''' web query module for the kegg pathway map
    ''' </summary>
    Public Class WebQuery : Inherits WebQueryModule(Of entry)

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

        Friend Shared Function getID(entry As entry) As String
            If entry.entry.name.IsPattern("\d+") Then
                Return "map" & entry.EntryId
            ElseIf entry.EntryId.IsPattern("map\d+") OrElse entry.EntryId.IsPattern("[a-z]+\d+") Then
                Return entry.EntryId
            Else
                Dim s As String = entry.entry.text
                s = r.Match(s, "\[PATH:.+?\]", RegexICSng).Value
                s = s.GetStackValue("[", "]").Split(":"c).Last
                Return s
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function doParseUrl(context As entry) As String
            Return $"https://www.kegg.jp/pathway/{getID(context)}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function doParseObject(html As String, schema As Type) As Object
            html = Strings.Trim(html).Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF)

            If html.StringEmpty Then
                Return Nothing
            Else
                Return ParseHtmlExtensions.ParseHTML(html, fs:=cache)
            End If
        End Function

        Protected Overrides Function doParseGuid(context As entry) As String
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
