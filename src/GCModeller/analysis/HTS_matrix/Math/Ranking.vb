#Region "Microsoft.VisualBasic::44473cfeb1ca5661ed53d25d72033a6c, analysis\HTS_matrix\Math\Ranking.vb"

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

    '   Total Lines: 12
    '    Code Lines: 9 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (25.00%)
    '     File Size: 356 B


    ' Class Ranking
    ' 
    '     Properties: expression, geneID, pvalue, ranking
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class Ranking

    Public Property geneID As String
    Public Property ranking As Dictionary(Of String, Double)
    Public Property pvalue As Dictionary(Of String, Double)
    Public Property expression As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return geneID
    End Function

End Class
