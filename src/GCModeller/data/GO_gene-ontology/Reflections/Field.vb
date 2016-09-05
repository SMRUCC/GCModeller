#Region "Microsoft.VisualBasic::846fb53df83e41ad13bb26a50fb0c90d, ..\GCModeller\data\GO_gene-ontology\Reflections\Field.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports binding = System.Collections.Generic.KeyValuePair(Of SMRUCC.genomics.Data.GeneOntology.Field, System.Reflection.PropertyInfo)

<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
Public Class Field : Inherits Attribute

    Dim _Name As String, _toLower As Boolean

    Public ReadOnly Property Index As Integer
    Public Shared ReadOnly Property TypeInfo As Type = GetType(Field)

    Sub New(Optional Name As String = "", Optional toLower As Boolean = True)
        Me._Name = Name
        Me._toLower = toLower
    End Sub

    Sub New(Index As Integer)
        _Index = Index
    End Sub

    Public Shared Function LoadData(Of T As Class)(strValue As String()) As T
        Dim Schema As Dictionary(Of Field, PropertyInfo) = LoadClassSchema(Of T)()
        Dim DictModel As Dictionary(Of String, String()) = __createModel(strValue)
        Dim Target As T = Activator.CreateInstance(Of T)()

        For Each Field As KeyValuePair(Of Field, PropertyInfo) In Schema
            Dim EntryName As String = Field.Key._Name

            If Not DictModel.ContainsKey(EntryName) Then
                Continue For
            End If

            If Field.Value.PropertyType = GetType(String) Then
                Call Field.Value.SetValue(Target, DictModel(EntryName).First)
            Else
                Call Field.Value.SetValue(Target, DictModel(EntryName))
            End If
        Next

        Return Target
    End Function

    Const TAG As String = ".+?: "

    Private Shared Function __createModel(strValue As String()) As Dictionary(Of String, String())
        Dim LQuery = From strLine As String
                     In strValue
                     Let TagName As String = Regex.Match(strLine, TAG).Value
                     Where Not String.IsNullOrEmpty(TagName)
                     Select DataEntry = InternalGetValue(strLine, TagName)
                     Group DataEntry By DataEntry.Key Into Group

        Return LQuery.ToDictionary(Function(x) x.Key,
                                   Function(x) x.Group.ToArray(Function(value) value.Value))
    End Function

    Private Shared Function InternalGetValue(strLine As String, TagName As String) As KeyValuePair(Of String, String)
        Dim Value As String = strLine.Replace(TagName, "")
        Dim Key As String = TagName.Replace(": ", "")
        Return New KeyValuePair(Of String, String)(Key, Value)
    End Function

    Public Shared Function LoadClassSchema(Of T As Class)() As Dictionary(Of Field, PropertyInfo)
        Dim type As TypeInfo = GetType(T)
        Dim Properties = type.GetProperties(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public)
        Dim LQuery As binding() = LinqAPI.Exec(Of binding) <=
 _
            From [property] As PropertyInfo
            In Properties
            Let attrs As Object() = [property].GetCustomAttributes(
                attributeType:=Field.TypeInfo,
                inherit:=True)
            Where Not attrs.IsNullOrEmpty AndAlso
                DataFramework.IsPrimitive([property].PropertyType)
            Select New KeyValuePair(Of Field, PropertyInfo)(
                DirectCast(attrs.First, Field), [property])

        If LQuery.IsNullOrEmpty Then
            Return Nothing
        Else
            Dim schema As New Dictionary(Of Field, Reflection.PropertyInfo)

            For Each Line In LQuery
                If String.IsNullOrEmpty(Line.Key._Name) Then
                    Line.Key._Name = If(Line.Key._toLower, Line.Value.Name.ToLower, Line.Value.Name)
                End If
                Call schema.Add(Line.Key, Line.Value)
            Next

            Return schema
        End If
    End Function

    Public Shared Function GenerateValueCollection(Of T As Class)(target As T) As String()
        Dim typeInfo As Type = GetType(T)
        Dim Schema = LoadClassSchema(Of T)()
        Dim bufs As New List(Of String)

        For Each [property] As binding In Schema

            If [property].Value.PropertyType = GetType(String) Then
                Dim value As Object = [property].Value.GetValue(target)
                If value Is Nothing Then
                    Continue For
                End If
                Call bufs.Add(String.Format("{0}: {1}", [property].Key._Name, value.ToString))
            Else
                Dim vals As Object() = [property].Value.GetValue(target)

                If vals.IsNullOrEmpty Then
                    Continue For
                End If

                Dim pvalue = From o As Object
                             In vals
                             Let strValue As String = o.ToString
                             Select strValue

                bufs += From value As String
                        In pvalue
                        Select String.Format("{0}: {1}", [property].Key._Name, value)
            End If
        Next

        Return bufs.ToArray
    End Function
End Class
