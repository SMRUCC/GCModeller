#Region "Microsoft.VisualBasic::774d40e0820fbe112a5a84fcaf12bfdc, engine\IO\GCMarkupLanguage\v2\Xml\Genome\Genome.vb"

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
                For Each gene As gene In replicon.GetGeneList
                    Yield gene.locus_tag
                Next
            Next
        End Function
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
