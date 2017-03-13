#Region "Microsoft.VisualBasic::696415243cc90c64c255d39bd0dc630a, ..\GCModeller\core\Bio.Assembly\ComponentModel\Equations\DefaultTypes.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.EquaionModel.DefaultTypes

    Public Class CompoundSpecieReference : Implements ICompoundSpecies

        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property StoiChiometry As Double Implements ICompoundSpecies.StoiChiometry
        <XmlAttribute> Public Property ID As String Implements ICompoundSpecies.Key

        Sub New()
        End Sub

        Sub New(x As ICompoundSpecies)
            StoiChiometry = x.StoiChiometry
            ID = x.Key
        End Sub

        Public Overloads Function Equals(b As ICompoundSpecies, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        Public Overrides Function ToString() As String
            If StoiChiometry > 1 Then
                Return String.Format("{0} {1}", StoiChiometry, ID)
            Else
                Return ID
            End If
        End Function
    End Class

    ''' <summary>
    ''' 默认类型的反应表达式的数据结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Equation : Inherits Equation(Of CompoundSpecieReference)
        Implements IEquation(Of CompoundSpecieReference)

        Sub New()
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies), canReverse As Boolean)
            Reactants = left.ToArray(Function(x) New CompoundSpecieReference(x))
            Products = right.ToArray(Function(x) New CompoundSpecieReference(x))
            Reversible = canReverse
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies),
                idMaps As Dictionary(Of String, String),
                canReverse As Boolean)

            Reactants = left.ToArray(
                Function(x) New CompoundSpecieReference With {
                    .ID = idMaps(x.Key),
                    .StoiChiometry = x.StoiChiometry
                })
            Products = right.ToArray(
                Function(x) New CompoundSpecieReference With {
                    .ID = idMaps(x.Key),
                    .StoiChiometry = x.StoiChiometry
                })
            Reversible = canReverse
        End Sub

        Protected Overrides Function __equals(a As CompoundSpecieReference, b As CompoundSpecieReference, strict As Boolean) As Object
            Return a.Equals(b, strict)
        End Function
    End Class
End Namespace
