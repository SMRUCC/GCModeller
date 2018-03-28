Imports System.Reflection
Imports Newtonsoft.Json

''' <summary>
''' 
''' </summary>
''' <remarks>
''' https://stackoverflow.com/questions/11934487/custom-json-serialization-of-class
''' </remarks>
Public Class LambdaWriter : Inherits JsonConverter

    Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
        If value Is Nothing Then
            Call serializer.Serialize(writer, Nothing)
        End If

        Dim lambda$ = Nothing
        Dim properties = value _
            .GetType _
            .GetProperties _
            .Where(Function(p)
                       Return p.PropertyType Is GetType(Lambda)
                   End Function)

        Call writer.WriteStartObject()

        For Each [property] As PropertyInfo In properties
            ' write property name
            Call writer.WritePropertyName([property].Name)
            ' let the serializer serialize the value itself
            ' (so this converter will work with any other type, Not just int)
            Call lambda.SetValue(TryCast([property].GetValue(value, Nothing), Lambda)?.ToString())
            Call serializer.Serialize(writer, lambda)
        Next

        Call writer.WriteEndObject()
    End Sub

    Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
        Throw New NotImplementedException()
    End Function

    Public Overrides Function CanConvert(objectType As Type) As Boolean
        If objectType Is GetType(Lambda) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class

Public Class Lambda

    Public Property args As String()
    Public Property [function] As String

    Public Overrides Function ToString() As String
        Return $"function({args.JoinBy(", ")}) {{
    {[function]}
}}"
    End Function
End Class