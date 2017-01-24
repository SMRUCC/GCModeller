#Region "Microsoft.VisualBasic::17f0f34425dfab7f7c8b270fc54e3176, ..\GCModeller\sub-system\FBA_DP\rFBA\rFBASolver.vb"

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

'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
'Imports Microsoft.VisualBasic.DocumentFormat.Csv
'Imports Microsoft.VisualBasic.Linq.Extensions

'Public Class rFBASolver : Inherits IterationMathEngine(Of rFBA.DataModel.CellSystem)

'    Public Property LpSolver As FBA.FBAlpRSolver
'    Public Property ObjectModel As rFBARModel
'    Public Property MetabolismData As IO.File = New IO.File
'    Public Property Transcriptome As IO.File = New IO.File

'    Dim _resultTemp As KeyValuePair(Of String, String())

'    Sub New(Model As rFBA.DataModel.CellSystem)
'        Call MyBase.New(Model)
'    End Sub

'    Public Overrides Function Initialize() As Integer
'        Call ObjectModel.Initialize()

'        Call MetabolismData.Add(New String() {"Objective Function"})
'        Call MetabolismData.First.AddRange((From item In ObjectModel.CellSystem.MetabolismFluxs Select item.Identifier).ToArray)
'        Call Transcriptome.Add((From item In ObjectModel.CellSystem.ExpressionRegulation Select item.Identifier).ToArray)

'        MyBase.IterationLoops = If(ObjectModel.CellSystem.IteractionLoops = 0, 10, ObjectModel.CellSystem.IteractionLoops)
'        MyBase.__runDataAdapter = AddressOf Me.Update

'        Return 0
'    End Function

'    Public Sub ExportData(ExportedDir As String)
'        Call MetabolismData.Save(String.Format("{0}/MetabolismData.csv", ExportedDir), False)
'        Call Transcriptome.Save(String.Format("{0}/TranscriptomeData.csv", ExportedDir), False)
'    End Sub

'    Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
'        Me._resultTemp = LpSolver.RSolving(ObjectModel)
'        Return 0
'    End Function

'    Private Sub Update()
'        Dim FluxValues = Me._resultTemp.Value
'        Dim MetabolismFluxsValue = (From Handle In ObjectModel._MetabolismFluxCounts.Sequence Select FluxValues(Handle)).ToArray
'        Call MetabolismData.Add(New String() {_resultTemp.Key})
'        Call MetabolismData.Last.AddRange(MetabolismFluxsValue)
'        Call Console.WriteLine("Objective Function value is '{0}' at kernel loop {1}", _resultTemp.Key, MyBase._RTime)
'        Console.WriteLine("----------------------------------------------" & vbCrLf & vbCrLf)

'        Dim TranscriptomeFluxsValue = (From Handle In ObjectModel._TotalFluxCounts.Sequence.Skip(ObjectModel._MetabolismFluxCounts) Select FluxValues(Handle)).ToArray
'        Call Transcriptome.Add(TranscriptomeFluxsValue)

'        Dim CurrentValue As Double() = (From strData As String In _resultTemp.Value Select Val(strData)).ToArray
'        ObjectModel.IterationFluxValue = CurrentValue

'        Call ObjectModel.UpdateFluxConstraints()
'    End Sub
'End Class
