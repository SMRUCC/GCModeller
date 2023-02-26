#Region "Microsoft.VisualBasic::c86829c44a9122a3d9ef34b4762ef915, GCModeller\core\Bio.Assembly\ComponentModel\Equations\Equation.vb"

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


    ' Code Statistics:

    '   Total Lines: 168
    '    Code Lines: 95
    ' Comment Lines: 50
    '   Blank Lines: 23
    '     File Size: 6.14 KB


    '     Class Equation
    ' 
    '         Properties: Id, Products, Reactants, reversible
    ' 
    '         Function: (+2 Overloads) Consume, Equals, GetCoEfficient, getDictionary, GetMetabolites
    '                   (+2 Overloads) Produce, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.EquaionModel

    Public MustInherit Class Equation(Of T As ICompoundSpecies) : Implements IEquation(Of T), INamedValue

#Region "SBML接口"

        ''' <summary>
        ''' list of metabolism reaction substrates
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("listOfReactants")> Public Overridable Property Reactants As T() Implements IEquation(Of T).Reactants
            Get
                Return left
            End Get
            Set(value As T())
                If value.IsNullOrEmpty Then
                    leftTable = New Dictionary(Of String, T())
                Else
                    leftTable = getDictionary(value)
                End If

                left = value
            End Set
        End Property

        <XmlAttribute> Public Overridable Property reversible As Boolean Implements IEquation(Of T).Reversible

        ''' <summary>
        ''' list of metabolism reaction products
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("listOfProducts")> Public Overridable Property Products As T() Implements IEquation(Of T).Products
            Get
                Return right
            End Get
            Set(value As T())
                If value.IsNullOrEmpty Then
                    rightTable = New Dictionary(Of String, T())
                Else
                    rightTable = getDictionary(value)
                End If

                right = value
            End Set
        End Property
#End Region

        Protected left As T()
        Protected right As T()

        ' 为了兼容KEGG里面的方程式，因为有些方程式可能会在一边出现相同的化合物

        Protected leftTable As Dictionary(Of String, T())
        Protected rightTable As Dictionary(Of String, T())

        Public Overridable Property Id As String Implements INamedValue.Key

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function getDictionary(value As T()) As Dictionary(Of String, T())
            Return (From x As T
                    In value
                    Select x
                    Group x By x.Key.ToLower Into Group) _
 _
                .ToDictionary(Function(x)
                                  Return x.Group.First.Key
                              End Function,
                              Function(x)
                                  Return x.Group.ToArray
                              End Function)
        End Function

        ''' <summary>
        ''' 得到这个代谢反应过程之中的所有的代谢物，即左边加右边
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetMetabolites() As IEnumerable(Of T)
            For Each compound In Reactants
                Yield compound
            Next
            For Each compound In Products
                Yield compound
            Next
        End Function

        ''' <summary>
        ''' 获取得到某个代谢物在本反应过程之中的化学计量数；
        ''' [
        ''' 左边，返回负数；
        ''' 右边，返回正数；
        ''' 不存在则返回0。
        ''' ]
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        Public Overridable Function GetCoEfficient(ID As String) As Double
            If Not (leftTable.ContainsKey(ID) OrElse rightTable.ContainsKey(ID)) Then
                ID = ID.ToLower
            End If

            If leftTable.ContainsKey(ID) Then
                Return -1 * leftTable(ID).Select(Function(x) x.Stoichiometry).Sum
            ElseIf rightTable.ContainsKey(ID) Then
                Return rightTable(ID).Select(Function(x) x.Stoichiometry).Sum
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 判断当前的代谢反应过程是否产生所给定的目标代谢物作为反应产物？
        ''' </summary>
        ''' <param name="Metabolite">Id</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Produce(metabolite As T) As Boolean
            Return rightTable.ContainsKey(metabolite.Key)
        End Function

        ''' <summary>
        ''' 判断当前的代谢过程是否消耗所给定的目标代谢物作为反应底物？
        ''' </summary>
        ''' <param name="metabolite"></param>
        ''' <returns></returns>
        Public Function Consume(metabolite As T) As Boolean
            Return leftTable.ContainsKey(metabolite.Key)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Metabolite">Metabolite.Species</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Produce(metabolite As String) As Boolean
            Return rightTable.ContainsKey(metabolite)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Metabolite">Metabolite.Species</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Consume(metabolite As String) As Boolean
            Return leftTable.ContainsKey(metabolite)
        End Function

        Protected MustOverride Function __equals(a As T, b As T, strict As Boolean)

        Public Overridable Overloads Function Equals(b As Equation(Of T), strict As Boolean) As Boolean
            Return Me.Equals(b, AddressOf __equals, strict)
        End Function

        ''' <summary>
        ''' Equation expression
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return EquationBuilder.ToString(Of T)(Equation:=Me)
        End Function
    End Class
End Namespace
