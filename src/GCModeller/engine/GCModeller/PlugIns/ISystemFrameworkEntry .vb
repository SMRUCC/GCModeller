#Region "Microsoft.VisualBasic::cc1fad6312b572822497acca5a2fbff9, engine\GCModeller\PlugIns\ISystemFrameworkEntry .vb"

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

    '     Class ISystemFrameworkEntry
    ' 
    ' 
    '         Interface ISystemFramework
    ' 
    '             Properties: DataSource
    ' 
    '             Function: CreateServiceInstanceSerials, Initialzie
    ' 
    '         Enum Types
    ' 
    '             CellSystem, DelegateSystem, ExpressionRegulationNetwork, Metabolism
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: Articles, Authors, Description, ISystem, ModelName
    '                 Type
    ' 
    '     Function: GetDescription, Initialize, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace PlugIns

    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
    Public Class ISystemFrameworkEntry : Inherits Attribute

        Public Interface ISystemFramework
            ReadOnly Property DataSource As DataSource()
            Function Initialzie() As Integer
            'Function Tick() As Integer

            ''' <summary>
            ''' 从本系统模块之中创建一个数据采集服务对象实例
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Function CreateServiceInstanceSerials() As IDataAcquisitionService()
        End Interface

        Public Enum Types As Integer
            ''' <summary>
            ''' 外部系统模块加载失败
            ''' </summary>
            ''' <remarks></remarks>
            BROKEN_ASSEMBLY = -1

            CellSystem
            DelegateSystem
            ExpressionRegulationNetwork
            Metabolism
        End Enum

        ''' <summary>
        ''' Name of this computational model.(这个计算模型模块的名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ModelName As String
        ''' <summary>
        ''' 这个计算模型的代码编写作者列表，建议使用分号分隔
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Authors As String
        ''' <summary>
        ''' 这个计算模型的引用文献列表，建议使用分号分隔
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Articles As String
        Public Property Type As ISystemFrameworkEntry.Types
        ''' <summary>
        ''' ISystemFramework interface type information of the external model assembly.(外部对象模型代码的类型信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ISystem As TypeInfo

        Public Property Description As String

        Public Function GetDescription() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendFormat("Model Name:        [{0}]  {1}" & vbCrLf, Type.ToString, ModelName)
            Call sBuilder.AppendFormat("EntryPoint:         {1}" & vbCrLf, Type.ToString, ISystem.FullName)
            Call sBuilder.AppendFormat("Authors:            {0}" & vbCrLf, Authors)
            Call sBuilder.AppendFormat("Reference Articles: {0}" & vbCrLf, Articles)
            Call sBuilder.AppendFormat("Description:        {0}" & vbCrLf, Description)

            Return sBuilder.ToString
        End Function

        Public Function Initialize(System As TypeInfo) As ISystemFrameworkEntry
            Me.ISystem = System
            Return Me
        End Function

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(ModelName) Then
                Return Type.ToString
            Else
                Return String.Format("[{0}] {1}", ModelName, Type.ToString)
            End If
        End Function
    End Class
End Namespace
