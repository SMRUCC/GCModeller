Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Topologically

    Public Class ImperfectPalindrome : Inherits NucleotideModels.Contig
        Implements ILoci

        ''' <summary>
        ''' 种子生成的序列
        ''' </summary>
        ''' <returns></returns>
        Public Property Site As String
        ''' <summary>
        ''' 种子序列在基因组上面的位置
        ''' </summary>
        ''' <returns></returns>
        Public Property Left As Integer Implements ILoci.Left
        ''' <summary>
        ''' 回文片段的位点位置
        ''' </summary>
        ''' <returns></returns>
        Public Property Paloci As Integer
        ''' <summary>
        ''' 回文片段的序列
        ''' </summary>
        ''' <returns></returns>
        Public Property Palindrome As String
        ''' <summary>
        ''' 片段相似度距离大小
        ''' </summary>
        ''' <returns></returns>
        Public Property Distance As Double
        ''' <summary>
        ''' 相似度高低
        ''' </summary>
        ''' <returns></returns>
        Public Property Score As Double
        ''' <summary>
        ''' 匹配的碱基
        ''' </summary>
        ''' <returns></returns>
        Public Property Matches As String
        ''' <summary>
        ''' 演化的路径
        ''' </summary>
        ''' <returns></returns>
        Public Property Evolr As String
        Public Property MaxMatch As Integer

        Public Overrides Function ToString() As String
            Return $"{Site} <==> {Palindrome}, {Matches}"
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(Left, Paloci)
        End Function
    End Class
End Namespace