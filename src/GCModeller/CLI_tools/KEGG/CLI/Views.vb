#Region "Microsoft.VisualBasic::d272175d1c6a1274a023fe7d66f3feb2, CLI_tools\KEGG\CLI\Views.vb"

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

    ' Module CLI
    ' 
    '     Function: CutSequence_Upstream, GetsProteinMotifs, KEGGOrganismTable, ProteinMotifs, Stats
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.ProteinModel

Partial Module CLI

    <ExportAPI("/Organism.Table")>
    <Usage("/Organism.Table [/in <br08601-htext.keg> /Bacteria /out <out.csv>]")>
    <ArgumentAttribute("/in", True, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.keg, *.txt",
              Description:="If this kegg brite file is not presented in the cli arguments, the internal kegg resource will be used.")>
    Public Function KEGGOrganismTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$
        Dim htext As htext
        Dim bacteria As Boolean = args.IsTrue("/Bacteria")

        If [in].FileExists Then
            out = args("/out") Or ([in].TrimSuffix & ".table.csv")
            htext = htext.StreamParser([in])
        Else
            out = args("/out") Or (App.CurrentDirectory & $"/{NameOf(KEGGOrganismTable)}.csv")
            htext = Organism.GetResource
        End If

        If bacteria Then
            Return htext _
                .GetBacteriaList _
                .SaveTo(out) _
                .CLICode
        Else
            Return htext _
                .FillTaxonomyTable _
                .SaveTo(out) _
                .CLICode
        End If
    End Function

    <ExportAPI("/Cut_sequence.upstream")>
    <Usage("/Cut_sequence.upstream /in <list.txt> /PTT <genome.ptt> /org <kegg_sp> [/len <100bp> /overrides /out <outDIR>]")>
    Public Function CutSequence_Upstream(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim PTT$ = args("/PTT")
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

    <ExportAPI("/Views.mod_stat")>
    <Usage("/Views.mod_stat /in <KEGG_Modules/Pathways_DIR> /locus <in.csv> [/locus_map Gene /pathway /out <out.csv>]")>
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
        Dim locusId As String() = EntityObject.LoadDataSet(locus, locus_map).Select(Function(x) x.ID)
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

    <ExportAPI("/Get.prot_motif")>
    <Usage("/Get.prot_motif /query <sp:locus> [/out out.json]")>
    Public Function ProteinMotifs(args As CommandLine) As Integer
        Dim query As String = args - "/query"
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & $"/{query.NormalizePathString}.json")
        Dim prot As Protein = ProtMotifsQuery.Query(query)
        Return prot.GetJson.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Gets.prot_motif")>
    <Usage("/Gets.prot_motif /query <query.txt/genome.PTT> [/PTT /sp <kegg-sp> /out <out.json> /update]")>
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
            result = New Dictionary(Of Protein)(out.ReadAllText.LoadJSON(Of Protein()))
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
                prot = tmp.ReadAllText.LoadJSON(Of Protein)
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
End Module
