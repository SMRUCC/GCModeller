#Region "Microsoft.VisualBasic::d6838e67556f218ddc6f4bf2aeade2b3, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAcquisitionService\DataAcquisitionService.vb"

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

    '     Class DataAcquisitionService
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Close, Connect, GetDataChunk, GetDefinitions, Initialize
    '                   Tick, ToString
    ' 
    '         Sub: GenerateDefinition, SetUpCommitInterval
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.Services.DataAcquisition.Services

    ''' <summary>
    ''' 数据采集服务的基类型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataAcquisitionService : Inherits MySQL.Service
        Implements IDataAcquisitionService

        Protected Friend DataStorage As DataSerializer.DataSerializer
        Protected Friend DataSource As IDataAdapter
        Protected Friend Id As Long = 0
        ''' <summary>
        ''' 向数据库服务器提交计算数据的时间间隔
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend CommitInterval As Integer
        Protected Friend SystemLogging As LogFile

        Protected IRuntimeContainer As IContainerSystemRuntimeEnvironment

        Dim _SuppressPeriodicMessage As Boolean = False

        Public Overridable ReadOnly Property TableName As String Implements MySQL.IDataAcquisitionService.TableName
            Get
                Return DataSource.TableName
            End Get
        End Property

        Sub New(Adapter As IDataAdapter, RuntimeContainer As EngineSystem.Engine.GCModeller)
            Me.DataSource = Adapter
            Me.SystemLogging = RuntimeContainer.SystemLogging
            Me.IRuntimeContainer = RuntimeContainer
            If Not String.IsNullOrEmpty(RuntimeContainer.GetArguments("-suppress_periodic_message")) Then
                Me._SuppressPeriodicMessage = String.Equals(RuntimeContainer.GetArguments("-suppress_periodic_message").First.ToString, "T", StringComparison.OrdinalIgnoreCase)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return TableName
        End Function

        ''' <summary>
        ''' 数据采集服务连接数据存储服务
        ''' </summary>
        ''' <param name="StorageService"></param>
        ''' <remarks></remarks>
        Public Overridable Function Connect(StorageService As DataSerializer.DataSerializer) As Integer Implements MySQL.IDataAcquisitionService.Connect
            DataStorage = StorageService
            Return 0
        End Function

        Public Overridable Function Tick(RTime As Integer) As Integer Implements MySQL.IDataAcquisitionService.Tick
            Dim DataPackage = (From e In DataSource.DataSource().AsParallel Select DataFlowF.CreateObject(e, RTime)).ToArray
            Call DataStorage.Append(DataPackage)

            If Id = CommitInterval Then
                If Not _SuppressPeriodicMessage Then Call Console.WriteLine("[{0}]  Commit data to the database server. //{1}", RTime, TableName)
                If DataStorage.CommitData(arg:=TableName) <> 0 Then
                    SystemLogging.WriteLine("[EXCEPTION][RTime:=" & RTime & "]Data commit failure, roll back the data commit transaction.",
                                                        "DataAcquisition->CommitData(); Table:= " & TableName,
                                                        MSG_TYPES.ERR)
                    SystemLogging.WriteLine("System unexpected break down!", "", MSG_TYPES.ERR)
                    Call DataStorage.GetErrMessage.SaveTo(Settings.TEMP & "/TRANSACTION_DUMP.log")
                    Throw New Exception(Me.DataStorage.GetErrMessage.Split(CChar("</SQL>")).First)
                Else
                    Id = 0
                End If
            Else
                Id += 1
            End If
            Return Id
        End Function

        Public Overridable Function Close() As Integer Implements MySQL.IDataAcquisitionService.Close
            If TypeOf DataStorage Is EngineSystem.Services.DataAcquisition.DataSerializer.MySQL Then
                Call Me.SystemLogging.WriteLine(String.Format("[TABLE: {0}]  Commit the remaining data to the mysql server...", TableName), "GCModeller -> main_thread_data_acquisition_service")
            Else
                Call Me.SystemLogging.WriteLine(String.Format("Write the data to local file: {0}", DataStorage._Url), "GCModeller -> main_thread_data_acquisition_service")
            End If

            Call GenerateDefinition()
            Call DataStorage.Close(arg:=TableName)
            Return 0
        End Function

        Public Overridable Sub GenerateDefinition() Implements MySQL.IDataAcquisitionService.GenerateDefinition
            Dim [Handles] = DataSource.DefHandles()
            If Not [Handles].IsNullOrEmpty Then
                Call DataStorage.CreateHandle([Handles], TableName)
            End If
        End Sub

        Public Overridable Function Initialize() As Integer Implements MySQL.IDataAcquisitionService.Initialize
            Call DataStorage.Initialize(arg:=TableName)
            DataStorage._SuppressPeriodicMessage = Me._SuppressPeriodicMessage
            Return 0
        End Function

        Public Sub SetUpCommitInterval(Interval As Integer) Implements MySQL.IDataAcquisitionService.SetUpCommitInterval
            Me.CommitInterval = Interval
        End Sub

        Public Function GetDataChunk() As DataFlowF() Implements MySQL.IDataAcquisitionService.GetDataChunk
            Dim RTime As Integer = Me.IRuntimeContainer.RuntimeTicks
            Dim DataPackage = (From e In DataSource.DataSource().AsParallel Select DataFlowF.CreateObject(e, RTime)).ToArray
            Return DataPackage
        End Function

        ''' <summary>
        ''' 这个是应用于<see cref="EngineSystem.Services.DataAcquisition.ManageSystem.DumpData.WriteDumpData">DUMP数据</see>的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDefinitions() As HandleF() Implements MySQL.IDataAcquisitionService.GetDefinitions
            Return DataSource.DefHandles
        End Function
    End Class
End Namespace
