#Region "Microsoft.VisualBasic::f162ce5dfc68ec716855946bf32b0f16, ..\core\Bio.Assembly\ComponentModel\Equations\Reaction(Of T).vb"

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

Imports Microsoft.VisualBasic.Linq
Imports System.Xml.Serialization

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
