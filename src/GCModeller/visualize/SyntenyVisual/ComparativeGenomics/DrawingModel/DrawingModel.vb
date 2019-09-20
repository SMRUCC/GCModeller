#Region "Microsoft.VisualBasic::c1e1869b3a2bb1a955ab039cc6f630b9, visualize\SyntenyVisual\ComparativeGenomics\DrawingModel\DrawingModel.vb"

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

    '     Class DrawingModel
    ' 
    '         Properties: Genome1, Genome2, Links, RibbonScoreColors
    ' 
    '         Function: AutoReverse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace ComparativeGenomics

    ''' <summary>
    ''' 比较两个基因组的差异的模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DrawingModel

        Public Property Genome1 As GenomeModel
        Public Property Genome2 As GenomeModel

        Public Property Links As GeneLink()
        Public Property RibbonScoreColors As (scoreRange As DoubleRange, profiles As Color())

        Public Overrides Function ToString() As String
            Return $"{Genome1.Title} vs. {Genome2.Title}"
        End Function

        ''' <summary>
        ''' 调用这个函数尝试对reference进行自动反转
        ''' </summary>
        ''' <param name="threshold">[0, 1]之间的百分比数</param>
        ''' <returns></returns>
        Public Function AutoReverse(threshold As Double) As DrawingModel
            Dim middle = Genome2.Length / 2
            Dim middleQuery = Genome1.Length / 2
            Dim cross As Integer
            Dim query = Genome1.genes.ToDictionary(Function(g) g.locus_tag)
            Dim ref = Genome2.genes.ToDictionary(Function(g) g.locus_tag)

            For Each link As GeneLink In Links
                If query(link.genome1).Left < middleQuery Then
                    '  在左边，则ref也应该在左边
                    If ref(link.genome2).Left > middle Then
                        cross += 1
                    End If
                Else
                    If ref(link.genome2).Left < middle Then
                        cross += 1
                    End If
                End If
            Next

            If cross / Links.Length >= threshold Then
                Return New DrawingModel With {
                    .Genome1 = Genome1,
                    .Genome2 = Genome2.Reverse,
                    .Links = Links
                }
            Else
                Return Me
            End If
        End Function
    End Class
End Namespace
