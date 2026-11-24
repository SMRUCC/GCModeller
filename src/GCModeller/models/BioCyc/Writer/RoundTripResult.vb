Imports System.IO
Imports System.Text

Namespace Writer

    ''' <summary>
    ''' 序列化往返测试(<see cref="ModelSlotSerializer.Serialize"/> 之后重新由
    ''' ``AttrDataCollection(Of T).LoadFile`` 读回)的结果
    ''' </summary>
    Public Class RoundTripResult

        Public Property uniqueId As String
        Public Property slotCount As Integer
        Public Property success As Boolean
        Public Property message As String
        Public Property text As String

        Public Overrides Function ToString() As String
            Return $"{If(success, "OK", "FAILED")} {uniqueId} ({slotCount} slots) {message}"
        End Function
    End Class

    Public Module RoundTrip

        ''' <summary>
        ''' 对单个数据模型对象执行「序列化 → 反序列化」往返测试。
        ''' 
        ''' 该测试用于保证新写入器与既有解析器之间的格式对称性：任何写入格式上的偏差(续行、注解、
        ''' 转义)都会在这里表现为对象或者槽位的丢失。
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Function CheckRoundTrip(Of T As Model)(data As T) As RoundTripResult
            Dim serializer As ModelSlotSerializer = ModelSlotSerializer.GetSerializer(GetType(T))
            Dim record As AttrRecord = serializer.Serialize(data)
            Dim text As String = AttrFileWriter.Format({record})
            Dim result As New RoundTripResult With {
                .uniqueId = data.uniqueId,
                .slotCount = record.slots.Count,
                .text = text
            }

            Try
                Using reader As New StringReader(text)
                    Dim data2 As AttrDataCollection(Of T) = AttrDataCollection(Of T).LoadFile(reader)
                    Dim [object] As T = data2.getFeature(data.uniqueId)

                    If [object] Is Nothing Then
                        result.success = False
                        result.message = "the object was lost after round trip!"
                    Else
                        result.success = True
                        result.message = $"{record.slots.Count} slots"
                    End If
                End Using
            Catch ex As Exception
                result.success = False
                result.message = ex.Message
            End Try

            Return result
        End Function

    End Module

End Namespace
