#Region "Microsoft.VisualBasic::6aa4df9d463fae8bc389098c89cd129c, ..\GCModeller\foundation\OBO_Foundry\ParserIO.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module ParserIO

    ''' <summary>
    ''' header or term object.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="strValue"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadData(Of T As Class)(strValue As IEnumerable(Of String)) As T
        Dim Schema As Dictionary(Of BindProperty(Of Field)) = LoadClassSchema(Of T)()
        Dim data As Dictionary(Of String, String()) = __createModel(strValue.ToArray)
        Return Schema.LoadData(Of T)(data)
    End Function

#Const DEVELOPMENT = 1

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="schema">对象的定义</param>
    ''' <param name="data">从文件之中读取出来的一段数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadData(Of T As Class)(schema As Dictionary(Of BindProperty(Of Field)), data As Dictionary(Of String, String())) As T
        Dim o As T = Activator.CreateInstance(Of T)()

        For Each f As BindProperty(Of Field) In schema.Values
            Dim name As String = f.Field._Name

            If Not data.ContainsKey(name) Then  ' Class之中有定义但是文件之中没有数据，这个是正常现象，则跳过
                Continue For
            End If

            Dim array$() = data(name)

            If f.Type = GetType(String) Then
#If DEVELOPMENT Then
                If array.Length > 1 Then
                    Throw New InvalidCastException(name & ": " & TypeMissMatch1)   ' Class之中定义为字符串，但是文件之中是数组，则定义出错了
                End If
#End If
                Call f.SetValue(o, array$(Scan0%))
            Else
                Call f.SetValue(o, array$)
            End If
        Next

#If DEVELOPMENT Then
        Dim names As String() = schema.Values.Select(Function(x) x.Field._Name).ToArray

        For Each key$ In data.Keys
            If Array.IndexOf(names, key$) = -1 Then
                ' 文件之中定义有的但是Class之中没有被定义
                Dim array$() = data(key$)
                Dim type$ = If(array.Length = 1, GetType(String), GetType(String())).ToString
                Dim msg$ =
                    $"Missing property definition in the object: <Field(""{key$}"")>Public Property {key} As {type$}"

                Throw New Exception(msg)
            End If
        Next
#End If

        Return o
    End Function

    Const TypeMissMatch1 As String = "The type of the property is ""System.String"", but the data from file is ""Array(Of System.String)""!"
    Const TAG As String = ".+?: "

    ''' <summary>
    ''' Parsing a term object as data model
    ''' </summary>
    ''' <param name="strValue"></param>
    ''' <returns></returns>
    Private Function __createModel(strValue As String()) As Dictionary(Of String, String())
        Dim LQuery = From strLine As String
                     In strValue
                     Let x = strLine.GetTagValue(": ")
                     Where Not String.IsNullOrEmpty(x.Name)
                     Select x
                     Group x By x.Name Into Group

        Return LQuery.ToDictionary(Function(x) x.Name,
                                   Function(x) x.Group.Select(Function(value) value.Value).ToArray)
    End Function

    ''' <summary>
    ''' Parsing the object fields template in the obo files
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function LoadClassSchema(Of T As Class)() As Dictionary(Of BindProperty(Of Field))
        Dim type As TypeInfo = GetType(T)
        Dim Properties = type.GetProperties(BindingFlags.Instance Or BindingFlags.Public)
        Dim LQuery = LinqAPI.Exec(Of BindProperty(Of Field)) <=
 _
            From [property] As PropertyInfo
            In Properties
            Let attrs As Object() = [property].GetCustomAttributes(
                attributeType:=Field.TypeInfo,
                inherit:=True)
            Where Not attrs.IsNullOrEmpty AndAlso
                DataFramework.IsPrimitive([property].PropertyType) OrElse
                [property].PropertyType = GetType(String())
            Select New BindProperty(Of Field)(DirectCast(attrs.First, Field), [property])

        If LQuery.IsNullOrEmpty Then Return Nothing

        Dim schema As New Dictionary(Of BindProperty(Of Field))

        For Each f As BindProperty(Of Field) In LQuery
            If String.IsNullOrEmpty(f.Field._Name) Then
                f.Field._Name = If(f.Field._toLower, f.Identity.ToLower, f.Identity)
            End If
            Call schema.Add(f)
        Next

        Return schema
    End Function

    ''' <summary>
    ''' For generates the obo document and save data model into the file system.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="target"></param>
    ''' <param name="schema"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ToLines(Of T As Class)(target As T, schema As Dictionary(Of BindProperty(Of Field))) As String()
        Dim bufs As New List(Of String)

        For Each [property] As BindProperty(Of Field) In schema.Values
            If [property].Type = GetType(String) Then
                Dim value As Object = [property].GetValue(target)
                If value Is Nothing Then
                    Continue For
                End If
                Call bufs.Add(String.Format("{0}: {1}", [property].Field._Name, value.ToString))
            Else
                Dim vals As Object() = [property].GetValue(target)

                If vals.IsNullOrEmpty Then
                    Continue For
                End If

                Dim pvalue = From o As Object
                             In vals
                             Let strValue As String = o.ToString
                             Select strValue

                bufs += From value As String
                        In pvalue
                        Select String.Format("{0}: {1}", [property].Field._Name, value)
            End If
        Next

        Return bufs.ToArray
    End Function

    ''' <summary>
    ''' For generates the obo document and save data model into the file system.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="target"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ToLines(Of T As Class)(target As T) As String()
        Return target.ToLines(LoadClassSchema(Of T)())
    End Function
End Module
