' ============================================================================
' IModelPersistable.vb - 模型持久化契约
'
' 序列化格式刻意不使用已从 .NET 移除的 BinaryFormatter：采用
'   magic + schemaVersion + 条目表
' 的自描述二进制容器（见 ModelParameters），加载时逐项校验形状与版本。
' ============================================================================

Namespace Core

    ''' <summary>
    ''' 可持久化模型的契约。
    ''' </summary>
    Public Interface IModelPersistable

        ''' <summary>当前模型的数据结构版本号。</summary>
        ReadOnly Property SchemaVersion As Integer

        ''' <summary>把模型参数写入指定路径。</summary>
        Sub Save(path As String)

        ''' <summary>
        ''' 从指定路径还原模型参数。失败时返回 <c>False</c> 并给出原因，
        ''' 不抛出异常、不破坏当前已加载的参数。
        ''' </summary>
        Function TryLoad(path As String, ByRef errorMessage As String) As Boolean

    End Interface

End Namespace
