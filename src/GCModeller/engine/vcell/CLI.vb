#Region "Microsoft.VisualBasic::a28707685f4345ed7272827727a7fffc, engine\vcell\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: createDriver, getDeletionList, Run
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports vcellkit

<CLI> Module CLI

    <Extension>
    Private Function getDeletionList(res As DefaultString) As String()
        If res.FileExists Then
            Return res.ReadAllLines _
                .Select(Function(l) l.Split.First) _
                .ToArray
        Else
            Return res.Split(","c)
        End If
    End Function

    <ExportAPI("/run")>
    <Usage("/run /model <model.gcmarkup> [/deletes <genelist> /time <default=100> /json /out <raw/result_directory>]")>
    <Description("Run GCModeller VirtualCell.")>
    <ArgumentAttribute("/deletes", True, CLITypes.String,
              AcceptTypes:={GetType(String())},
              Description:="The ``locus_tag`` id list that will removes from the genome, 
              use the comma symbol as delimiter. Or a txt file path for the gene id list.")>
    Public Function Run(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim deletes As String() = args("/deletes").getDeletionList
        Dim jsonFormat As Boolean = args("/json")
        Dim out$ = args("/out") Or If(jsonFormat, $"{in$.TrimSuffix}.vcell_simulation/", $"{in$.TrimSuffix}.vcell_simulation.raw")
        Dim iterations% = args("/time") Or 100
        Dim model As VirtualCell = [in].LoadXml(Of VirtualCell)
        Dim def As Definition = model.CreateUnifyDefinition
        Dim cell As CellularModule = model.CreateModel

        If jsonFormat Then
            Dim massIndex = OmicsDataAdapter.GetMassTuples(cell)
            Dim fluxIndex = OmicsDataAdapter.GetFluxTuples(cell)
            Dim engine As Engine = New Engine(def, New FluxBaseline, iterations) _
                .LoadModel(cell, deletes) _
                .AttachBiologicalStorage(New FinalSnapshotDriver)

            Call engine.Run()
            Call engine.TakeStatusSnapshot(massIndex, fluxIndex, save:=out)

            Return 0
        Else
            Dim loader As Loader = Nothing
            Dim engine As New Engine(def, New FluxBaseline, iterations)
            Dim snapshot As New FinalSnapshotDriver

            Call engine.LoadModel(cell, deletes, getLoader:=loader).AttachBiologicalStorage(snapshot)

            Using rawStorage As New Raw.StorageDriver(out, loader, cell)
                Call engine.AttachBiologicalStorage(rawStorage).Run()
            End Using

            Return 0
        End If
    End Function

    <Extension>
    Private Function createDriver(save As WriteStream(Of DataSet)) As SnapshotDriver
        Return Sub(i, data)
                   Dim snapshot As New DataSet With {
                      .ID = "#" & (i + 1),
                      .Properties = data
                   }

                   Call save.Flush(snapshot)
               End Sub
    End Function
End Module
