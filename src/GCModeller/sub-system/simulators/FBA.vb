#Region "Microsoft.VisualBasic::6cb3f27e2f01f4ad1e5ce25acefea07b, GCModeller\sub-system\simulators\FBA.vb"

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

    '   Total Lines: 166
    '    Code Lines: 127
    ' Comment Lines: 14
    '   Blank Lines: 25
    '     File Size: 6.00 KB


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
Imports SMRUCC.genomics.Analysis.FBA.Core
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.FBA.Core.Matrix
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Flux Balance Analysis
''' </summary>
<Package("FBA")>
Module FBA

    Sub New()
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
    ''' <param name="model"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function Matrix(<RRawVectorArgument> model As Object,
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
        Else
            Dim stream = pipeline.TryCreatePipeline(Of Reaction)(model, env)

            If stream.isError Then
                Return stream.getError
            End If

            Return stream _
                .populates(Of Reaction)(env) _
                .CreateKeggMatrix
        End If
    End Function

    <ExportAPI("objective")>
    Public Function SetObjective(matrix As Matrix, target As Object, Optional env As Environment = Nothing) As Matrix
        If TypeOf target Is list Then
            Dim upper As list = DirectCast(target, list)
            Dim value As Double()

            matrix.Targets = upper.slots.Keys.ToArray

            For Each rId As String In upper.slots.Keys.Where(Function(id) matrix.Flux.ContainsKey(id))
                value = REnv.asVector(Of Double)(upper.slots(rId))

                If value.Length = 1 Then
                    matrix.Flux(rId).Max = value(0)
                Else
                    matrix.Flux(rId) = New DoubleRange(value)
                End If
            Next
        Else
            matrix.Targets = REnv.asVector(Of String)(target)
        End If

        Return matrix
    End Function

    <ExportAPI("lppModel")>
    Public Function GetLppModel(model As Matrix, Optional name As String = "Flux Balance Analysis LppModel") As LPPModel
        Return LinearProgrammingEngine.ToLppModel(model, name)
    End Function

    ''' <summary>
    ''' Solve a FBA matrix model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    <ExportAPI("lpsolve")>
    Public Function lpsolve(model As Matrix) As Object
        Dim lpp As LPPSolution = New LinearProgrammingEngine().Run(model)
        Dim result As New list
        Dim solution As New Dictionary(Of String, Double)

        For Each val As SeqValue(Of Double) In lpp.GetSolution(model.Targets).SeqIterator
            Call solution.Add(model.Targets(val.i), val.value)
        Next

        Call result.add("objective", lpp.ObjectiveFunctionValue)
        Call result.add("flux", solution)

        Return result
    End Function

End Module
