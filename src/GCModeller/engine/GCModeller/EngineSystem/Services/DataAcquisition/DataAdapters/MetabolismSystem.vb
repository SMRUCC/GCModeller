#Region "Microsoft.VisualBasic::a8c054a4b4478ff5cda8058ff52ae9f8, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\MetabolismSystem.vb"

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

    '     Class MetabolismSystem
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    '     Class EnzymeActivity
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    '     Class ConstraintsMetabolite
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    ''' <summary>
    ''' (数据采集服务获取代谢组中的每一种代谢物在每一次迭代计算之后的数量)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MetabolismSystem : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "metabolism"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return System.get_DataSerializerHandles
        End Function
    End Class

    ''' <summary>
    ''' 统计酶动力学活性的数据接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnzymeActivity : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
        Implements IDataAdapter

        Sub New(System As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(System)
        End Sub

        ''' <summary>
        ''' 主要记录的是酶浓度，PH值和温度所带来的效应
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DataSource() As DataSource()
            Return System.EnzymeActivitiesDataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return System.EnzymeActivitySerialHandles
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "EnzymeActivity"
            End Get
        End Property
    End Class

    Public Class ConstraintsMetabolite : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
        Implements IDataAdapter

        Sub New(System As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From EntityCompound In System.ConstraintMetabolite Let value = EntityCompound.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From EntityCompound In System.ConstraintMetabolite Select EntityCompound.SerialsHandle).ToArray
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "ConstraintsMetabolites"
            End Get
        End Property
    End Class
End Namespace
