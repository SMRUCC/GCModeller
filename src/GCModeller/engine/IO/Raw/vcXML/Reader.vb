' ============================================================
' Reader.vb - vcXML 2.0 读取器
' ============================================================
' 读取策略（与 Writer 的物理布局对称）：
'   1. 从文件尾部 4 KB 窗口定位 <indexOffset> 与 <checksum>，
'      再 seek 到索引起点逐行解析 frame/entity 索引；
'   2. 从文件头逐行解析元数据块，遇到 <raw> 停止（不扫描数据区）；
'   3. 读单帧时 seek 到 offset 后只读一行，解码 Base64(GZip)。
'
' 兼容性：只接受 vcellXML_2.0；遇到 1.x 命名空间直接抛 NotSupportedException。
' 公开 API（allFrames / getStreamIndex / getStreamEntities / getFrameVector）
' 保持 1.x 的签名，供 R# vcellkit 与 Extensions 直接调用。
' ============================================================

Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports XmlOffset = SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML.offset

Namespace vcXML

    ''' <summary>vcXML 2.0 读取器</summary>
    Public Class Reader : Implements IDisposable

        Dim fs As StreamReader

        ''' <summary>[module -> [type -> offsets]]</summary>
        Dim index As Dictionary(Of String, Dictionary(Of String, List(Of XmlOffset)))
        ''' <summary>[module -> [type -> [species -> id array]]]</summary>
        Dim entities As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, String())))
        Dim entityOffsetList As New List(Of XmlOffset)
        Dim hash As String
        Dim m_indexOffset As Long

        Public ReadOnly Property basename As String

        ''' <summary>全部帧索引（跨物种、跨组学、跨时间）</summary>
        Public ReadOnly Property allFrames As XmlOffset()
            Get
                If index Is Nothing Then
                    Return New XmlOffset() {}
                End If

                Return index.Values _
                    .IteratesALL _
                    .Values _
                    .IteratesALL _
                    .ToArray
            End Get
        End Property

        ''' <summary>实体索引（诊断用）</summary>
        Public ReadOnly Property EntityOffsets As XmlOffset()
            Get
                Return entityOffsetList.ToArray
            End Get
        End Property

        Private m_cvList As vcCVList
        Private m_runInfo As XML.run
        Private m_experiment As vcExperiment
        Private m_speciesList As vcSpeciesList
        Private m_growthSpace As vcGrowthSpace
        Private m_cultureMedium As vcMedium
        Private m_perturbations As vcPerturbationList
        Private m_layout As vcLayout

        ''' <summary>词表声明</summary>
        Public ReadOnly Property CVList As vcCVList
            Get
                Return m_cvList
            End Get
        End Property

        ''' <summary>运行标识与命令行参数</summary>
        Public ReadOnly Property RunInfo As XML.run
            Get
                Return m_runInfo
            End Get
        End Property

        ''' <summary>实验级元数据</summary>
        Public ReadOnly Property Experiment As vcExperiment
            Get
                Return m_experiment
            End Get
        End Property

        ''' <summary>物种清单</summary>
        Public ReadOnly Property SpeciesList As vcSpeciesList
            Get
                Return m_speciesList
            End Get
        End Property

        ''' <summary>生长空间</summary>
        Public ReadOnly Property GrowthSpace As vcGrowthSpace
            Get
                Return m_growthSpace
            End Get
        End Property

        ''' <summary>培养基</summary>
        Public ReadOnly Property CultureMedium As vcMedium
            Get
                Return m_cultureMedium
            End Get
        End Property

        ''' <summary>虚拟扰动</summary>
        Public ReadOnly Property PerturbationsList As vcPerturbationList
            Get
                Return m_perturbations
            End Get
        End Property

        ''' <summary>时空布局</summary>
        Public ReadOnly Property Layout As vcLayout
            Get
                Return m_layout
            End Get
        End Property
        ''' <summary>文件校验和</summary>
        Public ReadOnly Property Checksum As String
            Get
                Return hash
            End Get
        End Property

        ''' <summary>索引起始位置</summary>
        Public ReadOnly Property IndexOffset As Long
            Get
                Return m_indexOffset
            End Get
        End Property

        Sub New(file As String)
            If String.IsNullOrWhiteSpace(file) Then
                Throw New ArgumentNullException(NameOf(file))
            End If
            If Not System.IO.File.Exists(file) Then
                Throw New FileNotFoundException("vcXML file not found!", file)
            End If

            fs = file.OpenReader()
            basename = file.BaseName

            Call loadOffsets()
            Call parseHeader()
        End Sub

        ''' <summary>物种数组（无 speciesList 时返回空数组）</summary>
        Public ReadOnly Property Species As vcSpecies()
            Get
                If SpeciesList Is Nothing OrElse SpeciesList.items Is Nothing Then
                    Return New vcSpecies() {}
                End If

                Return SpeciesList.items
            End Get
        End Property

        ''' <summary>扰动数组（无 perturbations 块时返回空数组）</summary>
        Public ReadOnly Property Perturbations As vcPerturbation()
            Get
                If PerturbationsList Is Nothing OrElse PerturbationsList.items Is Nothing Then
                    Return New vcPerturbation() {}
                End If

                Return PerturbationsList.items
            End Get
        End Property

        ' ---------------------------------------------------------------
        ' 索引
        ' ---------------------------------------------------------------

        Public Function getStreamIndex(name As String) As Dictionary(Of String, List(Of XmlOffset))
            If index Is Nothing Then
                Throw New MissingPrimaryKeyException(name)
            End If

            Dim byType As Dictionary(Of String, List(Of XmlOffset)) = Nothing

            If Not index.TryGetValue(name, byType) Then
                Throw New MissingPrimaryKeyException(name)
            End If

            Return byType
        End Function

        ''' <summary>
        ''' 按物种过滤的流索引（新增重载；旧调用仍然拿到全部物种的帧）。
        ''' </summary>
        Public Function getStreamIndex(name As String, species As String) As Dictionary(Of String, List(Of XmlOffset))
            Dim all As Dictionary(Of String, List(Of XmlOffset)) = getStreamIndex(name)
            Dim result As New Dictionary(Of String, List(Of XmlOffset))

            For Each pair As KeyValuePair(Of String, List(Of XmlOffset)) In all
                Dim filtered As List(Of XmlOffset) = pair.Value _
                    .Where(Function(o) String.IsNullOrEmpty(species) OrElse
                                       String.Equals(o.species, species, StringComparison.OrdinalIgnoreCase)) _
                    .ToList

                If filtered.Count > 0 Then
                    result(pair.Key) = filtered
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 取某 (group, content_type) 的 id 列表。
        ''' 
        ''' 2.0 多物种文件里返回第一个物种的列表；精确取值请用三参数重载。
        ''' </summary>
        Public Function getStreamEntities(moduleName$, contentType$) As String()
            For Each sp As vcSpecies In Species
                Dim ids As String() = getStreamEntities(moduleName, contentType, sp.id)

                If Not ids.IsNullOrEmpty Then
                    Return ids
                End If
            Next

            ' 回退：任取一个已解析的 species
            Dim byType As Dictionary(Of String, Dictionary(Of String, String())) = Nothing

            If entities IsNot Nothing AndAlso entities.TryGetValue(If(moduleName, ""), byType) Then
                Dim bySpecies As Dictionary(Of String, String()) = Nothing

                If byType.TryGetValue(If(contentType, ""), bySpecies) Then
                    Return bySpecies.Values.FirstOrDefault()
                End If
            End If

            Return New String() {}
        End Function

        ''' <summary>按物种取实体 id 列表</summary>
        Public Function getStreamEntities(moduleName$, contentType$, species$) As String()
            Dim byType As Dictionary(Of String, Dictionary(Of String, String())) = Nothing

            If entities Is Nothing OrElse Not entities.TryGetValue(If(moduleName, ""), byType) Then
                Return New String() {}
            End If

            Dim bySpecies As Dictionary(Of String, String()) = Nothing

            If Not byType.TryGetValue(If(contentType, ""), bySpecies) Then
                Return New String() {}
            End If

            Dim ids As String() = Nothing

            If bySpecies.TryGetValue(If(species, ""), ids) Then
                Return ids
            End If

            Return New String() {}
        End Function

        ' ---------------------------------------------------------------
        ' 帧读取
        ' ---------------------------------------------------------------

        ''' <summary>按字节偏移读取一帧的完整向量（跨全部体素展平）。</summary>
        Public Function getFrameVector(offset As Long) As Double()
            fs.BaseStream.Position = offset
            fs.DiscardBufferedData()

            Dim line As String = fs.ReadLine()

            If String.IsNullOrWhiteSpace(line) Then
                Throw New InvalidDataException($"empty frame at offset {offset}")
            End If

            Return decodeFrameLine(line, offset)
        End Function

        ''' <summary>读取一帧中某个体素的数据块。</summary>
        ''' <param name="offset">帧索引条目</param>
        ''' <param name="voxelIndex">layout 中的体素序号</param>
        Public Function getFrameVector(offset As XmlOffset, voxelIndex As Integer) As Double()
            Dim vector As Double() = getFrameVector(offset.offset)
            Dim local As Integer = voxelIndex - offset.voxelStart
            Dim count As Integer = System.Math.Max(1, offset.voxelCount)

            If local < 0 OrElse local >= count Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(voxelIndex),
                    $"voxel index {voxelIndex} is outside the frame range [{offset.voxelStart}, {offset.voxelStart + count})")
            End If

            Dim entityCount As Integer = vector.Length \ count

            If entityCount <= 0 Then
                Return New Double() {}
            End If

            Dim start As Integer = local * entityCount
            Dim part As Double() = New Double(entityCount - 1) {}

            Call Array.Copy(vector, start, part, 0, entityCount)

            Return part
        End Function

        ''' <summary>读取一帧中某个体素、某个实体（分子/基因/反应）的值。</summary>
        Public Function getFrameVector(offset As XmlOffset, voxelIndex As Integer, entityIndex As Integer) As Double
            Dim part As Double() = getFrameVector(offset, voxelIndex)

            If entityIndex < 0 OrElse entityIndex >= part.Length Then
                Throw New ArgumentOutOfRangeException(NameOf(entityIndex))
            End If

            Return part(entityIndex)
        End Function

        ''' <summary>按条件筛选帧索引。</summary>
        Public Function getFrames(Optional tick As Integer? = Nothing,
                                  Optional species As String = Nothing,
                                  Optional moduleName As String = Nothing,
                                  Optional contentType As String = Nothing) As XmlOffset()

            Return allFrames _
                .Where(Function(o) Not tick.HasValue OrElse o.tick = tick.Value) _
                .Where(Function(o) String.IsNullOrEmpty(species) OrElse
                                   String.Equals(o.species, species, StringComparison.OrdinalIgnoreCase)) _
                .Where(Function(o) String.IsNullOrEmpty(moduleName) OrElse
                                   String.Equals(o.module, moduleName, StringComparison.OrdinalIgnoreCase)) _
                .Where(Function(o) String.IsNullOrEmpty(contentType) OrElse
                                   String.Equals(o.content_type, contentType, StringComparison.OrdinalIgnoreCase)) _
                .OrderBy(Function(o) o.tick) _
                .ThenBy(Function(o) o.module) _
                .ToArray
        End Function

        ''' <summary>
        ''' 解码一行 &lt;frame&gt;：Base64 → GZip → 按 precision/byteOrder 转 Double。
        ''' </summary>
        Private Function decodeFrameLine(line As String, offset As Long) As Double()
            Dim frame As XElement = XElement.Parse(line.Trim())
            Dim vectorNode As XElement = frame _
                .Descendants() _
                .Where(Function(e) String.Equals(e.Name.LocalName, "vector", StringComparison.OrdinalIgnoreCase)) _
                .FirstOrDefault()

            If vectorNode Is Nothing Then
                Throw New InvalidDataException($"frame at offset {offset} has no <vector> element")
            End If

            Dim precision As Integer = getInt(vectorNode, "precision", 64)
            Dim byteOrder As String = getAttr(vectorNode, "byteOrder", "network")
            Dim expected As Integer = getInt(vectorNode, "uncompressedLen", 0)
            Dim data As String = vectorNode.Value.Trim()

            If String.IsNullOrEmpty(data) Then
                Return New Double() {}
            End If

            Dim buffer As Byte() = Convert.FromBase64String(data).UnGzipStream.ToArray

            If String.Equals(byteOrder, "network", StringComparison.OrdinalIgnoreCase) AndAlso
               BitConverter.IsLittleEndian Then

                Call Array.Reverse(buffer)
            End If

            If expected > 0 AndAlso buffer.Length <> expected Then
                Throw New InvalidDataException(
                    $"frame at offset {offset}: decoded {buffer.Length} bytes but header says {expected}")
            End If

            If precision = 32 Then
                Dim count As Integer = buffer.Length \ 4
                Dim singles As Single() = New Single(count - 1) {}

                Call System.Buffer.BlockCopy(buffer, 0, singles, 0, count * 4)

                Return singles.Select(Function(s) CDbl(s)).ToArray
            Else
                Dim count As Integer = buffer.Length \ 8
                Dim doubles As Double() = New Double(count - 1) {}

                Call System.Buffer.BlockCopy(buffer, 0, doubles, 0, count * 8)

                Return doubles
            End If
        End Function

        ' ---------------------------------------------------------------
        ' 头部元数据
        ' ---------------------------------------------------------------

        Private Sub loadOffsets()
            fs.BaseStream.Seek(0, SeekOrigin.End)

            Dim length As Long = fs.BaseStream.Length
            Dim window As Integer = CInt(System.Math.Min(4096L, length))

            fs.BaseStream.Seek(-window, SeekOrigin.End)
            fs.DiscardBufferedData()

            Dim tail As String = fs.ReadToEnd()
            Dim indexMatch As Match = Regex.Match(tail, "<indexOffset>\s*(\d+)\s*</indexOffset>")

            If Not indexMatch.Success Then
                Throw New InvalidDataException(
                    "missing <indexOffset> in the last 4KB; the file is not a complete vcXML 2.0 document")
            End If

            m_indexOffset = Long.Parse(indexMatch.Groups(1).Value)

            Dim checksumMatch As Match = Regex.Match(tail, "<checksum[^>]*>([^<]*)</checksum>")

            If checksumMatch.Success Then
                hash = checksumMatch.Groups(1).Value
            End If

            fs.BaseStream.Seek(m_indexOffset, SeekOrigin.Begin)
            fs.DiscardBufferedData()

            Call parseIndexes()
        End Sub

        Private Sub parseIndexes()
            index = New Dictionary(Of String, Dictionary(Of String, List(Of XmlOffset)))
            entityOffsetList = New List(Of XmlOffset)

            Dim current As String = Nothing
            Dim line As String

            Do
                line = fs.ReadLine()

                If line Is Nothing Then
                    Exit Do
                End If

                line = line.Trim()

                If line.Length = 0 Then
                    Continue Do
                End If

                If line.StartsWith("<index ", StringComparison.Ordinal) Then
                    ' <index> 起始标签单独一行、闭合在后续行，不能直接 XElement.Parse
                    current = getTagAttr(line, "name")
                ElseIf line.StartsWith("</index>", StringComparison.Ordinal) Then
                    current = Nothing
                ElseIf line.StartsWith("<offset ", StringComparison.Ordinal) Then
                    Dim o As XmlOffset = parseOffset(line)

                    If String.Equals(current, "frame", StringComparison.OrdinalIgnoreCase) Then
                        Dim byType As Dictionary(Of String, List(Of XmlOffset)) = Nothing
                        Dim moduleName As String = If(o.module, "")

                        If Not index.TryGetValue(moduleName, byType) Then
                            byType = New Dictionary(Of String, List(Of XmlOffset))
                            index(moduleName) = byType
                        End If

                        Dim contentType As String = If(o.content_type, "")
                        Dim list As List(Of XmlOffset) = Nothing

                        If Not byType.TryGetValue(contentType, list) Then
                            list = New List(Of XmlOffset)
                            byType(contentType) = list
                        End If

                        Call list.Add(o)
                    ElseIf String.Equals(current, "entity", StringComparison.OrdinalIgnoreCase) Then
                        Call entityOffsetList.Add(o)
                    End If
                ElseIf line.StartsWith("<indexOffset>", StringComparison.Ordinal) Then
                    Exit Do
                End If
            Loop
        End Sub

        Private Function parseOffset(line As String) As XmlOffset
            Dim element As XElement = XElement.Parse(line)

            Return New XmlOffset With {
                .id = getInt(element, "id", 0),
                .tick = getInt(element, "tick", 0),
                .[module] = getAttr(element, "module", Nothing),
                .content_type = getAttr(element, "content_type", Nothing),
                .species = getAttr(element, "species", Nothing),
                .voxelStart = getInt(element, "voxelStart", 0),
                .voxelCount = getInt(element, "voxelCount", 1),
                .offset = Long.Parse(element.Value.Trim())
            }
        End Function

        Private Sub parseHeader()
            fs.BaseStream.Seek(0, SeekOrigin.Begin)
            fs.DiscardBufferedData()

            Dim first As String = fs.ReadLine()

            If first Is Nothing OrElse Not first.TrimStart().StartsWith("<?xml", StringComparison.Ordinal) Then
                Throw New InvalidDataException("not a vcXML file: missing XML declaration")
            End If

            Dim root As String = fs.ReadLine()

            If root Is Nothing OrElse Not root.TrimStart().StartsWith("<vcXML", StringComparison.Ordinal) Then
                Throw New InvalidDataException("not a vcXML file: missing <vcXML> root element")
            End If

            ' 版本门槛：只接受 vcellXML_2.0
            If root.IndexOf(Writer.SchemaNamespace, StringComparison.OrdinalIgnoreCase) < 0 Then
                Throw New NotSupportedException(
                    $"vcXML 1.x is no longer supported; expected namespace '{Writer.SchemaNamespace}'. " &
                    "Please regenerate the file with a vcXML 2.0 writer.")
            End If

            entities = New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, String())))()

            Dim line As String

            Do
                line = fs.ReadLine()

                If line Is Nothing Then
                    Exit Do
                End If

                line = line.Trim()

                If line.Length = 0 Then
                    Continue Do
                End If

                If line.StartsWith("<cvList", StringComparison.Ordinal) Then
                    m_cvList = parse(Of vcCVList)(line)
                ElseIf line.StartsWith("<run", StringComparison.Ordinal) Then
                    m_runInfo = parseRun(line)
                ElseIf line.StartsWith("<vcExperiment", StringComparison.Ordinal) Then
                    m_experiment = parse(Of vcExperiment)(line)
                ElseIf line.StartsWith("<speciesList", StringComparison.Ordinal) Then
                    m_speciesList = parse(Of vcSpeciesList)(line)
                ElseIf line.StartsWith("<growthSpace", StringComparison.Ordinal) Then
                    m_growthSpace = parse(Of vcGrowthSpace)(line)
                ElseIf line.StartsWith("<cultureMedium", StringComparison.Ordinal) Then
                    m_cultureMedium = parse(Of vcMedium)(line)
                ElseIf line.StartsWith("<perturbations", StringComparison.Ordinal) Then
                    m_perturbations = parse(Of vcPerturbationList)(line)
                ElseIf line.StartsWith("<layout", StringComparison.Ordinal) Then
                    m_layout = parse(Of vcLayout)(line)
                ElseIf line.StartsWith("<dataEntity", StringComparison.Ordinal) Then
                    Call parseDataEntity()
                ElseIf line.StartsWith("<raw", StringComparison.Ordinal) Then
                    Exit Do
                ElseIf line.StartsWith("</vcXML", StringComparison.Ordinal) Then
                    Exit Do
                End If
            Loop

            If m_layout Is Nothing Then
                ' 最小文件（只有 dataEntity + raw）退化为单物种、单体素布局
                m_layout = New vcLayout With {
                    .frameMode = "sparse",
                    .blockOrder = "voxel,molecule"
                }

                Call Console.WriteLine("[vcXML][warn] missing <layout>, fallback to single species / single voxel")
            End If
        End Sub

        Private Sub parseDataEntity()
            Dim line As String

            Do
                line = fs.ReadLine()

                If line Is Nothing Then
                    Exit Do
                End If

                line = line.Trim()

                If line.StartsWith("</dataEntity>", StringComparison.Ordinal) Then
                    Exit Do
                End If
                If line.Length = 0 Then
                    Continue Do
                End If
                If Not line.StartsWith("<omicsDataEntities", StringComparison.Ordinal) Then
                    Continue Do
                End If

                Dim entry As XML.omicsDataEntities = parse(Of XML.omicsDataEntities)(line)

                If entry Is Nothing Then
                    Continue Do
                End If

                Dim ids As String() = decodeEntities(entry.entities)

                If entry.count > 0 AndAlso ids.Length <> entry.count Then
                    Throw New InvalidDataException(
                        $"entity {entry.module}/{entry.content_type}/{entry.species}: " &
                        $"decoded {ids.Length} ids but header says {entry.count}")
                End If

                Dim moduleName As String = If(entry.module, "")
                Dim contentType As String = If(entry.content_type, "")
                Dim speciesId As String = If(entry.species, "")
                Dim byType As Dictionary(Of String, Dictionary(Of String, String())) = Nothing

                If Not entities.TryGetValue(moduleName, byType) Then
                    byType = New Dictionary(Of String, Dictionary(Of String, String()))
                    entities(moduleName) = byType
                End If

                Dim bySpecies As Dictionary(Of String, String()) = Nothing

                If Not byType.TryGetValue(contentType, bySpecies) Then
                    bySpecies = New Dictionary(Of String, String())
                    byType(contentType) = bySpecies
                End If

                bySpecies(speciesId) = ids
            Loop
        End Sub

        Private Shared Function decodeEntities(data As String) As String()
            If String.IsNullOrWhiteSpace(data) Then
                Return New String() {}
            End If

            Dim buffer As Byte() = Convert.FromBase64String(data.Trim()).UnGzipStream.ToArray
            Dim ids As New List(Of String)

            ' Writer 用 BinaryDataWriter.Write(id, ZeroTerminated) 写出，
            ' 因此这里必须用 BinaryDataReader 的 ZeroTerminated 方式读回；
            ' BinaryReader.ReadString() 是 7-bit 长度前缀，会读错。
            Using reader As New BinaryDataReader(New MemoryStream(buffer))
                Do While Not reader.EndOfStream
                    Dim id As String = reader.ReadString(BinaryStringFormat.ZeroTerminated)

                    If String.IsNullOrEmpty(id) Then
                        Exit Do
                    End If

                    Call ids.Add(id)
                Loop
            End Using

            Return ids.ToArray
        End Function

        ''' <summary>
        ''' 反序列化单行元数据：先剥离命名空间，避免 XmlSerializer 的 XmlRoot 命名空间不匹配。
        ''' </summary>
        Private Shared Function parse(Of T)(line As String) As T
            Dim clean As XElement = stripNamespace(XElement.Parse(line))

            Using reader As New StringReader(clean.ToString())
                Dim serializer As New XmlSerializer(GetType(T))

                Return DirectCast(serializer.Deserialize(reader), T)
            End Using
        End Function

        Private Shared Function parseRun(line As String) As XML.run
            Dim element As XElement = XElement.Parse(line)
            Dim result As New XML.run With {
                .id = getAttr(element, "id", Nothing),
                .startTimeStamp = getAttr(element, "startTimeStamp", Nothing),
                .software = getAttr(element, "software", Nothing),
                .version = getAttr(element, "version", Nothing)
            }

            Return result
        End Function

        Private Shared Function stripNamespace(element As XElement) As XElement
            Dim copy As New XElement(XName.Get(element.Name.LocalName))

            For Each attribute As XAttribute In element.Attributes()
                If Not attribute.IsNamespaceDeclaration Then
                    Call copy.Add(New XAttribute(XName.Get(attribute.Name.LocalName), attribute.Value))
                End If
            Next

            Dim children As XElement() = element.Elements().ToArray

            If children.Length > 0 Then
                For Each child As XElement In children
                    Call copy.Add(stripNamespace(child))
                Next
            Else
                Dim text As String = element.Value

                If Not String.IsNullOrEmpty(text) Then
                    copy.Value = text
                End If
            End If

            Return copy
        End Function

        ''' <summary>从不完整的起始标签行（例如 &lt;index ...&gt;）提取属性值</summary>
        Private Shared Function getTagAttr(line As String, name As String) As String
            Dim match As Match = Regex.Match(line, name & "\s*=\s*""([^""]*)""")

            If match.Success Then
                Return match.Groups(1).Value
            End If

            Return Nothing
        End Function

        Private Shared Function getAttr(element As XElement, name As String, fallback As String) As String
            Dim attribute As XAttribute = element.Attribute(name)

            If attribute Is Nothing Then
                Return fallback
            End If

            Return attribute.Value
        End Function

        Private Shared Function getInt(element As XElement, name As String, fallback As Integer) As Integer
            Dim text As String = getAttr(element, name, Nothing)
            Dim value As Integer

            If String.IsNullOrWhiteSpace(text) OrElse Not Integer.TryParse(text, value) Then
                Return fallback
            End If

            Return value
        End Function

        Public Overrides Function ToString() As String
            Return $"{basename} ({hash})"
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Call fs.Close()
                    Call fs.Dispose()
                End If
            End If

            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub
#End Region

    End Class

End Namespace
