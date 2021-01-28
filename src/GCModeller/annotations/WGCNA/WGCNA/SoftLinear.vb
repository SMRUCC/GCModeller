#Region "Microsoft.VisualBasic::b321fc08c54882a199fbfd281c4b5be3, WGCNA\WGCNA\SoftLinear.vb"

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

    ' Module SoftLinear
    ' 
    '     Function: CreateLinear
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' 把连通性分隔，分隔内连通性的平均值取log10，跟频率的概率取log10，两者之间有线性关系。
''' </summary>
Module SoftLinear

    Public Function CreateLinear(k As Vector) As FitResult
        Dim cut1 As SampleDistribution() = CutBins _
            .FixedWidthBins(k, 10) _
            .Where(Function(b) b.size > 0) _
            .ToArray
        Dim bin As Vector = cut1.Select(Function(b) b.average).AsVector
        Dim freq1 As Vector = 0.00000001 + New Vector(cut1.Select(Function(b) b.size)) / k.Dim
        Dim X As Vector = bin.Log(10)
        Dim Y As Vector = freq1.Log(10)

        Return LeastSquares.LinearFit(X, Y)
    End Function
End Module

