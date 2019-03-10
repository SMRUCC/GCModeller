﻿#Region "Microsoft.VisualBasic::5e9cfd8c559238192f2334433e86acaa, Microsoft.VisualBasic.Core\Serialization\JSON\JsonSerialization.vb"

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

    '     Module JsonContract
    ' 
    '         Function: EnsureDate, GetJson, GetObjectJson, LoadJSON, LoadJsonFile
    '                   LoadJSONObject, (+2 Overloads) LoadObject, MatrixJson, RemoveJsonNullItems, WriteLargeJson
    ' 
    '         Sub: writeJsonInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Factory = System.Runtime.Serialization.Json.DataContractJsonSerializer
Imports r = System.Text.RegularExpressions.Regex

Namespace Serialization.JSON

    ''' <summary>
    ''' Only works on the Public visible type.
    ''' (使用.NET系统环境之中自带的框架进行JSON序列化和反序列化)
    ''' </summary>
    <Package("Json.Contract")> Public Module JsonContract

        <Extension>
        Public Function MatrixJson(matrix As Double()()) As String
            Dim rows = matrix.Select(Function(row) $"[ {row.JoinBy(", ")} ]")
            Dim json = $"[ {rows.JoinBy("," & ASCII.LF)} ]"
            Return json
        End Function

        ''' <summary>
        ''' 使用<see cref="ScriptIgnoreAttribute"/>来屏蔽掉不想序列化的属性
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        <ExportAPI("Get.Json")>
        <Extension>
        Public Function GetObjectJson(type As Type, obj As Object,
                                      Optional indent As Boolean = True,
                                      Optional simpleDict As Boolean = True,
                                      Optional knownTypes As IEnumerable(Of Type) = Nothing) As String

            Using ms As New MemoryStream()
                Call ms.writeJsonInternal(
                    obj:=obj,
                    type:=type,
                    simpleDict:=simpleDict,
                    knownTypes:=knownTypes
                )

                If indent Then
                    Return Formatter.Format(Encoding.UTF8.GetString(ms.ToArray()))
                Else
                    Return Encoding.UTF8.GetString(ms.ToArray())
                End If
            End Using
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Sub writeJsonInternal(output As Stream, obj As Object, type As Type, simpleDict As Boolean, knownTypes As IEnumerable(Of Type))
            If simpleDict Then
                Dim settings As New DataContractJsonSerializerSettings With {
                    .UseSimpleDictionaryFormat = True,
                    .SerializeReadOnlyTypes = True,
                    .KnownTypes = knownTypes _
                        .SafeQuery _
                        .ToArray
                }
                Call New Factory(type, settings).WriteObject(output, obj)
            Else
                Call New Factory(type).WriteObject(output, obj)
            End If
        End Sub

        ''' <summary>
        ''' 将目标对象保存为json文件
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteLargeJson(Of T)(obj As T, path$, Optional simpleDict As Boolean = True) As Boolean
            Using ms As FileStream = path.Open(, doClear:=True)
                Call ms.writeJsonInternal(obj, GetType(T), simpleDict, Nothing)
            End Using

            Return True
        End Function

        ''' <summary>
        ''' 有些javascript程序(例如highcharts.js)要求json里面不可以出现null的属性，可以使用这个方法进行移除
        ''' </summary>
        ''' <param name="json"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function RemoveJsonNullItems(json As String) As String
            Return r.Replace(json, """[^""]+""[:]\s*null\s*,?", "", RegexICSng)
        End Function

        ''' <summary>
        ''' Gets the json text value of the target object, the attribute <see cref="ScriptIgnoreAttribute"/> 
        ''' can be used for block the property which is will not serialize to the text.
        ''' (使用<see cref="ScriptIgnoreAttribute"/>来屏蔽掉不想序列化的属性)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 2016-11-9 对字典进行序列化的时候，假若对象类型是从字典类型继承而来的，则新的附加属性并不会被序列化，只会序列化字典本身
        ''' 2018-10-5 不可以序列化匿名类型
        ''' </remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function GetJson(Of T)(obj As T,
                                                  Optional indent As Boolean = False,
                                                  Optional simpleDict As Boolean = True,
                                                  Optional knownTypes As IEnumerable(Of Type) = Nothing) As String
            Return GetType(T).GetObjectJson(obj, indent, simpleDict, knownTypes)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="json">null -> Nothing</param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        <ExportAPI("LoadObject")>
        <Extension>
        Public Function LoadObject(json$,
                                   type As Type,
                                   Optional simpleDict As Boolean = True,
                                   Optional throwEx As Boolean = True,
                                   Optional ByRef exception As Exception = Nothing) As Object

            If String.Equals(json, "null", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            ElseIf json.StringEmpty Then
                Throw New NullReferenceException("Empty json text!")
            End If

            Using MS As New MemoryStream(Encoding.UTF8.GetBytes(json))
                Dim settings As New DataContractJsonSerializerSettings With {
                    .UseSimpleDictionaryFormat = simpleDict,
                    .SerializeReadOnlyTypes = True
                }
                Dim ser As New DataContractJsonSerializer(type, settings)
                Dim de As Func(Of Object) = Function() ser.ReadObject(MS)
                Dim obj = TryCatch(de, $"Incorrect JSON string format => >>>{json}<<<", throwEx, exception)
                Return obj
            End Using
        End Function

        <Extension>
        Public Function LoadJSONObject(jsonStream As Stream, type As Type, Optional simpleDict As Boolean = True) As Object
            If jsonStream Is Nothing Then
                Return Nothing
            Else
                Dim settings As New DataContractJsonSerializerSettings With {
                    .UseSimpleDictionaryFormat = simpleDict,
                    .SerializeReadOnlyTypes = True
                }
                Return New DataContractJsonSerializer(type, settings) _
                    .ReadObject(jsonStream)
            End If
        End Function

        ''' <summary>
        ''' 从文本文件或者文本内容之中进行JSON反序列化
        ''' </summary>
        ''' <param name="json">This string value can be json text or json file path.</param>
        <Extension> Public Function LoadJSON(Of T)(json$,
                                                   Optional simpleDict As Boolean = True,
                                                   Optional throwEx As Boolean = True,
                                                   Optional ByRef exception As Exception = Nothing) As T
            Dim text$ = json.SolveStream(Encodings.UTF8)
            Dim value As Object = text.LoadObject(GetType(T), simpleDict, throwEx, exception)
            Dim obj As T = DirectCast(value, T)
            Return obj
        End Function

        ''' <summary>
        ''' XML CDATA to json
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="json"></param>
        ''' <param name="simpleDict"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function LoadObject(Of T As New)(json As XElement, Optional simpleDict As Boolean = True) As T
            Return json.Value.LoadJSON(Of T)(simpleDict:=simpleDict)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function LoadJsonFile(Of T)(file$, Optional encoding As Encoding = Nothing, Optional simpleDict As Boolean = True) As T
            Return (file.ReadAllText(encoding Or UTF8, throwEx:=False, suppress:=True) Or "null".AsDefault) _
                .LoadJSON(Of T)(simpleDict)
        End Function

        Const JsonLongTime$ = "\d+-\d+-\d+T\d+:\d+:\d+\.\d+"

        Public Function EnsureDate(json$, Optional propertyName$ = Nothing) As String
            Dim pattern$ = $"""{JsonLongTime}"""

            If Not propertyName.StringEmpty Then
                pattern = $"""{propertyName}""\s*:\s*" & pattern
            End If

            Dim dates = r.Matches(json, pattern, RegexICSng)
            Dim sb As New StringBuilder(json)
            Dim [date] As Date

            For Each m As Match In dates
                Dim s$ = m.Value

                If Not propertyName.StringEmpty Then
                    With r.Replace(s, $"""{propertyName}""\s*:", "", RegexICSng) _
                        .Trim _
                        .Trim(ASCII.Quot)

                        [date] = Date.Parse(.ByRef)
                    End With
                    sb.Replace(s, $"""{propertyName}"":" & [date].GetJson)
                Else
                    [date] = Date.Parse(s.Trim(ASCII.Quot))
                    sb.Replace(s, [date].GetJson)
                End If
            Next

            Return sb.ToString
        End Function
    End Module
End Namespace
