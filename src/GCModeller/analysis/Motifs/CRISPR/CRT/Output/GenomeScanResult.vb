#Region "Microsoft.VisualBasic::37f2cae6db342f483c6eb236b35e5da5, analysis\Motifs\CRISPR\CRT\Output\GenomeScanResult.vb"

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

    '     Class GenomeScanResult
    ' 
    '         Properties: KMerProfile, Length, Sites, Tag, title
    ' 
    '         Function: CreateObject, ExportFasta, ExportSpacerFasta
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Output

    ''' <summary>
    ''' CRISPR位点的基因组搜索的结果，可以使用这个对象将CRISPR的结果保存为XML格式的结果文件，最后通过xlst将结果以html的形式格式化显示出来
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("CRISPR.output", Namespace:="http://GCModeller.org/analysis/CRISPR/")>
    Public Class GenomeScanResult

        <XmlText>
        Public Property title As String
        <XmlAttribute>
        Public Property Length As Integer
        Public Property KMerProfile As KmerProfile
        Public Property Sites As CRISPR()

        ''' <summary>
        ''' 可以使用本标签信息进行基因组的LocusID的信息的存储
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute>
        Public Property Tag As String

        ''' <summary>
        ''' 导出每一个位点之间的重复片段的序列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportFasta() As FASTA.FastaFile
            Dim LQuery = LinqAPI.Exec(Of FastaSeq) <=
 _
                From site As CRISPR
                In Sites
                Select From rp As Loci
                       In site.RepeatLocis
                       Let attrs = New String() {
                           String.Format("{0}_{1}_{2}", Tag, site.ID, rp.Left)
                       }
                       Select New FastaSeq With {
                           .Headers = attrs,
                           .SequenceData = rp.SequenceData
                       }

            Return New FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' 导出每一个位点之中的重复片段之间的间隔序列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportSpacerFasta() As FastaFile
            Dim LQuery = LinqAPI.Exec(Of FastaSeq) <=
 _
                From site As CRISPR
                In Sites
                Select From sp As Loci
                       In site.SpacerLocis
                       Let attrs = New String() {
                           String.Format("{0}_{1}_{2}", Tag, site.ID, sp.Left)
                       }
                       Select New FastaSeq With {
                           .Headers = attrs,
                           .SequenceData = sp.SequenceData
                       }

            Return New FastaFile(LQuery)
        End Function

        Public Shared Function CreateObject(nt As FastaSeq, tag$, dat As IEnumerable(Of SearchingModel.CRISPR), ScanProfile As KmerProfile) As GenomeScanResult
            Dim Result As New GenomeScanResult With {
                .title = nt.Title,
                .KMerProfile = ScanProfile,
                .Length = nt.Length,
                .Tag = tag
            }
            Result.Sites = LinqAPI.Exec(Of CRISPR) <=
 _
                From o As SeqValue(Of SearchingModel.CRISPR)
                In dat.SeqIterator
                Let c As SearchingModel.CRISPR = o.value
                Let spaces = c.NumberOfSpacers _
                    .Sequence _
                    .Select(Function(i) New Loci With {
                        .Left = c.SpacingAt(i),
                        .SequenceData = c.RepeatStringAt(i)
                    }).ToArray
                Let repeats = c.NumberOfRepeats _
                    .Sequence _
                    .Select(Function(i) New Loci With {
                        .Left = c.RepeatAt(i),
                        .SequenceData = c.RepeatStringAt(i)
                    }).ToArray
                Select New CRISPR With {
                    .Start = c.StartLeft,
                    .SpacerLocis = spaces,
                    .RepeatLocis = repeats,
                    .ID = o.i
                }

            Return Result
        End Function
    End Class
End Namespace
