Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Partial Module CLI

    ''' <summary>
    ''' 假若蛋白质组的原始检测结果之中含有多个物种的蛋白，则可以使用这个方法利用bbh将其他的物种的蛋白映射回某一个指定的物种的蛋白
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Sample.Species.Normalization",
               Usage:="/Sample.Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML/DIR> /idMapping <refSeq2uniprotKB_mappings.tsv> /sample <sample.csv> [/ID <columnName> /out <out.csv>]")>
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
        Dim mappingsID = Retrieve_IDmapping.MappingReader(mappings)
        Dim output As New List(Of EntityObject)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(bh) bh.Matched) _
            .ToDictionary(Function(bh) bh.QueryName.Split("|"c).First)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniprotXML.LoadDictionary(uniprot)
        Dim ORF$

        For Each protein As EntityObject In sampleData

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.ID) Then
                Dim bbhHit As String = bbhData(protein.ID).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.IsBlank AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)

                    protein.ID = uniprotData.accession
                    ORF = uniprotData.ORF
                    If ORF.IsBlank Then
                        ORF = protein.ID
                    End If
                    protein.Properties.Add("ORF", ORF)
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
                If ORF.IsBlank Then
                    ORF = uniprotData.accession
                End If
                protein.Properties.Add("ORF", ORF)
            End If

            output += protein
        Next

        Return output.SaveTo(out).CLICode
    End Function
End Module
