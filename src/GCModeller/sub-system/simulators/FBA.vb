#Region "Microsoft.VisualBasic::50856cdc70ea0f2b146b0164438a6131, sub-system\simulators\FBA.vb"

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

' Module FBA
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.FBA.Core
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.FBA.Core.Matrix

''' <summary>
''' Flux Balance Analysis
''' </summary>
<Package("FBA")>
Module FBA

    ''' <summary>
    ''' create FBA model matrix
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function Matrix(<RRawVectorArgument> model As Object, Optional env As Environment = Nothing) As Object
        If TypeOf model Is CellularModule Then
            Return New LinearProgrammingEngine().CreateMatrix(DirectCast(model, CellularModule))
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
