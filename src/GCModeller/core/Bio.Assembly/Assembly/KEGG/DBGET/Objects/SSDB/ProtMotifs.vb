#Region "Microsoft.VisualBasic::7630b30c7e4c887abbe2eb7b9fca4883, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\ProtMotifs.vb"

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

    '   Total Lines: 66
    '    Code Lines: 51
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.60 KB


    '     Module ProtMotifsQuery
    ' 
    '         Function: Fetch, parsingDomain, (+2 Overloads) Query
    ' 
    '         Sub: fillBasicInfo, fillMotifs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    Public Module ProtMotifsQuery

        Const ssdb_motifs As String = "http://www.kegg.jp/ssdb-bin/ssdb_motif?kid="

        Public Function Query(sp As String, locus As String) As ProteinModel.Protein
            Dim url As String = ssdb_motifs & $"{sp}:{locus}"
            Return Fetch(url)
        End Function

        Public Function Query(entry As String) As ProteinModel.Protein
            Dim url As String = ssdb_motifs & entry
            Return Fetch(url)
        End Function

        Public Function Fetch(url As String) As ProteinModel.Protein
            Dim html As String = url.GET
            Dim form As String = Regex.Match(html, "<form.+?</form>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).Value
            Dim tables As String() = HtmlParser.GetTablesHTML(form)
            Dim prot As New ProteinModel.Protein

            Call fillBasicInfo(prot, tables(Scan0))
            Call fillMotifs(prot, tables(1))

            Return prot
        End Function

        Private Sub fillBasicInfo(ByRef prot As ProteinModel.Protein, table As String)
            Dim rows As String() = HtmlParser.GetRowsHTML(table)
            prot.Organism = HtmlParser.GetColumnsHTML(rows(0)).Last
            prot.ID = HtmlParser.GetColumnsHTML(rows(1)).Last.GetValue
            prot.Description = HtmlParser.GetColumnsHTML(rows(2)).Last
        End Sub

        Private Sub fillMotifs(ByRef prot As ProteinModel.Protein, table As String)
            Dim rows As String() = HtmlParser.GetRowsHTML(table)
            prot.Domains = rows.Skip(1).Select(Function(s) s.parsingDomain).ToArray
        End Sub

        <Extension>
        Private Function parsingDomain(row As String) As ProteinModel.DomainObject
            Dim cols As String() = HtmlParser.GetColumnsHTML(row)
            Dim motif As New ProteinModel.DomainObject

            motif.Name = cols(0).GetValue
            motif.Describes = cols(3)
            motif.EValue = Val(cols(4))
            motif.BitScore = cols(5)

            Dim left As Integer = Scripting.CTypeDynamic(Of Integer)(cols(1))
            Dim right As Integer = Scripting.CTypeDynamic(Of Integer)(cols(2))

            motif.Position = New Location(left, right)

            Return motif
        End Function
    End Module
End Namespace
