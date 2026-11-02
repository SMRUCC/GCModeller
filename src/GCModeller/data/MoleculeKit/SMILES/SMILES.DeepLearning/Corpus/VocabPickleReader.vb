Imports System.Collections
Imports System.IO
Imports PyPickle = Microsoft.VisualBasic.Data.IO.Pickle

Namespace Corpus

    ''' <summary>
    ''' 只读解析 Python 侧的 <c>vocab.pkl</c>（<c>build_vocab.WordVocab</c> 实例）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Python 的 pickle 保存的是整个对象，其 <c>__dict__</c> 里含 <c>itos</c>、<c>stoi</c>、
    ''' <c>freqs</c> 与若干 <c>*_index</c> 常量。反序列化器把未知类包装成
    ''' <see cref="PyPickle.PythonObject"/>，并把它交给 <see cref="PyPickle.PythonObject.State"/>。
    ''' </para>
    ''' <para>
    ''' 这里<b>不重排</b> token 顺序：embedding 的行号必须与训练时一致，
    ''' 否则加载官方权重后每一行都错位。
    ''' </para>
    ''' </remarks>
    Public Module VocabPickleReader

        ''' <summary>
        ''' 从 <c>vocab.pkl</c> 读取词表。
        ''' </summary>
        ''' <param name="path">pickle 文件路径。</param>
        ''' <exception cref="FileNotFoundException">文件不存在。</exception>
        ''' <exception cref="InvalidDataException">pickle 内容不是可识别的词表对象。</exception>
        Public Function Load(path As String) As WordVocab
            If Not File.Exists(path) Then Throw New FileNotFoundException($"vocabulary pickle not found: {path}", path)

            Dim obj As Object

            Using stream As Stream = File.OpenRead(path)
                obj = PyPickle.MinimalPickleUnpickler.UnpickleFromStream(stream)
            End Using

            Return FromObject(obj, path)
        End Function

        ''' <summary>
        ''' 从已反序列化的对象构建词表。
        ''' </summary>
        ''' <param name="obj">反序列化结果（通常是 <see cref="PyPickle.PythonObject"/>）。</param>
        ''' <param name="source">来源描述，仅用于异常信息。</param>
        Public Function FromObject(obj As Object, Optional source As String = Nothing) As WordVocab
            Dim pyObj As PyPickle.PythonObject = TryCast(obj, PyPickle.PythonObject)

            If pyObj Is Nothing Then
                Throw New InvalidDataException(
                    $"'{source}' 不是一个 Python 对象实例（实际类型：{If(obj Is Nothing, "Nothing", obj.GetType().FullName)}）")
            End If

            Dim itos As Object = getState(pyObj, "itos")
            Dim stoi As Object = getState(pyObj, "stoi")
            Dim freqs As Object = getState(pyObj, "freqs")

            If itos Is Nothing Then
                Throw New InvalidDataException(
                    $"'{source}' 中缺少 itos 字段（对象类型：{pyObj.FullName}）")
            End If

            Dim tokens As New List(Of String)

            For Each token As Object In DirectCast(itos, IEnumerable)
                tokens.Add(If(token Is Nothing, String.Empty, token.ToString()))
            Next

            Dim frequencies As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            If freqs IsNot Nothing Then
                For Each entry As DictionaryEntry In DirectCast(freqs, IDictionary)
                    Dim token As String = TryCast(entry.Key, String)

                    If token IsNot Nothing Then frequencies(token) = toInt32(entry.Value)
                Next
            End If

            validate(tokens, stoi, source)
            validateSpecials(tokens, source)

            Return New WordVocab(tokens, frequencies)
        End Function

        ''' <summary>取状态字典中指定字段。</summary>
        Private Function getState(pyObj As PyPickle.PythonObject, name As String) As Object
            Dim value As Object = Nothing

            If pyObj.State IsNot Nothing AndAlso pyObj.State.TryGetValue(name, value) Then Return value

            Return Nothing
        End Function

        ''' <summary>
        ''' 校验 <c>stoi</c> 与 <c>itos</c> 的一致性：两者必须互为逆映射。
        ''' </summary>
        Private Sub validate(tokens As List(Of String), stoi As Object, source As String)
            If tokens.Count = 0 Then
                Throw New InvalidDataException($"'{source}' 的 itos 为空")
            End If

            If stoi Is Nothing Then Return

            Dim mapping As IDictionary = TryCast(stoi, IDictionary)

            If mapping Is Nothing Then Return

            If mapping.Count <> tokens.Count Then
                Throw New InvalidDataException(
                    $"'{source}' 的 stoi（{mapping.Count}）与 itos（{tokens.Count}）长度不一致")
            End If

            For Each entry As DictionaryEntry In mapping
                Dim token As String = TryCast(entry.Key, String)
                Dim index As Integer = toInt32(entry.Value)

                If token Is Nothing Then Continue For

                If index < 0 OrElse index >= tokens.Count Then
                    Throw New InvalidDataException(
                        $"'{source}' 中 token '{token}' 的下标 {index} 超出 itos 范围")
                End If

                If Not String.Equals(tokens(index), token, StringComparison.Ordinal) Then
                    Throw New InvalidDataException(
                        $"'{source}' 中 itos[{index}] = '{tokens(index)}' 与 stoi['{token}'] = {index} 不一致")
                End If
            Next
        End Sub

        ''' <summary>校验 5 个特殊 token 的位置（否则 <see cref="WordVocab.PadIndex"/> 等常量会失效）。</summary>
        Private Sub validateSpecials(tokens As List(Of String), source As String)
            If tokens.Count < WordVocab.Specials.Length Then
                Throw New InvalidDataException($"'{source}' 的 itos 少于 5 个特殊 token")
            End If

            For i As Integer = 0 To WordVocab.Specials.Length - 1
                If Not String.Equals(tokens(i), WordVocab.Specials(i), StringComparison.Ordinal) Then
                    Throw New InvalidDataException(
                        $"'{source}' 的特殊 token #{i} 为 '{tokens(i)}'，期望 '{WordVocab.Specials(i)}'")
                End If
            Next
        End Sub

        ''' <summary>把 pickle 中的数值统一转换为 <see cref="Int32"/>。</summary>
        Private Function toInt32(value As Object) As Integer
            If value Is Nothing Then Return 0

            Return Convert.ToInt32(value, Globalization.CultureInfo.InvariantCulture)
        End Function

    End Module

End Namespace
