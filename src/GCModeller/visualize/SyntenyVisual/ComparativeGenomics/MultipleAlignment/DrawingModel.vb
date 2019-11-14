#Region "Microsoft.VisualBasic::ae21776d47176226d37e64d7c6631723, visualize\SyntenyVisual\ComparativeGenomics\MultipleAlignment\DrawingModel.vb"

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

    '     Structure ColorLegend
    ' 
    '         Properties: color, type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: TCSColors, ToString
    ' 
    '     Class DrawingModel
    ' 
    '         Properties: aligns, COGColors, Legends, links, nt
    '                     Query
    ' 
    '         Function: EnumerateTitles
    ' 
    '     Class Orthology
    ' 
    '         Properties: spPairs
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel

Namespace ComparativeAlignment

    Public Structure ColorLegend : Implements INamedValue

        Public Property color As Color

        Sub New(s As String, color As Color)
            Me.type = s
            Me.color = color
        End Sub

        Public Property type As String Implements INamedValue.Key

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function TCSColors() As Dictionary(Of ColorLegend)
            Dim colours As New Dictionary(Of ColorLegend)
            colours += New ColorLegend("Gene encoding HyHK", Color.Red)
            colours += New ColorLegend("Gene encoding orthordox HK", Color.Cyan)
            colours += New ColorLegend("Gene encoding RR", Color.Purple)
            Return colours
        End Function
    End Structure

    ''' <summary>
    ''' 一对多的共线性比对作图
    ''' </summary>
    Public Class DrawingModel

#Region "PTT regions"
        Public Property Query As ComparativeGenomics.GenomeModel
        Public Property aligns As ComparativeGenomics.GenomeModel()
#End Region

        ''' <summary>
        ''' BBH result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property links As Orthology()

        ''' <summary>
        ''' <see cref="Query"></see>的基因组序列，这个适用于计算GCSkew的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property nt As FASTA.FastaSeq
        Public Property COGColors As Dictionary(Of String, Brush)
        Public Property Legends As Dictionary(Of ColorLegend)

        Public Iterator Function EnumerateTitles() As IEnumerable(Of String)
            Yield Query.Title

            For Each align As ComparativeGenomics.GenomeModel In aligns
                Yield Query.Title
            Next
        End Function
    End Class

    Public Class Orthology : Inherits ComparativeGenomics.GeneLink

        ''' <summary>
        ''' query与subject的基因组的组合字符串
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks> 
        Public Property spPairs As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}  <-->  {1}", Me.genome1, Me.genome2)
        End Function
    End Class
End Namespace
