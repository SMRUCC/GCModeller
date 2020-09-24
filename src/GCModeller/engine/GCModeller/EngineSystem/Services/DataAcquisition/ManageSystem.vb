#Region "Microsoft.VisualBasic::d96bcb89c4baf8797724eb4bd867aca0, engine\GCModeller\EngineSystem\Services\DataAcquisition\ManageSystem.vb"

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

    '     Class ManageSystem
    ' 
    ' 
    '         Enum DataStorageServiceTypes
    ' 
    '             CSV, MySQL
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: SystemLogging
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Connect, GetTypeOfDataStorageServiceOfMySQL, TestMySQL
    ' 
    '     Sub: CloseStorageService, Initialize, Join, Tick
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.Services.DataAcquisition

    ''' <summary>
    ''' 整个计算引擎的数据采集服务的中枢控制模块
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ManageSystem : Inherits Service

        Public Enum DataStorageServiceTypes
            CSV
            MySQL
        End Enum

        Dim DataAcquisitionServices As List(Of IDataAcquisitionService) = New List(Of IDataAcquisitionService)
        Dim Kernel As Engine.GCModeller
        Dim TriggerSystem As TriggerSystem
        Dim DumpDataTrigger As DumpData
        Protected Friend _SuppressPeriodicMessage As Boolean = False

        Protected Friend ReadOnly Property SystemLogging As LogFile
            Get
                Return Kernel.SystemLogging
            End Get
        End Property

        Public Sub New(ModellerKernel As Engine.GCModeller)
            Kernel = ModellerKernel
        End Sub

        ''' <summary>
        ''' 向管理层中的处理队列之中添加一个服务实例集合
        ''' </summary>
        ''' <param name="ServiceInstanceSerials">目标数据采集服务对象实例的集合</param>
        ''' <remarks></remarks>
        Public Sub Join(ServiceInstanceSerials As IDataAcquisitionService())
            Call DataAcquisitionServices.AddRange(ServiceInstanceSerials)
            For i As Integer = 0 To ServiceInstanceSerials.Count - 1
                Call ServiceInstanceSerials(i).SetUpCommitInterval(Kernel.KernelProfile.CommitLoopsInterval)
            Next

            For Each item In ServiceInstanceSerials
                Call SystemLogging.WriteLine(String.Format("  DataService <---- *{0}", item.TableName))
            Next
        End Sub

        Public Sub Initialize()
            TriggerSystem = New TriggerSystem(Me.Kernel)
            Call TriggerSystem.Initialize()
            DumpDataTrigger = New DumpData(Me, Kernel.GetArguments("-dump"), Kernel.GetArguments("-dump_data"))
            If Not DumpDataTrigger.Invalid Then Call TriggerSystem.PendingTrigger(DumpDataTrigger.GetTrigger)

            'If Not String.IsNullOrEmpty(Kernel.GetArguments("-suppress_periodic_message")) Then
            '    Me._SuppressPeriodicMessage = String.Equals(Kernel.GetArguments("-suppress_periodic_message").First.ToString, "T", StringComparison.OrdinalIgnoreCase)
            'End If
            Me._SuppressPeriodicMessage = Me.Kernel.ConfigurationData.SuppressPeriodicMessage
        End Sub

        ''' <summary>
        ''' 创建总表定义的SQL语句
        ''' </summary>
        ''' <remarks></remarks>
        Const SQL_CREATE_STORAGES_TABLE As String =
            "CREATE TABLE `storages` (" &
            "`idstorages` INT UNSIGNED NOT NULL AUTO_INCREMENT," &
            "`storage_table` VARCHAR(128) NOT NULL," &
            "PRIMARY KEY (`idstorages`)," &
            "UNIQUE INDEX `idstorages_UNIQUE` (`idstorages` ASC), " &
            "UNIQUE INDEX `storage_table_UNIQUE` (`storage_table` ASC));"

        Const SQL_INSERT_TABLE_ENTRY As String = "INSERT INTO `storages` (`storage_table`) VALUES ('%s');"

        Dim StorageServiceType As DataAcquisition.ManageSystem.DataStorageServiceTypes

        ''' <summary>
        ''' 数据采集服务与MYSQL数据库服务相连接
        ''' </summary>
        ''' <param name="Url"></param>
        ''' <remarks></remarks>
        Public Function Connect(Url As String) As Integer
            Dim LQuery As Generic.IEnumerable(Of Integer)

            If GetTypeOfDataStorageServiceOfMySQL(Url) Then
                Call Me.SystemLogging.WriteLine("Data storage service pointer to a mysql service.", "data_manage -> connect(data_storage_service_url)")
                If TestMySQL(Url) = False Then
                    Call Me.SystemLogging.WriteLine("MySQL connection test failure, operation abort!", "data_manage -> connect(data_storage_service_url)",
                                      MSG_TYPES.ERR)
                    Return -1
                End If

                LQuery = From ServiceInstance In Me.DataAcquisitionServices Select ServiceInstance.Connect(New DataSerializer.MySQL(Url)) '
                LQuery = LQuery.ToArray

                MyBase.MYSQL = ConnectionUri.CreateObject(Url)

                Call MYSQL.Execute("DROP TABLE `storages`;")
                Call MYSQL.Execute(SQL_CREATE_STORAGES_TABLE)
                For Each service In Me.DataAcquisitionServices
                    Call MYSQL.Execute(SQL_INSERT_TABLE_ENTRY.Replace("%s", service.TableName))
                Next

                StorageServiceType = DataStorageServiceTypes.MySQL
            Else
                Call Me.SystemLogging.WriteLine("Data storage service pointer to a local directory.", "data_manage -> connect(data_storage_service_url)")
                If Not Environment.Is64BitOperatingSystem AndAlso Kernel.KernelModule.Metabolism.Metabolites.Count > 1000 Then
                    Call Me.SystemLogging.WriteLine("[OUT_OF_MEMORY_EXCEPTION_WARNING]  gchost have detected that the running bacterial model is much larger than the gchost program running on a 32 bit platform could handle!" & vbCrLf &
                                      "please consider using the mysql database as storage service, or you may encounter an out of memory exception at later on this 32 bit platform.",
                                      "data_manage -> connect(data_storage_service_url)",
                                      MSG_TYPES.WRN)
                End If

                Call FileIO.FileSystem.CreateDirectory(directory:=Url)

                LQuery = From ServiceInstance As IDataAcquisitionService In Me.DataAcquisitionServices
                         Let ServiceURL As String = String.Format("{0}/{1}.csv", Url, ServiceInstance.TableName)
                         Select ServiceInstance.Connect(New DataSerializer.Csv(ServiceURL)) '
                LQuery = LQuery.ToArray

                StorageServiceType = DataStorageServiceTypes.CSV
            End If

            Dim ServiceInitializeLQuery = (From ServiceInstance As IDataAcquisitionService In Me.DataAcquisitionServices Select ServiceInstance.Initialize).ToArray

            Call Me.SystemLogging.WriteLine("[End of function]::ManageSystem->connect()", "ManagementSystem", MSG_TYPES.INF)

            Return 0
        End Function

        ''' <summary>
        ''' 数据管理服务测试与数据库服务器之间的连接
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TestMySQL(URL As String) As Boolean
            Dim MySql As ConnectionUri = URL

            Using MySQL_DbAdapter As MySqli = MySql
                Dim p = MySQL_DbAdapter.Ping

                If p < 0 Then  '数据库服务器通信连接测试失败
                    Call Me.SystemLogging.WriteLine("Could not establish the mysql server connection, please check out the connection parameter or the server running state.",
                                                    "gchost -> main()", MSG_TYPES.ERR)
                    Return False
                Else
                    Call Me.SystemLogging.WriteLine(String.Format("[{0}]  Established mysql server connection, ping {1} ms", Now.ToString, p), "gchost -> main()")
                    Return True
                End If
            End Using
        End Function

        ''' <summary>
        ''' 判断数据存储服务是否为MYSQL数据库服务
        ''' </summary>
        ''' <returns>True, 是MYSQL数据库服务，FALSE，是本地数据文件</returns>
        ''' <remarks></remarks>
        Private Shared Function GetTypeOfDataStorageServiceOfMySQL(Url As String) As Boolean
            Return Regex.Match(Url, pattern:="http://.+[:]\d+/client[?](.+=.+){1,3}").Success
        End Function

        Public Sub Tick(RTime As Integer)
            Call TriggerSystem.Tick(RTime) '由于数据采集服务在提交数据之后会将原有的数据清空，所有Trigger系统必须要在数据采集服务之前运行
            Dim ServiceRunningLQuery = (From ServiceInstance As IDataAcquisitionService In Me.DataAcquisitionServices.AsParallel Select ServiceInstance.Tick(RTime)).ToArray.Sum
            If Not Me._SuppressPeriodicMessage AndAlso ServiceRunningLQuery < Me.DataAcquisitionServices.Count Then
                Call Console.WriteLine("-------------------------------------------------------------------------" & vbCrLf)
            End If
        End Sub

        ''' <summary>
        ''' 关闭数据采集服务，断开与数据存储服务的连接并将数据写入文件系统之中
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CloseStorageService()
            Call Me.SystemLogging.WriteLine(String.Format("[{0}] Disconnect and close the datastorage service...", Kernel.RuntimeTicks), "GCModeller -> DataAcquisition_Service")

            If Me.StorageServiceType = DataStorageServiceTypes.CSV Then
                Dim ServiceInstanceCloseLQuery = (From Serviceinstance As IDataAcquisitionService In Me.DataAcquisitionServices.AsParallel Select Serviceinstance.Close()).ToArray
            Else
                Dim ServiceInstanceCloseLQuery = (From Serviceinstance As IDataAcquisitionService In Me.DataAcquisitionServices Select Serviceinstance.Close()).ToArray
            End If

            If Not DumpDataTrigger.Invalid Then Call DumpDataTrigger.WriteDumpData()
        End Sub
    End Class
End Namespace
