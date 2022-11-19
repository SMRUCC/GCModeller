#Region "Microsoft.VisualBasic::3cd34191875da78695336fb35f748d57, GCModeller\engine\BootstrapLoader\Engine\VCell\Engine.vb"

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

    '   Total Lines: 113
    '    Code Lines: 70
    ' Comment Lines: 22
    '   Blank Lines: 21
    '     File Size: 4.11 KB


    '     Class Engine
    ' 
    '         Properties: dataStorageDriver, debugView, dynamics, initials, model
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AttachBiologicalStorage, getMassPool, LoadModel, Run
    ' 
    '         Sub: Reset
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
                Optional showProgress As Boolean = True)

            Call MyBase.New(Nothing, iterations, timeResolution, showProgress)

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
                .CreateEnvironment(virtualCell) _
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
