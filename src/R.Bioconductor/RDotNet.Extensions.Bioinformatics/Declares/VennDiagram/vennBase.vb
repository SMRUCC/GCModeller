#Region "Microsoft.VisualBasic::4317aae58fc34a2143393cab4f92a091, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\VennDiagram\vennBase.vb"

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

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace VennDiagram

    Public MustInherit Class vennBase : Inherits IRToken

        Sub New()
            Requires = {"VennDiagram"}
        End Sub
    End Class
End Namespace
