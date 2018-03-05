#Region "Microsoft.VisualBasic::a158c59fa1a7b15c82d3f49f1417dbb4, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\base\row+colnames.vb"

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

    '     Module DataFrameAPI
    ' 
    '         Function: colnames, rownames
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.base

    Public Module DataFrameAPI

        Public Function rownames(x As String, Optional doNULL As Boolean = True, Optional prefix As String = "row") As String
            Return $"rownames({x}, do.NULL = {New RBoolean(doNULL)}, prefix= ""{prefix}"")"
        End Function

        Public Function colnames(x As String, Optional doNULL As Boolean = True, Optional prefix As String = "col") As String
            Return $"colnames({x}, do.NULL = {New RBoolean(doNULL)}, prefix= ""{prefix}"")"
        End Function
    End Module
End Namespace
