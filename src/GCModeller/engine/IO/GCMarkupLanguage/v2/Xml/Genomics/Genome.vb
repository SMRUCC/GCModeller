#Region "Microsoft.VisualBasic::644d464eb06a32bff3e96d3ab7a969b0, engine\IO\GCMarkupLanguage\v2\Xml\Genomics\Genome.vb"

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

'   Total Lines: 70
'    Code Lines: 37 (52.86%)
' Comment Lines: 19 (27.14%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 14 (20.00%)
'     File Size: 2.28 KB


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
'         Constructor: (+2 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
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

        Public Property proteins As protein()

        Sub New()
        End Sub

        Sub New(copy As Genome)
            replicons = copy.replicons.SafeQuery.ToArray
            regulations = copy.regulations.SafeQuery.ToArray
            proteins = copy.proteins.SafeQuery.ToArray
        End Sub

        Public Iterator Function GetAllGenes(Optional skipPlasmids As Boolean = False) As IEnumerable(Of gene)
            Dim source As IEnumerable(Of replicon)

            If skipPlasmids Then
                source = replicons.Where(Function(r) Not r.isPlasmid)
            Else
                source = replicons
            End If

            For Each replicon As replicon In source
                For Each gene As gene In replicon.GetGeneList
                    Yield gene
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllGeneLocusTags(Optional skipPlasmids As Boolean = False) As IEnumerable(Of String)
            Return From gene As gene In GetAllGenes(skipPlasmids) Select gene.locus_tag
        End Function
    End Class

    ''' <summary>
    ''' 只记录tRNA，rRNA和其他RNA的数据，对于mRNA则不做记录
    ''' </summary>
    Public Class RNA

        <XmlAttribute> Public Property id As String

        ''' <summary>
        ''' the trranscription source template gene <see cref="v2.gene.locus_tag"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property gene As String
        ''' <summary>
        ''' the rna type
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As RNATypes
        ''' <summary>
        ''' usually be the:
        ''' 
        ''' 1. amino acid code for tRNA
        ''' 2. 16s,5s,23s for rRNA
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property val As String

        Public Property note As String

        Sub New()
        End Sub

        Sub New(gene_id As String, type As RNATypes, val As String)
            Me.gene = gene_id
            Me.type = type
            Me.val = val
        End Sub

        Public Overrides Function ToString() As String
            Return $"{gene} ({type}); ""{val}"""
        End Function

    End Class

End Namespace
