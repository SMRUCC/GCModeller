﻿#Region "Microsoft.VisualBasic::f9c27c501e842f46862f7c767d185b2d, engine\BootstrapLoader\Engine\VCell\Engine.vb"

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

    '   Total Lines: 142
    '    Code Lines: 93 (65.49%)
    ' Comment Lines: 22 (15.49%)
    '    - Xml Docs: 86.36%
    ' 
    '   Blank Lines: 27 (19.01%)
    '     File Size: 5.40 KB


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
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

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
        Public ReadOnly Property model As CellularModule
        ''' <summary>
        ''' The compound map definition and the initial status
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property initials As Definition
        Public ReadOnly Property debugView As DebuggerView

        Sub New(def As Definition, dynamics As FluxBaseline,
                Optional iterations% = 500,
                Optional timeResolution# = 10000,
                Optional showProgress As Boolean = True,
                Optional debug As Boolean = False)

            Call MyBase.New(Nothing, iterations, timeResolution,
                            showProgress:=showProgress,
                            debug:=debug)

            Me.initials = def
            Me.dynamics = dynamics
            Me.debugView = New DebuggerView(Me)
        End Sub

        Friend Function getMassPool() As MassTable
            Return New MassTable(core.m_massIndex)
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

            Return Me
        End Function

        Public Function LoadModel(virtualCell As CellularModule,
                                  Optional deletions As IEnumerable(Of String) = Nothing,
                                  Optional ByRef getLoader As Loader = Nothing) As Engine

            getLoader = New Loader(initials, dynamics)
            core = getLoader _
                .CreateEnvironment(virtualCell, core) _
                .Initialize()
            _model = virtualCell

            Call Reset()

            ' 在这里完成初始化后
            ' 再将对应的基因模板的数量设置为0
            ' 达到无法执行转录过程反应的缺失突变的效果
            For Each geneTemplateId As String In deletions.SafeQuery
                core.m_massIndex(geneTemplateId).Value = 0

                Call $"Deletes '{geneTemplateId}'...".__INFO_ECHO
            Next

            Return Me
        End Function

        Public Sub DumpDynamicsCore(s As StreamWriter)
            If core Is Nothing Then
                Throw New InvalidProgramException("Please load model at first!")
            End If

            Call s.WriteLine($"----======== {core.MassEnvironment.Length} molecules =========----")

            For Each mass In core.MassEnvironment
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
                If initials.status.ContainsKey(mass.ID) Then
                    mass.Value = initials.status(mass.ID)
                Else
                    mass.Value = 1
                End If
            Next
        End Sub

        Public Overrides Function Run() As Integer Implements ITaskDriver.Run
            If dataStorageDriver Is Nothing Then
                Call "Data storage driver not found! The simulation result can only be get from snapshot property...".Warning
                Call VBDebugger.WaitOutput()
                Call Console.WriteLine()
            End If

            Return MyBase.Run
        End Function
    End Class
End Namespace
