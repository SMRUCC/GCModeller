#Region "Microsoft.VisualBasic::587815e33b55230ec537f56b46b4f170, sub-system\PLAS.NET\SSystem\Script\Models\Model.vb"

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

'   Total Lines: 125
'    Code Lines: 65 (52.00%)
' Comment Lines: 45 (36.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 15 (12.00%)
'     File Size: 4.36 KB


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

        Dim __symbols As Dictionary(Of var)

        ''' <summary>
        ''' A collection of the system variables.
        ''' (系统中的运行变量的集合)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property Vars As var()
            Get
                If __symbols Is Nothing Then
                    Return New var() {}
                Else
                    Return __symbols.Values.ToArray
                End If
            End Get
            Set(value As var())
                If value Is Nothing Then
                    __symbols = New Dictionary(Of var)
                Else
                    __symbols = New Dictionary(Of var)

                    For Each x As var In value
                        If String.IsNullOrEmpty(x.Id) Then
                            Continue For
                        End If
                        If __symbols.ContainsKey(x.Id) Then
                            ' 重复的申明不应该被静默的覆盖掉，这里给出一条可见的诊断信息
                            Call Console.WriteLine($"[WARN] Duplicated initial value declaration of the variable ""{x.Id}"", the last one was applied.")
                            __symbols(x.Id) = x
                        Else
                            Call __symbols.Add(x)
                        End If
                    Next
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
        ''' (整个内核运行的退出时间，脚本之中的 FINALTIME 允许为浮点数值，
        ''' 故这里必须为双精度浮点数，否则会出现 4.5 被舍入为整数的问题) 
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Property FinalTime As Double

        <ScriptIgnore> Public ReadOnly Property Summary As String
            Get
                Return $"{Vars.Length} Substrates and {sEquations.Length} reaction channels."
            End Get
        End Property

        Public Sub Add(x As var)
            Call __symbols.Add(x)
        End Sub

        Public Function FindObject(x As String) As var
            Return __symbols(x)
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
