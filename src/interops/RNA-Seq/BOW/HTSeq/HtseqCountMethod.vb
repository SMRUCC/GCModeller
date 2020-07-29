#Region "Microsoft.VisualBasic::fa1c94809c720e8e1b7ca73668a9b350, RNA-Seq\BOW\HTSeq\HtseqCountMethod.vb"

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

    ' Module HtseqCountMethod
    ' 
    '     Function: DataFrame, HtseqCount, (+2 Overloads) RPKM, TrimGFF
    '     Delegate Function
    ' 
    '         Properties: HtSeqMethods
    ' 
    '         Function: __getFeatures, __htSeqCount, GetMethod, HtseqCount, HtseqCountBatchParallel
    '                   (+2 Overloads) IntersectionNonempty, (+2 Overloads) IntersectionStrict, ToDoc, (+2 Overloads) Union
    ' 
    '         Sub: __getFeatures
    '     Class CountResult
    ' 
    '         Properties: Counts, Feature
    ' 
    '         Function: Load, ToString
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.SAM

''' <summary>
''' Counting reads in features with htseq-count, Given a file with aligned sequencing reads and a list of genomic features, a common task is to count how many reads map 
''' to each feature.
''' </summary>
''' <remarks>为了得到比较好的计算性能，SAM文件之中的Reads数据首先被转换为位置数据进行缓存</remarks>
<Package("HTSeq-count",
                        Description:="Given a file with aligned sequencing reads and a list of genomic features, a common task is to count how many reads map to each feature.",
                        Cites:="Anders, S., et al. (2015). ""HTSeq--a Python framework To work With high-throughput sequencing data."" Bioinformatics 31(2): 166-169.
<p>MOTIVATION: A large choice of tools exists for many standard tasks in the analysis of high-throughput sequencing (HTS) data. However, once a project deviates from standard workflows, custom scripts are needed. RESULTS: We present HTSeq, a Python library to facilitate the rapid development of such scripts. HTSeq offers parsers for many common data formats in HTS projects, as well as classes to represent data, such as genomic coordinates, sequences, sequencing reads, alignments, gene model information and variant calls, and provides data structures that allow for querying via genomic coordinates. We also present htseq-count, a tool developed with HTSeq that preprocesses RNA-Seq data for differential expression analysis by counting the overlap of reads with genes. AVAILABILITY AND IMPLEMENTATION: HTSeq is released as an open-source software under the GNU General Public Licence and available from http://www-huber.embl.de/HTSeq or from the Python Package Index at https://pypi.python.org/pypi/HTSeq.",
                        Publisher:="sanders@fs.tum.de")>
<Cite(Title:="HTSeq--a Python framework To work With high-throughput sequencing data.",
          Journal:="Bioinformatics",
          Pages:="166-169",
          Keywords:="*Gene Expression Regulation
*Genome, Human
Genomics/*methods
High-Throughput Nucleotide Sequencing/*methods
Humans
*Software",
          Issue:="2",
          ISSN:="1367-4811 (Electronic)
1367-4803 (Linking)",
          DOI:="10.1093/bioinformatics/btu638",
          Authors:="Anders, S.
Pyl, P. T.
Huber, W.",
          Abstract:="MOTIVATION: A large choice of tools exists for many standard tasks in the analysis of high-throughput sequencing (HTS) data. 
However, once a project deviates from standard workflows, custom scripts are needed. 
<p><p>RESULTS: We present HTSeq, a Python library to facilitate the rapid development of such scripts. 
HTSeq offers parsers for many common data formats in HTS projects, as well as classes to represent data, such as genomic coordinates, sequences, sequencing reads, alignments, gene model information and variant calls, and provides data structures that allow for querying via genomic coordinates. 
We also present htseq-count, a tool developed with HTSeq that preprocesses RNA-Seq data for differential expression analysis by counting the overlap of reads with genes. 
<p><p>AVAILABILITY AND IMPLEMENTATION: HTSeq is released as an open-source software under the GNU General Public Licence and available from http://www-huber.embl.de/HTSeq or from the Python Package Index at https://pypi.python.org/pypi/HTSeq.",
          AuthorAddress:="Genome Biology Unit, European Molecular Biology Laboratory, 69111 Heidelberg, Germany.",
          Volume:=31,
          Year:=2015,
          PubMed:=25260700,
          URL:="http://www-huber.embl.de/HTSeq")>
Public Module HtseqCountMethod

    ''' <summary>
    ''' 有时候假若不需要使用基因名称，而是想要使用基因编号来表示一个基因，则可以通过这个方法将gff文件之中的基因名转换为基因号
    ''' </summary>
    ''' <param name="gff"></param>
    ''' <param name="ptt"></param>
    ''' <returns></returns>
    <ExportAPI("gff.Trim",
                   Info:="The gene identifier in a gff file some times can be a gene name, if you don't want to using the gene name to identified a gene and more prefer using its locus_tag data, 
                   then you can using this function replace the gene name to locus_tag.")>
    Public Function TrimGFF(gff As GFFTable, PTT As PTT) As GFFTable
        ' 通过位置来进行替换
        Dim Locihash As Dictionary(Of String, String) =
                PTT.GeneObjects.ToDictionary(Function(GeneObject) GeneObject.Location.ToString,
                                             Function(GeneObject) GeneObject.Synonym)
        Dim LQuery = (From gFeature As Feature
                          In gff.Features
                      Let Loci As String = gFeature.MappingLocation.ToString
                      Select Loci, gFeature).ToArray

        For Each GeneObject In LQuery
            If Not Locihash.ContainsKey(GeneObject.Loci) Then
                Continue For
            End If
            GeneObject.gFeature.attributes("name") = Locihash(GeneObject.Loci)
        Next

        gff.Features = (From gene In LQuery
                        Select gene.gFeature
                        Order By gFeature.attributes("name") Ascending).ToArray
        Return gff
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="readsCount"></param>
    ''' <param name="totalLength"></param>
    ''' <param name="mappedReads"></param>
    ''' <returns></returns>
    ''' <remarks>好像有问题？？？</remarks>
    <ExportAPI("RPKM")>
    Public Function RPKM(readsCount As Integer, totalLength As Integer, mappedReads As Integer) As Double
        Return readsCount / (mappedReads * 0.000000001 * totalLength)
    End Function

    ''' <summary>
    ''' 包括重新排序，然后将基因名称重新换回基因号，并且在这里除以序列本身的长度，得到RPKM值
    ''' </summary>
    ''' <param name="FileList"></param>
    ''' <param name="PTT">由于有些基因是不表达的，所以htseq-count计数的时候会少了一些基因，使用PTT文件的原因是补全这些基因</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ##                 SRR479052.bam SRR479053.bam SRR479054.bam
    ''' ## ENSG00000000003      0             0              1
    ''' ## ENSG00000000005      0             0              0
    ''' ## ENSG00000000419      0             0              0
    ''' ## ENSG00000000457      0             1              0
    ''' ## ENSG00000000460      0             0              0
    ''' ## ENSG00000000938      0             0              0
    ''' </remarks>
    ''' 
    <ExportAPI("Data.Frame", Info:="Generates the data input for the DESeq2 R package.")>
    Public Function DataFrame(<Parameter("List.Files")> FileList As IEnumerable(Of String),
                                  PTT As PTT,
                                  <Parameter("RNA.Statics?")> Optional StaticsRNA As Boolean = False) As IO.File
        Dim LQuery = (From File As String In FileList.AsParallel
                      Let ID As String = Path.GetFileNameWithoutExtension(File)
                      Select ID, data = CountResult.Load(File)).ToArray
        Dim AllFeatures = (From Experiment In LQuery
                           Select (From obj In Experiment.data
                                   Select FeatureGeneName = obj.Feature).ToArray).Unlist.Distinct.ToArray.OrderBy(Of String)(Function(str) str).ToArray
        Dim ChunkBuffer As New IO.File

        '生成表头
        Dim Head As New IO.RowObject From {""}
        Call Head.AddRange((From Experiment In LQuery Select Experiment.ID).ToArray) '为了保持一一对应关系，从这里开始不可以再使用并行拓展
        Call ChunkBuffer.Add(Head)

        For Each Feature As String In AllFeatures
            Dim dataExpr0 = (From Experiemtn In LQuery
                             Let expr = (From loci In Experiemtn.data
                                         Where String.Equals(Feature, loci.Feature)
                                         Select loci).ToArray
                             Select Experiemtn, expr).ToArray

            Dim locusId As String = PTT.TryGetGenesId(Feature)

            If String.IsNullOrEmpty(locusId) Then
                If StaticsRNA Then
                    locusId = Feature
                Else
                    Continue For
                End If
            End If

            Dim Row As New RowObject From {locusId}

            If Not (From obj In dataExpr0 Where obj.expr.Count > 1 Select obj).ToArray.IsNullOrEmpty Then
                Call $"Warning! There are duplicated tag value ""{Feature}"" ===> ""{locusId}"" in the experiment data!".__DEBUG_ECHO
            End If

            Dim data As String() = (From obj In dataExpr0 Select If(obj.expr.IsNullOrEmpty, 0, obj.expr.First.Counts)).Select(Function(x) CStr(x)).ToArray

            Call Row.AddRange(data)
            Call ChunkBuffer.Add(Row)
        Next

        Return ChunkBuffer
    End Function

    ''' <summary>
    ''' (函数只是得到了原始计数，还需要与序列的长度相除才可以得到RPKM) 
    ''' Anders, S., Pyl, P. T., &amp; Huber, W. (2015). HTSeq--a Python framework to work with high-throughput sequencing data. Bioinformatics, 31(2), 166-169. doi: 10.1093/bioinformatics/btu638
    ''' </summary>
    ''' <param name="SAM"></param>
    ''' <param name="GFF"></param>
    ''' <param name="Mode"></param>
    ''' <returns>
    ''' The script outputs a table with counts for each feature, followed by the special counters, 
    ''' which count reads that were not counted for any feature for various reasons. 
    ''' 
    ''' 脚本程序输出一个包含有每一个Feature的Reads计数的表
    ''' 
    ''' The names of the special counters all start with a double underscore, to facilitate filtering. 
    ''' (Note: The double unscore was absent up to version 0.5.4). 
    ''' 
    ''' 
    ''' The special counters are:
    '''     __no_feature: reads (or read pairs) which could not be assigned to any feature (setSas described above was empty).
    '''     __ambiguous: reads (or read pairs) which could have been assigned to more than one feature and hence were not counted for any of these (setShad mroe than one element).
    '''     __too_low_aQual: reads (or read pairs) which were skipped due to the-aoption, see below
    '''     __not_aligned: reads (or read pairs) in the SAM file without alignment
    '''     __alignment_not_unique: reads (or read pairs) with more than one reported alignment. 
    ''' 
    '''     __no_feature: reads或者reads对不可以被分配到任何一个Feature之上
    '''     __ambiguous: reads或者reads对可以被分配到任意一个Feature之上，
    ''' 
    ''' These reads are recognized from theNHoptional SAM field tag. 
    ''' (If the aligner does not set this field, multiply aligned reads will be counted multiple times, 
    ''' unless they getv filtered out by due to the-aoption.)
    ''' 
    ''' Important:
    ''' The default for strandedness isyes. If your RNA-Seq data has not been made with a strand-specific protocol, 
    ''' this causes half of the reads to be lost. Hence, make sure to set the option --stranded=nounless you have 
    ''' strand-specific data!
    ''' </returns>
    <ExportAPI("HTSeq-count", Info:="This counter function returns a text value of the count table, you can easily write the table to file using the string io operations.
The output value of the function is the rpkm value if the optional function parameter RPKM is set to TRUE value, 
if FALSE for the original feature mapping reads.
Anders, S., Pyl, P. T., & Huber, W. (2015). HTSeq--a Python framework to work with high-throughput sequencing data. Bioinformatics, 31(2), 166-169. doi: 10.1093/bioinformatics/btu638")>
    Public Function HtseqCount(SAM As IEnumerable(Of AlignmentReads),
                                   GFF As GFFTable,
                                   <Parameter("Mode", "The value of this parameter specific the counter of the function will be used, " &
                                   "the available counter values are: union, intersection_strict and intersection_nonempty")>
                                   Optional Mode As String = "intersection_nonempty",
                                   Optional RPKM As Boolean = False,
                                   Optional feature As Features = Features.CDS) As String

        Dim HtSeq As HtSeqMethod = GetMethod(Mode)
        Dim data As CountResult() = HtSeq(SAM, GFF, feature)

        If Not RPKM Then
            Return data.ToDoc
        Else
            Call "Calculating RPKM value...".__DEBUG_ECHO
            Return data.RPKM(GFF).ToDoc
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dataExpr0">这里的表达量的计数全部都是原始计数</param>
    ''' <returns></returns>
    <ExportAPI("/RPKM"), Extension>
    Public Function RPKM(dataExpr0 As IEnumerable(Of CountResult), gff As GFFTable) As CountResult()
        Dim MappedReads As Double = (From loci As CountResult In dataExpr0 Select loci.Counts).Sum  '总的Reads计数
        Dim totalLen As Integer = gff.Size
        Dim RPKMValues = (From loci As CountResult In dataExpr0
                          Select loci,
                                  RPKMValue = HtseqCountMethod.RPKM(loci.Counts, totalLen, MappedReads)
                          Order By loci.Feature Ascending).ToArray
        Dim array As CountResult() = RPKMValues _
            .Select(Function(x)
                        Return New CountResult With {
                            .Feature = x.loci.Feature,
                            .Counts = x.RPKMValue
                        }
                    End Function) _
            .ToArray

        Return array
    End Function

    Delegate Function HtSeqMethod(SAM As IEnumerable(Of AlignmentReads), GFF As GFFTable, feature As Features) As CountResult()

    Public ReadOnly Property HtSeqMethods As IReadOnlyDictionary(Of String, HtSeqMethod) =
            New Dictionary(Of String, HtSeqMethod) From {
 _
            {"union", AddressOf Union},
            {"intersection_strict", AddressOf IntersectionStrict},
            {"intersection_nonempty", AddressOf IntersectionNonempty}
        }

    <ExportAPI("HtSeq-count.Mode")>
    Public Function GetMethod(mode As String) As HtSeqMethod
        Dim key As String = mode.ToLower

        If HtSeqMethods.ContainsKey(key) Then
            Return _HtSeqMethods(key)
        End If

        Call $"mode={mode} currently is not supported yet, using default ""union"".".__DEBUG_ECHO
        Return AddressOf Union
    End Function

    Public Class CountResult
        Public Property Feature As String
        Public Property Counts As Double

        Public Overrides Function ToString() As String
            Return Feature & vbTab & CStr(Counts)
        End Function

        Public Shared Function Load(path As String) As CountResult()
            Dim lines As String() = path.ReadAllLines
            Dim array As CountResult() = LinqAPI.Exec(Of CountResult) <=
 _
                From line As String
                In lines
                Let Tokens As String() = Strings.Split(line, vbTab)
                Where Tokens.Length >= 2
                Let expr As Double = Val(Tokens(1))
                Let value As CountResult = New CountResult With {
                    .Feature = Tokens(Scan0),
                    .Counts = expr
                }
                Select value

            Return array
        End Function
    End Class

    <ExportAPI("HtSeq-count.Doc"), Extension>
    Public Function ToDoc(source As IEnumerable(Of CountResult)) As String
        Dim array As String() = source.Select(Function(x) x.ToString).ToArray
        Dim doc As String = String.Join(vbCrLf, array)
        Return doc
    End Function

    ''' <summary>
    ''' (函数只是得到了原始计数，还需要与序列的长度相除才可以得到RPKM)
    ''' </summary>
    ''' <param name="SAM"></param>
    ''' <param name="GFF"></param>
    ''' <param name="Mode"></param>
    ''' <returns></returns>
    <ExportAPI("HTSeq-count", Info:="This counter function returns a text value of the count table, you can easily write the table to file using the string io operations.
The output value of the function is the rpkm value if the optional function parameter RPKM is set to TRUE value, 
if FALSE for the original feature mapping reads.
Anders, S., Pyl, P. T., & Huber, W. (2015). HTSeq--a Python framework to work with high-throughput sequencing data. Bioinformatics, 31(2), 166-169. doi: 10.1093/bioinformatics/btu638")>
    Public Function HtseqCount(SAM As String, GFF As String,
                                   <Parameter("Mode", "The value of this parameter specific the counter of the function will be used, " &
                                   "the available counter values are: union, intersection_strict and intersection_nonempty")> Optional Mode As String = "intersection_nonempty",
                                   Optional RPKM As Boolean = False,
                                   Optional feature As Features = Features.CDS) As String

        Dim SAMFile As New SAMStream(SAM)
        Dim GFFFile As GFFTable = GFFTable.LoadDocument(GFF)
        Return HtseqCount(SAMFile.IteratesAllReads, GFFFile, Mode, RPKM)
    End Function

    ''' <summary>
    ''' 执行脚本调用本身进行批量计算(函数只是得到了原始计数，还需要与序列的长度相除才可以得到RPKM)
    ''' </summary>
    ''' <param name="GFF"></param>
    ''' <param name="Mode"></param>
    ''' <param name="Parallel">内存足够大的时候可以使用这个参数，要不然计算会非常的缓慢</param>
    ''' <returns></returns>
    <ExportAPI("HTSeq-count", Info:="This counter function returns a text value of the count table, you can easily write the table to file using the string io operations.
The output value of the function is the rpkm value if the optional function parameter RPKM is set to TRUE value, 
if FALSE for the original feature mapping reads.
Anders, S., Pyl, P. T., & Huber, W. (2015). HTSeq--a Python framework to work with high-throughput sequencing data. Bioinformatics, 31(2), 166-169. doi: 10.1093/bioinformatics/btu638")>
    Public Function HtseqCountBatchParallel(GFF As String,
                                                <Parameter("DIR.SAM", "The program will search all of the *.sam file for the calculation.")> SAM_DIR As String,
                                                <Parameter("DIR.Export")> Export As String,
                                                <Parameter("Mode", "The value of this parameter specific the counter of the function will be used, " &
                                                "the available counter values are: union, intersection_strict and intersection_nonempty")>
                                                Optional Mode As String = "intersection_nonempty",
                                                Optional RPKM As Boolean = False,
                                                Optional Parallel As Boolean = False) As String()

        Export = FileIO.FileSystem.GetDirectoryInfo(Export).FullName
        Call FileIO.FileSystem.CreateDirectory(Export)

        Dim ScriptBuilder = (From File As KeyValuePair(Of String, String)
                                 In SAM_DIR.LoadSourceEntryList({"*.sam"})
                             Let Script As String = New String(My.Resources.HTSeq_Count_Invoked)
                             Select Script.Replace("{$Out.Dir}", Export.CLIPath) _
                                              .Replace("{$File.Sam}", File.Value.CLIPath) _
                                              .Replace("{$GFF}", GFF.CLIPath) _
                                              .Replace("{$Mode}", Mode.CLIPath) _
                                              .Replace("{$RPKM}", If(RPKM, "TRUE", "FALSE"))).ToArray

        Dim ShoalShell As String = Settings.Session.TryGetShoalShellBin
        If Not ShoalShell.FileExists Then
            Call $"Could not determine the shoalshell environment at ""{ShoalShell}""!".__DEBUG_ECHO
            Call $"Program run failure....".__DEBUG_ECHO
            Return Nothing
        End If

        If Parallel Then
            Dim LQuery = (From Script As String In ScriptBuilder.AsParallel
                          Let SavedCli As String = FileIO.FileSystem.GetTempFileName
                          Let HTSeqCount = New IORedirectFile(ShoalShell, SavedCli)
                          Where Script.SaveTo(SavedCli)
                          Select HTSeqCount.Run).ToArray
        Else
            For Each Script As String In ScriptBuilder
                Dim SavedCli As String = FileIO.FileSystem.GetTempFileName
                Dim HTSeqCount = New IORedirectFile(ShoalShell, SavedCli)
                Call Script.SaveTo(SavedCli)
                Call HTSeqCount.Run()
            Next
        End If

        Return FileIO.FileSystem.GetFiles(Export, FileIO.SearchOption.SearchTopLevelOnly, "*.txt").ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gff"></param>
    ''' <param name="forwards"></param>
    ''' <param name="reversed"></param>
    Private Sub __getFeatures(gff As GFFTable, ByRef forwards As Feature(), ByRef reversed As Feature(), feature As Features)
        forwards = __getFeatures(gff, direct:=Strands.Forward, feature:=feature)
        reversed = __getFeatures(gff, direct:=Strands.Reverse, feature:=feature)
    End Sub

    Private Function __getFeatures(gff As GFFTable, direct As Strands, feature As Features) As Feature()
        Dim key As String = feature.ToString
        Dim features As Feature() = (From loci As Feature In gff.Features.AsParallel
                                     Where Not loci Is Nothing AndAlso
                                             loci.strand = direct AndAlso
                                             String.Equals(key, loci.feature, StringComparison.OrdinalIgnoreCase)
                                     Select loci).ToArray
        Return features
    End Function

    <ExportAPI("Union")>
    Public Function Union(SAM As IEnumerable(Of AlignmentReads), GFF As GFFTable, Optional feature As Features = Features.CDS) As CountResult()
        Dim GFF_Forwards As Feature() = Nothing, GFF_Reversed As Feature() = Nothing
        Call __getFeatures(GFF, GFF_Forwards, GFF_Reversed, feature)
        Dim FeaturesCounting As IEnumerable(Of Feature()) = From Read As AlignmentReads
                                                                In SAM.AsParallel
                                                            Select Union(Read, GFF_Forwards, GFF_Reversed) '
        Dim counts As CountResult() = __htSeqCount(FeaturesCounting, GFF_Forwards, GFF_Reversed)
        Return counts
    End Function

    ''' <summary>
    ''' [Feature][<see cref="vbTab"/>][Counts]
    ''' </summary>
    ''' <param name="csource"></param>
    ''' <returns></returns>
    Private Function __htSeqCount(csource As IEnumerable(Of Feature()), forwards As Feature(), reversed As Feature()) As CountResult()
        Dim features As Dictionary(Of String, Value(Of Integer)) =
                forwards.Join(reversed).ToDictionary(Function(x) x.attributes("name").ToUpper,
                                                     Function(x) New Value(Of Integer))
        For Each block As Feature() In csource
            Dim counts = (From feature As Feature
                              In block.AsParallel
                          Select name = feature.attributes("name").ToUpper).ToArray
            For Each loci In counts
                Dim feature As Value(Of Integer) = features(loci)
                feature.Value += 1
            Next
        Next

        Dim Lines As CountResult() = (From Feature In features
                                      Let count As CountResult = New CountResult With {
                                              .Feature = Feature.Key,
                                              .Counts = CDbl(Feature.Value.Value)
                                          }
                                      Select count).ToArray
        Return Lines
    End Function

    <ExportAPI("Intersection.Strict")>
    Public Function IntersectionStrict(SAM As IEnumerable(Of AlignmentReads), GFF As GFFTable, Optional feature As Features = Features.CDS) As CountResult()
        Dim GFF_Forwards As Feature() = Nothing, GFF_Reversed As Feature() = Nothing
        Call __getFeatures(GFF, GFF_Forwards, GFF_Reversed, feature)
        Dim FeaturesCounting = From Read As AlignmentReads
                                   In SAM.AsParallel
                               Select IntersectionStrict(Read, GFF_Forwards, GFF_Reversed) '
        Dim counts As CountResult() = __htSeqCount(FeaturesCounting, GFF_Forwards, GFF_Reversed)
        Return counts
    End Function

    <ExportAPI("Intersection.Nonempty")>
    Public Function IntersectionNonempty(SAM As IEnumerable(Of AlignmentReads), GFF As GFFTable, Optional feature As Features = Features.CDS) As CountResult()
        Dim GFF_Forwards As Feature() = Nothing, GFF_Reversed As Feature() = Nothing
        Call __getFeatures(GFF, GFF_Forwards, GFF_Reversed, feature)
        Dim FeaturesCounting = From Read As AlignmentReads
                                   In SAM.AsParallel
                               Select IntersectionNonempty(Read, GFF_Forwards, GFF_Reversed)
        Dim counts As CountResult() = __htSeqCount(FeaturesCounting, GFF_Forwards, GFF_Reversed)
        Return counts
    End Function

#Region "special counters"

    ''' <summary>
    ''' 只要有接触的都进行计数
    ''' </summary>
    ''' <param name="Read"></param>
    ''' <param name="GFF_Forwards"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Union")>
    Public Function Union(Read As AlignmentReads, GFF_Forwards As Feature(), GFF_Reversed As Feature()) As Feature()
        '首先得到所有和该Reads有接触的Feature，然后在依照模式进行统计
        Dim ReadsLocation = Read.GetLocation
        Dim FeatureSource = If(ReadsLocation.Strand = Strands.Forward, GFF_Forwards, GFF_Reversed)
        Dim LQuery = (From Feature As Feature In FeatureSource
                      Where ReadsLocation.LociIsContact(Feature)
                      Select Feature).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' Read只可以出现在Feature的内部
    ''' </summary>
    ''' <param name="Read"></param>
    ''' <param name="GFF_Forwards"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Intersection.Strict")>
    Public Function IntersectionStrict(Read As AlignmentReads, GFF_Forwards As Feature(), GFF_Reversed As Feature()) As Feature()
        '首先得到所有和该Reads有接触的Feature，然后在依照模式进行统计
        Dim ReadsLocation = Read.GetLocation
        Dim FeatureSource = If(ReadsLocation.Strand = Strands.Forward, GFF_Forwards, GFF_Reversed)
        Dim LQuery As Feature() =
                LinqAPI.Exec(Of Feature) <=
                From Feature As Feature
                In FeatureSource
                Where Feature.MappingLocation.GetRelationship(ReadsLocation) = SegmentRelationships.Inside
                Select Feature
        Return LQuery
    End Function

    ''' <summary>
    ''' 当Feature重叠在一起的时候，在内部的都计数，没有重叠的时候，可以计数
    ''' </summary>
    ''' <param name="Read"></param>
    ''' <param name="GFF_Forwards"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Intersection.Nonempty")>
    Public Function IntersectionNonempty(Read As AlignmentReads, GFF_Forwards As Feature(), GFF_Reversed As Feature()) As Feature()
        Dim ReadsLocation = Read.GetLocation
        Dim FeatureSource = If(ReadsLocation.Strand = Strands.Forward, GFF_Forwards, GFF_Reversed)
        Dim LQuery = (From Feature As Feature
                          In FeatureSource
                      Where Feature.MappingLocation.LociIsContact(ReadsLocation) '首先找出有接触的Feature
                      Select Feature).ToArray

        ' 假若LQuery的数目只有1，则说明没有重叠，直接返回
        If LQuery.Length <= 1 Then
            Return LQuery
        End If

        '当有重叠的时候，找出在内部的Feature就可以了
        LQuery = (From Feature As Feature In LQuery
                  Where Feature.MappingLocation.GetRelationship(ReadsLocation) = SegmentRelationships.Inside
                  Select Feature).ToArray
        Return LQuery
    End Function
#End Region
End Module
