#Region "Microsoft.VisualBasic::947cddeac9f05154438af68d367aa6a8, engine\IO\GCMarkupLanguage\v2\Xml\Genome.vb"

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

'     Class Genome
' 
'         Properties: regulations, replicons
' 
'         Function: GetAllGeneLocusTags
' 
'     Class replicon
' 
'         Properties: genes, genomeName, isPlasmid, RNAs
' 
'         Function: ToString
' 
'     Class gene
' 
'         Properties: amino_acid, left, locus_tag, nucleotide_base, product
'                     protein_id, right, strand
' 
'     Class RNA
' 
'         Properties: gene, type, val
' 
'     Class transcription
' 
'         Properties: biological_process, centralDogma, effector, mode, motif
'                     regulator, target
' 
'     Class Motif
' 
'         Properties: distance, family, left, right, sequence
'                     strand
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace v2

    Public Class Genome

        ''' <summary>
        ''' 一个完整的基因组是由若干个复制子所构成的，复制子主要是指基因组和细菌的质粒基因组
        ''' </summary>
        ''' <returns></returns>
        <XmlElement(NameOf(replicon))>
        Public Property replicons As replicon()

        ''' <summary>
        ''' 转录调控网络
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 如果这个基因组是由多个复制子构成的，那么在这里面会由染色体上面的调控因子和质粒上的
        ''' 调控因子之间的相互调控作用网络的数据而构成
        ''' </remarks>
        Public Property regulations As transcription()

        Public Iterator Function GetAllGeneLocusTags(Optional skipPlasmids As Boolean = False) As IEnumerable(Of String)
            Dim source As IEnumerable(Of replicon)

            If skipPlasmids Then
                source = replicons.Where(Function(r) Not r.isPlasmid)
            Else
                source = replicons
            End If

            For Each replicon As replicon In source
                For Each gene In replicon.genes
                    Yield gene.locus_tag
                Next
            Next
        End Function
    End Class

    ''' <summary>
    ''' 复制子
    ''' </summary>
    Public Class replicon

        ''' <summary>
        ''' 当前的这个复制子对象是否是质粒基因组？
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property isPlasmid As Boolean
        <XmlAttribute> Public Property genomeName As String

        ''' <summary>
        ''' 基因列表，在这个属性之中定义了该基因组之中的所有基因的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property genes As XmlList(Of gene)
        ''' <summary>
        ''' 除了mRNA的其他的一切非蛋白编码RNA
        ''' </summary>
        ''' <returns></returns>
        Public Property RNAs As XmlList(Of RNA)

        Public Overrides Function ToString() As String
            Dim type$ = "Genome" Or "Plasmid genome".When(isPlasmid)
            Return $"[{type}] {genomeName}"
        End Function

    End Class

    Public Class gene

        <XmlAttribute> Public Property locus_tag As String
        <XmlAttribute> Public Property protein_id As String

        <XmlElement>
        Public Property product As String

        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 因为字符类型在进行XML序列化的时候会被转换为ASCII代码，影响阅读
        ''' 所以在这里使用字符串类型来解决这个问题
        ''' </remarks>
        <XmlAttribute> Public Property strand As String

        ''' <summary>
        ''' 对于rRNA和tRNA不存在
        ''' </summary>
        ''' <returns></returns>
        Public Property amino_acid As NumericVector
        ''' <summary>
        ''' mRNA, tRNA, rRNA, etc
        ''' </summary>
        ''' <returns></returns>
        Public Property nucleotide_base As NumericVector

    End Class

    ''' <summary>
    ''' 只记录tRNA，rRNA和其他RNA的数据，对于mRNA则不做记录
    ''' </summary>
    Public Class RNA

        ''' <summary>
        ''' <see cref="v2.gene.locus_tag"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property gene As String
        <XmlAttribute> Public Property type As RNATypes
        <XmlAttribute> Public Property val As String

    End Class

    ''' <summary>
    ''' 基因表达转录调控
    ''' </summary>
    Public Class transcription

        <XmlAttribute> Public Property regulator As String

        <XmlAttribute> Public Property mode As String

        ''' <summary>
        ''' 这个是效应物物质编号列表
        ''' </summary>
        ''' <returns></returns>
        Public Property effector As String()
        ''' <summary>
        ''' 这个调控关系所影响到的中心法则的事件名称
        ''' </summary>
        ''' <returns></returns>
        Public Property centralDogma As String
        Public Property biological_process As String
        Public Property motif As Motif

        Public ReadOnly Property target As String
            Get
                Return centralDogma.Split.First
            End Get
        End Property

    End Class

    ''' <summary>
    ''' 调控的motif位点
    ''' </summary>
    Public Class Motif

        <XmlAttribute> Public Property family As String
        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer
        <XmlAttribute> Public Property strand As Char

        ''' <summary>
        ''' 这个motif位点到被调控的基因的ATG位点的最短距离
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("atg-distance")>
        Public Property distance As Integer

        <XmlText> Public Property sequence As String
    End Class
End Namespace
