#Region "Microsoft.VisualBasic::78472298caebd25e0006cbb290458ff8, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\Services\RPackage\base\row+colnames.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.base

    Public Module DataFrameAPI

        Public Function rownames(x As String, Optional doNULL As Boolean = True, Optional prefix As String = "row") As String
            Return $"rownames({x}, do.NULL = {New RBoolean(doNULL)}, prefix= ""{prefix}"")"
        End Function

        Public Function colnames(x As String, Optional doNULL As Boolean = True, Optional prefix As String = "col") As String
            Return $"colnames({x}, do.NULL = {New RBoolean(doNULL)}, prefix= ""{prefix}"")"
        End Function
    End Module
End Namespace
