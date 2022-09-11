#Region "Microsoft.VisualBasic::bbfe07f260983f8adaa5f9a675d8dd49, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\ReferenceMap\Reaction.vb"

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

    '   Total Lines: 54
    '    Code Lines: 38
    ' Comment Lines: 6
    '   Blank Lines: 10
    '     File Size: 2.14 KB


    '     Class ReferenceReaction
    ' 
    '         Properties: SSDBs
    ' 
    '         Function: Download
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.ReferenceMap

    <XmlType("KEGG-RefRxnDef", Namespace:="http://code.google.com/p/genome-in-code/kegg/reference_reaction")>
    Public Class ReferenceReaction : Inherits bGetObject.Reaction

        ''' <summary>
        ''' 酶分子的直系同源的参考序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SSDBs As NamedCollection(Of QueryEntry)()

        Const ENTRY_PATTERN As String = "<a href=""/dbget-bin/www_bget\?ko:K\d+"

        Public Overloads Shared Function Download(entry As ListEntry) As ReferenceReaction
            Dim html As New WebForm(resource:=entry.url)

            If html.Count = 0 Then
                Return Nothing
            End If

            Dim r As ReferenceReaction = ReactionQuery.webFormParser(Of ReferenceReaction)(html)
            Dim sValue As String = html("Orthology").FirstOrDefault

            If Not String.IsNullOrEmpty(sValue) Then
                Dim IDs As String() = Regex _
                    .Matches(sValue, ENTRY_PATTERN, RegexOptions.IgnoreCase) _
                    .ToArray(Function(m) m.Split(CChar(":")).Last)
                Dim genes = LinqAPI.Exec(Of NamedCollection(Of QueryEntry)) <=
 _
                    From EntryID As String
                    In IDs
                    Select New NamedCollection(Of QueryEntry) With {
                        .Name = EntryID,
                        .Value = SSDB.API.HandleDownload(KO_ID:=EntryID)
                    }

                r.SSDBs = genes
            End If

            Return r
        End Function
    End Class
End Namespace
