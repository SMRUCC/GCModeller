#Region "Microsoft.VisualBasic::0b1186e16cd75878671b561cb9124378, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAcquisitionService\DataAcquisition.vb"

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

    '     Class DataAdapter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports SMRUCC.genomics.GCModeller.Framework
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver

Namespace EngineSystem.Services.DataAcquisition

    ''' <summary>
    ''' 子系统模块的数据输出至数据采集服务的数据转接器
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class DataAdapter(Of TSystem As IDataSource)
        Inherits Kernel_Driver.DataAdapter(Of Double, EntityQuantitySample)
        Implements IDataAdapter

        Public MustOverride Function DataSource() As DataSource() Implements IDataAdapter.DataSource

        ''' <summary>
        '''  本数据采集对象所采集的目标模块对象
        ''' </summary>
        ''' <remarks></remarks>
        Protected System As TSystem

        Sub New(DataSourceSystem As TSystem)
            Me.System = DataSourceSystem
        End Sub

        Public MustOverride Function DefHandles() As HandleF() Implements IDataAdapter.DefHandles

        Public MustOverride ReadOnly Property TableName As String Implements IDataAdapter.TableName

        Public Overrides Function ToString() As String
            Return TableName
        End Function
    End Class
End Namespace
