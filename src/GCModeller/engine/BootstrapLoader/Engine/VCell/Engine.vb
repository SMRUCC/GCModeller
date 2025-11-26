#Region "Microsoft.VisualBasic::0df09a92bd06ed7448ef05bdaa57a6e9, engine\BootstrapLoader\Engine\VCell\Engine.vb"

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

'   Total Lines: 143
'    Code Lines: 94 (65.73%)
' Comment Lines: 22 (15.38%)
'    - Xml Docs: 86.36%
' 
'   Blank Lines: 27 (18.88%)
'     File Size: 5.52 KB


'     Class Engine
' 
'         Properties: dataStorageDriver, debugView, dynamics, initials, model
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: AttachBiologicalStorage, getMassPool, LoadModel, Run
' 
'         Sub: DumpDynamicsCore, Reset
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Engine

    ''' <summary>
    ''' The GCModeller VirtualCell dynamics engine module
    ''' </summary>
    Public Class Engine : Inherits FluxEmulator
        Implements ITaskDriver

        Public ReadOnly Property dataStorageDriver As IOmicsDataAdapter

        ''' <summary>
        ''' The argument of the cellular flux dynamics
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property dynamics As FluxBaseline
        ''' <summary>
        ''' The compound map definition and the initial status
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property initials As Definition
        Public ReadOnly Property debugView As DebuggerView
        Public Property models As CellularModule()
        Public Property fluxIndex As Dictionary(Of String, String())

        Sub New(def As Definition, dynamics As FluxBaseline, cellular_id As String(),
                Optional iterations% = 500,
                Optional timeResolution# = 10000,
                Optional showProgress As Boolean = True,
                Optional debug As Boolean = False)

            Call MyBase.New(Nothing, iterations, timeResolution,
                            showProgress:=showProgress,
                            debug:=debug)

            Me.initials = def
            Me.dynamics = dynamics
            Me.debugView = New DebuggerView(Me, cellular_id)
        End Sub

        Public Function getMassPool() As MassTable
            Dim table As New MassTable

            For Each factor As Factor In core.m_massIndex.Values
                Call table.copy(factor)
            Next

            Return table
        End Function

        ''' <summary>
        ''' Attach the biological data storage driver
        ''' </summary>
        ''' <param name="driver"></param>
        ''' <returns></returns>
        Public Function AttachBiologicalStorage(driver As IOmicsDataAdapter) As Engine
            _dataStorageDriver = driver

            Call AttatchMassDriver(AddressOf driver.MassSnapshot)
            Call AttatchFluxDriver(AddressOf driver.FluxSnapshot)
            Call AttachRegulationDriver(
                AddressOf driver.ForwardRegulation,
                AddressOf driver.ReverseRegulation
            )

            Return Me
        End Function

        Public Function MakeNetworkSnapshot(storage As IFileSystemEnvironment) As Engine
            Dim root_dir As String = "/cellular_graph.jsonl"
            Dim buffer As New MemoryStream
            Dim str As New StreamWriter(buffer)

            For Each flux As Channel In TqdmWrapper.Wrap(core.Channels, wrap_console:=App.EnableTqdm)
                Call str.WriteLine(flux.jsonView)
            Next

            Call storage.DeleteFile(root_dir)

            Using file = storage.OpenFile(root_dir, FileMode.OpenOrCreate, FileAccess.Write)
                Call str.Flush()
                Call buffer.Seek(Scan0, SeekOrigin.Begin)
                Call buffer.CopyTo(file)
                Call file.Flush()
            End Using

            Return Me
        End Function

        Public Function SetModel(mass As MassTable, biologicalProcesses As IEnumerable(Of Channel)) As Engine
            Call core _
                .load(mass.AsEnumerable) _
                .load(biologicalProcesses) _
                .Initialize(boost:=dynamics.boost)
            Call Reset()

            Return Me
        End Function

        Public Function LoadModel(virtualCell As CellularModule,
                                  Optional ByRef getLoader As Loader = Nothing,
                                  Optional unitTest As Boolean = False) As Engine

            getLoader = New Loader(initials, dynamics, unitTest)
            models = {virtualCell}

            With getLoader.CreateEnvironment(virtualCell)
                Call core _
                    .load(.massTable.AsEnumerable) _
                    .load(.processes) _
                    .Initialize(boost:=dynamics.boost)
            End With

            Call Reset()

            Return Me
        End Function

        ''' <summary>
        ''' set all genes its copy number to a given value
        ''' </summary>
        ''' <param name="copyNum"></param>
        ''' <returns></returns>
        Public Function SetCopyNumbers(copyNum As Integer) As Engine
            For Each mass As Factor In core.m_massIndex.Values
                If mass.role = MassRoles.gene Then
                    Call mass.reset(copyNum)
                End If
            Next

            Return Me
        End Function

        Public Function SetCellCopyNumber(copyNum As Dictionary(Of String, Integer)) As Engine
            If core Is Nothing Then
                Throw New InvalidProgramException("Please load model at first!")
            End If

            Dim genes As Dictionary(Of String, Factor()) = core.m_massIndex.Values _
                .Where(Function(a) a.role = MassRoles.gene) _
                .GroupBy(Function(a) a.cellular_compartment) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)

            For Each cellCopy As KeyValuePair(Of String, Integer) In copyNum
                If genes.ContainsKey(cellCopy.Key) Then
                    Dim copyNumber As Double = CDbl(cellCopy.Value)

                    For Each gene As Factor In genes(cellCopy.Key)
                        gene.reset(copyNumber)
                    Next
                End If
            Next

            Return Me
        End Function

        ''' <summary>
        ''' set gene copy numbers
        ''' </summary>
        ''' <param name="copyNum">a tuple list of [gene_id => copyNumber]</param>
        ''' <returns></returns>
        Public Function SetCopyNumbers(copyNum As Dictionary(Of String, Integer)) As Engine
            If core Is Nothing Then
                Throw New InvalidProgramException("Please load model at first!")
            End If

            For Each mass As Factor In core.m_massIndex.Values
                If copyNum.ContainsKey(mass.template_id) AndAlso mass.role = MassRoles.gene Then
                    Call mass.reset(copyNum(mass.template_id))
                End If
            Next

            Return Me
        End Function

        Public Function SetCultureMedium(cultureMedium As Dictionary(Of String, Double)) As Engine
            For Each mass As Factor In core.m_massIndex.Values
                If mass.cellular_compartment = initials.CultureMedium Then
                    Call mass.reset(0)
                End If
            Next
            For Each mass As Factor In core.m_massIndex.Values
                If cultureMedium.ContainsKey(mass.template_id) AndAlso mass.cellular_compartment = initials.CultureMedium Then
                    Call mass.reset(cultureMedium(mass.template_id))
                End If
            Next

            Return Me
        End Function

        ''' <summary>
        ''' should be call after the model was loaded, via function <see cref="SetModel(MassTable, IEnumerable(Of Channel))"/> or 
        ''' <see cref="LoadModel(CellularModule, ByRef Loader, Boolean)"/>
        ''' </summary>
        ''' <param name="knockouts"></param>
        ''' <returns></returns>
        Public Function MakeKnockout(knockouts As IEnumerable(Of String)) As Engine
            If core Is Nothing Then
                Throw New InvalidProgramException("Please load model at first!")
            End If

            Dim index As Dictionary(Of String, Factor) = core.m_massIndex
            Dim compartment_id As String() = models _
                .Select(Function(cell) cell.CellularEnvironmentName) _
                .ToArray

            ' 在这里完成初始化后
            ' 再将对应的基因模板的数量设置为0
            ' 达到无法执行转录过程反应的缺失突变的效果
            For Each gene_id As String In knockouts.SafeQuery
                If index.ContainsKey(gene_id) Then
                    Call index(gene_id).reset(0)
                Else
                    For Each cid As String In compartment_id
                        Dim full_id As String = $"{gene_id}@{cid}"

                        If index.ContainsKey(full_id) Then
                            Call index(full_id).reset(0)
                        End If
                    Next
                End If

                Call $"deletes '{gene_id}'...".info
            Next

            Return Me
        End Function

        Public Sub DumpDynamicsCore(s As StreamWriter)
            If core Is Nothing Then
                Throw New InvalidProgramException("Please load model at first!")
            End If

            Call s.WriteLine($"----======== {core.MassEnvironment.Length} molecules =========----")

            For Each mass As Factor In core.MassEnvironment
                Call s.WriteLine(mass.ToString)
            Next

            Call s.WriteLine()
            Call s.WriteLine()

            Call s.WriteLine($"----========= {core.Channels.Length} dynamics channels ===========----")

            For Each flux As Channel In core.Channels
                Call s.WriteLine($"[{flux.ID}] {ModellingEngine.Dynamics.Core.ToString(flux)}")
                Call s.WriteLine(flux.bounds.ToString)
                Call s.WriteLine($"forward: {flux.forward.ToString}")
                Call s.WriteLine($"reverse: {flux.reverse.ToString}")
                Call s.WriteLine()
            Next
        End Sub

        ''' <summary>
        ''' Reset the reactor engine. (Do reset of the biological mass contents)
        ''' </summary>
        Public Sub Reset()
            If initials Is Nothing Then
                Return
            End If

            For Each mass As Factor In core.m_massIndex.Values
                Dim status As Dictionary(Of String, Double) = initials.status(mass.cellular_compartment)

                If status.ContainsKey(mass.ID) Then
                    ' instance id has the highest order
                    Call mass.reset(status(mass.ID))
                Else
                    If status.ContainsKey(mass.template_id) Then
                        Call mass.reset(status(mass.template_id))
                    Else
                        Call mass.reset(randf.NextDouble(10, 250))
                    End If
                End If
            Next

            If initials.status.ContainsKey(initials.CultureMedium) Then
                Call SetCultureMedium(initials.status(initials.CultureMedium))
            End If
        End Sub

        Public Overrides Function Run() As Integer Implements ITaskDriver.Run
            If dataStorageDriver Is Nothing Then
                Call "Data storage driver not found! The simulation result can only be get from snapshot property...".warning
                Call VBDebugger.WaitOutput()
                Call Console.WriteLine()
            End If

            Return MyBase.Run
        End Function
    End Class
End Namespace
