#Region "Microsoft.VisualBasic::2ded2dd0662a2213de1b3c41b7264db4, sub-system\simulators\FBA.vb"

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

'   Total Lines: 182
'    Code Lines: 130 (71.43%)
' Comment Lines: 26 (14.29%)
'    - Xml Docs: 96.15%
' 
'   Blank Lines: 26 (14.29%)
'     File Size: 6.73 KB


' Module FBA
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetLppModel, lpsolve, Matrix, MatrixTable, SetObjective
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.FBA
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports KEGGReaction = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction
Imports Matrix = SMRUCC.genomics.Analysis.FBA.Matrix

''' <summary>
''' Flux Balance Analysis
''' </summary>
<Package("FBA")>
Module FBA

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(Matrix), AddressOf MatrixTable)
    End Sub

    Private Function MatrixTable(mat As Matrix, args As list, env As Environment) As dataframe
        Dim matrix As New Dictionary(Of String, Array)
        Dim rId = mat.Flux.Keys.ToArray
        Dim tMat As Double()() = mat.Matrix.MatrixTranspose.ToArray
        Dim rowNames As String() = New String() {
            "isGap", "isTarget", "max", "min"
        } _
        .JoinIterates(mat.Compounds) _
        .ToArray

        Dim isGap As Integer
        Dim isTarget As Integer
        Dim max As Double
        Dim min As Double

        Dim gapsId As Index(Of String) = mat.Gaps.Indexing
        Dim objsId As Index(Of String) = mat.Targets.Indexing

        For i As Integer = 0 To rId.Length - 1
            isGap = If(rId(i) Like gapsId, 1, 0)
            isTarget = If(rId(i) Like objsId, 1, 0)
            max = mat.Flux(rId(i)).Max
            min = mat.Flux(rId(i)).Min
            matrix(rId(i)) = New Double() {
                isGap, isTarget, max, min
            } _
            .JoinIterates(tMat(i)) _
            .ToArray
        Next

        Dim data As New dataframe With {
            .columns = matrix,
            .rownames = rowNames
        }

        Return data
    End Function

    ''' <summary>
    ''' create FBA model matrix
    ''' </summary>
    ''' <param name="model">should be a GCModeller <see cref="VirtualCell"/> or <see cref="CellularModule"/> model object, or a collection of the kegg <see cref="KEGGReaction"/>.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function LppMatrix(<RRawVectorArgument> model As Object,
                           Optional terms As String() = Nothing,
                           Optional env As Environment = Nothing) As Object

        If TypeOf model Is CellularModule Then
            Return New LinearProgrammingEngine().CreateMatrix(DirectCast(model, CellularModule))
        ElseIf TypeOf model Is ReactionRepository Then
            Dim repo = DirectCast(model, ReactionRepository)
            Dim stream As Index(Of String) = repo _
                .GetByKOMatch(terms) _
                .Select(Function(r) r.ID) _
                .ToArray
            Dim network As Matrix = repo.metabolicNetwork.CreateKeggMatrix
            Dim gaps As New List(Of String)

            For Each key As String In network.Flux.Keys.ToArray
                If Not key Like stream Then
                    If repo.GetByKey(key).Enzyme.IsNullOrEmpty Then
                        network.Flux(key) = New DoubleRange(-5, 5)
                    Else
                        network.Flux(key) = New DoubleRange(-0.5, 0.5)
                        gaps.Add(key)
                    End If
                Else
                    network.Flux(key) = New DoubleRange(-5, 10)
                End If
            Next

            network.Gaps = gaps.ToArray

            Return network
        ElseIf TypeOf model Is VirtualCell Then
            Dim gem As VirtualCell = DirectCast(model, VirtualCell)

            Console.WriteLine("=========================================================")
            Console.WriteLine(" genome scale GEM model FBA test")
            Console.WriteLine($" model: {gem.cellular_id}")
            Console.WriteLine("=========================================================")

            Dim watch As Stopwatch = Stopwatch.StartNew
            Dim reactions = gem.metabolismStructure.reactions.AsEnumerable.ToArray
            Dim reversible As New Dictionary(Of String, Boolean)

            Console.WriteLine($"load GEM model in {watch.ElapsedMilliseconds} ms, {reactions.Length} reactions")

            ' 可逆性判定：方程式中出现 "<=>" 即为可逆反应
            For Each reaction In reactions
                Dim note As String = If(reaction.note, "")

                reversible(reaction.ID) = note.Contains("<=>")
            Next

            watch.Restart()

            Dim metabolic As Equation() = reactions _
                .Select(Function(r) r.BuildEquation) _
                .ToArray
            Dim matrix As Matrix = metabolic.BuildMatrix()
            Dim nReversible As Integer = 0

            For Each id As String In matrix.Flux.Keys.ToArray
                If reversible.ContainsKey(id) AndAlso reversible(id) Then
                    matrix.Flux(id) = New DoubleRange(-1000, 1000)
                    nReversible += 1
                Else
                    matrix.Flux(id) = New DoubleRange(0, 1000)
                End If
            Next

            Console.WriteLine($"build stoichiometric matrix in {watch.ElapsedMilliseconds} ms: {matrix.Stoichiometry}")
            Console.WriteLine($"  {nReversible} reversible reactions, {matrix.Flux.Count - nReversible} irreversible reactions")

            Return matrix
        Else
            Dim stream As PipeIterator(Of KEGGReaction) = pipeline.Stream(Of KEGGReaction)(model, env)

            If stream.isError Then
                Return stream.getError
            End If

            Return stream.CreateKeggMatrix
        End If
    End Function

    ''' <summary>
    ''' set lpp objective targets for FBA analysis
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="target">
    ''' should be a character vector of the target reaction id 
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("objective")>
    <RApiReturn(GetType(Matrix))>
    Public Function SetObjective(matrix As Matrix, target As Object, Optional env As Environment = Nothing) As Matrix
        If TypeOf target Is list Then
            Dim upper As list = DirectCast(target, list)
            Dim value As Double()

            matrix.Targets = upper.slots.Keys.ToArray

            For Each rId As String In upper.slots.Keys.Where(Function(id) matrix.Flux.ContainsKey(id))
                value = CLRVector.asNumeric(upper.slots(rId))

                If value.Length = 1 Then
                    matrix.Flux(rId).Max = value(0)
                Else
                    matrix.Flux(rId) = New DoubleRange(value)
                End If
            Next
        Else
            matrix.Targets = CLRVector.asCharacter(target)
        End If

        Return matrix
    End Function

    ''' <summary>
    ''' convert the flux matrix as the general Linear Programming model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="name"></param>
    ''' <returns>a general Linear Programming model</returns>
    ''' <remarks>
    ''' the flux matrix encoded as the general lpp model via:
    ''' 
    ''' 1. mapping the flux as the <see cref="LPPModel.variables"/>
    ''' 2. mapping the compound and flux coefficient factor as the <see cref="LPPModel.constraintCoefficients"/> data.
    ''' </remarks>
    <ExportAPI("lppModel")>
    <RApiReturn(GetType(LPPModel))>
    Public Function GetLppModel(model As Matrix, Optional name As String = "Flux Balance Analysis LppModel") As LPPModel
        Return LinearProgrammingEngine.ToLppModel(model, name)
    End Function

    ''' <summary>
    ''' Solve a FBA matrix model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns>
    ''' a tuple list of the FBA lpp solver result:
    ''' 
    ''' + objective, target objective function value
    ''' + flux, the flux distribution result tuple list, key name is the flux id and the value is the flux value. 
    ''' </returns>
    <ExportAPI("lpsolve")>
    <RApiReturn("objective", "flux")>
    Public Function lpsolve(model As Matrix) As Object
        Dim lpp As LPPSolution = New LinearProgrammingEngine().Run(model)
        Dim result As New list
        Dim solution As New Dictionary(Of String, Double)

        For Each val As SeqValue(Of Double) In lpp.GetSolution(model.Targets).SeqIterator
            Call solution.Add(model.Targets(val.i), val.value)
        Next

        Call result.add("objective", lpp.ObjectiveFunctionValue)
        Call result.add("flux", solution)
        Call result.setAttribute("lpp", lpp)

        Return result
    End Function

End Module
