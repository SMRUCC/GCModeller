﻿#Region "Microsoft.VisualBasic::4d07ccc056b91ae4dbc47911b7af40b7, Bio.Assembly\ComponentModel\Equations\Reaction.vb"

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

    '     Class Reaction
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __equals
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ComponentModel.EquaionModel

    Public Class Reaction(Of T As ICompoundSpecies) : Inherits Equation(Of T)

        ReadOnly _equals As Func(Of T, T, Boolean, Boolean)

        Sub New(equals As Func(Of T, T, Boolean, Boolean))
            _equals = equals
        End Sub

        Protected Overrides Function __equals(a As T, b As T, strict As Boolean) As Object
            Return _equals(a, b, strict)
        End Function
    End Class
End Namespace
