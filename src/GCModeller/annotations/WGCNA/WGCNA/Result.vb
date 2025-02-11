#Region "Microsoft.VisualBasic::c8242282d49ea0e8ffb49ea28b71bfd7, annotations\WGCNA\WGCNA\Result.vb"

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

    '   Total Lines: 20
    '    Code Lines: 13 (65.00%)
    ' Comment Lines: 4 (20.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (15.00%)
    '     File Size: 709 B


    ' Class Result
    ' 
    '     Properties: beta, hclust, K, modules, network
    '                 softBeta, TOM
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

Public Class Result

    Public Property beta As BetaTest
    Public Property network As NetworkGraph
    Public Property K As Vector
    Public Property TOM As GeneralMatrix
    Public Property hclust As Cluster
    ''' <summary>
    ''' modules is clustered based on the <see cref="TOM"/> matrix
    ''' </summary>
    ''' <returns></returns>
    Public Property modules As Dictionary(Of String, String())
    Public Property softBeta As BetaTest()

End Class
