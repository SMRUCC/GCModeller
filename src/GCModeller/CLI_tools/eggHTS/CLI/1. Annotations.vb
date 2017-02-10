Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.GoStat
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA


Partial Module CLI

    ''' <summary>
    ''' 1. 总蛋白注释
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("/protein.annotations",
               Info:="Total proteins functional annotation by using uniprot database.",
               Usage:="/protein.annotations /list <uniprot.id.list.txt> /uniprot <uniprot.XML> [/out <out.csv>]")>
    <Group(CLIGroups.Annotation_CLI)>
    Public Function SampleAnnotations(args As CommandLine) As Integer
        Dim list As String = args("/list")
        Dim uniprot As String = args("/uniprot")
        Dim out As String = args.GetValue("/out", list.TrimSuffix & "-proteins-uniprot-annotations.csv")

        Return list.ReadAllLines _
            .GenerateAnnotations(uniprot) _
            .Select(Function(x) x.Item1) _
            .ToArray _
            .SaveDataSet(out).CLICode
    End Function

    ''' <summary>
    ''' 总蛋白注释绘制GO分布图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/proteins.Go.plot",
               Usage:="/proteins.Go.plot /in <proteins-uniprot-annotations.csv> [/GO <go.obo> /top 25 /size <2000,4000> /out <out.DIR>]")>
    Public Function ProteinsGoPlot(args As CommandLine) As Integer
        Dim goDB As String = args.GetValue("/go", GCModeller.FileSystem.GO & "/go.obo")
        Dim in$ = args("/in")
        Dim size As Size = args.GetValue("/size", New Size(2000, 4000))
        Dim out As String = args.GetValue("/out", [in].ParentPath & "/GO/")
        Dim top% = args.GetValue("/top", 25)

        ' 绘制GO图
        Dim goTerms As Dictionary(Of String, Term) = GO_OBO.Open(goDB).ToDictionary(Function(x) x.id)
        Dim sample = [in].LoadSample
        Dim selector = Function(x As IO.EntityObject) x("GO").Split(";"c).Select(AddressOf Trim).ToArray
        Dim data As Dictionary(Of String, NamedValue(Of Integer)()) =
            sample.CountStat(selector, goTerms)

        Call data.SaveCountValue(out & "/plot.csv")
        Call CatalogPlots.Plot(data, orderTakes:=top).SaveAs(out & "/plot.png")

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
            sp.IsBlank,
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
                    .Attributes = {prot.accession, orf$}
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
        Dim uniprotXML As UniprotXML = UniprotXML.Load(uniprot)
        Dim mappingsID = Retrieve_IDmapping.MappingReader(mappings)
        Dim output As New List(Of UniprotAnnotations)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(bh) bh.Matched) _
            .ToDictionary(Function(bh) bh.QueryName.Split("|"c).First)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = uniprotXML.entries.ToDictionary

        For Each protein As UniprotAnnotations In annotationData

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.uniprot) Then
                Dim bbhHit As String = bbhData(protein.uniprot).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.IsBlank AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)

                    protein.uniprot = uniprotData.accession
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
End Module