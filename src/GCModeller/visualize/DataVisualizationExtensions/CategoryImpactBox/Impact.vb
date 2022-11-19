#Region "Microsoft.VisualBasic::e1de302ae9bf195bcb1f3ffbe88b81bf, GCModeller\visualize\DataVisualizationExtensions\CategoryImpactBox\Impact.vb"

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

    '   Total Lines: 33
    '    Code Lines: 27
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.13 KB


    ' Class Impact
    ' 
    '     Properties: classLabel, impactFactors
    ' 
    '     Function: FromDEGList, GetSampleModel, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Distributions
Imports stdnum = System.Math

Public Class Impact

    Public Property classLabel As String
    Public Property impactFactors As Double()

    Public Function GetSampleModel() As SampleDistribution
        Return New SampleDistribution(impactFactors)
    End Function

    Public Overrides Function ToString() As String
        Return classLabel
    End Function

    Public Shared Function FromDEGList(genes As IEnumerable(Of DEGModel)) As Impact()
        Return genes _
            .GroupBy(Function(g) g.class) _
            .Select(Function(group)
                        Return New Impact With {
                            .classLabel = group.Key,
                            .impactFactors = group _
                                .Select(Function(deg)
                                            Return deg.VIP * (-stdnum.Log10(deg.pvalue)) * (stdnum.Abs(deg.logFC))
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
    End Function

End Class
