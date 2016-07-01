#Region "Microsoft.VisualBasic::08bbc12719c05031d398490ca684f0c7, ..\GCModeller\sub-system\PLAS.NET\SSystem\SBML\SBML.vb"

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

Imports System.Text
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.Assembly.SBML.Level2
Imports SMRUCC.genomics.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq

Public Class SBML : Inherits Compiler(Of Script.Model)

    Dim SBMLFile As XmlFile

    Public Property AutoFixError As Boolean = False

    Public Overrides Function Compile(Optional args As CommandLine = Nothing) As Script.Model
        Dim Model As Script.Model = New Script.Model

        Call GenerateSystem(Model)

        Model.Title = SBMLFile.Model.name
        Model.FinalTime = 100

        Return Model
    End Function

    ''' <summary>
    ''' Generate the equation group of the target modelling system.(生成目标模型系统的方程组)
    ''' </summary>
    ''' <param name="Model"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerateSystem(Model As Script.Model) As Boolean
        Dim ReactionList As List(Of Equation) = New List(Of Equation)
        Dim Metabolites As List(Of Var) = New List(Of Var)

        For Each Metabolite In SBMLFile.Model.listOfSpecies
            If Not IsEntry(SBMLFile, Metabolite.ID) Then
                Dim Reaction As Equation = New Equation(Metabolite.ID, GenerateFunction(Metabolite.ID))
                ReactionList.Add(Reaction)
            End If
            Dim Init As Var =
                New Kernel.ObjectModels.Var With {
                    .UniqueId = Metabolite.ID,
                    .Title = Metabolite.name,
                    .Value = Metabolite.InitialAmount
            }
            Metabolites.Add(Init)
        Next

        Model.sEquations = ReactionList.ToArray(Function(x) x.Model)
        Model.Vars = Metabolites.ToArray

        Return True
    End Function

    Private Function GenerateFunction(Metabolite As String) As String
        Dim Expression As StringBuilder = New StringBuilder(1024)
        For Each Reaction In GetAllProduce(SBMLFile, Metabolite)
            Expression.AppendFormat("{0}+", __contact(Reaction.Reactants))
        Next
        For Each Reaction In GetAllConsume(SBMLFile, Metabolite)
            Expression.AppendFormat("{0}-", __contact(Reaction.Products))
        Next

        Return Expression.ToString
    End Function

    Private Function __contact(e As IEnumerable(Of speciesReference)) As String
        Dim sBuilder As StringBuilder = New StringBuilder(128)
        For Each Metabolite In e
            sBuilder.AppendFormat("{0}*", Metabolite.species)
        Next
        Call sBuilder.Remove(sBuilder.Length - 1, 1)
        Return sBuilder.ToString
    End Function

    Public Overloads Shared Function Compile(Path As String, Optional AutoFix As Boolean = False) As Script.Model
        Using Compiler As SBML = New SBML With {.SBMLFile = Path, .AutoFixError = AutoFix}
            Return Compiler.Compile
        End Using
    End Function

    Public Shared Widening Operator CType(Path As String) As SBML
        Return New SBML With {.SBMLFile = Path}
    End Operator

    Protected Overrides Function Link() As Integer
        Return -1
    End Function

    Public Overrides Function PreCompile(ARGV As CommandLine) As Integer
        Return -1
    End Function
End Class
