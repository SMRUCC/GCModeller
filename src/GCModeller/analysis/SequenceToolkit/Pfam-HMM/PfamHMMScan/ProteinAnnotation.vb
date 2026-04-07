#Region "Microsoft.VisualBasic::a28dd03f89e53c7337db07eaed214beb, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\ProteinAnnotation.vb"

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

    '   Total Lines: 256
    '    Code Lines: 143 (55.86%)
    ' Comment Lines: 75 (29.30%)
    '    - Xml Docs: 73.33%
    ' 
    '   Blank Lines: 38 (14.84%)
    '     File Size: 9.18 KB


    ' Class ProteinAnnotator
    ' 
    '     Properties: BitScoreThreshold, DatabaseSize, EValueThreshold, ModelCount, ModelNames
    ' 
    '     Function: Annotate, AnnotateAll, AnnotateTop, CalculateConfidence, CompareSequence
    ' 
    '     Sub: LoadModel, LoadModelContent, LoadModelsFromDirectory
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' 蛋白质序列分类注释模块
' 
' 实现蛋白质FASTA序列的读取和基于HMMER3模型的分类注释功能
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 蛋白质序列分类注释器
''' 使用Profile HMM对蛋白质序列进行功能注释
''' </summary>
Public Class ProteinAnnotator

    ' 已加载的HMM模型字典
    Private _models As New Dictionary(Of String, ProfileHMM)()

    ' E值阈值
    Private _eValueThreshold As Double = 0.01

    ' 比特得分阈值
    Private _bitScoreThreshold As Double = 25.0

    ' 数据库大小（用于E值计算）
    Private _databaseSize As Integer = 10000

    ''' <summary>
    ''' 获取或设置E值阈值
    ''' </summary>
    Public Property EValueThreshold As Double
        Get
            Return _eValueThreshold
        End Get
        Set(value As Double)
            _eValueThreshold = value
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置比特得分阈值
    ''' </summary>
    Public Property BitScoreThreshold As Double
        Get
            Return _bitScoreThreshold
        End Get
        Set(value As Double)
            _bitScoreThreshold = value
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置数据库大小
    ''' </summary>
    Public Property DatabaseSize As Integer
        Get
            Return _databaseSize
        End Get
        Set(value As Integer)
            _databaseSize = value
        End Set
    End Property

    ''' <summary>
    ''' 加载单个HMMER3模型文件
    ''' </summary>
    ''' <param name="filePath">模型文件路径</param>
    Public Sub LoadModel(filePath As String)
        Dim model As ProfileHMM = HMMER3Parser.Parse(filePath)
        If model IsNot Nothing AndAlso Not String.IsNullOrEmpty(model.Name) Then
            _models(model.Name) = model
        End If
    End Sub

    ''' <summary>
    ''' 从目录加载所有HMMER3模型文件
    ''' </summary>
    ''' <param name="directoryPath">目录路径</param>
    ''' <param name="searchPattern">文件搜索模式（默认*.hmm）</param>
    Public Sub LoadModelsFromDirectory(directoryPath As String, Optional searchPattern As String = "*.hmm")
        If Not Directory.Exists(directoryPath) Then
            Throw New DirectoryNotFoundException($"Directory not found: {directoryPath}")
        End If

        Dim files As String() = Directory.GetFiles(directoryPath, searchPattern)
        For Each file As String In files
            Try
                LoadModel(file)
            Catch ex As Exception
                ' 记录错误但继续加载其他模型
                Console.WriteLine($"Error loading model {file}: {ex.Message}")
            End Try
        Next
    End Sub

    ''' <summary>
    ''' 加载模型内容字符串
    ''' </summary>
    ''' <param name="modelContent">模型文本内容</param>
    Public Sub LoadModelContent(modelContent As String)
        Dim model As ProfileHMM = HMMER3Parser.ParseContent(modelContent)
        If model IsNot Nothing AndAlso Not String.IsNullOrEmpty(model.Name) Then
            _models(model.Name) = model
        End If
    End Sub

    ''' <summary>
    ''' 获取已加载的模型数量
    ''' </summary>
    Public ReadOnly Property ModelCount As Integer
        Get
            Return _models.Count
        End Get
    End Property

    ''' <summary>
    ''' 获取所有已加载的模型名称
    ''' </summary>
    Public ReadOnly Property ModelNames As IEnumerable(Of String)
        Get
            Return _models.Keys
        End Get
    End Property

    ''' <summary>
    ''' 对单个蛋白质序列进行注释
    ''' </summary>
    ''' <param name="protein">蛋白质序列</param>
    ''' <returns>最佳注释结果</returns>
    Public Function Annotate(protein As FastaSeq) As IEnumerable(Of AnnotationResult)
        If protein Is Nothing OrElse String.IsNullOrEmpty(protein.SequenceData) Then
            Return Nothing
        End If

        Dim all As New List(Of AnnotationResult)

        For Each kvp As KeyValuePair(Of String, ProfileHMM) In _models
            Dim model As ProfileHMM = kvp.Value
            Dim result As AnnotationResult = CompareSequence(protein.SequenceData, model)

            If result IsNot Nothing Then
                result.SeqId = protein.Headers(0)
                result.IsSignificant = (result.EValue <= _eValueThreshold AndAlso
                                         result.BitScore >= _bitScoreThreshold)
                result.Confidence = CalculateConfidence(result.BitScore, result.EValue)
                all.Add(result)
            End If
        Next

        Return From result As AnnotationResult
               In all
               Order By result.BitScore Descending
    End Function

    ''' <summary>
    ''' 对单个蛋白质序列进行注释
    ''' </summary>
    ''' <param name="protein">蛋白质序列</param>
    ''' <returns>最佳注释结果</returns>
    Public Function AnnotateTop(protein As FastaSeq) As AnnotationResult
        If protein Is Nothing OrElse String.IsNullOrEmpty(protein.SequenceData) Then
            Return Nothing
        End If

        Dim bestResult As AnnotationResult = Nothing
        Dim bestScore As Double = Double.NegativeInfinity

        For Each kvp As KeyValuePair(Of String, ProfileHMM) In _models
            Dim model As ProfileHMM = kvp.Value
            Dim result As AnnotationResult = CompareSequence(protein.SequenceData, model)

            If result IsNot Nothing AndAlso result.BitScore > bestScore Then
                bestScore = result.BitScore
                bestResult = result
            End If
        Next

        ' 判断是否显著
        If bestResult IsNot Nothing Then
            bestResult.IsSignificant = (bestResult.EValue <= _eValueThreshold AndAlso
                                           bestResult.BitScore >= _bitScoreThreshold)
            bestResult.Confidence = CalculateConfidence(bestResult.BitScore, bestResult.EValue)
        End If

        Return bestResult
    End Function

    ''' <summary>
    ''' 对蛋白质序列列表进行批量注释
    ''' </summary>
    ''' <param name="proteins">蛋白质序列列表</param>
    Public Iterator Function AnnotateAll(proteins As IEnumerable(Of FastaSeq)) As IEnumerable(Of AnnotationResult)
        For Each protein As FastaSeq In proteins
            Yield AnnotateTop(protein)
        Next
    End Function

    ''' <summary>
    ''' 使用指定模型对序列进行比对
    ''' </summary>
    ''' <param name="sequence">氨基酸序列</param>
    ''' <param name="model">Profile HMM模型</param>
    ''' <returns>注释结果</returns>

    Private Function CompareSequence(sequence As String, model As ProfileHMM) As AnnotationResult
        Dim result As New AnnotationResult() With {
            .ModelName = model.Name
        }

        ' 计算比特得分
        Dim scoreResult As BitScoreResult = model.CalculateBitScore(sequence)
        result.BitScore = scoreResult.Score

        ' 计算E值
        result.EValue = model.CalculateEValue(result.BitScore, _databaseSize)

        ' 确定比对区域
        If scoreResult.AlignmentPath IsNot Nothing AndAlso scoreResult.AlignmentPath.Count > 0 Then
            Dim firstPos = scoreResult.AlignmentPath.First()
            Dim lastPos = scoreResult.AlignmentPath.Last()

            result.AlignmentStart = firstPos.SequencePosition
            result.AlignmentEnd = lastPos.SequencePosition

            ' 生成比对字符串
            Dim alignedSeqBuilder As New Text.StringBuilder()
            For Each pos In scoreResult.AlignmentPath
                If pos.State <> TraceState.DELETE AndAlso pos.SequencePosition > 0 AndAlso
                   pos.SequencePosition <= sequence.Length Then
                    alignedSeqBuilder.Append(sequence(pos.SequencePosition - 1))
                Else
                    alignedSeqBuilder.Append("-")
                End If
            Next

            result.AlignedSequence = alignedSeqBuilder.ToString()
        End If

        Return result
    End Function

    ''' <summary>
    ''' 计算置信度
    ''' </summary>
    Private Function CalculateConfidence(bitScore As Double, eValue As Double) As Double
        ' 基于比特得分和E值计算置信度
        ' 使用sigmoid函数将得分映射到[0,1]区间
        Dim x As Double = bitScore / 100.0 - Math.Log10(Math.Max(eValue, 1.0E-300))
        Return 1.0 / (1.0 + Math.Exp(-x))
    End Function

End Class

