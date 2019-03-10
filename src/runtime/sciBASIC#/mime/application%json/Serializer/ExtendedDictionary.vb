﻿#Region "Microsoft.VisualBasic::e28bd4de63f763a8df5c94606a903d68, mime\application%json\Serializer\ExtendedDictionary.vb"

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

    ' Module ExtendedDictionary
    ' 
    '     Function: __getSpecificProperties, GetExtendedJson, LoadExtendedJson
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.json.Parser
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module ExtendedDictionary

    Public Function LoadExtendedJson(Of V, T As Dictionary(Of String, V))(json$) As T
        Dim model As JsonObject = DirectCast(ParseJson(json$), JsonObject) ' 因为所需要反序列化的对象是一个字典的继承对象，所以这里得到的一定是字典对象
        Dim type As Type = GetType(T)
        Dim obj As Object = Activator.CreateInstance(type)
        Dim defines = type.__getSpecificProperties(PropertyAccess.Writeable)

        For Each key$ In defines.Keys
            If model.ContainsKey(key$) Then
                Dim o As JsonElement = model(key)
                Dim j$ = o.BuildJsonString
                Dim entry As PropertyInfo = defines(key$)
                Dim value = LoadObject(j$, entry.PropertyType)

                Call entry.SetValue(obj, value)
                Call model.Remove(key)
            End If
        Next

        ' 剩下的元素都是字典的
        Dim out As T = DirectCast(obj, T)
        type = GetType(V)
        For Each key In model
            Dim j As String = key.Value.BuildJsonString
            Dim value As V = DirectCast(LoadObject(j$, type,), V)
            Call out.Add(key.Name, value)
        Next

        Return out
    End Function

    ''' <summary>
    ''' 得到除去字典以外的所有继承类的可写、可读属性
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    <Extension>
    Private Function __getSpecificProperties(type As Type, acc As PropertyAccess) As Dictionary(Of String, PropertyInfo)
        Dim defines = DataFramework.Schema(type, acc,, True)
        Call defines.Remove("Keys")   ' 忽略掉系统字典对象的自有属性
        Call defines.Remove("Values")
        Call defines.Remove("Comparer")
        Call defines.Remove("Count")
        Return defines
    End Function

    ''' <summary>
    ''' 对继承自字典对象的Class类型进行序列化处理
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="V"></typeparam>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1. 首先序列化字典本身
    ''' 2. 然后添加属性
    ''' </remarks>
    Public Function GetExtendedJson(Of V, T As Dictionary(Of String, V))(obj As T, Optional indent As Boolean = False) As String
        Dim br As New JsonObject

        For Each key$ In obj.Keys
            Call br.Add(key, obj(key$).GetJson)
        Next

        Dim defines As Dictionary(Of String, PropertyInfo) =
            obj _
            .GetType _
            .__getSpecificProperties(PropertyAccess.Readable)

        For Each key$ In defines.Keys
            Dim o = defines(key$).GetValue(obj)
            Dim value$ = If(
                o Is Nothing,
                "null",
                JsonContract.GetObjectJson(o, o.GetType, False))

            Call br.Add(key, value)
        Next

        Dim json$ = br.BuildJsonString
        If indent Then
            json = Formatter.Format(json:=json$)
        End If

        Return json
    End Function
End Module
