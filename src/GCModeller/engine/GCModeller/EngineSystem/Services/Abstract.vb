#Region "Microsoft.VisualBasic::bd658ae83718360b59e43bbcd8b26a68, engine\GCModeller\EngineSystem\Services\Abstract.vb"

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

    '     Interface IDataAcquisitionService
    ' 
    '         Properties: TableName
    ' 
    '         Function: Close, Connect, GetDataChunk, GetDefinitions, Initialize
    '                   Tick
    ' 
    '         Sub: GenerateDefinition, SetUpCommitInterval
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer

Namespace EngineSystem.Services.MySQL

    ''' <summary>
    ''' 子系统模块之中的数据采集服务对象类型的抽象接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IDataAcquisitionService

        ''' <summary>
        ''' 本数据采集服务所映射到的数据库中的表或者CSV文件名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property TableName As String
        ''' <summary>
        ''' 本数据采集服务对象实例连接至数据存储服务
        ''' </summary>
        ''' <param name="StorageService"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Connect(StorageService As DataSerializer) As Integer
        ''' <summary>
        ''' 执行一次数据采集操作
        ''' </summary>
        ''' <param name="RTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Tick(RTime As Integer) As Integer
        ''' <summary>
        ''' Disconnect the connection between the data acquisition service and data storage service and then write the cache data into the filesystem.
        ''' (数据采集服务关闭与数据存储服务之间的连接并将缓冲区中的数据写入磁盘文件之中)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Close() As Integer
        Sub GenerateDefinition()
        ''' <summary>
        ''' Initialize the data acquisition service module.(数据采集服务模块执行初始化操作)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialize() As Integer

        Sub SetUpCommitInterval(Interval As Integer)

        Function GetDataChunk() As DataFlowF()
        Function GetDefinitions() As HandleF()
    End Interface
End Namespace
