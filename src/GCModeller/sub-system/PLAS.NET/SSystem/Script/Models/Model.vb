#Region "Microsoft.VisualBasic::7657be5b43582b8bbd2183fbbaa3875b, sub-system\PLAS.NET\SSystem\Script\Models\Model.vb"

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

    '     Class Model
    ' 
    '         Properties: [Constant], Comment, Experiments, FinalTime, sEquations
    '                     Summary, Title, UserFunc, Vars
    ' 
    '         Function: FindObject, Load, ToString
    ' 
    '         Sub: Add
    ' 
    '         Operators: +
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace Script

    ''' <summary>
    ''' 可以被保存至文件的脚本模型对象
    ''' </summary>
    Public Class Model : Inherits ModelBaseType

        ''' <summary>
        ''' The user define function.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property UserFunc As [Function]()
        ''' <summary>
        ''' 假若在脚本里面，常数值是表达式，则求值的顺序会是从上到下
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property [Constant] As NamedValue(Of String)()

        Dim __varHash As Dictionary(Of var)

        ''' <summary>
        ''' A collection of the system variables.
        ''' (系统中的运行变量的集合)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property Vars As var()
            Get
                If __varHash Is Nothing Then
                    Return New var() {}
                Else
                    Return __varHash.Values.ToArray
                End If
            End Get
            Set(value As var())
                If value Is Nothing Then
                    __varHash = New Dictionary(Of var)
                Else
                    __varHash = value.ToDictionary
                End If
            End Set
        End Property

        ''' <summary>
        ''' The data channel in this system kernel.
        ''' (系统中的反应过程数据通道)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property sEquations As SEquation()
        ''' <summary>
        ''' The disturbing factors in this system.
        ''' (系统中的干扰因素的集合)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property Experiments As Experiment()

        Public Property Title As String
        Public Property Comment As String

        ''' <summary>
        ''' The ticks count value of the time that exit this simulation.
        ''' (整个内核运行的退出时间) 
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Property FinalTime As Integer

        <ScriptIgnore> Public ReadOnly Property Summary As String
            Get
                Return $"{Vars.Length} Substrates and {sEquations.Length} reaction channels."
            End Get
        End Property

        Public Sub Add(x As var)
            Call __varHash.Add(x)
        End Sub

        Public Function FindObject(x As String) As var
            Return __varHash(x)
        End Function

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Comment) Then
                Return Title
            Else
                Return String.Format("{0}; //{1}", Title, Comment)
            End If
        End Function

        ''' <summary>
        ''' Load a model from a compiled xml model file.
        ''' (从一个已经编译好的XML文件加载)
        ''' </summary>
        ''' <param name="Path">The target compiled xml model file.(目标已经编译好的XML模型文件)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(Path As String) As Model
            Return Path.LoadXml(Of Model)()
        End Function

        ''' <summary>
        ''' Load from a script file.
        ''' (从一个脚本源文件中获取模型数据)
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Widening Operator CType(Path As String) As Model
            Return ScriptCompiler.Compile(Path)
        End Operator

        Public Shared Operator +(model As Model, x As var) As Model
            Call model.Add(x)
            Return model
        End Operator
    End Class
End Namespace
