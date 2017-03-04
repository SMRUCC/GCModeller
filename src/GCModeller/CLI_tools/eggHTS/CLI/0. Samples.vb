Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.csv
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Partial Module CLI

    ''' <summary>
    ''' 将perseus软件的输出转换为csv文档并且导出uniprot编号以方便进行注释
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Perseus.Table",
               Usage:="/Perseus.Table /in <proteinGroups.txt> [/out <out.csv>]")>
    Public Function PerseusTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".csv")
        Dim data As Perseus() = [in].LoadTsv(Of Perseus)
        Dim idlist As String() = data _
            .Select(Function(prot) prot.ProteinIDs) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim uniprotIDs$() = idlist _
            .Select(Function(s) s.Split("|"c, ":"c)(1)) _
            .Distinct _
            .ToArray

        Call idlist.SaveTo(out.TrimSuffix & ".proteinIDs.txt")
        Call uniprotIDs.SaveTo(out.TrimSuffix & ".uniprotIDs.txt")

        Return data.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Perseus.Stat", Usage:="/Perseus.Stat /in <proteinGroups.txt> [/out <out.csv>]")>
    Public Function PerseusStatics(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".perseus.Stat.csv")
        Dim data As Perseus() = [in].LoadTsv(Of Perseus)
        Dim csv As New IO.File

        Call csv.AppendLine({"MS/MS", CStr(Perseus.TotalMSDivideMS(data))})
        Call csv.AppendLine({"Peptides", CStr(Perseus.TotalPeptides(data))})
        Call csv.AppendLine({"ProteinGroups", CStr(data.Length)})

        Return csv.Save(out, Encodings.ASCII).CLICode
    End Function

    ''' <summary>
    ''' 假若蛋白质组的原始检测结果之中含有多个物种的蛋白，则可以使用这个方法利用bbh将其他的物种的蛋白映射回某一个指定的物种的蛋白
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Sample.Species.Normalization",
               Usage:="/Sample.Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML/DIR> /idMapping <refSeq2uniprotKB_mappings.tsv> /sample <sample.csv> [/Description <Description.FileName> /ID <columnName> /out <out.csv>]")>
    <Argument("/bbh", False, CLITypes.File,
              Description:="The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function NormalizeSpecies_samples(args As CommandLine) As Integer
        Dim bbh As String = args <= "/bbh"
        Dim uniprot As String = args <= "/uniprot"
        Dim mappings As String = args <= "/idMapping"
        Dim sample As String = args <= "/sample"
        Dim out As String = args.GetValue("/out", sample.TrimSuffix & "-sample.species.normalization.csv")
        Dim sampleData As IEnumerable(Of EntityObject) = EntityObject.LoadDataSet(sample, args("/ID"))
        Dim mappingsID As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(mappings)
        Dim output As New List(Of EntityObject)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(bh) bh.Matched) _
            .ToDictionary(Function(bh) bh.QueryName.Split("|"c).First)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniprotXML.LoadDictionary(uniprot)
        Dim describKey As String = args("/Description")
        Dim ORF$

        If Not describKey.StringEmpty Then
            Call $"Substitute description in field: '{describKey}'.".Warning
        End If

        For Each protein As EntityObject In sampleData
            Dim uniprotHit_raw As String = protein.ID

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.ID) Then
                Dim bbhHit As String = bbhData(protein.ID).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.StringEmpty AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)

                    protein.ID = uniprotData.accession
                    ORF = uniprotData.ORF
                    If ORF.StringEmpty Then
                        ORF = protein.ID
                    End If
                    protein.Properties.Add("ORF", ORF)
                    If Not describKey.StringEmpty Then
                        protein(describKey) = uniprotData.proteinFullName
                    End If
                Else
                    ORF = bbhHit
                    protein.Properties.Add("ORF", ORF)
                End If

                ' 可能有些编号在uniprot之中还不存在，则记录下来这个id
                protein.Properties.Add("bbh", bbhHit)
            Else
                ' 直接查找
                Dim uniprotData As Uniprot.XML.entry = uniprotTable(protein.ID)
                ORF = uniprotData.ORF
                If ORF.StringEmpty Then
                    ORF = uniprotData.accession
                End If
                protein.Properties.Add("ORF", ORF)
            End If

            protein.Properties.Add(NameOf(uniprotHit_raw), uniprotHit_raw)
            output += protein
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Data.Add.Mappings",
               Usage:="/Data.Add.Mappings /in <data.csv> /bbh <bbh.csv> /ID.mappings <uniprot.ID.mappings.tsv> /uniprot <uniprot.XML> [/ID <fieldName> /out <out.csv>]")>
    Public Function AddReMapping(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim bbh As String = args("/bbh")
        Dim IDMappings As String = args("/ID.mappings")
        Dim uniprot As String = args("/uniprot")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".ID.mappings.csv")
        Dim ID As String = args("/ID")
        Dim data = EntityObject.LoadDataSet([in], uidMap:=ID)
        Dim bbhData As IEnumerable(Of BBHIndex) = bbh.LoadCsv(Of BBHIndex)
        Dim IDMappingsTable = Retrieve_IDmapping.MappingReader(IDMappings)
        Dim result As EntityObject() = data.AddReMapping(bbhData, IDMappingsTable, uniprot)

        Return result.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 有时候并不需要重新计算数据，则使用这个函数在原来的数据基础上面添加重新映射的结果即可
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AddReMapping(proteins As IEnumerable(Of EntityObject), bbh As IEnumerable(Of BBHIndex), mappingsID As Dictionary(Of String, String()), uniprot As String) As EntityObject()
        Dim output As New List(Of EntityObject)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .Where(Function(bh) bh.Matched) _
            .ToDictionary(Function(bh) bh.QueryName.Split("|"c).First)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniprotXML.LoadDictionary(uniprot)
        Dim ORF$

        ' 由于只是为了添加重新mapping的信息，所以在这里不需要进行ID替换
        For Each protein As EntityObject In proteins

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.ID) Then
                Dim bbhHit As String = bbhData(protein.ID).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.StringEmpty AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)
                    ORF = uniprotData.ORF
                    If ORF.StringEmpty Then
                        ORF = uniprotData.accession
                    End If
                    protein.Properties.Add("ORF", ORF)
                Else
                    ORF = bbhHit
                    protein.Properties.Add("ORF", ORF)
                End If
            Else
                ' 直接查找
                Dim uniprotData As Uniprot.XML.entry = uniprotTable(protein.ID)
                ORF = uniprotData.ORF
                If ORF.StringEmpty Then
                    ORF = uniprotData.accession
                End If
                protein.Properties.Add("ORF", ORF)
            End If

            output += protein
        Next

        Return output
    End Function

    ''' <summary>
    ''' 可能uniprot的标识符的识别度会比较低，在这里添加上比较熟悉的ORF编号
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Data.Add.ORF", Usage:="/Data.Add.ORF /in <data.csv> /uniprot <uniprot.XML> [/ID <fieldName> /out <out.csv>]")>
    Public Function DataAddORF(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim uniprot As String = args("/uniprot")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".ORF_ID.csv")
        Dim ID As String = args("/ID")
        Dim data = EntityObject.LoadDataSet([in], uidMap:=ID).ToArray
        Dim uniprotTable = UniprotXML.LoadDictionary(uniprot)

        For Each protein As EntityObject In data
            uniprot = protein.ID

            If uniprotTable.ContainsKey(uniprot) Then
                Dim gene As String = uniprotTable(uniprot).ORF

                If Not gene.StringEmpty Then
                    Call protein.Properties.Add("ORF", gene)
                Else
                    Call protein.Properties.Add("ORF", "*")
                End If
            End If
        Next

        Return data.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Data.Add.uniprotIDs",
               Usage:="/Data.Add.uniprotIDs /in <annotations.csv> /data <data.csv> [/out <out.csv>]")>
    Public Function DataAddUniprotIDs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim data As String = args("/data")
        Dim out As String = args.GetValue("/out", data.TrimSuffix & ".uniprotIDs.csv")
        Dim annotations = EntityObject.LoadDataSet([in]).ToDictionary
        Dim dataTable = EntityObject.LoadDataSet(data)

        For Each protein As EntityObject In dataTable
            Call protein.Properties.Add("uniprot", annotations(protein.ID)("uniprot"))
        Next

        Return dataTable.SaveTo(out).CLICode
    End Function

    <ExportAPI("/plot.pimw",
               Usage:="/plot.pimw /in <samples.csv> [/field.pi <calc. pI> /field.mw <MW [kDa]> /out <pimw.png> /size <1600,1200> /color <black> /pt.size <8>]")>
    Public Function pimwScatterPlot(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim pi$ = args.GetValue("/field.pi", "calc. pI")
        Dim mw$ = args.GetValue("/field.mw", "MW [kDa]")
        Dim size As Size = args.GetValue("/size", New Size(1600, 1200))
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".pI_MW.png")
        Dim color As String = args.GetValue("/color", "black")
        Dim ptSize! = args.GetValue("/pt.Size", 8.0!)
        Dim res As Image = {
            ScatterSerials(File.Load([in]), pi, mw, color, ptSize)
        }.Plot(size:=size,
               drawLine:=False,
               XaxisAbsoluteScalling:=True,
               absoluteScaling:=False,
               Xlabel:="Calc.pI",
               Ylabel:="MW [kDa]")

        Return res.SaveAs(out).CLICode
    End Function
End Module
