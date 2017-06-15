Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.KEGG.KEGGOrthology
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.GeneOntology.GoStat
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize

Partial Module CLI

    ''' <summary>
    ''' 生成总蛋白的表达状况的文氏图，主要统计的是蛋白表达与否？
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/proteinGroups.venn")>
    <Usage("/proteinGroups.venn /in <proteinGroups.csv> /designer <designer.csv> [/label <tag label> /deli <delimiter, default=_> /out <out.venn.DIR>]")>
    Public Function proteinGroupsVenn(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer$ = args <= "/designer"
        Dim label$ = args <= "/label"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".venn/")
        Dim proteins = EntityObject.LoadDataSet([in])
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim groups = designers _
            .GroupBy(Function(d) d.GroupLabel) _
            .ToArray
        Dim venn As New List(Of EntityObject)
        Dim delimiter$ = args.GetValue("/deli", "_")
        Dim groupLabels As New Dictionary(Of String, String())
        Dim gl$
        Dim labels$() = Nothing
        Dim addLabels = Sub()
                            labels = labels _
                                .Select(Function(l)
                                            If label.StringEmpty Then
                                                Return l
                                            Else
                                                Return $"{label}{delimiter}{l}"
                                            End If
                                        End Function) _
                                .Select(Function(l) l.Replace("-", ".")) _
                                .ToArray
                            gl = labels.LongestTag
                            groupLabels.Add(gl, labels)
                        End Sub

        For Each group In groups
            labels = group _
                .Select(Function(d) d.Experiment) _
                .ToArray
            addLabels()
            labels = group _
                .Select(Function(d) d.Control) _
                .ToArray
            addLabels()
        Next

        Call groupLabels.GetJson(True).__DEBUG_ECHO

        For Each protein As EntityObject In proteins
            Dim x As New EntityObject(protein.ID)

            ' 先遍历每一个蛋白
            ' 再遍历每一个实验设计
            ' 如果实验重复超过半数以上都是零，则为空
            For Each group As KeyValuePair(Of String, String()) In groupLabels
                Dim vals#() = group.Value _
                    .Select(Function(l) Val(protein(l))) _
                    .Where(Function(v)
                               Return (Not v.IsNaNImaginary) AndAlso (Not v = 0R)
                           End Function) _
                    .ToArray
                Dim n% = Fix(group.Value.Length / 2)

                If vals.Length >= n Then
                    ' 超过半数，没有表达
                    x.Properties.Add(group.Key, vals.Average)
                Else
                    ' 置为零的时候，venn模型任然会认为有数据，会出错，在这里置为空字符串
                    x.Properties.Add(group.Key, "")
                End If
            Next

            venn += x
        Next

        Dim dataOUT$ = out & "/proteinGroups.venn.csv"

        Call venn.SaveTo(dataOUT)
        Call Apps.VennDiagram.Draw(
            dataOUT,
            "proteinGroups.venn",
            out:=out & "/venn.tiff")

        Return 0
    End Function

    <ExportAPI("/update.uniprot.mapped",
               Usage:="/update.uniprot.mapped /in <table.csv> /mapping <mapping.tsv/tab> [/source /out <out.csv>]")>
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

    <ExportAPI("/Samples.IDlist",
               Info:="Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.",
               Usage:="/Samples.IDlist /in <samples.csv> [/Perseus /shotgun /pair <samples2.csv> /out <out.list.txt>]")>
    <Argument("/Perseus", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this flag was presented, that means the input sample data is the Perseus analysis output file ``ProteinGroups.txt``, or the input sample data is the iTraq result.")>
    Public Function GetIDlistFromSampleTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim isPerseus As Boolean = args.GetBoolean("/Perseus")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".geneIDs.txt")
        Dim list$()
        Dim isShotgun As Boolean = args.GetBoolean("/shotgun")

        If isPerseus Then
            list = {}
        ElseIf isShotgun Then
            Dim pair$ = args <= "/pair"
            Dim pairData As EntityObject() = If(
                pair.FileExists(True),
                EntityObject.LoadDataSet(pair).ToArray,
                {})
            Dim input = EntityObject.LoadDataSet([in])

            list = input.Values("UniprotID") _
                .JoinIterates(pairData.Values("UniprotID")) _
                .Distinct _
                .ToArray
            out = [in].TrimSuffix & "-" & pair.BaseName(allowEmpty:=True) & ".geneIDs.txt"
        Else
            Dim table = EntityObject.LoadDataSet([in])
            list$ = table.Keys(distinct:=True)
        End If

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
    <ExportAPI("/protein.annotations",
               Info:="Total proteins functional annotation by using uniprot database.",
               Usage:="/protein.annotations /uniprot <uniprot.XML> [/accession.ID /iTraq /list <uniprot.id.list.txt/rawtable.csv> /mapping <mappings.tab/tsv> /out <out.csv>]")>
    <Argument("/list", True, CLITypes.File,
              AcceptTypes:={GetType(String())},
              Description:="Using for the iTraq method result.")>
    <Argument("/iTraq", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Using for the iTraq method result.")>
    <Argument("/mapping",
              Description:="The id mapping table, only works when the argument ``/list`` is presented.")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function SampleAnnotations(args As CommandLine) As Integer
        Dim list As String = args("/list")
        Dim uniprot As String = args("/uniprot")
        Dim out As String
        Dim iTraq As Boolean = args.GetBoolean("/iTraq")
        Dim accID As Boolean = args.GetBoolean("/accession.ID")

        If list.FileExists(True) Then
            Dim geneIDs$()
            Dim mapping$ = args <= "/mapping"
            Dim mappings As Dictionary(Of String, String()) = Nothing

            out = args.GetValue("/out", list.TrimSuffix & "-proteins-uniprot-annotations.csv")

            If list.ExtensionSuffix.TextEquals("csv") Then
                geneIDs = EntityObject.LoadDataSet(list) _
                    .Select(Function(x) x.ID) _
                    .Distinct _
                    .ToArray
            Else
                geneIDs = list.ReadAllLines
            End If
            If mapping.FileExists Then
                mappings = Retrieve_IDmapping.MappingReader(mapping)
            End If

            Return geneIDs _
                .GenerateAnnotations(uniprot, iTraq, accID, mappings:=mappings) _
                .Select(Function(x) x.Item1) _
                .ToArray _
                .SaveDataSet(out) _
                .CLICode
        Else
            out = args.GetValue("/out", uniprot.ParentPath & "/proteins-uniprot-annotations.csv")

            Return uniprot _
                .ExportAnnotations(iTraq:=iTraq, accID:=accID) _
                .Select(Function(x) x.Item1) _
                .ToArray _
                .SaveDataSet(out) _
                .CLICode
        End If
    End Function

    <ExportAPI("/protein.annotations.shotgun",
               Usage:="/protein.annotations.shotgun /p1 <data.csv> /p2 <data.csv> /uniprot <data.DIR/*.xml,*.tab> [/remapping /out <out.csv>]")>
    Public Function SampleAnnotations2(args As CommandLine) As Integer
        Dim p1$ = args <= "/p1"
        Dim p2$ = args <= "/p2"
        Dim uniprot$ = args <= "/uniprot"
        Dim remapping As Boolean = args.GetBoolean("/remapping")
        Dim out As String = args.GetValue("/out", p1.TrimSuffix & "-" & p2.BaseName & ".uniprot-annotations.csv")
        Dim proteins As EntityObject()
        Dim p1Data = EntityObject.LoadDataSet(p1).ToArray
        Dim p2Data = EntityObject.LoadDataSet(p2).ToArray
        Dim list$() = p1Data.Keys _
            .JoinIterates(p2Data.Keys) _
            .Distinct _
            .ToArray
        Dim mappings As Dictionary(Of String, String()) = Nothing

        If remapping Then
            mappings = Retrieve_IDmapping.MappingsReader(DIR:=uniprot)
            list = mappings.Keys.ToArray
            proteins = list _
                .GenerateAnnotations(mappings, uniprot, , , , iTraq:=True) _
                .Select(Function(t) t.Item1) _
                .ToArray
        Else
            proteins = list _
                .GenerateAnnotations(uniprot, iTraq:=True) _
                .Select(Function(t) t.Item1) _
                .ToArray
        End If

        proteins = proteins _
            .MergeShotgunAnnotations(
                New NamedCollection(Of EntityObject)(p1.BaseName, p1Data),
                New NamedCollection(Of EntityObject)(p2.BaseName, p2Data),
                mappings)

        Return proteins.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 这个函数除了会生成注释表格之外，还会将原表格添加新的编号，以及导出蛋白表达的数据表
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Perseus.Table.annotations",
               Usage:="/Perseus.Table.annotations /in <proteinGroups.csv> /uniprot <uniprot.XML> [/scientifcName <""""> /out <out.csv>]")>
    Public Function PerseusTableAnnotations(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim uniprot As String = args("/uniprot")
        Dim out = args.GetValue("/out", [in].TrimSuffix & ".proteins.annotation.csv")
        Dim table As Perseus() = [in].LoadCsv(Of Perseus)
        Dim uniprotTable = UniprotXML.LoadDictionary(uniprot)
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
    <ExportAPI("/proteins.Go.plot",
               Info:="ProteinGroups sample data go profiling plot from the uniprot annotation data.",
               Usage:="/proteins.Go.plot /in <proteins-uniprot-annotations.csv> [/GO <go.obo> /tick 50 /top 20 /size <2000,4000> /out <out.DIR>]")>
    Public Function ProteinsGoPlot(args As CommandLine) As Integer
        Dim goDB As String = args.GetValue("/go", GCModeller.FileSystem.GO & "/go.obo")
        Dim in$ = args("/in")
        Dim size As Size = args.GetValue("/size", New Size(2000, 4000))
        Dim out As String = args.GetValue("/out", [in].ParentPath & "/GO/")
        Dim top% = args.GetValue("/top", 20)
        Dim tick! = args.GetValue("/tick", 50.0!)

        ' 绘制GO图
        Dim goTerms As Dictionary(Of String, Term) = GO_OBO.Open(goDB).ToDictionary(Function(x) x.id)
        Dim sample = [in].LoadSample
        Dim selector = Function(x As EntityObject)
                           Return x("GO") _
                               .Split(";"c) _
                               .Select(AddressOf Trim) _
                               .ToArray
                       End Function
        Dim data As Dictionary(Of String, NamedValue(Of Integer)()) =
            sample.CountStat(selector, goTerms)

        Call data.SaveCountValue(out & "/plot.csv")
        Call CatalogPlots.Plot(
            data, orderTakes:=top,
            tick:=tick,
            size:=size).Save(out & "/plot.png")

        Return 0
    End Function

    ''' <summary>
    ''' 总蛋白注释绘制KEGG分布图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/proteins.KEGG.plot")>
    <Usage("/proteins.KEGG.plot /in <proteins-uniprot-annotations.csv> [/custom <sp00001.keg> /size <2000,4000> /tick 20 /out <out.DIR>]")>
    <Description("KEGG function catalog profiling plot of the TP sample.")>
    <Argument("/custom",
              Description:="Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg")>
    Public Function proteinsKEGGPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim size As Size = args.GetValue("/size", New Size(2000, 4000))
        Dim tick! = args.GetValue("/tick", 20.0!)
        Dim out As String = args.GetValue("/out", [in].ParentPath & "/KEGG/")
        Dim sample = [in].LoadSample
        Dim maps As NamedValue(Of String)() = sample _
            .Where(Function(prot) Not prot("KO").StringEmpty) _
            .Select(Function(prot)
                        Return prot("KO").StringSplit(";\s+") _
                            .Select(Function(KO) New NamedValue(Of String) With {
                                .Name = prot.ID,
                                .Value = KO
                            })
                    End Function) _
            .IteratesALL _
            .ToArray

        Dim KO_counts As KOCatalog() = Nothing
        Dim profile = maps.LevelAKOStatics(KO_counts).AsDouble
        Dim catalogs As NamedValue(Of Dictionary(Of String, String))()

        With args <= "/custom"
            If .FileExists(True) Then
                catalogs = maps.KOCatalog(.ref)   ' 如果自定义的KO分类数据文件存在的话，则使用自定义库
            Else
                ' 直接使用系统库进行分析
                catalogs = maps.KOCatalog
            End If
        End With

        profile.ProfilesPlot("KEGG Orthology Profiling", size:=size, tick:=tick).Save(out & "/plot.png")
        KO_counts.SaveTo(out & "/KO_counts.csv")
        catalogs.DataFrame.SaveTo(out & "/KOCatalogs.csv")

        Return 0
    End Function

    <ExportAPI("/protein.EXPORT",
             Usage:="/protein.EXPORT /in <uniprot.xml> [/sp <name> /exclude /out <out.fasta>]")>
    <Argument("/sp", True, CLITypes.String,
            AcceptTypes:={GetType(String)},
            Description:="The organism scientific name.")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function proteinEXPORT(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim sp As String = args <= "/sp"
        Dim exclude As Boolean = args.GetBoolean("/exclude")
        Dim suffix$ = If(
            sp.StringEmpty,
            "",
            If(exclude, "-exclude", "") & "-" & sp.NormalizePathString.Replace(" ", "_"))
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"{suffix}.fasta")
        Dim uniprotXML As UniprotXML = UniprotXML.Load([in])

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            Dim source As IEnumerable(Of Uniprot.XML.entry) = uniprotXML.entries

            If Not String.IsNullOrEmpty(sp) Then
                If exclude Then
                    source = source _
                        .Where(Function(gene) Not gene.organism.scientificName = sp) _
                        .ToArray
                Else
                    source = source _
                        .Where(Function(gene) gene.organism.scientificName = sp) _
                        .ToArray
                End If
            End If

            For Each prot As Uniprot.XML.entry In source _
                .Where(Function(g) Not g.sequence Is Nothing)

                Dim orf$ = If(prot.gene Is Nothing, "", prot.gene.ORF.JoinBy(","))
                Dim fa As New FastaToken With {
                    .SequenceData = prot.sequence.sequence.lTokens.JoinBy(""),
                    .Attributes = {prot.accessions.First, orf$}
                }
                Call writer.WriteLine(fa.GenerateDocument(-1))
            Next
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 假若蛋白质组的原始检测结果之中含有多个物种的蛋白，则可以使用这个方法利用bbh将其他的物种的蛋白映射回某一个指定的物种的蛋白
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Species.Normalization",
               Usage:="/Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML> /idMapping <refSeq2uniprotKB_mappings.tsv> /annotations <annotations.csv> [/out <out.csv>]")>
    <Argument("/bbh", False, CLITypes.File,
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
            .Where(Function(bh) bh.Matched) _
            .ToDictionary(Function(bh) bh.QueryName.Split("|"c).First)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniprotXML.LoadDictionary(uniprot)

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

    <ExportAPI("/COG.profiling.plot",
               Info:="Plots the COGs category statics profiling of the target genome from the COG annotation file.",
               Usage:="/COG.profiling.plot /in <myvacog.csv> [/size <image_size, default=1800,1200> /out <out.png>]")>
    Public Function COGCatalogProfilingPlot(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim size As Size = args.GetValue("/size", New Size(1800, 1200))
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
                    Function(v) v.Value.ToArray(
                    Function(x) New NamedValue(Of Double)(x.Name, x.Value))) _
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