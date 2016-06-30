Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.CRISPR.SearchingModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.gbExportService
Imports LANS.SystemsBiology
Imports LANS.SystemsBiology.NCBI.Extensions.Analysis
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports LANS.SystemsBiology.SequenceModel.FASTA

Namespace Output

    ''' <summary>
    ''' CRISPR位点的基因组搜索的结果，可以使用这个对象将CRISPR的结果保存为XML格式的结果文件，最后通过xlst将结果以html的形式格式化显示出来
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("RepeatsSiteGenomeScanningResult", Namespace:="http://code.google.com/p/genome-in-code/motif_tools/crt")>
    Public Class GenomeScanResult

        Public Property FastaTitle As String
        <XmlAttribute> Public Property Length As Integer
        Public Property KMerProfile As KmerProfile
        Public Property Sites As CRISPR()

        ''' <summary>
        ''' 可以使用本标签信息进行基因组的LocusID的信息的存储
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Tag As String

        ''' <summary>
        ''' 导出每一个位点之间的重复片段的序列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportFasta() As FASTA.FastaFile
            Dim LQuery = (From site As CRISPR In Sites
                          Select (From rp As Loci
                              In site.RepeatLocis
                                  Let attrs As String() = New String() {String.Format("{0}_{1}_{2}", Tag, site.ID, rp.Left)}
                                  Select New FASTA.FastaToken With {
                                      .Attributes = attrs,
                                      .SequenceData = rp.SequenceData})).MatrixToList

            Return New FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' 导出每一个位点之中的重复片段之间的间隔序列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportSpacerFasta() As FastaFile
            Dim LQuery = (From site As CRISPR In Sites
                          Select (From sp As Loci In site.SpacerLocis
                                  Let attrs As String() = New String() {String.Format("{0}_{1}_{2}", Tag, site.ID, sp.Left)}
                                  Select New FastaToken With {
                                      .Attributes = attrs,
                                      .SequenceData = sp.SequenceData})).MatrixToList
            Return New FastaFile(LQuery)
        End Function

        Public Shared Function CreateObject(FastaSource As FastaToken, FastaTag As String, dat As IEnumerable(Of SearchingModel.CRISPR), ScanProfile As KmerProfile) As GenomeScanResult
            Dim Result As GenomeScanResult = New GenomeScanResult With {
                .FastaTitle = FastaSource.Title,
                .KMerProfile = ScanProfile,
                .Length = FastaSource.Length,
                .Tag = FastaTag
            }
            Result.Sites = (From item As SearchingModel.CRISPR In dat
                            Let spaces = (From i As Integer In item.NumberOfSpacers.Sequence Select New Loci With {.Left = item.SpacingAt(i), .SequenceData = item.RepeatStringAt(i)}).ToArray
                            Let repeats = (From i As Integer In item.NumberOfRepeats.Sequence
                                           Select New Loci With {.Left = item.RepeatAt(i), .SequenceData = item.RepeatStringAt(i)}).ToArray
                            Select New CRISPR With {
                                   .Start = item.StartLeft,
                                   .SpacerLocis = spaces,
                                   .RepeatLocis = repeats}).ToArray.AddHandle.ToArray

            Return Result
        End Function
    End Class
End Namespace