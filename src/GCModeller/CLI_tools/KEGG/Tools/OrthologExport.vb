#Region "Microsoft.VisualBasic::4fed1ef840fea4ff0c34d62bdd633516, CLI_tools\KEGG\Tools\OrthologExport.vb"

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

    ' Module OrthologExport
    ' 
    '     Function: __createRowObject, Export
    ' 
    '     Sub: (+5 Overloads) HandleQuery
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("KEGG.Ortholog.Export",
                  Category:=APICategories.ResearchTools,
                  Description:="KEGG ortholog annotation database.",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene={sp}:{locus_tag}")>
Public Module OrthologExport

    <ExportAPI("Export")>
    Public Function Export(ortholog As SSDB.OrthologREST) As IO.File
        Dim CsvData As IO.File =
            New IO.File +
            {"KEGG_Entry", "Definition", "KO", "Length", "Sw-Score", "Margin", "bits", "identity", "overlap", "best(all)"} +
            From resultRow As SSDB.SShit In ortholog.Orthologs Select __createRowObject(resultRow)
        Return CsvData
    End Function

    Private Function __createRowObject(resultRow As SSDB.SShit) As IO.RowObject
        Dim ArrayData As String() = New String() {
            resultRow.Entry.ToString,
            resultRow.Entry.description,
            resultRow.KO.Value,
            resultRow.Length,
            resultRow.SWScore,
            resultRow.Margin,
            resultRow.Bits,
            resultRow.Identity,
            resultRow.Overlap,
            String.Format("{0} {1}", resultRow.Best.Key, resultRow.Best.Value)
        }
        Return New IO.RowObject(ArrayData)
    End Function

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(lstId As String(), outDIR As String)
        For Each GeneId As String In lstId
            Call HandleQuery(GeneId, outDIR)
        Next
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(lstId As String(), outDIR As String, sp As String)
        For Each locusId As String In lstId
            If $"{outDIR}/{locusId}.xml".FileExists AndAlso $"{outDIR}/{locusId}.csv".FileExists Then
                Continue For
            End If

            Call HandleQuery(locusId, sp, outDIR)
        Next
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(locusId As String, sp As String, outDIR As String)
        Dim query As New QueryEntry With {
            .locusID = locusId,
            .speciesID = sp
        }
        Call HandleQuery(query, outDIR)
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(query As QueryEntry, outDIR As String)
        Dim Ortholog = SSDB.OrthologREST.Download(query)

        Call Ortholog.GetXml.SaveTo($"{outDIR}/{query.locusID}.xml")
        Call OrthologExport.Export(Ortholog).Save($"{outDIR}/{query.locusID}.csv", Text.Encoding.ASCII)
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(locusId As String, outDIR As String)
        Dim Entries = WebRequest.HandleQuery(keyword:=locusId)

        If Entries.IsNullOrEmpty Then
            Call Console.WriteLine($"[KEGG_ENTRY_NOT_FOUND] KEYWORD:={locusId}")
            Return
        End If

        Dim LQuery = (From entry As QueryEntry
                      In Entries
                      Where String.Equals(entry.locusID, locusId, StringComparison.OrdinalIgnoreCase)
                      Select entry).FirstOrDefault
        If LQuery Is Nothing Then
            Call Console.WriteLine($"[ENTRY_IS_NULL] GeneId:={locusId}")
        Else
            Call HandleQuery(LQuery, outDIR)
        End If
    End Sub
End Module
