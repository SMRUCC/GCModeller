Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language

Public Module Writer

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

                Call bufs.Add(String.Format("{0}: {1}", [property].field._Name, value.ToString))
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
                        Let Name As String = [property].field._Name
                        Select String.Format("{0}: {1}", Name, value)
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
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToLines(Of T As Class)(target As T) As String()
        Return target.ToLines(LoadClassSchema(Of T)())
    End Function
End Module
