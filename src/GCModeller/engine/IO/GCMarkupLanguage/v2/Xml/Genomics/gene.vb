﻿#Region "Microsoft.VisualBasic::af6deeeee1064b4b779b2a09eb31a434, engine\IO\GCMarkupLanguage\v2\Xml\Genome\gene.vb"

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

    '   Total Lines: 39
    '    Code Lines: 15 (38.46%)
    ' Comment Lines: 16 (41.03%)
    '    - Xml Docs: 93.75%
    ' 
    '   Blank Lines: 8 (20.51%)
    '     File Size: 1.18 KB


    '     Class gene
    ' 
    '         Properties: amino_acid, left, locus_tag, nucleotide_base, product
    '                     protein_id, right, strand
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace v2

    ''' <summary>
    ''' the gene model
    ''' </summary>
    Public Class gene

        ''' <summary>
        ''' the gene id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property locus_tag As String
        ''' <summary>
        ''' the corresponding ORF protein id, this will link to the metabolisom model
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property protein_id As String
        ''' <summary>
        ''' the protein function description
        ''' </summary>
        ''' <returns></returns>
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

        Sub New()
        End Sub

        Sub New(loci As NucleotideLocation)
            left = loci.left
            right = loci.right
            strand = loci.Strand.Description.ToLower
        End Sub

        Public Overrides Function ToString() As String
            Return $"({locus_tag}) " & product
        End Function

    End Class
End Namespace