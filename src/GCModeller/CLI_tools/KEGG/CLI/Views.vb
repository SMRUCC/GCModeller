#Region "Microsoft.VisualBasic::c9600f1b56f1837d68f4a42defbb3819, ..\GCModeller\CLI_tools\KEGG\CLI\Views.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.KEGG.KEGGOrthology
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Cut_sequence.upstream", Usage:="/Cut_sequence.upstream /in <list.txt> /PTT <genome.ptt> /org <kegg_sp> [/len <100bp> /overrides /out <outDIR>]")>
    Public Function CutSequence_Upstream(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim PTT = args("/PTT")
        Dim len = args.GetValue("/len", 100)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-{len}bp.fasta")
        Dim genome As PTT = SMRUCC.genomics.Assembly.NCBI.GenBank.LoadPTT(PTT)
        Dim code$ = args("/org")
        Dim [overrides] As Boolean = args.GetBoolean("/overrides")

        Call genome.CutSequence_Upstream(
            geneIDs:=[in].ReadAllLines,
            len:=len,
            save:=out,
            code:=code,
            [overrides]:=[overrides])

        Return 0
    End Function

    <ExportAPI("/Views.mod_stat", Usage:="/Views.mod_stat /in <KEGG_Modules/Pathways_DIR> /locus <in.csv> [/locus_map Gene /pathway /out <out.csv>]")>
    Public Function Stats(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim locus As String = args("/locus")
        Dim locus_map As String = args.GetValue("/locus_map", "Gene")
        Dim ispathway As String = args.GetBoolean("/pathway")
        Dim out As String = args.GetValue("/out", locus.TrimSuffix & $"-{inDIR.BaseName}.csv")
        Dim modsCls As ModuleClassAPI =
            If(ispathway,
            ModuleClassAPI.FromPathway(inDIR),
            ModuleClassAPI.FromModules(inDIR))
        Dim locusId As String() = EntityObject.LoadDataSet(locus, locus_map).ToArray(Function(x) x.ID)
        Dim LQuery = LinqAPI.MakeList(Of (entryId$, locus_id As String())) <=
            From x As PathwayBrief
            In modsCls.Modules
            Let locus_id As String() = x _
                .GetPathwayGenes _
                .Where(Function(sid) Array.IndexOf(locusId, sid) > -1) _
                .ToArray
            Select (x.EntryId, locus_id)

        Return LQuery > out
    End Function

    <ExportAPI("/Get.prot_motif", Usage:="/Get.prot_motif /query <sp:locus> [/out out.json]")>
    Public Function ProteinMotifs(args As CommandLine) As Integer
        Dim query As String = args - "/query"
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & $"/{query.NormalizePathString}.json")
        Dim prot As Protein = ProtMotifsQuery.Query(query)
        Return prot.GetJson.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Gets.prot_motif",
               Usage:="/Gets.prot_motif /query <query.txt/genome.PTT> [/PTT /sp <kegg-sp> /out <out.json> /update]")>
    Public Function GetsProteinMotifs(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & ".prot_motifs.json")
        Dim TEMP As String = out & ".temp/"

        ' If out.FileExists AndAlso Not args.GetBoolean("/update") Then
        ' Return 0
        ' Else
        Call $"Data will save to {out.ToFileURL}".__DEBUG_ECHO
        ' End If

        Dim isPTT As Boolean = args.GetBoolean("/PTT")
        Dim locus As QuerySource

        If isPTT Then
            Dim PTT As PTT = TabularFormat.PTT.Load(query)
            locus = New QuerySource With {
                .genome = PTT.Title,
                .locusId = PTT.GeneIDList.ToArray
            }
        Else
            locus = QuerySource.DocParser(query)
        End If

        Dim sp As String = args.GetValue("/sp", locus.QuerySpCode)
        Dim result As New Dictionary(Of Protein)

        If String.IsNullOrEmpty(sp) Then
            Return -100
        End If

        If out.FileExists Then
            result = New Dictionary(Of Protein)(out.ReadAllText.LoadObject(Of Protein()))
        End If

        For Each locusId As String In locus.locusId
            Dim tmp As String = TEMP & $"/{locusId}.json"

            If result.ContainsKey(locusId) Then
                If Not result(locusId).Domains.IsNullOrEmpty Then
                    Continue For
                Else
                    result -= locusId
                End If
            End If

            Dim prot As Protein

            If tmp.FileExists Then
                prot = tmp.ReadAllText.LoadObject(Of Protein)
                If Not prot.Domains.IsNullOrEmpty Then
                    result += prot
                    Continue For
                End If
            End If

            prot = ProtMotifsQuery.Query(sp, locusId)

            If Not prot Is Nothing Then
                result += prot
                Call prot.GetJson.SaveTo(tmp)
            End If
        Next

        Return result.Values.ToArray.GetJson.SaveTo(out)
    End Function

    ''' <summary>
    ''' 显示KEGG注释结果的barplot
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KO.Catalogs", Usage:="/KO.Catalogs /in <blast.mapping.csv> /ko <ko_genes.csv> [/key <Query_id> /mapTo <Subject_id> /out <outDIR>]")>
    Public Function KOCatalogs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ko As String = args("/ko")
        Dim key As String = args.GetValue("/key", "Query_id")
        Dim mapTo As String = args.GetValue("/mapTo", "Subject_id")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-KO.Catalogs/")
        Dim mappings As Map(Of String, String)() =
            [in].LoadMappings(key, mapTo).ToArray
        Dim KO_genes As KO_gene() = ko.LoadCsv(Of KO_gene)

        For Each level As String In {"A", "B", "C"}
            Dim result = CatalogProfiling(mappings, KO_genes, level)
            Dim csv = (From part
                       In result
                       Select part.Value _
                           .Select(Function(c) New With {
                                .A = part.Key,
                                .catalog = c.Name,
                                .counts = c.Value
                       })).IteratesALL _
                          .Where(Function(c) c.counts > 0) _
                          .ToArray

            Call csv.SaveTo(out & $"/{[in].BaseName}-KO.Catalogs-level-{level}.csv")

            If level = "A" Then
                Call result.ToDictionary(Function(x) x.Key, Function(v) v.Value.ToArray(Function(x) New NamedValue(Of Double)(x.Name, x.Value))).Plot().SaveAs(out & "/kegg-level-A.png")
            End If
        Next

        Return 0
    End Function
End Module
