#Region "Microsoft.VisualBasic::c7b56e0c5dae8f4dbf470acf1c55ce72, GCModeller\PlugIns\ModuleRegistry.vb"

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

    '     Class ModuleRegistry
    ' 
    '         Properties: Modules
    ' 
    '         Function: GetModule, Load, LoadModule, Registry, Save
    '                   UnRegistry
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace PlugIns

    ''' <summary>
    ''' The registry object for the externel system module assembly.(系统外部模块的注册表对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ModuleRegistry : Inherits ITextFile

        Public Shared ReadOnly XmlFile As String = My.Application.Info.DirectoryPath & "/___EXTERNAL_MODULES.xml"

        ''' <summary>
        ''' 这个对象记录着在当前模块注册表文件之中所注册的外部系统模块
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Modules As List(Of KeyValuePair)

        ''' <summary>
        ''' 注册一个外部的系统模块
        ''' </summary>
        ''' <param name="AssemblyPath">目标系统外部模块文件的文件路径</param>
        ''' <remarks></remarks>
        Public Function Registry(AssemblyPath As String) As PlugIns.ISystemFrameworkEntry
            Dim ModuleEntry As PlugIns.ISystemFrameworkEntry = LoadModule(AssemblyPath)
            If ModuleEntry Is Nothing Then
                Return Nothing
            Else
                Dim RegistryItem As New KeyValuePair With {
                        .Key = ModuleEntry.ModelName, .Value = AssemblyPath}
                Dim LQuery = From Item In Modules Where String.Equals(RegistryItem.Key, Item.Key) Select Item  '
                If LQuery.ToArray.Count = 0 Then
                    Call Modules.Add(RegistryItem)
                Else
                    LQuery.First.Value = AssemblyPath
                End If

                Return ModuleEntry
            End If
        End Function

        Public Function UnRegistry(AssemblyPath As String) As Boolean
            Dim LQuery = From Item In Modules Where String.Equals(IO.Path.GetFullPath(AssemblyPath), IO.Path.GetFullPath(Item.Value)) Select Item '
            Dim Result = LQuery.ToArray
            If Result.Count > 0 Then
                Call Modules.Remove(Result.First)
            End If
            Return True
        End Function

        ''' <summary>
        ''' 获取一个已经在系统内注册的外部模块的文件路径
        ''' </summary>
        ''' <param name="ModuleName">目标外部模块的模块名称或者文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModule(ModuleName As String) As String
            If FileIO.FileSystem.FileExists(ModuleName) Then
                Return ModuleName
            End If
            Dim LQuery = From Item In Modules Where String.Equals(ModuleName, Item.Key) Select Item '
            Dim Result = LQuery.ToArray
            If Result.IsNullOrEmpty Then
                Return ""
            Else
                Return Result.First.Value
            End If
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(getPath(FilePath), Encoding)
        End Function

        ''' <summary>
        ''' 加载一个注册表文件
        ''' </summary>
        ''' <param name="File"></param>
        ''' <returns>返回读取得到的一个注册表文件</returns>
        ''' <remarks></remarks>
        Public Shared Function Load(File As String) As ModuleRegistry
            If Not FileIO.FileSystem.FileExists(File) Then
                Return New ModuleRegistry With {
                    .FilePath = File,
                    .Modules = New List(Of KeyValuePair)
                }
            End If
            Dim Registry = File.LoadXml(Of ModuleRegistry)()
            Registry.FilePath = File
            Return Registry
        End Function

        ''' <summary>
        ''' 分析一个外部的系统模块编译文档文件
        ''' </summary>
        ''' <param name="AssemblyPath">目标外部模块的文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadModule(AssemblyPath As String) As PlugIns.ISystemFrameworkEntry
            If Not FileIO.FileSystem.FileExists(AssemblyPath) Then 'When the filesystem object can not find the assembly file, then this loading operation was abort.
                Return Nothing
            Else
                AssemblyPath = IO.Path.GetFullPath(AssemblyPath) 'Assembly.LoadFile required full path of a program assembly file.
            End If
            Return ModuleLoader.LoadMainModule(AssemblyPath)
        End Function
    End Class
End Namespace
