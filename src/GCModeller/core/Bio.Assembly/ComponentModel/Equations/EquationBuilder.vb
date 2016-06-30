#Region "Microsoft.VisualBasic::63eb65d809dd78152d799d249b407bf4, ..\Bio.Assembly\ComponentModel\Equations\EquationBuilder.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.EquaionModel

    Public Module EquationBuilder

        Public Const EQUATION_DIRECTIONS_REVERSIBLE As String = " <=> "
        Public Const EQUATION_DIRECTIONS_IRREVERSIBLE As String = " --> "
        Public Const EQUATION_SPECIES_CONNECTOR As String = " + "

        Public Function CreateObject(Of TCompound As ICompoundSpecies, TEquation As IEquation(Of TCompound))(Equation As String) As TEquation
            Dim EquationObject As IEquation(Of TCompound) = Activator.CreateInstance(Of TEquation)()
            EquationObject.Reversible = CType(InStr(Equation, EQUATION_DIRECTIONS_REVERSIBLE), Boolean)

            Dim deli As String = If(EquationObject.Reversible,
                EQUATION_DIRECTIONS_REVERSIBLE,
                EQUATION_DIRECTIONS_IRREVERSIBLE)
            Dim Tokens As String() = Strings.Split(Equation, deli)

            Try
                EquationObject.Reactants = GetSides(Of TCompound)(Tokens(Scan0))
                EquationObject.Products = GetSides(Of TCompound)(Tokens(1))
            Catch ex As Exception   ' 生成字典的时候可能会因为重复的代谢物而出错
                Dim msg As String = String.Format(Duplicated, Equation)
                Throw New Exception(msg, ex)
            End Try

            Return EquationObject
        End Function

        Const Duplicated As String = "Could not process ""{0}"", duplicated found!"

        Public Function CreateObject(Equation As String) As DefaultTypes.Equation
            Return CreateObject(Of DefaultTypes.CompoundSpecieReference, DefaultTypes.Equation)(Equation)
        End Function

        Private Function GetSides(Of T As ICompoundSpecies)(Expr As String) As T()
            If String.IsNullOrEmpty(Expr) Then
                Return New T() {}
            End If

            Dim tokens As String() = Strings.Split(Expr, EQUATION_SPECIES_CONNECTOR)
            Dim LQuery As T() = tokens.ToArray(AddressOf __tryParse(Of T))
            Return LQuery
        End Function

        Private Function __tryParse(Of T As ICompoundSpecies)(token As String) As T
            Dim CompoundSpecie As T = Activator.CreateInstance(Of T)()
            Dim SC As String = Regex.Match(token, "(^| )\d+ ", RegexOptions.Singleline).Value

            If String.IsNullOrEmpty(SC) Then
                Dim tokens As String() = token.Trim.Split
                If tokens.Length > 1 Then
                    CompoundSpecie.StoiChiometry = Scripting.CTypeDynamic(Of Double)(tokens(Scan0))
                    CompoundSpecie.Identifier = token
                Else
                    CompoundSpecie.StoiChiometry = 1
                    CompoundSpecie.Identifier = token
                End If
            Else
                CompoundSpecie.StoiChiometry = Val(SC)
                CompoundSpecie.Identifier = Trim(token.Replace(SC, ""))
            End If

            Return CompoundSpecie
        End Function

        Public Function ToString(GetLeftSide As Func(Of KeyValuePair(Of Double, String)()),
                                 GetRightSide As Func(Of KeyValuePair(Of Double, String)()),
                                 Reversible As Boolean) As String
            Return ToString(GetLeftSide(), GetRightSide(), Reversible)
        End Function

        Public Function ToString(LeftSide As KeyValuePair(Of Double, String)(), RightSide As KeyValuePair(Of Double, String)(), Reversible As Boolean) As String
            Dim sBuilder As New StringBuilder(1024)
            Dim DirectionFlag As String =
                If(Reversible,
                EQUATION_DIRECTIONS_REVERSIBLE,
                EQUATION_DIRECTIONS_IRREVERSIBLE)

            Call EquationBuilder.AppendSides(sBuilder, Compounds:=LeftSide)
            Call sBuilder.Append(DirectionFlag)
            Call EquationBuilder.AppendSides(sBuilder, Compounds:=RightSide)

            Return sBuilder.ToString
        End Function

        Private Sub AppendSides(sb As StringBuilder, Compounds As KeyValuePair(Of Double, String)())
            Call Compounds.__appendSide(sb, Function(x) x.Key, Function(x) x.Value)
        End Sub

        Public Function ToString(Of TCompound As ICompoundSpecies)(Equation As IEquation(Of TCompound)) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim DirectionFlag As String =
                If(Equation.Reversible,
                EQUATION_DIRECTIONS_REVERSIBLE,
                EQUATION_DIRECTIONS_IRREVERSIBLE)

            Call EquationBuilder.AppendSides(sBuilder, Compounds:=Equation.Reactants)
            Call sBuilder.Append(DirectionFlag)
            Call EquationBuilder.AppendSides(sBuilder, Compounds:=Equation.Products)

            Return sBuilder.ToString
        End Function

        Public Function ToString(Equation As DefaultTypes.Equation) As String
            Return ToString(Of DefaultTypes.CompoundSpecieReference)(Equation)
        End Function

        <Extension>
        Private Sub __appendSide(Of T)(compounds As IEnumerable(Of T), sb As StringBuilder, getSto As Func(Of T, Double), getId As Func(Of T, String))
            If Not compounds.IsNullOrEmpty Then
                Dim array As String() =
                    LinqAPI.Exec(Of String) <= From cp As T
                                               In compounds
                                               Let sto As Double = getSto(cp)
                                               Let id As String = getId(cp)
                                               Select If(sto > 1, $"{sto} {id}", id)
                Dim side As String = String.Join(EQUATION_SPECIES_CONNECTOR, array)
                Call sb.Append(side)
            End If
        End Sub

        Private Sub AppendSides(sBuilder As StringBuilder, Compounds As ICompoundSpecies())
            Call Compounds.__appendSide(sBuilder, Function(x) x.StoiChiometry, Function(x) x.Identifier)
        End Sub
    End Module
End Namespace
