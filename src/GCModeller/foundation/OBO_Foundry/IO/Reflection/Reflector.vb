Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language

Namespace IO.Reflection

    Public Module Reflector

        ReadOnly cache As New Dictionary(Of Type, Dictionary(Of BindProperty(Of Field)))

        ''' <summary>
        ''' Parsing the object fields template in the obo files
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadClassSchema(Of T As Class)() As Dictionary(Of BindProperty(Of Field))
            Return cache.ComputeIfAbsent(
                key:=GetType(T),
                lazyValue:=Function(type)
                               Return type _
                                   .GetProperties(PublicProperty) _
                                   .parseBindFields _
                                   .schemaParser
                           End Function)
        End Function

        ReadOnly stringListType As Type = GetType(String())

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function parseBindFields(properties As PropertyInfo()) As BindProperty(Of Field)()
            Return LinqAPI.Exec(Of BindProperty(Of Field)) _
 _
               () <= From [property] As PropertyInfo
                     In properties
                     Let attrs As Object() = [property].GetCustomAttributes(
                         attributeType:=GetType(Field),
                         inherit:=True
                     )
                     Let tName = [property].PropertyType
                     Where Not attrs.IsNullOrEmpty AndAlso DataFramework.IsPrimitive(tName) OrElse tName = stringListType
                     Let field = DirectCast(attrs.First, Field)
                     Select New BindProperty(Of Field)(field, [property])
        End Function

        <Extension>
        Private Function schemaParser(fields As BindProperty(Of Field)()) As Dictionary(Of BindProperty(Of Field))
            Dim schema As New Dictionary(Of BindProperty(Of Field))
            Dim field As Field
            Dim name As String

            For Each bField As BindProperty(Of Field) In fields
                field = bField.field

                If String.IsNullOrEmpty(field.name) Then
                    If field.toLower Then
                        name = bField.Identity.ToLower
                    Else
                        name = bField.Identity
                    End If
                Else
                    name = Nothing
                End If

                Call field.SetFields(name:=name)
                Call schema.Add(bField)
            Next

            Return schema
        End Function
    End Module
End Namespace