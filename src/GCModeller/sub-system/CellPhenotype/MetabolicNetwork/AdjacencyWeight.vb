#Region "Microsoft.VisualBasic::9e0320ee030cd27f0c64ce8da9b81511, sub-system\CellPhenotype\MetabolicNetwork\AdjacencyWeight.vb"

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

    '   Total Lines: 19
    '    Code Lines: 13 (68.42%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (31.58%)
    '     File Size: 385 B


    ' Class AdjacencyWeight
    ' 
    '     Properties: Target, Weight
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region


Public Class AdjacencyWeight

    Public Property Target As String
    Public Property Weight As Double

    Sub New()
    End Sub

    Sub New(target As String, Optional w As Double = 1)
        Me.Target = target
        Me.Weight = w
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Target} = {Weight}"
    End Function

End Class
