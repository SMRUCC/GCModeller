Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module ParserIO

    Public Function LoadData(Of T As Class)(strValue As String()) As T
        Dim Schema As Dictionary(Of BindProperty(Of Field)) = LoadClassSchema(Of T)()
        Dim data As Dictionary(Of String, String()) = __createModel(strValue)
        Dim o As T = Activator.CreateInstance(Of T)()

        For Each f As BindProperty(Of Field) In Schema.Values
            Dim EntryName As String = f.Field._Name

            If Not data.ContainsKey(EntryName) Then
                Continue For
            End If

            If f.Type = GetType(String) Then
                Call f.SetValue(o, data(EntryName).First)
            Else
                Call f.SetValue(o, data(EntryName))
            End If
        Next

        Return o
    End Function

    Const TAG As String = ".+?: "

    Private Function __createModel(strValue As String()) As Dictionary(Of String, String())
        Dim LQuery = From strLine As String
                     In strValue
                     Let x = strLine.GetTagValue(": ")
                     Where Not String.IsNullOrEmpty(x.Name)
                     Select x
                     Group x By x.Name Into Group

        Return LQuery.ToDictionary(Function(x) x.Name,
                                   Function(x) x.Group.ToArray(Function(value) value.x))
    End Function

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
                DataFramework.IsPrimitive([property].PropertyType)
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

    <Extension>
    Public Function ToLines(Of T As Class)(target As T) As String()
        Return target.ToLines(LoadClassSchema(Of T)())
    End Function
End Module
