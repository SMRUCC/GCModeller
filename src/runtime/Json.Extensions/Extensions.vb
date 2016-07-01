Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' Provides methods for converting between common language runtime types and JSON types.
''' </summary>
<PackageNamespace("Json",
                  Url:="http://www.newtonsoft.com/json",
                  Description:="Json.NET is a popular high-performance JSON framework for .NET.<br />Provides methods for converting between common language runtime types and JSON types.",
                  Publisher:="James Newton-King")>
Public Module Extensions

    ''' <summary>
    ''' Serializes the specified object to a JSON string.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj">The object to serialize.</param>
    ''' <returns>A JSON string representation of the object.</returns>
    <Extension> Public Function Json(Of T)(obj As T) As String
        Try
            Return Newtonsoft.Json.JsonConvert.SerializeObject(obj)
        Catch ex As Exception
            Return App.LogException(ex, $"{GetType(Extensions).FullName}::{NameOf(Json)}")
        End Try
    End Function

    ''' <summary>
    ''' 加载匿名类型的数据，由于匿名类型找不到定义，所以需要使用参数来对泛型产生类型约束
    ''' </summary>
    ''' <typeparam name="T">匿名对象的类型定义，但是这个是没有太多用途的，只是为了产生泛型约束</typeparam>
    ''' <param name="json"></param>
    ''' <param name="obj">真正的匿名类型的信息是来自于函数的这个参数的</param>
    <Extension> Public Sub LoadAnonymousObject(Of T As Class)(json As String, ByRef obj As T)
        obj = json.LoadObject(Of T)
    End Sub

    ''' <summary>
    ''' Serializes the specified object to a JSON string.
    ''' </summary>
    ''' <param name="obj">The object to serialize.</param>
    ''' <returns>A JSON string representation of the object.</returns>
    <ExportAPI("Json", Info:="Serializes the specified object to a JSON string.")>
    <Extension> Public Function Json(<Parameter("obj", "The object to serialize.")> obj As Object) As <FunctionReturns("A JSON string representation of the object.")> String
        Try
            Return Newtonsoft.Json.JsonConvert.SerializeObject(obj)
        Catch ex As Exception
            Return App.LogException(ex, $"{GetType(Extensions).FullName}::{NameOf(Json)}")
        End Try
    End Function

    ''' <summary>
    ''' Create a class object from the input json data. Deserializes the JSON to the specified .NET type.
    ''' </summary>
    ''' <param name="json">The JSON to deserialize.</param>
    ''' <param name="type">The <see cref="Type"/> of object being deserialized.</param>
    ''' <returns>The deserialized object from the JSON string.</returns>
    ''' 
    <ExportAPI("Json.LoadObject", Info:="Create a class object from the input json data. Deserializes the JSON to the specified .NET type.")>
    <Extension> Public Function CreateObject(<Parameter("JSON", "The JSON to deserialize.")> json As String,
                                             <Parameter("type", "The <see cref=""Type""/> of object being deserialized.")> type As System.Type) As <FunctionReturns("The deserialized object from the JSON string.")> Object
        Try
            Return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type)
        Catch ex As Exception
            Return App.LogException(New Exception($"{NameOf(json)}:={vbCrLf}{json}", ex), $"{GetType(Extensions).FullName}::{NameOf(CreateObject)}")
        End Try
    End Function

    ''' <summary>
    ''' Json deserializes.(JSON反序列化)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="json"></param>
    ''' <returns></returns>
    <Extension> Public Function LoadObject(Of T)(json As String) As T
        Try
            Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of T)(json)
        Catch ex As Exception
            Return App.LogException(
                New Exception($"{NameOf(json)}:={vbCrLf}{json}", ex), $"{GetType(Extensions).FullName}::{NameOf(CreateObject)}")
        End Try
    End Function

End Module
