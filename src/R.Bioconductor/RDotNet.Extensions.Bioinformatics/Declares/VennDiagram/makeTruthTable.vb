#Region "Microsoft.VisualBasic::0d2653a9c7d8f8284df88e23a021c118, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\VennDiagram\makeTruthTable.vb"

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

Imports RDotNet.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace VennDiagram

    ''' <summary>
    ''' Makes a truth table of the inputs.
    ''' 
    ''' A data frame with length(x) logical vector columns and 2 ^ length(x) rows.
    ''' </summary>
    <RFunc("make.truth.table")> Public Class makeTruthTable : Inherits vennBase

        ''' <summary>
        ''' A short vector.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
    End Class
End Namespace
