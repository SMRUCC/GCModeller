﻿#Region "Microsoft.VisualBasic::f29e28b0827e8ee720da9d0fcd6aaba0, engine\IO\GCMarkupLanguage\v2\Xml\Genomics\Genome.vb"

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

    '   Total Lines: 61
    '    Code Lines: 30 (49.18%)
    ' Comment Lines: 19 (31.15%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (19.67%)
    '     File Size: 2.07 KB


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
    '         Function: ToString
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

        Public Overrides Function ToString() As String
            Return $"{gene} ({type}); ""{val}"""
        End Function

    End Class

End Namespace
