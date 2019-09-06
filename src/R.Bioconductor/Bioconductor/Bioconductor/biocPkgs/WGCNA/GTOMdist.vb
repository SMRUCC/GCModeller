#Region "Microsoft.VisualBasic::be207b40b24081c9de6a12a1d7299543, Bioconductor\Bioconductor\biocPkgs\WGCNA\GTOMdist.vb"

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

    '     Class GTOMdist
    ' 
    '         Properties: adjMat, degree
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace WGCNA

    ''' <summary>
    ''' Generalized Topological Overlap Measure, taking into account interactions of higher degree.
    ''' </summary>
    <RFunc("GTOMdist")> Public Class GTOMdist : Inherits WGCNAFunction

        ''' <summary>
        ''' adjacency matrix. See details below.
        ''' </summary>
        ''' <returns></returns>
        Public Property adjMat As RExpression
        ''' <summary>
        ''' Integer specifying the maximum degree To be calculated.
        ''' </summary>
        ''' <returns></returns>
        Public Property degree As Integer = 1
    End Class
End Namespace
