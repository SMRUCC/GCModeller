#Region "Microsoft.VisualBasic::c4828bc35e83b02396c02f1b6cf4ab2e, visualize\Circos\Circos\Karyotype\Chromosome.vb"

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

    '     Class KaryotypeChromosomes
    ' 
    '         Properties: size
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: GenerateDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Karyotype

    ''' <summary>
    ''' The very basically genome skeleton information description.(基因组的基本框架的描述信息)
    ''' </summary>
    Public Class KaryotypeChromosomes : Inherits SkeletonInfo

        Public Overrides ReadOnly Property size As Integer

        ''' <summary>
        ''' 这个构造函数是用于单个染色体的
        ''' </summary>
        ''' <param name="gSize">The genome size.</param>
        ''' <param name="color"></param>
        ''' <param name="bandData"><see cref="NamedTuple(Of String).Name"/>为颜色，其余的两个属性分别为左端起始和右端结束</param>
        Sub New(gSize As Integer, color As String, Optional bandData As IEnumerable(Of NamedTuple(Of String)) = Nothing)
            Me.size = gSize
            Me.bands = New List(Of Band)(GenerateDocument(bandData))

            Call singleKaryotypeChromosome(color)
        End Sub

        Sub New(Karyotypes As IEnumerable(Of Karyotype))
            karyos = Karyotypes.AsList
        End Sub

        Protected Sub New()
        End Sub

        Private Overloads Shared Iterator Function GenerateDocument(data As IEnumerable(Of NamedTuple(Of String))) As IEnumerable(Of Band)
            If Not data Is Nothing Then
                Dim i As Integer

                For Each x As NamedTuple(Of String) In data
                    Yield New Band With {
                        .chrName = "chr1",
                        .bandX = "band" & i,
                        .bandY = "band" & i,
                        .start = x.Item1.ParseInteger,
                        .end = x.Item2.ParseInteger,
                        .color = x.Name
                    }

                    i += 1
                Next
            End If
        End Function
    End Class
End Namespace
