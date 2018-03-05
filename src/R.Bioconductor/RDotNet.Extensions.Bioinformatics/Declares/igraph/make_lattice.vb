#Region "Microsoft.VisualBasic::9c9fa1dd98cd30eeea9bfbee6311c066, RDotNet.Extensions.Bioinformatics\Declares\igraph\make_lattice.vb"

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

    '  
    ' 
    '     Properties: [dim], circular, dimvector, directed, length
    '                 mutual, nei
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes
Imports RDotNet.Extensions.VisualBasic

Namespace igraph

    ''' <summary>
    ''' make_lattice is a flexible function, it can create lattices of arbitrary dimensions, periodic or unperiodic ones. It has two forms. 
    ''' In the first form you only supply dimvector, but not length and dim. In the second form you omit dimvector and supply length and dim.
    ''' </summary>
    <RFunc("make_lattice")> Public Class make_lattice : Inherits igraph

        ''' <summary>
        ''' A vector giving the size of the lattice in each dimension.
        ''' </summary>
        ''' <returns></returns>
        Public Property dimvector As RExpression = NULL
        ''' <summary>
        ''' Integer constant, for regular lattices, the size of the lattice in each dimension.
        ''' </summary>
        ''' <returns></returns>
        Public Property length As RExpression = NULL
        ''' <summary>
        ''' Integer constant, the dimension of the lattice.
        ''' </summary>
        ''' <returns></returns>
        Public Property [dim] As RExpression = NULL
        ''' <summary>
        ''' The distance within which (inclusive) the neighbors on the lattice will be connected. This parameter is not used right now.
        ''' </summary>
        ''' <returns></returns>
        Public Property nei As Integer = 1
        ''' <summary>
        ''' Whether to create a directed lattice.
        ''' </summary>
        ''' <returns></returns>
        Public Property directed As Boolean = False
        ''' <summary>
        ''' Logical, if TRUE directed lattices will be mutually connected.
        ''' </summary>
        ''' <returns></returns>
        Public Property mutual As Boolean = False
        ''' <summary>
        ''' Logical, if TRUE the lattice or ring will be circular.
        ''' </summary>
        ''' <returns></returns>
        Public Property circular As Boolean = False
    End Class

    ''' <summary>
    ''' Create a lattice graph
    ''' </summary>
    <RFunc("lattice")> Public Class lattice : Inherits make_lattice

    End Class
End Namespace
