Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace DocumentFormat.MEME.LDM

    ''' <summary>
    ''' 生成<see cref="motif"/>位点的序列
    ''' </summary>
    Public Class Site : Inherits SequenceModel.ISequenceBuilder
        Implements sIdEnumerable

        ''' <summary>
        ''' Site name，该目标序列的Fasta文件的文件头，一般是基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overrides Property Name As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' 位点的序列
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Site As String
        <XmlAttribute> Public Overridable Property Pvalue As Double
        ''' <summary>
        ''' 在整条序列之中的起始位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overridable Property Start As Long
        <XmlAttribute> Public Property Right As Long
        ''' <summary>
        ''' 用作分析的原始序列的长度
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Size As Integer

        Public Overrides Function ToString() As String
            Return $"{Name}    {NameOf(Start)}:={Start}"
        End Function

        ''' <summary>
        ''' 解析MEME Text文件之中的位点数据
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InternalParser(s As String) As Site
            Dim Tokens As String() = (From ss As String In s.Split
                                      Where Not String.IsNullOrEmpty(ss)
                                      Select ss).ToArray
            Return New Site With {
                .Name = Tokens(0),
                .Start = CLng(Val(Tokens(1))),
                .Pvalue = Val(Tokens(2)),
                .Site = Tokens(4)
            }
        End Function

        Public Overrides Function GetSequenceData() As String
            Return Site
        End Function

        ''' <summary>
        ''' <see cref="Name"/>:<see cref="Start"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function ToFasta(Optional prefix As String = "") As FASTA.FastaToken
            If String.IsNullOrEmpty(prefix) Then
                Return New FASTA.FastaToken({$"{Name}:{Start}"}, Site)
            Else
                Dim uid As String = prefix & $"-{Name}:{Start}"
                Return New FASTA.FastaToken({uid}, Site)
            End If
        End Function

        Public Const BLOCK As String = "\d+_\[\d+\](_\d+)?"
    End Class
End Namespace