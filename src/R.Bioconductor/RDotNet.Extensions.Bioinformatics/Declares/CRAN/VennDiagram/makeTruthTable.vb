#Region "Microsoft.VisualBasic::0238fe58f1810fdf750c84e0ad318a0f, RDotNet.Extensions.Bioinformatics\Declares\CRAN\VennDiagram\makeTruthTable.vb"

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

    '     Class makeTruthTable
    ' 
    '         Properties: x
    ' 
    ' 
    ' /********************************************************************************/

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
