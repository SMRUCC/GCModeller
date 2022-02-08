#Region "Microsoft.VisualBasic::58ef026876bb06c0f479eb138deab390, core\Bio.Assembly\ComponentModel\Equations\DefaultTypes.vb"

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

    '     Class CompoundSpecieReference
    ' 
    '         Properties: ID, StoiChiometry
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: AsFactor, Equals, ToString
    ' 
    '     Class Equation
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: __equals, GetCoEfficient, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.EquaionModel.DefaultTypes

    Public Class CompoundSpecieReference : Implements ICompoundSpecies

        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property StoiChiometry As Double Implements ICompoundSpecies.StoiChiometry
        <XmlText> Public Property ID As String Implements ICompoundSpecies.Key
        <XmlAttribute> Public Property Compartment As String

        Sub New()
        End Sub

        Sub New(ref As ICompoundSpecies)
            StoiChiometry = ref.StoiChiometry
            ID = ref.Key
        End Sub

        Public Overloads Function Equals(b As ICompoundSpecies, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AsFactor() As FactorString(Of Double)
            Return New FactorString(Of Double) With {
                .factor = StoiChiometry,
                .text = ID
            }
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
    ''' 默认类型的反应表达式的数据结构，可以使用<see cref="EquationBuilder.CreateObject(String)"/>来进行构建
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Equation : Inherits Equation(Of CompoundSpecieReference)
        Implements IEquation(Of CompoundSpecieReference)

        Sub New()
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies), canReverse As Boolean)
            Reactants = left.Select(Function(x) New CompoundSpecieReference(x)).ToArray
            Products = right.Select(Function(x) New CompoundSpecieReference(x)).ToArray
            reversible = canReverse
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies),
                idMaps As Dictionary(Of String, String),
                canReverse As Boolean)

            Reactants = left _
                .Select(Function(x)
                            Return New CompoundSpecieReference With {
                                .ID = idMaps(x.Key),
                                .StoiChiometry = x.StoiChiometry
                            }
                        End Function) _
                .ToArray
            Products = right _
                .Select(Function(x)
                            Return New CompoundSpecieReference With {
                                .ID = idMaps(x.Key),
                                .StoiChiometry = x.StoiChiometry
                            }
                        End Function) _
                .ToArray
            reversible = canReverse
        End Sub

        Public Overloads Function GetCoEfficient(id As String, Optional directional As Boolean = False) As Double
            Dim factor As Double = MyBase.GetCoEfficient(id)

            If reversible AndAlso directional Then
                Return factor / 2
            Else
                Return factor
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function TryParse(expression As String) As Equation
            Return EquationBuilder.CreateObject(expression)
        End Function

        Protected Overrides Function __equals(a As CompoundSpecieReference, b As CompoundSpecieReference, strict As Boolean) As Object
            Return a.Equals(b, strict)
        End Function
    End Class
End Namespace
