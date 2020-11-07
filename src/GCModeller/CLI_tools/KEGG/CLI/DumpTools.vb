#Region "Microsoft.VisualBasic::742e39d96e658cf1c06151b291e4c3a6, CLI_tools\KEGG\CLI\DumpTools.vb"

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
    '     Function: __create, __queryKO, DownloadHumanGenes, DumpDb, DumpOrganisms
    '               GetKOAnnotation, GetSource, QueryKOAnno, ShowOrganism, TransmembraneKOlist
    ' 
    '     Sub: __queryKO2
    '     Class KOAnno
    ' 
    '         Properties: [Class], Category, COG, Definition, GO
    '                     KO, Name, PathwayId, PathwayName, QueryId
    '                     Reactions
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.Archives
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/KO.list")>
    <Description("Export a KO functional id list which all of the gene in this list is involved with the given pathway kgml data.")>
    <Usage("/KO.list /kgml <pathway.kgml> [/skip.empty /out <list.csv>]")>
    <ArgumentAttribute("/out", True, CLITypes.File, PipelineTypes.std_out,
              Extensions:="*.csv",
              Description:="If this argument is not presented in the commandline input, then result list will print on the console in tsv format.")>
    Public Function TransmembraneKOlist(args As CommandLine) As Integer
        Using out As StreamWriter = args.OpenStreamOutput("/out")
            Dim kgml As KGML.pathway = args("/kgml").LoadXml(Of KGML.pathway)
            Dim skipEmpty As Boolean = args("/skip.empty")
            Dim KOlist As String() = kgml.KOlist
            Dim enzymes = EnzymeEntry.ParseEntries _
                .GroupBy(Function(entry) entry.KO) _
                .ToDictionary(Function(entry) entry.Key,
                              Function(g)
                                  Return g.ToArray
                              End Function)
            Dim list As EntityObject() = KOlist _
                .Select(Iterator Function(id)
                            Dim enzyme = enzymes.TryGetValue(id)

                            If enzyme.IsNullOrEmpty Then
                                If Not skipEmpty Then
                                    Yield New EntityObject With {.ID = id}
                                End If
                            Else
                                For Each entry As EnzymeEntry In enzyme
                                    Yield New EntityObject With {
                                        .ID = id,
                                        .Properties = New Dictionary(Of String, String) From {
                                            {"geneNames", entry.geneNames.JoinBy("; ")},
                                            {"fullName", entry.fullName},
                                            {"EC_number", entry.EC},
                                            {"function", entry.ECName}
                                        }
                                    }
                                Next
                            End If
                        End Function) _
                .IteratesALL _
                .OrderByDescending(Function(d) d!function) _
                .ToArray

            If args.ContainsParameter("/out") Then
                ' csv
                Call list.ToCsvDoc.Save(out)
            Else
                ' print
                Call list.ToCsvDoc.Save(out, isTsv:=True)
            End If
        End Using

        Return 0
    End Function

    <ExportAPI("/Download.human.genes")>
    <Usage("/Download.human.genes /in <geneID.list/DIR> [/batch /out <save.DIR>]")>
    Public Function DownloadHumanGenes(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim batch As Boolean = args.GetBoolean("/batch")
        Dim download = Sub(path$, out$)
                           Dim list$() = path.ReadAllLines
                           Dim failures$() = list.DownloadHumanGenome(EXPORT:=out)
                           Call failures _
                                .SaveTo(out & "/failures.txt") _
                                .CLICode
                       End Sub

        If batch Then
            Dim out As String = args.GetValue("/out", [in].TrimDIR & "-KEGG_human_genes/")
            Dim EXPORT$

            For Each file As String In ls - l - r - "*.list" <= [in]
                EXPORT = $"{out}/{file.BaseName}/"
                download(file, out:=EXPORT)
            Next
        Else
            Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".human_genes/")
            Call download([in], out)
        End If

        Return 0
    End Function

    <ExportAPI("--Dump.Db")>
    <Usage("--Dump.Db /KEGG.Pathways <DIR> /KEGG.Modules <DIR> /KEGG.Reactions <DIR> /sp <sp.Code> /out <out.Xml>")>
    Public Function DumpDb(args As CommandLine) As Integer
        Dim Pathways As String = args("/KEGG.Pathways")
        Dim Modules As String = args("/KEGG.Modules")
        Dim Reactions As String = args("/KEGG.Reactions")
        Dim sp As String = args("/sp")
        Dim outXml As String = args("/out")
        Dim Modle = SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Compile(Pathways, Modules, Reactions, sp)
        Return Modle.GetXml.SaveTo(outXml).CLICode
    End Function

    <ExportAPI("--Get.KO")>
    <Usage("--Get.KO /in <KASS-query.txt>")>
    Public Function GetKOAnnotation(args As CommandLine) As Integer
        Dim input As String = args("/in")
        Dim buffer = input.ReadAllLines.Select(Function(x) Strings.Split(x, vbTab))
        Dim tbl = buffer.Select(Function(x) New KeyValuePair With {.Key = x(Scan0), .Value = x.ElementAtOrDefault(1)})
        Dim brite = BriteHEntry.Pathway.LoadDictionary
        Dim LQuery = (From prot In tbl Select __queryKO(prot.Key, prot.Value, brite)).ToArray.Unlist
        Return LQuery.SaveTo(input.TrimSuffix & ".KO.csv")
    End Function

    <ExportAPI("/Query.KO")>
    <Usage("/Query.KO /in <blastnhits.csv> [/out <out.csv> /evalue 1e-5 /batch]")>
    Public Function QueryKOAnno(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim batch As Boolean = args.GetBoolean("/batch")
        Dim evalue As Double = args.GetValue("/evalue", 0.00001)

        If batch Then
            Dim files = FileIO.FileSystem.GetFiles(inFile, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
            Dim out As String = args.GetValue("/out", inFile & "/PathwayInfo/")

            For Each file As String In files
                Dim outFile As String = out & $"/{file.BaseName}.PathwayInfo.csv"
                Call __queryKO2(file, outFile, evalue)
            Next
        Else
            Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".PathwayInfo.Csv")
            Call __queryKO2(inFile, out, evalue)
        End If

        Return 0
    End Function

    Private Sub __queryKO2(infile As String, out As String, evalue As Double)
        Dim inHits = infile.LoadCsv(Of SSDB.BlastnHit)
        inHits = (From x In inHits Where x.Eval <= evalue Select x).AsList
        Dim KO As String() = inHits _
            .Where(Function(s) Not String.IsNullOrWhiteSpace(s.KO)) _
            .Select(Function(x) x.KO) _
            .Distinct _
            .ToArray
        Dim brite = BriteHEntry.Pathway.LoadDictionary
        Dim name As String = infile.BaseName
        Dim anno = KO.Select(Function(sKO) __queryKO(name, sKO, brite)).Unlist
        Call anno.SaveTo(out)
    End Sub

    Private Function __queryKO(prot$, KO$, brites As Dictionary(Of String, BriteHEntry.Pathway)) As KOAnno()
        If String.IsNullOrEmpty(KO) Then
            Return {New KOAnno With {.QueryId = prot}}
        End If

        Dim orthology = SSDB.API.Query(KO)

        If orthology.pathway.IsNullOrEmpty Then
            Dim anno As KOAnno = __create(prot, KO, Nothing, orthology, brites)
            Return {anno}
        End If

        Dim LQuery = (From pathway In orthology.pathway Select __create(prot, KO, pathway, orthology, brites)).ToArray
        Return LQuery
    End Function

    Private Function __create(prot As String,
                              KO As String,
                              pathway As NamedValue,
                              orthology As SSDB.Orthology,
                              brites As Dictionary(Of String, BriteHEntry.Pathway)) As KOAnno
        Dim pwyBrite As BriteHEntry.Pathway

        If Not pathway Is Nothing Then
            pwyBrite = brites.TryGetValue(Regex.Match(pathway.name, "\d+").Value)
            If pwyBrite Is Nothing Then
                Call $"{pathway.name} is not exists in the KEGG!".__DEBUG_ECHO
                GoTo Null
            End If
        Else
            pathway = New NamedValue
Null:       pwyBrite = New BriteHEntry.Pathway With {
                .entry = New NamedValue
            }
        End If

        Return New KOAnno With {
            .QueryId = prot,
            .KO = KO,
            .COG = orthology.GetXRef("COG").Select(Function(x) x.comment).JoinBy("; "),
            .Definition = orthology.Definition,
            .Name = orthology.Name,
            .GO = orthology.GetXRef("GO").Select(Function(x) "GO:" & x.comment).ToArray,
            .Category = pwyBrite.category,
            .Class = pwyBrite.class,
            .PathwayId = pathway.name,
            .PathwayName = pwyBrite.entry.text,
            .Reactions = orthology.GetXRef("RN").Select(Function(x) x.comment).ToArray
        }
    End Function

    Public Class KOAnno
        Public Property QueryId As String
        Public Property KO As String
        Public Property Name As String
        Public Property Definition As String
        Public Property PathwayId As String
        Public Property PathwayName As String
        Public Property [Class] As String
        Public Property Category As String
        Public Property COG As String
        Public Property GO As String()
        <Collection("lst.RN", "; ")>
        Public Property Reactions As String()
    End Class

    <ExportAPI("--part.from",
               Usage:="--part.from /source <source.fasta> /ref <referenceFrom.fasta> [/out <out.fasta> /brief]",
               Info:="source and ref should be in KEGG annotation format.")>
    Public Function GetSource(args As CommandLine) As Integer
        Dim sourceFile$ = args <= "/source"
        Dim source As New FastaFile(sourceFile)
        Dim ref As New FastaFile(args("/ref"))
        Dim out As String = args.GetValue("/out", sourceFile.TrimSuffix & $".{(args <= "/ref").BaseName}.fasta")
        Dim sourceKEGG As SequenceDump() = source.Select(Function(x) SequenceDump.Create(x))
        Dim refKEGG As SequenceDump() = ref.Select(Function(x) SequenceDump.Create(x))
        Dim sourceDict = (From x As SequenceDump
                          In sourceKEGG
                          Select x
                          Group x By x.SpeciesId Into Group) _
                             .ToDictionary(Function(x) x.SpeciesId,
                                           Function(x) x.Group.ToArray)
        Dim refId As String() = refKEGG.Select(Function(x) x.SpeciesId).Distinct.ToArray
        Dim LQuery = (From sId As String In refId Where sourceDict.ContainsKey(sId) Select sourceDict(sId)).ToArray.Unlist
        Dim outFa As FastaFile = New FastaFile(LQuery)

        If args.GetBoolean("/brief") Then  ' 将基因号去除，只保留三字母的基因组编号，因为在做16S_rRNA进化树的时候，只需要一个基因就可以了
            Dim briefFa = (From x In outFa Select New FastaSeq({x.Headers.First.Split(":"c).First}, x.SequenceData)).ToArray
            outFa = New FastaFile(briefFa)
        End If

        Return outFa.ToUpper.Save(out).CLICode
    End Function

    ''' <summary>
    ''' 导出KEGG物种信息表格
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这个工具一般没有太多用途，主要是用于测试html文档解析器是否正确工作
    ''' </remarks>
    <ExportAPI("/Dump.sp")>
    <Usage("/Dump.sp [/res <sp.html, default=weburl.html> /out <out.csv>]")>
    <Description("Dump all of the KEGG organism and write table data in csv file format.")>
    <ArgumentAttribute("/res", True, CLITypes.File,
              AcceptTypes:={GetType(String)},
              Extensions:="*.txt, *.html",
              Description:="By default is fetch table resource from web url: 'http://www.kegg.jp/kegg/catalog/org_list.html'.")>
    Public Function DumpOrganisms(args As CommandLine) As Integer
        Dim res As String = args("/res") Or "http://www.kegg.jp/kegg/catalog/org_list.html"
        Dim result As KEGGOrganism = EntryAPI.FromResource(res)
        Dim table As List(Of Prokaryote) = result.Prokaryote.AsList + result.Eukaryotes.Select(Function(x) New Prokaryote(x))
        Dim out As String = args("/out") Or (App.CurrentDirectory & "/KEGG_Organism.csv")

        Return table.SaveTo(out).CLICode
    End Function

    <ExportAPI("/show.organism")>
    <Usage("/show.organism /code <kegg_sp> [/out <out.json>]")>
    <Description("Save the summary information about the specific given kegg organism.")>
    <ArgumentAttribute("/code", False, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The kegg organism brief code.")>
    Public Function ShowOrganism(args As CommandLine) As Integer
        Dim code$ = args <= "/code"
        Dim out$ = args("/out") Or $"./{code}.json"
        Dim organism As OrganismInfo = OrganismInfo.ShowOrganism(
            code:=code,
            cache:=$"{out.ParentPath}/.kegg/show_organism/"
        )

        Return organism _
            .GetJson(indent:=True) _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function
End Module
