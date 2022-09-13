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
Imports Microsoft.VisualBasic.Net.Http
Imports PathwayEntry = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    Public Class MapQuery : Inherits WebQuery(Of PathwayEntry)

        Public Sub New(briteFile$,
                       <CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            Call MyBase.New(
                createUrl(briteFile),
                AddressOf getID(briteFile).Invoke,
                AddressOf ParseHtml,
                prefix:=Nothing,
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Friend Shared Function getID(briteFile As String) As Func(Of PathwayEntry, String)
            Return Function(entry As PathwayEntry)
                       If briteFile.StringEmpty Then
                           Return "map" & entry.EntryId
                       Else
                           Dim s = entry.entry.text
                           s = r.Match(s, "\[PATH:.+?\]", RegexICSng).Value
                           s = s.GetStackValue("[", "]").Split(":"c).Last
                           Return s
                       End If
                   End Function
        End Function

        Private Shared Function ParseHtml(html$, schema As Type) As Object
            Return ParseHtmlExtensions.ParseHTML(html)
        End Function

        Private Shared Function createUrl(briteFile As String) As Func(Of PathwayEntry, String)
            Dim getID = MapQuery.getID(briteFile)

            Return Function(entry)
                       Return $"http://www.genome.jp/kegg-bin/show_pathway?{getID(entry)}"
                   End Function
        End Function
    End Class
End Namespace
