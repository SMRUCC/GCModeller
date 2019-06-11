Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection
Imports Field = SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection.Field

Namespace IO

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
            Dim name$
            Dim value As Object
            Dim vals As Object()

            For Each [property] As BindProperty(Of Field) In schema.Values
                If [property].Type Is GetType(String) Then
                    name = [property].field.name
                    value = [property].GetValue(target)

                    If value Is Nothing Then
                        Continue For
                    End If

                    bufs += String.Format("{0}: {1}", name, value.ToString)
                Else
                    vals = [property].GetValue(target)

                    If vals.IsNullOrEmpty Then
                        Continue For
                    End If

                    Dim pvalue = From o As Object
                                 In vals
                                 Let str As String = Scripting.ToString(o)
                                 Select str

                    bufs += From val As String
                            In pvalue
                            Let pname As String = [property].field.name
                            Select String.Format("{0}: {1}", pname, val)
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
            Return target.ToLines(Reflector.LoadClassSchema(Of T)())
        End Function
    End Module
End Namespace