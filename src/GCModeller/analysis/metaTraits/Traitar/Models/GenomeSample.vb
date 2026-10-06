#Region "Microsoft.VisualBasic::d7fecfd066d939212a11db59efc503ca, analysis\Metagenome\MetaFunction\metaTraits\Traitar\Models\GenomeSample.vb"

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


    ' Code Statistics:

    '   Total Lines: 152
    '    Code Lines: 64 (42.11%)
    ' Comment Lines: 60 (39.47%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 28 (18.42%)
    '     File Size: 5.96 KB


    '     Class GenomeSample
    ' 
    '         Properties: PfamAnnotations, PfamCount, PhyleticProfile, Proteins, SampleId
    '                     SourceFile
    ' 
    '         Function: BuildPhyleticProfile, GetPresentPfamIds, HasPfam
    ' 
    '         Sub: BuildPhyleticProfile
    ' 
    '     Class ProteinSequence
    ' 
    '         Properties: [End], Product, ProteinId, Sequence, SequenceId
    '                     Start, Strand
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' GenomeSample.vb - 基因组样本数据结构
'
' 对应论文中：
'   - 输入基因组（DNA FASTA 或 蛋白质 FASTA）
'   - 基因预测结果（Prodigal输出）
'   - Pfam注释结果（HMMER输出）
'   - 系统发育谱（phyletic pattern，0/1特征向量）
' ============================================================================

Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER

Namespace metaTraits.Traitar.Models

    ''' <summary>
    ''' 基因组样本类
    ''' 封装从基因组到特征向量的全部中间数据
    ''' </summary>
    Public Class GenomeSample

        ''' <summary>样本名称/标识符</summary>
        Public Property SampleId As String

        ''' <summary>样本来源文件路径</summary>
        Public Property SourceFile As String

        ''' <summary>预测的基因/蛋白列表</summary>
        Public Property Proteins As New List(Of ProteinSequence)

        ''' <summary>Pfam家族注释列表</summary>
        Public Property PfamAnnotations As New List(Of PfamAnnotation)

        ''' <summary>
        ''' 系统发育谱（phyletic pattern）
        ''' PfamID -> 存在(1)/缺失(0)
        ''' 论文：将每个样本中各Pfam家族的数量转化为存在(1)或缺失(0)的二元矩阵X
        ''' </summary>
        Public Property PhyleticProfile As New Dictionary(Of String, Integer)

        ''' <summary>
        ''' 从Pfam注释构建系统发育谱（二值化）
        ''' 论文：设定阈值（比特分数阈值为25，E值阈值为1e-2），
        '''       过滤掉不可靠的命中，随后将每个样本中各Pfam家族
        '''       的数量转化为存在(1)或缺失(0)的二元矩阵
        ''' </summary>
        ''' <param name="bitScoreThreshold">比特分数阈值（默认25）</param>
        ''' <param name="evalueThreshold">E值阈值（默认1e-2）</param>
        Public Sub BuildPhyleticProfile(
            Optional bitScoreThreshold As Double = 25.0,
            Optional evalueThreshold As Double = 0.01)

            PhyleticProfile = BuildPhyleticProfile(PfamAnnotations, bitScoreThreshold, evalueThreshold)
        End Sub

        ''' <summary>
        ''' 从Pfam注释构建系统发育谱（二值化）
        ''' 论文：设定阈值（比特分数阈值为25，E值阈值为1e-2），
        '''       过滤掉不可靠的命中，随后将每个样本中各Pfam家族
        '''       的数量转化为存在(1)或缺失(0)的二元矩阵
        ''' </summary>
        ''' <param name="bitScoreThreshold">比特分数阈值（默认25）</param>
        ''' <param name="evalueThreshold">E值阈值（默认1e-2）</param>
        Public Shared Function BuildPhyleticProfile(PfamAnnotations As IEnumerable(Of PfamAnnotation),
                                                    Optional bitScoreThreshold As Double = 25.0,
                                                    Optional evalueThreshold As Double = 0.01) As Dictionary(Of String, Integer)

            Dim phyleticProfile As New Dictionary(Of String, Integer)
            ' 按Pfam家族分组，统计每个家族的命中数
            Dim pfamCounts As New Dictionary(Of String, Integer)

            For Each ann As PfamAnnotation In PfamAnnotations
                ' 过滤不可靠的命中
                If ann.BitScore < bitScoreThreshold Then Continue For
                If ann.EValue > evalueThreshold Then Continue For

                If pfamCounts.ContainsKey(ann.PfamId) Then
                    pfamCounts(ann.PfamId) += 1
                Else
                    pfamCounts(ann.PfamId) = 1
                End If
            Next

            ' 二值化：存在(1)/缺失(0)
            For Each kvp As KeyValuePair(Of String, Integer) In pfamCounts
                If kvp.Value > 0 Then
                    phyleticProfile(kvp.Key) = 1
                Else
                    phyleticProfile(kvp.Key) = 0
                End If
            Next

            Return phyleticProfile
        End Function

        ''' <summary>
        ''' 获取特征向量中存在的Pfam家族集合
        ''' </summary>
        Public Function GetPresentPfamIds() As List(Of String)
            Dim result As New List(Of String)
            For Each kvp As KeyValuePair(Of String, Integer) In PhyleticProfile
                If kvp.Value = 1 Then
                    result.Add(kvp.Key)
                End If
            Next
            Return result
        End Function

        ''' <summary>
        ''' 检查某Pfam家族是否存在
        ''' </summary>
        Public Function HasPfam(pfamId As String) As Boolean
            Return PhyleticProfile.ContainsKey(pfamId) AndAlso PhyleticProfile(pfamId) = 1
        End Function

        ''' <summary>
        ''' 获取特征向量中存在的Pfam家族数量
        ''' </summary>
        Public ReadOnly Property PfamCount As Integer
            Get
                Return PhyleticProfile.Where(Function(kvp) kvp.Value = 1).Count
            End Get
        End Property

    End Class

    ''' <summary>
    ''' 蛋白质序列类
    ''' </summary>
    Public Class ProteinSequence
        ''' <summary>蛋白ID</summary>
        Public Property ProteinId As String

        ''' <summary>所属序列/染色体</summary>
        Public Property SequenceId As String

        ''' <summary>起始位置</summary>
        Public Property Start As Integer

        ''' <summary>终止位置</summary>
        Public Property [End] As Integer

        ''' <summary>链方向（+/-）</summary>
        Public Property Strand As String

        ''' <summary>氨基酸序列</summary>
        Public Property Sequence As String

        ''' <summary>基因产物名称（来自GFF）</summary>
        Public Property Product As String
    End Class

End Namespace

