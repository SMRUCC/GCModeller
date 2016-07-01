#Region "Microsoft.VisualBasic::d3781cad1ba3fc7e2469eae14597ad56, ..\GCModeller\analysis\CRISPR\CRT\Output\GenomeScanResult.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.CRISPR.SearchingModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService
Imports LANS.SystemsBiology
Imports SMRUCC.genomics.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.SequenceModel.FASTA

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
