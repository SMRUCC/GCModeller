#Region "Microsoft.VisualBasic::a6428c24780e89c8f69006cfeb0a1601, CLI_tools\eggHTS\CLI\1. Annotations.vb"

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
'     Function: BBHReplace, BlastXFillORF, COGCatalogProfilingPlot, ColorKEGGPathwayMap, ExocartaHits
'               GetFastaIDlist, GetIDlistFromSampleTable, KOCatalogs, NormalizeSpecies, PerseusTableAnnotations
'               ProteinsGoPlot, proteinsKEGGPlot, SampleAnnotations, UniprotMappings, UniRef2UniprotKB
'               UniRefMap2Organism, Update2UniprotMappedID
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.KEGG.KEGGOrthology
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.GoStat
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.BlastX
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

Partial Module CLI

    <ExportAPI("/KEGG.Color.Pathway")>
    <Usage("/KEGG.Color.Pathway /in <protein.annotations.csv> /ref <KEGG.ref.pathwayMap.directory repository> [/out <out.directory>]")>
    <Group(CLIGroups.Annotation_CLI)>
    <ArgumentAttribute("/ref", False, CLITypes.File, AcceptTypes:={GetType(Map)},
              Extensions:="*.Xml",
              Description:="")>
    Public Function ColorKEGGPathwayMap(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim ref$ = args <= "/ref"
        Dim out$ = (args <= "/out") Or [in].TrimSuffix.AsDefault
        Dim listID$() = EntityObject.LoadDataSet([in]) _
            .Select(Function(protein) protein!KO) _
            .Where(Function(id) Not id.StringEmpty) _
            .Select(Function(s) s.StringSplit(";\s*")) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Return PathwayMapVisualize _
            .LocalRenderMaps(listID, repo:=ref, output:=out) _
            .GetJson _
            .SaveTo(out & "/list.json") _
            .CLICode
    End Function

    ''' <summary>
    ''' 将每一个参考cluster之中的代表序列的uniprot编号取出来生成映射
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/UniRef.UniprotKB")>
    <Usage("/UniRef.UniprotKB /in <uniref.xml> [/out <maps.csv>]")>
    <ArgumentAttribute("/in", False, CLITypes.File, Description:="The uniRef XML cluster database its file path.")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function UniRef2UniprotKB(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-uniref_uniprotKB.csv")
        Dim ref As NamedValue(Of String)() = UniRef _
            .PopulateALL([in]) _
            .Select(Function(entry)
                        Return New NamedValue(Of String) With {
                            .Name = entry.id,
                            .Value = entry.representativeMember.UniProtKB_accession
                        }
                    End Function) _
            .ToArray

        Return ref.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 将cluster之中的指定的物种名称的编号取出来，以方便应用于新测序的非参考基因组的数据项目的功能富集分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/UniRef.map.organism")>
    <Usage("/UniRef.map.organism /in <uniref.xml> [/org <organism_name> /out <out.csv>]")>
    <ArgumentAttribute("/in", False, CLITypes.File, Description:="The uniRef XML cluster database its file path.")>
    <ArgumentAttribute("/org", True, CLITypes.String, Description:="The organism scientific name. If this argument is presented in the CLI input, then this program will output the top organism in this input data.")>
    Public Function UniRefMap2Organism(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim org$ = args <= "/org"

        If org.StringEmpty Then
            org = UniRef _
                .PopulateALL([in]) _
                .Select(Function(x)
                            Return x.representativeMember.Join(x.members)
                        End Function) _
                .IteratesALL _
                .Select(Function(x) x.source_organism) _
                .GroupBy(Function(x) x) _
                .OrderByDescending(Function(g) g.Count) _
                .First _
                .Key
        End If

        Dim ref As NamedValue(Of String)() = UniRef _
            .PopulateALL([in]) _
            .Select(Function(entry)
                        Dim member = entry _
                            .representativeMember _
                            .Join(entry.members) _
                            .Where(Function(m) InStr(m.source_organism, org, CompareMethod.Text) > 0) _
                            .FirstOrDefault

                        If member Is Nothing Then
                            Return Nothing
                        End If

                        Return New NamedValue(Of String) With {
                            .Name = entry.id,
                            .Value = member.UniProtKB_accession,
                            .Description = member.source_organism
                        }
                    End Function) _
            .Where(Function(map) Not map.IsEmpty) _
            .ToArray

        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-" & org.NormalizePathString & ".csv")
        Return ref.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Exocarta.Hits")>
    <Usage("/Exocarta.Hits /in <list.txt> /annotation <annotations.csv> /exocarta <Exocarta.tsv> [/out <out.csv>]")>
    Public Function ExocartaHits(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim annotation$ = args <= "/annotation"
        Dim exocarta$ = args <= "/exocarta"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".Exocarta.Hits.csv")
        Dim list$() = [in].ReadAllLines
        Dim annotations = EntityObject.LoadDataSet(annotation).ToDictionary
        Dim species$ = annotations.First.Value!organism
        Dim exocartaData = EntityObject _
            .LoadDataSet(exocarta, tsv:=True) _
            .Where(Function(item)
                       Return item("CONTENT TYPE").TextEquals("protein") AndAlso
                             (item!SPECIES).TextEquals(species)
                   End Function) _
            .Select(Function(x) x("ENTREZ GENE ID")) _
            .Distinct _
            .Indexing
        Dim output As New List(Of EntityObject)

        For Each uniprotID In list
            Dim protein = annotations(uniprotID)
            Dim entrezID = protein!Entrez

            For Each id In entrezID.StringSplit(";\s*")
                If exocartaData.IndexOf(id) > -1 Then
                    output += protein
                End If
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/update.uniprot.mapped")>
    <Usage("/update.uniprot.mapped /in <table.csv> /mapping <mapping.tsv/tab> [/source /out <out.csv>]")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function Update2UniprotMappedID(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim mapping$ = args <= "/mapping"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".uniprotID.csv")
        Dim proteins = EntityObject.LoadDataSet([in])
        Dim mappings = Retrieve_IDmapping _
            .MappingReader(mapping) _
            .UniprotIDFilter
        Dim source As Boolean = args.GetBoolean("/source")

        For Each prot In proteins
            If source Then
                prot.Properties.Add(NameOf(source), prot.ID)
            End If
            If mappings.ContainsKey(prot.ID) Then
                prot.ID = mappings(prot.ID)
            End If
        Next

        Return proteins _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 如果结果表格之中的编号是包含有注释信息的完整的fasta头，则会需要使用这个工具来将其中的编号取出来
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Samples.IDlist")>
    <Description("Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.")>
    <Usage("/Samples.IDlist /in <samples.csv> [/uniprot /out <out.list.txt>]")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function GetIDlistFromSampleTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim isUniProt As Boolean = args("/uniprot")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.geneIDs.txt"
        Dim outCsv$ = out.TrimSuffix & ".csv"

        ' 假设第一列总是ID编号的数据
        Dim table As csv = csv.Load([in])
        Dim list As New List(Of String)

        For Each row In table.Skip(1)
            Dim id$ = row(Scan0)

            If isUniProt Then
                id = id.Split("|"c).ElementAtOrDefault(1)
            Else
                id = id.Split(" "c).FirstOrDefault
            End If

            row(Scan0) = id
            list += id
        Next

        Call table.Save(outCsv,)

        Return list.SaveTo(out, Encodings.ASCII.CodePage).CLICode
    End Function

    <ExportAPI("/Fasta.IDlist", Usage:="/Fasta.IDlist /in <prot.fasta> [/out <geneIDs.txt>]")>
    Public Function GetFastaIDlist(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".geneIDs.txt")
        Dim fasta As New FastaFile([in])
        Dim list$() = fasta _
            .Select(Function(fa) fa.Title.Split.First) _
            .ToArray
        Return list.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 1. 总蛋白注释
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("/protein.annotations")>
    <Description("Total proteins functional annotation by using uniprot database.")>
    <Usage("/protein.annotations /uniprot <uniprot.XML> [/accession.ID /iTraq /list <uniprot.id.list.txt/rawtable.csv/Xlsx> /mapping <mappings.tab/tsv> /out <out.csv>]")>
    <ArgumentAttribute("/list", True, CLITypes.File,
              AcceptTypes:={GetType(String())},
              Extensions:="*.txt, *.csv, *.xlsx",
              Description:="Using for the iTraq method result.")>
    <ArgumentAttribute("/iTraq", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="* Using for the iTraq method result. If this option was enabled, then the protein ID in the output table using be using the value from the uniprot ID field.")>
    <ArgumentAttribute("/mapping", True, CLITypes.File,
              Extensions:="*.tsv, *.txt",
              Description:="The id mapping table, only works when the argument ``/list`` is presented.")>
    <ArgumentAttribute("/uniprot", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(UniProtXML)},
              Extensions:="*.xml",
              Description:="The Uniprot protein database in XML file format.")>
    <ArgumentAttribute("/accession.ID", True, CLITypes.Boolean,
              Description:="Using the uniprot protein ID from the ``/uniprot`` input as the generated dataset's ID value, instead of using the numeric sequence as the ID value.")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              Extensions:="*.csv",
              Description:="The file path for output protein annotation table where to save.")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function SampleAnnotations(args As CommandLine) As Integer
        Dim list As String = args("/list")
        Dim uniprot As String = args("/uniprot")
        Dim out As String
        Dim iTraq As Boolean = args("/iTraq")
        Dim accID As Boolean = args("/accession.ID")

        If list.FileExists(True) Then
            Dim geneIDs$()
            Dim mapping$ = args <= "/mapping"
            Dim mappings As Dictionary(Of String, String()) = Nothing

            out = args.GetValue("/out", list.TrimSuffix & "-proteins-uniprot-annotations.csv")

            With list.ExtensionSuffix
                If .TextEquals("csv") OrElse .TextEquals("tsv") OrElse .TextEquals("tab") Then
                    geneIDs = EntityObject.LoadDataSet(list,, tsv:= .TextEquals("tsv") OrElse .TextEquals("tab")) _
                        .Select(Function(x) x.ID) _
                        .Distinct _
                        .ToArray

                ElseIf .TextEquals("xlsx") Then
                    Dim sheet$ = args("/sheetName") Or "Sheet1"
                    Dim csv As csv = Xlsx _
                        .Open(list) _
                        .GetTable(sheet)

                    geneIDs = csv.Column(0).ToArray
                Else
                    geneIDs = list.ReadAllLines
                End If
            End With

            If mapping.FileExists Then
                mappings = Retrieve_IDmapping.MappingReader(mapping)
            End If

            Return geneIDs _
                .GenerateAnnotations(uniprot, iTraq, accID, mappings:=mappings) _
                .Select(Function(x) x.Item1) _
                .Where(Function(protein) Not protein.ID.StringEmpty) _
                .ToArray _
                .SaveTo(out) _
                .CLICode
        Else
            out = args("/out") Or (uniprot.ParentPath & "/proteins-uniprot-annotations.csv")

            Return uniprot _
                .ExportAnnotations(iTraq:=iTraq, accID:=accID) _
                .Select(Function(x) x.Item1) _
                .Where(Function(protein) Not protein.ID.StringEmpty) _
                .ToArray _
                .SaveTo(out) _
                .CLICode
        End If
    End Function

    ''' <summary>
    ''' 这个函数除了会生成注释表格之外，还会将原表格添加新的编号，以及导出蛋白表达的数据表
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Perseus.Table.annotations",
               Usage:="/Perseus.Table.annotations /in <proteinGroups.csv> /uniprot <uniprot.XML> [/scientifcName <""""> /out <out.csv>]")>
    Public Function PerseusTableAnnotations(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim uniprot As String = args("/uniprot")
        Dim out = args.GetValue("/out", [in].TrimSuffix & ".proteins.annotation.csv")
        Dim table As Perseus() = [in].LoadCsv(Of Perseus)
        Dim uniprotTable = UniProtXML.LoadDictionary(uniprot)
        Dim scientifcName As String = args("/scientifcName")
        Dim output = table.GenerateAnnotations(uniprotTable, "uniprot", scientifcName).ToArray
        Dim annotations = output.Select(Function(prot) prot.Item1).ToArray

        For i As Integer = 0 To table.Length - 1
            table(i).geneID = annotations(i).ID
        Next

        Call table.SaveTo(out.TrimSuffix & ".sample.csv")

        Dim samples As IO.DataSet() = table _
            .Select(Function(prot) New IO.DataSet With {
                .ID = prot.geneID,
                .Properties = prot.ExpressionValues _
                .ToDictionary(Function(x) x.Name,
                              Function(x) x.Value)
            }) _
            .ToArray

        Call samples.SaveTo(out.TrimSuffix & ".values.csv")

        Return annotations.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 总蛋白注释绘制GO分布图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/proteins.Go.plot")>
    <Description("ProteinGroups sample data go profiling plot from the uniprot annotation data.")>
    <Usage("/proteins.Go.plot /in <proteins-uniprot-annotations.csv> [/GO <go.obo> /label.right /colors <default=Set1:c6> /tick <default=-1> /level <default=2> /selects Q3 /size <2000,2200> /out <out.DIR>]")>
    <ArgumentAttribute("/GO", True, CLITypes.File,
              Description:="The go database file path, if this argument is present in the CLI, then will using the GO.obo database file from GCModeller repository.")>
    <ArgumentAttribute("/level", True, CLITypes.Integer,
              Description:="The GO annotation level from the DAG, default is level 2. Argument value -1 means no level.")>
    <ArgumentAttribute("/label.right", True, CLITypes.Boolean,
              Description:="Plot GO term their label will be alignment on right. default is alignment left if this aegument is not present.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv",
              Description:="Uniprot XML database export result from ``/protein.annotations`` command.")>
    <ArgumentAttribute("/tick", True, CLITypes.Double,
              Description:="The Axis ticking interval, if this argument is not present in the CLI, then program will create this interval value automatically.")>
    <ArgumentAttribute("/size", True, CLITypes.String, AcceptTypes:={GetType(Size)},
              Description:="The size of the output plot image.")>
    <ArgumentAttribute("/selects", True, CLITypes.String,
              Description:="The quantity selector for the bar plot content, by default is using quartile Q3 value, which means the term should have at least greater than Q3 quantitle then it will be draw on the bar plot.")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              Extensions:="*.csv, *.png",
              Description:="A directory path which will created for save the output result. The output result from this command contains a bar plot png image and a csv file for view the Go terms distribution in the sample uniprot annotation data.")>
    <ArgumentAttribute("/colors", True, CLITypes.String, PipelineTypes.undefined,
              AcceptTypes:={GetType(String), GetType(String())},
              Description:="Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
              
              + <profile name term>: Set1:c6 
              Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
              + <color name list>: black,green,blue 
              Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function ProteinsGoPlot(args As CommandLine) As Integer
        Dim goDB$ = (args <= "/go") Or (GCModeller.FileSystem.GO & "/go.obo").AsDefault
        Dim in$ = args <= "/in"
        Dim size$ = (args <= "/size") Or "2000,2200".AsDefault
        Dim selects$ = args.GetValue("/selects", "Q3")
        Dim tick! = args.GetValue("/tick", -1.0!)
        Dim level% = args.GetValue("/level", 2)
        Dim labelRight As Boolean = args.IsTrue("/label.right")
        Dim out$ = (args <= "/out") Or ([in].ParentPath & "/GO/").AsDefault

        cat("\n")
        Call $"   ===> level={level}".__INFO_ECHO
        cat("\n")

        ' 绘制GO图
        Dim goTerms As Dictionary(Of String, Term) = GO_OBO _
            .Open(path:=goDB) _
            .ToDictionary(Function(x) x.id)
        Dim DAG As New Graph(goTerms.Values)
        Dim sample = [in].LoadSample
        Dim selector = Function(x As EntityObject)
                           Return x("GO") _
                               .Split(";"c) _
                               .Select(AddressOf Trim) _
                               .ToArray
                       End Function
        Dim data As Dictionary(Of String, NamedValue(Of Integer)()) =
            sample _
            .CountStat(selector, goTerms)

        If level > 0 Then
            data = data.LevelGOTerms(level, DAG)
            out &= $"/level={level}/"
        End If

        Call data.SaveCountValue(out & "/plot.csv")
        Call CatalogPlots.Plot(data, selects:=selects,
                               tick:=tick,
                               size:=size,
                               axisTitle:="Number Of Proteins",
                               labelAlignmentRight:=labelRight,
                               valueFormat:="F0", colorSchema:=args("colors") Or DefaultColorSchema) _
            .Save(out & $"/plot.png")

        Return 0
    End Function

    ''' <summary>
    ''' 总蛋白注释绘制KEGG分布图
    ''' 
    ''' ##### 20190707 也可以使用这个函数从sbh或者bbh的结果表格之中导出KO的注释信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/proteins.KEGG.plot")>
    <Usage("/proteins.KEGG.plot /in <proteins-uniprot-annotations.csv> [/field <default=KO> /not.human /geneId.field <default=nothing> /label.right /colors <default=Set1:c6> /custom <sp00001.keg> /size <2200,2000> /tick 20 /out <out.DIR>]")>
    <Description("KEGG function catalog profiling plot of the TP sample.")>
    <ArgumentAttribute("/custom",
              Description:="Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg. 
              You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&format=htext&filedir= for download the custom KO classification set.")>
    <ArgumentAttribute("/label.right", True, CLITypes.Boolean, Description:="Align the label from right.")>
    <ArgumentAttribute("/size", True, CLITypes.String, Description:="The canvas size value.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.Xlsx, *.csv",
              Description:="Total protein annotation from UniProtKB database. Which is generated from the command ``/protein.annotations``.")>
    <ArgumentAttribute("/colors", True, CLITypes.String, PipelineTypes.undefined,
              AcceptTypes:={GetType(String), GetType(String())},
              Description:="Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
              
              + <profile name term>: Set1:c6 
              Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
              + <color name list>: black,green,blue 
              Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function proteinsKEGGPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim fieldName$ = args("/field") Or "KO"
        Dim size$ = args("/size") Or "2200,2000"
        Dim tick! = args.GetValue("/tick", 20.0!)
        Dim out$ = args("/out") Or ([in].ParentPath & "/KEGG/")
        Dim geneIdField$ = args("/geneId.field")
        Dim sample As EntityObject() = [in].LoadSample(geneIdField)
        Dim labelRight As Boolean = args.IsTrue("/label.right")
        Dim isHuman As Boolean = Not args.IsTrue("/not.human")
        Dim maps As NamedValue(Of String)() = sample _
            .Where(Function(prot) Not prot(fieldName).StringEmpty) _
            .Select(Function(prot)
                        Return prot(fieldName) _
                            .StringSplit(";\s+") _
                            .Select(Function(KO)
                                        Return New NamedValue(Of String) With {
                                            .Name = prot.ID,
                                            .Value = KO
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray

        Dim KO_counts As KOCatalog() = Nothing
        Dim profile = maps.LevelAKOStatics(KO_counts).AsDouble
        Dim catalogs As NamedValue(Of Dictionary(Of String, String))()

        With args <= "/custom"
            If .FileExists(True) Then
                ' 如果自定义的KO分类数据文件存在的话，则使用自定义库
                catalogs = maps.KOCatalog(.ByRef)
            Else
                ' 直接使用系统库进行分析
                catalogs = maps.KOCatalog
            End If
        End With

        If Not isHuman Then
            Call "Removes [Human Diseases] pathway from result...".__INFO_ECHO
            Call KO_counts.Select(Function(k) k.Class).Distinct.GetJson.__DEBUG_ECHO

            catalogs = catalogs.Where(Function(cls) InStr(cls.Value!Class, "Human", CompareMethod.Text) = 0).ToArray
            KO_counts = KO_counts.Where(Function(cls) InStr(cls.Class, "Human", CompareMethod.Text) = 0).ToArray

            With profile.Keys.FirstOrDefault(Function(key) InStr(key, "Human", CompareMethod.Text) > 0)
                If Not .StringEmpty Then
                    Call profile.Remove(.ByRef)
                End If
            End With
        End If

        KO_counts.SaveTo(out & "/KO_counts.csv")
        catalogs _
            .DataFrame _
            .SaveTo(out & "/KOCatalogs.csv")

        Call New CatalogProfiles(profile) _
            .ProfilesPlot("KEGG Orthology Profiling",
                size:=size,
                tick:=tick,
                axisTitle:="Number Of Proteins",
                labelRightAlignment:=labelRight,
                valueFormat:="F0",
                colorSchema:=args("/colors") Or DefaultKEGGColorSchema
            ) _
            .Save(out & "/plot.png")

        Return 0
    End Function

    ''' <summary>
    ''' 假若蛋白质组的原始检测结果之中含有多个物种的蛋白，则可以使用这个方法利用bbh将其他的物种的蛋白映射回某一个指定的物种的蛋白
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Species.Normalization")>
    <Usage("/Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML> /idMapping <refSeq2uniprotKB_mappings.tsv> /annotations <annotations.csv> [/out <out.csv>]")>
    <ArgumentAttribute("/bbh", False, CLITypes.File,
              Description:="The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function NormalizeSpecies(args As CommandLine) As Integer
        Dim bbh As String = args <= "/bbh"
        Dim uniprot As String = args <= "/uniprot"
        Dim mappings As String = args <= "/idMapping"
        Dim annotations As String = args <= "/annotations"
        Dim out As String = args.GetValue("/out", annotations.TrimSuffix & "-species.normalization.csv")
        Dim annotationData As IEnumerable(Of UniprotAnnotations) = annotations.LoadCsv(Of UniprotAnnotations)
        Dim mappingsID = Retrieve_IDmapping.MappingReader(mappings)
        Dim output As New List(Of UniprotAnnotations)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(bh) bh.isMatched) _
            .ToDictionary(Function(bh)
                              Return bh.QueryName.Split("|"c).First
                          End Function)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniProtXML.LoadDictionary(uniprot)

        For Each protein As UniprotAnnotations In annotationData

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.uniprot) Then
                Dim bbhHit As String = bbhData(protein.uniprot).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.StringEmpty AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)

                    protein.uniprot = DirectCast(uniprotData, INamedValue).Key
                    protein.geneName = uniprotData.gene.names.First.value
                    protein.ORF = uniprotData.gene.ORF.First
                    protein.fullName = uniprotData.proteinFullName
                    protein.Data.Add("bbh", bbhHit)
                Else
                    ' 可能有些编号在uniprot之中还不存在，则记录下来这个id
                    protein.Data.Add("bbh", bbhHit)
                End If
            End If

            output += protein
        Next

        Return output.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' bbh的结果是通过<see cref="BBHIndex.QueryName"/>来和数据集中的<see cref="EntityObject.ID"/>进行关联的，所以这就要求<see cref="BBHIndex.QueryName"/>必须是唯一的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/ID.Replace.bbh")>
    <Description("LabelFree result helper: replace the source ID to a unify organism protein ID by using ``bbh`` method. 
        This tools required the protein in ``datatset.csv`` associated with the alignment result in ``bbh.csv`` by using the ``query_name`` property.")>
    <Usage("/ID.Replace.bbh /in <dataset.csv> /bbh <bbh/sbh.csv> [/description <fieldName, default=Description> /out <ID.replaced.csv>]")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function BBHReplace(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim bbh$ = args <= "/bbh"
        Dim out$ = (args <= "/out") Or $"{in$.TrimSuffix}_replaced.csv".AsDefault
        Dim dataset As EntityObject() = EntityObject _
            .LoadDataSet([in]) _
            .ToArray
        Dim fieldDesc$ = args("/description") Or "Description"

        ' 2019-03-10 一般是使用这个函数将自定义的序列编号映射到Uniprot之上
        ' 所以在这里应该尝试解析的是uniprot的编号
        Dim accessionParser As New ITryParse(AddressOf TryGetUniProtAccession)
        Dim alignments As IEnumerable(Of BBHIndex) =
            Iterator Function(input As IEnumerable(Of BBHIndex)) As IEnumerable(Of BBHIndex)
                For Each x As BBHIndex In input
                    x.HitName = accessionParser.TryParse(x.HitName, TryParseOptions.Source)

                    If x.QueryName.IndexOf("|"c) > -1 Then
                        x.QueryName = TrimAccessionVersion(x.QueryName.Split("|"c)(1))
                    Else
                        x.QueryName = TrimAccessionVersion(x.QueryName)
                    End If

                    Yield x
                Next
            End Function(bbh.LoadCsv(Of BBHIndex))
        ' 这里面的比对结果已经全部都是唯一的了
        Dim alignHits As Dictionary(Of String, BBHIndex) = alignments.UniqueAlignment

        For Each protein As EntityObject In dataset
            ' 可能是一个proteinGroup
            ' 选取最好的比对结果?
            Dim proteinGroup As String() = protein.ID _
                .StringSplit("\s*;\s*") _
                .Select(Function(id) id.Split("."c).First) _
                .ToArray
            ' 选出所有的唯一的比对结果
            Dim allHits As BBHIndex() = proteinGroup _
                .Where(Function(id) alignHits.ContainsKey(id)) _
                .Select(Function(id) alignHits(id)) _
                .OrderByDescending(Function(hit) hit.identities) _
                .ToArray

            If allHits.Length > 0 Then
                ' 有比对结果, 因为是降序排序,所以直接取第一个
                Dim hitUniProt = allHits(Scan0)

                protein.ID = hitUniProt.HitName
                protein(fieldDesc) = UniprotFasta.ParseHeader(hitUniProt!description, hitUniProt.HitName).ProtName
            Else
                ' 没有比对结果,则取出原来的第一个id
                protein.ID = proteinGroup.First
            End If
        Next

        Return dataset _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/blastX.fill.ORF")>
    <Description("")>
    <Usage("/blastX.fill.ORF /in <annotations.csv> /blastx <blastx.csv> [/out <out.csv>]")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function BlastXFillORF(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim blastx$ = args <= "/blastx"
        Dim out$ = (args <= "/out") Or $"{in$.TrimSuffix}-blastx.ORF.csv".AsDefault
        Dim proteins As EntityObject() = EntityObject _
            .LoadDataSet([in]) _
            .ToArray
        Dim blastXhits = blastx _
            .LoadCsv(Of BlastXHit) _
            .ToDictionary(Function(hit)
                              If hit.HitName.IndexOf("|"c) > -1 Then
                                  Return hit.HitName.Split("|"c)(1)
                              Else
                                  Return hit.HitName
                              End If
                          End Function,
                          Function(query) query.QueryName)

        For Each protein In proteins
            If blastXhits.ContainsKey(protein.ID) Then
                protein!ORF = blastXhits(protein.ID)
            End If
        Next

        Return proteins _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/COG.profiling.plot",
               Info:="Plots the COGs category statics profiling of the target genome from the COG annotation file.",
               Usage:="/COG.profiling.plot /in <myvacog.csv> [/size <image_size, default=1800,1200> /out <out.png>]")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv",
              Description:="The COG annotation result.")>
    Public Function COGCatalogProfilingPlot(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim size$ = args.GetValue("/size", "1800,1200")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".COG.profiling.png")
        Dim COGs As IEnumerable(Of MyvaCOG) = [in].LoadCsv(Of MyvaCOG)

        Return COGs.COGCatalogProfilingPlot(size) _
            .Save(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 显示KEGG注释结果的barplot
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KO.Catalogs",
               Info:="Display the barplot of the KEGG orthology match.",
               Usage:="/KO.Catalogs /in <blast.mapping.csv> /ko <ko_genes.csv> [/key <Query_id> /mapTo <Subject_id> /out <outDIR>]")>
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
            Dim result = SMRUCC.genomics.Analysis.KEGG _
                .KEGGOrthology _
                .CatalogProfiling(mappings, KO_genes, level)
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
                Call result.ToDictionary(
                    Function(x) x.Key,
                    Function(v) v.Value.Select(
                    Function(x) New NamedValue(Of Double)(x.Name, x.Value)).ToArray) _
                        .Plot() _
                        .Save(out & "/kegg-level-A.png")
            End If
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 这个函数总是会将目标输入编号mapping为uniprotKB编号
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Uniprot.Mappings",
               Info:="Retrieve the uniprot annotation data by using ID mapping operations.",
               Usage:="/Uniprot.Mappings /in <id.list> [/type <P_REFSEQ_AC> /out <out.DIR>]")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function UniprotMappings(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim type As ID_types = args.GetValue("/type", ID_types.P_REFSEQ_AC, AddressOf IDTypeParser)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-uniprot/")

        Call Retrieve_IDmapping.Mapping(
            uploadQuery:=[in].ReadAllLines,
            from:=type,
            [to]:=ID_types.ACC_ID,
            save:=out & "/proteins.txt")

        Return 0
    End Function
End Module
