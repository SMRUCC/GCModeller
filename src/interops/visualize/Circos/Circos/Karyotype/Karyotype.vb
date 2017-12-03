#Region "Microsoft.VisualBasic::085946951c52a692e6791a8074dd5d59, ..\interops\visualize\Circos\Circos\Karyotype\Karyotype.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Karyotype

    Public Module KaryotypeExtensions

        ''' <summary>
        ''' 缺口的大小，这个仅仅在单个染色体的基因组绘图模型之中有效
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LoopHole(x As SkeletonInfo) As PropertyValue(Of Integer)
            Return PropertyValue(Of Integer).Read(Of SkeletonInfo)(x, NameOf(LoopHole))
        End Function

        ''' <summary>
        ''' nt核苷酸基因组序列拓展属性
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function nt(x As Karyotype) As PropertyValue(Of FastaToken)
            Return PropertyValue(Of FastaToken).Read(Of Karyotype)(x, NameOf(nt))
        End Function

        <Extension>
        Public Function MapsRaw(x As Band) As PropertyValue(Of BlastnMapping)
            Return PropertyValue(Of BlastnMapping).Read(Of Band)(x, NameOf(MapsRaw))
        End Function
    End Module

    ''' <summary>
    ''' The ideogram using karyotype file to define the genome skeleton information, which defines the name, size and color of chromosomes. 
    ''' </summary>
    ''' <remarks>
    ''' A simple karyotype with 5 chromosomes:
    '''
    ''' ```
    ''' chr1 5Mb
    ''' chr2 10Mb
    ''' chr3 20Mb
    ''' chr4 50Mb
    ''' chr5 100Mb
    ''' ```
    ''' 
    ''' The format Of this file Is
    '''
    ''' ```
    ''' chr - CHRNAME CHRLABEL START End COLOR
    ''' ```
    ''' 
    ''' In data files, chromosomes are referred To by CHRNAME. 
    ''' On the image, they are labeled by CHRLABEL
    '''
    ''' Colors are taken from the spectral Brewer palette. 
    ''' To learn about Brewer palettes, see (www.colorbrewer.org)[http://www.colorbrewer.org]
    ''' </remarks>
    Public Class Karyotype : Inherits BaseClass
        Implements IKaryotype
        Implements INamedValue

        ''' <summary>
        ''' Id
        ''' </summary>
        ''' <returns></returns>
        Public Property chrName As String Implements IKaryotype.chrName, INamedValue.Key
        ''' <summary>
        ''' Display name title labels
        ''' </summary>
        ''' <returns></returns>
        Public Property chrLabel As String
        Public Property start As Integer Implements IKaryotype.start
        Public Property [end] As Integer Implements IKaryotype.end
        Public Property color As String Implements IKaryotype.color

        Public Overrides Function ToString() As String Implements IKaryotype.GetData
            Return $"chr - {chrName} {chrLabel} {start} {[end]} {color}"
        End Function
    End Class

    Public Interface IKaryotype
        Property start As Integer
        Property [end] As Integer
        Property color As String
        Property chrName As String

        Function GetData() As String
    End Interface

    ''' <summary>
    ''' Bands are defined using
    '''
    ''' ```
    ''' band CHRNAME BANDNAME BANDLABEL START End COLOR
    ''' ```
    ''' 
    ''' Currently ``BANDNAME`` And ``BANDLABEL`` are Not used
    '''
    ''' Colors correspond To levels Of grey To match
    ''' conventional shades Of banding found In genome
    ''' browsers. For example, ``gpos25`` Is a light grey.
    '''
    ''' For examples Of real karyotype files, see
    ''' ``data/karyotype`` In the Circos distribution directory.
    ''' Or data/karyotype In the course directory.
    ''' </summary>
    Public Class Band : Inherits BaseClass
        Implements IKaryotype

        Public Property chrName As String Implements IKaryotype.chrName
        Public Property color As String Implements IKaryotype.color
        Public Property [end] As Integer Implements IKaryotype.end
        Public Property start As Integer Implements IKaryotype.start
        Public Property bandX As String
        Public Property bandY As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function GetData() As String Implements IKaryotype.GetData
            Return $"band {chrName} {bandX} {bandY} {start} {[end]} {color}"
        End Function
    End Class
End Namespace
