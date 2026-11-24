' ============================================================================
' ModelParameters.vb - 模型参数容器与序列化
'
' 刻意不使用 BinaryFormatter（.NET 已移除且存在反序列化安全问题），
' 改用自描述的二进制容器：
'     magic(8B) | schemaVersion(I32) | modelType(string)
'     | tensors: count + (name, rank, shape[], raw doubles)
'     | scalars: count + (name, value)
'     | texts  : count + (name, value)
'
' 加载时逐项校验魔数与版本，损坏的文件会抛出带明确信息的异常，
' 不会静默还原出形状错误的参数。
' ============================================================================

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Core

    ''' <summary>
    ''' 命名张量 / 标量 / 文本的参数容器，负责模型的磁盘往返。
    ''' </summary>
    Public Class ModelParameters

        ''' <summary>文件魔数，用于快速识别非法文件。</summary>
        Public Const Magic As String = "VAEPARAM"

        ''' <summary>当前容器格式版本。</summary>
        Public Const CurrentFormatVersion As Integer = 1

        Private ReadOnly _tensors As New Dictionary(Of String, Tensor)
        Private ReadOnly _values As New Dictionary(Of String, Double)
        Private ReadOnly _texts As New Dictionary(Of String, String)

        ''' <summary>模型类型标识（用于加载时的兼容性提示）。</summary>
        Public Property ModelType As String

        ''' <summary>写入文件的数据结构版本。</summary>
        Public Property SchemaVersion As Integer = 1

        Public Sub New()
        End Sub

        Public Sub New(modelType As String, schemaVersion As Integer)
            Me.ModelType = modelType
            Me.SchemaVersion = schemaVersion
        End Sub

#Region "写入端"

        ''' <summary>登记一个张量参数。</summary>
        Public Sub Add(name As String, value As Tensor)
            If String.IsNullOrEmpty(name) Then
                Throw New ArgumentException("参数名不能为空", NameOf(name))
            End If

            If value Is Nothing Then
                Throw New ArgumentNullException(NameOf(value))
            End If

            _tensors(name) = value
        End Sub

        ''' <summary>登记一个标量参数。</summary>
        Public Sub AddValue(name As String, value As Double)
            _values(name) = value
        End Sub

        ''' <summary>登记一段文本参数。</summary>
        Public Sub AddText(name As String, value As String)
            _texts(name) = If(value, "")
        End Sub

#End Region

#Region "读取端"

        ''' <summary>全部张量参数名。</summary>
        Public ReadOnly Property TensorNames As String()
            Get
                Return _tensors.Keys.ToArray()
            End Get
        End Property

        Public Function Contains(name As String) As Boolean
            Return _tensors.ContainsKey(name)
        End Function

        ''' <summary>取出张量参数；不存在时返回 <c>Nothing</c>。</summary>
        Public Function GetTensor(name As String) As Tensor
            Dim value As Tensor

            If _tensors.TryGetValue(name, value) Then
                Return value
            End If

            Return Nothing
        End Function

        Public Function TryGetTensor(name As String, ByRef value As Tensor) As Boolean
            Return _tensors.TryGetValue(name, value)
        End Function

        Public Function TryGetValue(name As String, ByRef value As Double) As Boolean
            Return _values.TryGetValue(name, value)
        End Function

        Public Function GetValue(name As String, Optional fallback As Double = 0) As Double
            Dim value As Double

            If _values.TryGetValue(name, value) Then
                Return value
            End If

            Return fallback
        End Function

        Public Function TryGetText(name As String, ByRef value As String) As Boolean
            Return _texts.TryGetValue(name, value)
        End Function

        ''' <summary>
        ''' 取出张量并校验形状；形状不符时抛出 <see cref="InvalidDataException"/>。
        ''' </summary>
        Public Function RequireTensor(name As String, expectedShape As Integer()) As Tensor
            Dim value As Tensor = Nothing

            If Not TryGetTensor(name, value) Then
                Throw New InvalidDataException($"模型文件缺少参数 '{name}'")
            End If

            If Not ShapeEquals(value, expectedShape) Then
                Throw New InvalidDataException(
                    $"参数 '{name}' 的形状不符：期望 [{String.Join(", ", expectedShape)}]，" &
                    $"实际 [{String.Join(", ", value.Shape)}]")
            End If

            Return value
        End Function

#End Region

#Region "序列化"

        ''' <summary>写入指定路径。</summary>
        Public Sub Save(path As String)
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentException("路径不能为空", NameOf(path))
            End If

            Dim directory As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(directory) AndAlso Not IO.Directory.Exists(directory) Then
                IO.Directory.CreateDirectory(directory)
            End If

            Using stream As New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)
                Using writer As New BinaryWriter(stream, Encoding.UTF8)
                    Call writer.Write(Encoding.ASCII.GetBytes(Magic))
                    Call writer.Write(CurrentFormatVersion)
                    Call writer.Write(SchemaVersion)
                    Call writer.Write(If(ModelType, ""))

                    Call writer.Write(_tensors.Count)

                    For Each item As KeyValuePair(Of String, Tensor) In _tensors
                        Call writer.Write(item.Key)

                        Dim shape As Integer() = item.Value.Shape
                        Call writer.Write(shape.Length)

                        For Each size As Integer In shape
                            Call writer.Write(size)
                        Next

                        Dim data As Double() = item.Value.Data
                        Call writer.Write(data.Length)

                        For i As Integer = 0 To data.Length - 1
                            Call writer.Write(data(i))
                        Next
                    Next

                    Call writer.Write(_values.Count)

                    For Each item As KeyValuePair(Of String, Double) In _values
                        Call writer.Write(item.Key)
                        Call writer.Write(item.Value)
                    Next

                    Call writer.Write(_texts.Count)

                    For Each item As KeyValuePair(Of String, String) In _texts
                        Call writer.Write(item.Key)
                        Call writer.Write(item.Value)
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>从指定路径读取。</summary>
        Public Shared Function Load(path As String) As ModelParameters
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"模型文件不存在: {path}", path)
            End If

            Using stream As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Using reader As New BinaryReader(stream, Encoding.UTF8)
                    Dim magicBytes As Byte() = reader.ReadBytes(Magic.Length)

                    If magicBytes.Length <> Magic.Length OrElse
                       Encoding.ASCII.GetString(magicBytes) <> Magic Then
                        Throw New InvalidDataException($"不是合法的模型文件（魔数不匹配）: {path}")
                    End If

                    Dim formatVersion As Integer = reader.ReadInt32()

                    If formatVersion <> CurrentFormatVersion Then
                        Throw New InvalidDataException(
                            $"不支持的容器格式版本 {formatVersion}，当前支持 {CurrentFormatVersion}")
                    End If

                    Dim result As New ModelParameters With {
                        .SchemaVersion = reader.ReadInt32(),
                        .ModelType = reader.ReadString()
                    }

                    Dim tensorCount As Integer = reader.ReadInt32()

                    For i As Integer = 0 To tensorCount - 1
                        Dim name As String = reader.ReadString()
                        Dim rank As Integer = reader.ReadInt32()
                        Dim shape As Integer() = New Integer(rank - 1) {}

                        For r As Integer = 0 To rank - 1
                            shape(r) = reader.ReadInt32()
                        Next

                        Dim length As Integer = reader.ReadInt32()
                        Dim data As Double() = New Double(length - 1) {}

                        For d As Integer = 0 To length - 1
                            data(d) = reader.ReadDouble()
                        Next

                        Dim expected As Integer = 1

                        For Each size As Integer In shape
                            expected *= size
                        Next

                        If expected <> length Then
                            Throw New InvalidDataException(
                                $"参数 '{name}' 的数据长度 {length} 与形状 [{String.Join(", ", shape)}] 不匹配")
                        End If

                        result._tensors(name) = New Tensor(data, shape)
                    Next

                    Dim valueCount As Integer = reader.ReadInt32()

                    For i As Integer = 0 To valueCount - 1
                        result._values(reader.ReadString()) = reader.ReadDouble()
                    Next

                    Dim textCount As Integer = reader.ReadInt32()

                    For i As Integer = 0 To textCount - 1
                        result._texts(reader.ReadString()) = reader.ReadString()
                    Next

                    Return result
                End Using
            End Using
        End Function

#End Region

#Region "辅助"

        ''' <summary>判断张量的形状是否与期望一致。</summary>
        Public Shared Function ShapeEquals(t As Tensor, expectedShape As Integer()) As Boolean
            If t Is Nothing OrElse expectedShape Is Nothing Then
                Return False
            End If

            Dim actual As Integer() = t.Shape

            If actual.Length <> expectedShape.Length Then
                Return False
            End If

            For i As Integer = 0 To actual.Length - 1
                If actual(i) <> expectedShape(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Overrides Function ToString() As String
            Return $"ModelParameters(type={ModelType}, schema={SchemaVersion}, tensors={_tensors.Count}, values={_values.Count})"
        End Function

#End Region

    End Class

End Namespace
