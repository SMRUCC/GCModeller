#Region "Microsoft.VisualBasic::1c1247a1317bf756eab102bc66374f54, meme_suite\MEME.DocParser\MEME\LDM\Site.vb"

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

    '     Class Site
    ' 
    '         Properties: Name, Pvalue, Right, Site, Size
    '                     Start
    ' 
    '         Function: GetSequenceData, InternalParser, ToFasta, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel

Namespace DocumentFormat.MEME.LDM

    ''' <summary>
    ''' 生成<see cref="motif"/>位点的序列
    ''' </summary>
    Public Class Site : Inherits SequenceModel.ISequenceBuilder
        Implements INamedValue

        ''' <summary>
        ''' Site name，该目标序列的Fasta文件的文件头，一般是基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overrides Property Name As String Implements INamedValue.Key
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
        Friend Shared Function InternalParser(s As String) As Site
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
        Public Function ToFasta(Optional prefix As String = "") As FASTA.FastaSeq
            If String.IsNullOrEmpty(prefix) Then
                Return New FASTA.FastaSeq({$"{Name}:{Start}"}, Site)
            Else
                Dim uid As String = prefix & $"-{Name}:{Start}"
                Return New FASTA.FastaSeq({uid}, Site)
            End If
        End Function

        Public Const BLOCK As String = "\d+_\[\d+\](_\d+)?"
    End Class
End Namespace
