#Region "Microsoft.VisualBasic::0b04b22dadeba5519adba6f989b707d4, Bioconductor\Bioconductor\biocPkgs\WGCNA\WGCNA.vb"

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

    '     Class WGCNAFunction
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace WGCNA

    ''' <summary>
    ''' Functions necessary to perform Weighted Correlation Network Analysis. 
    ''' WGCNA is also known as weighted gene co-expression network analysis when dealing with gene expression data. 
    ''' Many functions of WGCNA can also be used for general association networks specified by a symmetric adjacency matrix.
    ''' (这只是一个WGCNA函数对象的基本类型)
    ''' </summary>
    Public Class WGCNAFunction : Inherits IRToken

        Sub New()
            Requires = {"WGCNA"}
        End Sub
    End Class
End Namespace
