#Region "Microsoft.VisualBasic::6d18beb6ab4989b334bcd7d18cda04ac, ..\GCModeller\visualize\visualizeTools\ComparativeGenomics\MultipleAlignment\DrawingModel.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComparativeAlignment

    Public Structure ColorLegend : Implements sIdEnumerable

        Public Property color As Color

        Sub New(s As String, color As Color)
            Me.type = s
            Me.color = color
        End Sub

        Public Property type As String Implements sIdEnumerable.Identifier

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
        Public Property nt As FASTA.FastaToken
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
