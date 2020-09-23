#Region "Microsoft.VisualBasic::f16775f7c4e457aa48df89b7daf384d1, sub-system\PLAS.NET\SSystem\Script\Models\SBML.vb"

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

    '     Class SBML
    ' 
    '         Properties: AutoFixError
    ' 
    '         Function: __contact, __generateSystem, __where, Compile, CompileImpl
    '                   GenerateFunction, PreCompile
    ' 
    '         Sub: __strip, __stripNumber
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.Model.SBML.Level2
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace Script

    ''' <summary>
    ''' SBML模型编译器
    ''' </summary>
    Public Class SBML : Inherits Compiler(Of Model)

        Dim SBMLFile As XmlFile

        Public Property AutoFixError As Boolean = False

        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            m_compiledModel = New Model
            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Call __strip()
            Call __generateSystem(m_compiledModel)

            m_compiledModel.Title = SBMLFile.Model.name
            m_compiledModel.FinalTime = 100

            Return 0
        End Function

        ''' <summary>
        ''' 需要在这里将``-``连接符替换为下划线``_``不然在解析数学表达式的时候会被当作为减号
        ''' </summary>
        Private Sub __strip()
            For Each sp In SBMLFile.Model.listOfSpecies
                Call __stripNumber(sp.ID)
            Next
            For Each rxn In SBMLFile.Model.listOfReactions
                For Each m As speciesReference In rxn.Reactants
                    Call __stripNumber(m.species)
                Next
                For Each m As speciesReference In rxn.Products
                    Call __stripNumber(m.species)
                Next
            Next
        End Sub

        Private Shared Sub __stripNumber(ByRef s As String)
            Dim n As String = Regex.Match(s, "\d").Value
            If Not String.IsNullOrEmpty(n) AndAlso InStr(s, n) = 1 Then
                s = "N_" & s
            End If
            s = s.Replace("-", "_").Replace("+", "_")
        End Sub

        ''' <summary>
        ''' Generate the equation group of the target modelling system.(生成目标模型系统的方程组)
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __generateSystem(Model As Script.Model) As Boolean
            Dim reactions As New List(Of SEquation)
            Dim metabolites As New Dictionary(Of var)

            For Each m As Specie In SBMLFile.Model.listOfSpecies
                ' If Not IsEntry(SBMLFile, m.ID) Then
                reactions += New SEquation(m.ID, GenerateFunction(m.ID))
                ' End If

                If metabolites & m.ID Then
                    Continue For
                End If

                metabolites += New var With {
                    .Id = m.ID,
                    .title = m.name,
                    .Value = m.InitialAmount
                }
            Next

            Model.sEquations = reactions _
                .Where(Function(x) Not String.IsNullOrEmpty(x.Expression)).ToArray
            Model.Vars = metabolites.Values _
                .Where(Function(x) __where(x, Model)).ToArray

            Return True
        End Function

        ''' <summary>
        ''' 检查目标反应物对象是否存在于模型的表达式之中
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Private Function __where(x As var, model As Script.Model) As Boolean
            Dim name As String = x.Id

            For Each eq As SEquation In model.sEquations
                If InStr(eq.Expression, name) > 0 Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Metabolite"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' + 在计算消耗的部分的时候，S系统方程中当前的底物是作为反应物而被消耗掉的
        ''' + 在计算生成的部分的时候，S系统方程之中是消耗来源的反应物而生成的当前底物
        ''' 
        ''' 故而两个方向都是取值来源为反应物
        ''' </remarks>
        Private Function GenerateFunction(Metabolite As String) As String
            Dim produce As String = GetAllProduce(SBMLFile, Metabolite).Select(Function(rxn) __contact(rxn.Reactants)).JoinBy("*")
            Dim consume As String = GetAllConsume(SBMLFile, Metabolite).Select(Function(rxn) __contact(rxn.Reactants)).JoinBy("*") ' 在计算消耗的部分的时候，S系统方程中目标是作为反应物而被消耗掉的
            Dim eq As String = ""

            If Not String.IsNullOrEmpty(produce) Then
                eq = "0.5*" & produce
            End If
            If Not String.IsNullOrEmpty(consume) Then
                eq = eq & "-0.5*" & consume
            End If

            Return eq
        End Function

        Private Function __contact(mlst As IEnumerable(Of speciesReference)) As String
            If mlst.Count = 1 Then
                Return mlst.First.species & "^0.5"
            Else
                Return mlst.Select(Function(x) x.species & "^0.5").JoinBy("*")
            End If
        End Function

        Public Overloads Shared Function Compile(path As String, Optional AutoFix As Boolean = False) As Script.Model
            Using Compiler As New SBML With {
                .SBMLFile = path,
                .AutoFixError = AutoFix
            }
                Call Compiler.Compile()
                Call Compiler.m_compiledModel.WriteScript(path & ".plas")

                Return Compiler.Return
            End Using
        End Function

        Public Shared Widening Operator CType(path As String) As SBML
            Return New SBML With {
                .SBMLFile = path
            }
        End Operator
    End Class
End Namespace
