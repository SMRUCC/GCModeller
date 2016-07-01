Imports System.Text
Imports LANS.SystemsBiology.Assembly.SBML.Level2.Elements
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver
Imports LANS.SystemsBiology.Assembly.SBML.Level2
Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels
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