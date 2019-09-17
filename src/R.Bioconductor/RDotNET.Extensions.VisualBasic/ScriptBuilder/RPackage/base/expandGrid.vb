#Region "Microsoft.VisualBasic::e848c0d0233a9d25ceda2a31e44a8e0b, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\base\expandGrid.vb"

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

    '     Class expandGrid
    ' 
    '         Properties: KEEP_OUT_ATTRS, stringsAsFactors, x
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.base

    ''' <summary>
    ''' Create a data frame from all combinations of the supplied vectors or factors. See the description of the return value for precise details of the way this is done.
    ''' 
    ''' A data frame containing one row for each combination of the supplied factors. The first factors vary fastest. The columns are labelled by the factors if these are supplied as named arguments or named components of a list. The row names are ‘automatic’.
    ''' Attribute "out.attrs" Is a list which gives the dimension And dimnames for use by predict methods.
    ''' </summary>
    <RFunc("expand.grid")> Public Class expandGrid : Inherits IRToken

        ''' <summary>
        ''' vectors, factors or a list containing these.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("...", ValueTypes.List, False, True)>
        Public Property x As RExpression()
        ''' <summary>
        ''' a logical indicating the "out.attrs" attribute (see below) should be computed and returned.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("KEEP.OUT.ATTRS")> Public Property KEEP_OUT_ATTRS As Boolean = True
        ''' <summary>
        ''' logical specifying if character vectors are converted to factors.
        ''' </summary>
        ''' <returns></returns>
        Public Property stringsAsFactors As Boolean = True
    End Class
End Namespace
