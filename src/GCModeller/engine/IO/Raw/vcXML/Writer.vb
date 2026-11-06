' ============================================================
' Writer.vb - vcXML 2.0 写入器
' ============================================================
' 物理布局：一帧一行，便于 Reader 用 seek + 读一行做随机访问。
'
'   <vcXML ...>
'   <cvList ...>...</cvList>                 ← 一行
'   <run ...>...</run>                       ← 一行
'   <vcExperiment ...>...</vcExperiment>     ← 一行
'   <speciesList ...>...</speciesList>       ← 一行
'   <growthSpace ...>...</growthSpace>       ← 一行
'   <cultureMedium ...>...</cultureMedium>   ← 一行
'   <perturbations ...>...</perturbations>   ← 一行
'   <layout ...>...</layout>                 ← 一行
'   <dataEntity>
'   <omicsDataEntities ...>base64</omicsDataEntities>   ← 一行
'   </dataEntity>
'   <raw>
'   <frame ...><vector ...>base64</vector></frame>      ← 一行
'   </raw>
'   <index name="frame" size="N">
'   <offset ...>123</offset>                            ← 一行
'   </index>
'   <index name="entity" size="M">...</index>
'   <indexOffset>...</indexOffset>
'   <checksum type="md5">...</checksum>
'   </vcXML>
' ============================================================

Imports System.IO
Imports System.Security
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML
Imports XmlOffset = SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML.offset

Namespace vcXML

    ''' <summary>vcXML 2.0 写入器</summary>
    Public Class Writer : Implements IDisposable

        ''' <summary>schema 版本，写入根元素的默认命名空间</summary>
        Public Const SchemaNamespace As String = "https://biocad.gcmodeller.org/XML/schema_revision/vcellXML_2.0"

        Dim frameCount As Integer = 0
        Dim fs As StreamWriter
        Dim doReverse As Boolean = False
        Dim frameOffsets As New List(Of XmlOffset)
        Dim entityOffsets As New List(Of XmlOffset)
        Dim serializer As New XmlSerializer(GetType(frame))
        Dim md5 As New List(Of String)
        Dim xmlConfig As XmlWriterSettings

        ''' <summary>单条 frame 的最大字节数（默认 64 MB），超限时按体素拆分为多条</summary>
        Public Property MaxFrameBytes As Long = 64L * 1024 * 1024

        ''' <summary>向量精度：64 = Double，32 = Single</summary>
        Public Property Precision As Integer = 64

        Friend Sub New(file As String, xmlSettings As XmlWriterSettings, Optional networkByteOrder As Boolean = True)
            fs = file.OpenWriter(Encodings.UTF8WithoutBOM)
            doReverse = networkByteOrder AndAlso BitConverter.IsLittleEndian
            xmlConfig = New XmlWriterSettings With {
                .Indent = False,
                .OmitXmlDeclaration = True,
                .Encoding = New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
            }
        End Sub

        ''' <summary>
        ''' 兼容旧版入口：由 <see cref="VcellAdapterDriver"/> 提供默认元数据。
        ''' </summary>
        Friend Sub writeInit(entities As VcellAdapterDriver, args As FluxBaseline)
            If entities Is Nothing Then
                Throw New ArgumentNullException(NameOf(entities))
            End If

            Call writeInit(entities.Experiment,
                           entities.Species,
                           entities.GrowthSpace,
                           entities.CultureMedium,
                           entities.Perturbations,
                           entities.Layout,
                           runInfo:=Nothing,
                           args:=args)
        End Sub

        ''' <summary>
        ''' 写出 2.0 头部：cvList / run / experiment / species / space / medium / perturbations / layout / dataEntity。
        ''' </summary>
        Friend Sub writeInit(experiment As vcExperiment,
                             species As vcSpecies(),
                             space As vcGrowthSpace,
                             medium As vcMedium,
                             perturbations As vcPerturbation(),
                             layout As vcLayout,
                             Optional runInfo As run = Nothing,
                             Optional args As FluxBaseline = Nothing)

            If experiment Is Nothing Then
                experiment = New vcExperiment With {
                    .engine = "GCModeller",
                    .seed = "auto",
                    .timeUnit = VcOntology.UnitMinute,
                    .timeUnitName = "minute"
                }
            End If

            If species.IsNullOrEmpty Then
                species = New vcSpecies() {}
            End If
            If layout Is Nothing Then
                layout = New vcLayout()
            End If
            If space Is Nothing Then
                space = New vcGrowthSpace With {
                    .shape = New vcShape With {.width = 1, .height = 1, .depth = 1, .voxelSize = 1, .occupied = 1, .total = 1}
                }
            End If

            ' 校验：occupied 与 layout 的 voxel 轴一致（若 layout 显式列出体素）
            Dim voxelAxis As vcAxis = layout.VoxelAxis

            If voxelAxis IsNot Nothing AndAlso Not voxelAxis.items.IsNullOrEmpty AndAlso
               space.shape IsNot Nothing AndAlso space.shape.occupied > 0 AndAlso
               space.shape.occupied <> voxelAxis.items.Length Then

                Call Console.WriteLine(
                    $"[vcXML][warn] growthSpace.occupied={space.shape.occupied} 与 layout.voxels={voxelAxis.items.Length} 不一致，以 layout 为准")
            End If

            fs.WriteLine("<?xml version=""1.0"" encoding=""utf8""?>")
            fs.WriteLine(
                $"<vcXML xmlns=""{SchemaNamespace}"" " &
                "xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" " &
                "xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" " &
                "xmlns:GCModeller=""https://gcmodeller.org"">")

            ' ---- cvList ----
            Call fs.WriteLine(XmlHelper.getXmlFragment(VcOntology.CvList(), xmlConfig))

            ' ---- run ----
            If runInfo Is Nothing Then
                runInfo = New run With {
                    .id = $"run_{DateTime.Now:yyyyMMddHHmmss}",
                    .startTimeStamp = DateTime.UtcNow.ToString("o"),
                    .software = "GCModeller",
                    .version = GetType(Writer).Assembly.GetName().Version.ToString
                }
            End If
            If runInfo.cvParams.IsNullOrEmpty Then
                runInfo.cvParams = {
                    VcOntology.Param(VcOntology.Simulation, "virtual cell simulation", "")
                }
            End If
            If args IsNot Nothing Then
                runInfo.parameters = createParameters(args)
            End If

            Call fs.WriteLine(XmlHelper.getXmlFragment(runInfo, xmlConfig))

            ' ---- vcExperiment ----
            Call fs.WriteLine(XmlHelper.getXmlFragment(experiment, xmlConfig))

            ' ---- speciesList ----
            Dim speciesList As New vcSpeciesList With {
                .count = species.Length,
                .items = species
            }

            Call fs.WriteLine(XmlHelper.getXmlFragment(speciesList, xmlConfig))

            ' ---- growthSpace ----
            Call fs.WriteLine(XmlHelper.getXmlFragment(space, xmlConfig))

            ' ---- cultureMedium ----
            If medium IsNot Nothing Then
                Call fs.WriteLine(XmlHelper.getXmlFragment(medium, xmlConfig))
            End If

            ' ---- perturbations ----
            If Not perturbations.IsNullOrEmpty Then
                Dim list As New vcPerturbationList With {
                    .count = perturbations.Length,
                    .items = perturbations
                }

                Call fs.WriteLine(XmlHelper.getXmlFragment(list, xmlConfig))
            End If

            ' ---- layout ----
            Call fs.WriteLine(XmlHelper.getXmlFragment(layout, xmlConfig))
            Call fs.Flush()

            ' dataEntity 段由 Driver 的 Initialize 控制：
            '   beginDataEntity -> writeHeader* -> endDataEntityAndBeginRaw
        End Sub

        ''' <summary>开始 dataEntity 段（在写实体之前调用）</summary>
        Friend Sub beginDataEntity()
            Call fs.WriteLine("<dataEntity>")
            Call fs.Flush()
        End Sub

        ''' <summary>结束 dataEntity 段并进入 raw 段</summary>
        Friend Sub endDataEntityAndBeginRaw()
            Call fs.WriteLine("</dataEntity>")
            Call fs.WriteLine("<raw>")
            Call fs.Flush()
        End Sub

        Private Function createParameters(args As FluxBaseline) As parameters
            Dim list As New List(Of NamedValue)

            ' 用反射枚举可读的公有属性，避免依赖 DataFramework.Schema 的具体重载
            For Each p As Reflection.PropertyInfo In GetType(FluxBaseline).GetProperties()
                If p.CanRead AndAlso p.GetIndexParameters().Length = 0 Then
                    Dim value As Object = p.GetValue(args)

                    Call list.Add(New NamedValue With {
                        .name = p.Name,
                        .text = If(value Is Nothing, "", value.ToString)
                    })
                End If
            Next

            Return New parameters With {.args = list.ToArray}
        End Function

        ''' <summary>
        ''' 写出一个 (species, group) 的实体 id 列表。
        ''' </summary>
        Friend Sub writeHeader(moduleName$, contentType$, species$, list As String())
            Dim ms As New MemoryStream

            Using write As New BinaryDataWriter(ms)
                For Each id As String In If(list, New String() {})
                    Call write.Write(id, BinaryStringFormat.ZeroTerminated)
                Next

                Call write.Flush()
            End Using

            Dim buffer As Byte() = ms.ToArray
            Dim uncompressedLen As Integer = buffer.Length

            Dim compressed As Byte() = New MemoryStream(buffer).GZipStream.ToArray

            Dim objects As New omicsDataEntities With {
                .entities = Convert.ToBase64String(compressed),
                .[module] = moduleName,
                .species = species,
                .content_type = contentType,
                .count = If(list Is Nothing, 0, list.Length)
            }
            Dim offset As New XmlOffset With {
                .id = entityOffsets.Count + 1,
                .offset = fs.BaseStream.Position,
                .content_type = contentType,
                .[module] = moduleName,
                .species = species,
                .voxelStart = 0,
                .voxelCount = 1
            }

            Call entityOffsets.Add(offset)
            Call fs.WriteLine(XmlHelper.getXmlFragment(objects, xmlConfig))
            Call fs.Flush()
        End Sub

        ''' <summary>
        ''' 兼容旧签名：写入单物种、单体素的一帧。
        ''' </summary>
        Public Sub addFrame(time As Double, moduleName$, contentType$, data As IEnumerable(Of Double))
            Dim values As Double() = If(data Is Nothing, New Double() {}, data.ToArray())

            Call addFrame(CInt(System.Math.Round(time)), time, moduleName, contentType, "", 0, 1, values.Length, values)
        End Sub

        ''' <summary>
        ''' 写入一帧。向量长度必须等于 <paramref name="voxelCount"/> × <paramref name="entityCount"/>。
        ''' </summary>
        Public Sub addFrame(tick As Integer, frameTime As Double, moduleName$, contentType$, species$,
                            voxelStart As Integer, voxelCount As Integer, entityCount As Integer,
                            data As IEnumerable(Of Double))

            Dim values As Double() = If(data Is Nothing, New Double() {}, data.ToArray())

            If entityCount <= 0 Then
                entityCount = values.Length
            End If

            ' ---- 大帧保护：按体素的整数倍拆分 ----
            Dim bytesPerVoxel As Long = CLng(System.Math.Max(1, entityCount)) * System.Math.Max(1, Precision \ 8)
            Dim maxVoxels As Integer = CInt(System.Math.Max(1, MaxFrameBytes \ bytesPerVoxel))

            If voxelCount > maxVoxels Then
                Dim written As Integer = 0

                Call Console.WriteLine(
                    $"[vcXML][warn] frame tick={tick} {moduleName}/{species}: {voxelCount} voxels exceed maxFrameBytes, " &
                    $"split into chunks of {maxVoxels} voxels")

                While written < voxelCount
                    Dim n As Integer = System.Math.Min(maxVoxels, voxelCount - written)
                    Dim start As Integer = written * entityCount
                    Dim take As Integer = n * entityCount
                    Dim part As Double() = New Double(take - 1) {}

                    If start + take <= values.Length Then
                        Call Array.Copy(values, start, part, 0, take)
                    End If

                    Call writeFrame(tick, frameTime, moduleName, contentType, species, voxelStart + written, n, entityCount, part)

                    written += n
                End While

                Return
            End If

            Call writeFrame(tick, frameTime, moduleName, contentType, species, voxelStart, voxelCount, entityCount, values)
        End Sub

        Private Sub writeFrame(tick As Integer, frameTime As Double, moduleName$, contentType$, species$,
                               voxelStart As Integer, voxelCount As Integer, entityCount As Integer,
                               values As Double())

            frameCount += 1

            Dim compressedLen As Integer = 0
            Dim uncompressedLen As Integer = 0
            Dim encoded As String = encode(values, Precision, compressedLen, uncompressedLen)
            Dim frame As New XML.frame With {
                .num = frameCount,
                .tick = tick,
                .frameTime = frameTime,
                .[module] = moduleName,
                .species = species,
                .voxelStart = voxelStart,
                .voxelCount = voxelCount,
                .entityCount = entityCount,
                .vector = New vector With {
                    .data = encoded,
                    .contentType = contentType,
                    .precision = Precision,
                    .compressedLen = compressedLen,
                    .uncompressedLen = uncompressedLen
                }
            }

            Call fs.Flush()

            Dim position As Long = fs.BaseStream.Position

            Call frameOffsets.Add(New XmlOffset With {
                .id = frame.num,
                .tick = tick,
                .offset = position,
                .content_type = contentType,
                .[module] = moduleName,
                .species = species,
                .voxelStart = voxelStart,
                .voxelCount = voxelCount
            })
            Call md5.Add(encoded.MD5)

            Dim output As New StringBuilder()
            Dim xml As XmlWriter = XmlWriter.Create(output, xmlConfig)

            Call serializer.Serialize(xml, frame, XmlHelper.emptyNamespace)
            Call xml.Flush()

            Call fs.WriteLine(output.ToString)
            Call fs.Flush()
        End Sub

        ''' <summary>
        ''' 编码向量：按精度写数值 → 大端 → GZip → Base64。
        ''' </summary>
        Private Function encode(data As IEnumerable(Of Double), precision As Integer,
                                ByRef compressedLen As Integer, ByRef uncompressedLen As Integer) As String
            Dim values As Double() = If(data Is Nothing, New Double() {}, data.ToArray())
            Dim ms As New MemoryStream

            Using write As New BinaryWriter(ms, Encoding.UTF8, leaveOpen:=True)
                If precision = 32 Then
                    For Each d As Double In values
                        Call write.Write(CSng(d))
                    Next
                Else
                    For Each d As Double In values
                        Call write.Write(d)
                    Next
                End If

                Call write.Flush()
            End Using

            Dim buffer As Byte() = ms.ToArray

            uncompressedLen = buffer.Length

            If doReverse Then
                Call Array.Reverse(buffer)
            End If

            Dim compressed As Byte() = New MemoryStream(buffer).GZipStream.ToArray

            compressedLen = compressed.Length

            Return Convert.ToBase64String(compressed)
        End Function

        ''' <summary>写出尾部索引、索引起点与校验和。</summary>
        Private Sub writeIndex()
            Call fs.WriteLine("</raw>")
            Call fs.Flush()

            Dim indexOffset As Long = fs.BaseStream.Position

            Call writeOffsetIndex("frame", frameOffsets)
            Call writeOffsetIndex("entity", entityOffsets)

            Call fs.WriteLine($"<indexOffset>{indexOffset}</indexOffset>")
            Call fs.WriteLine($"<checksum type=""md5"">{md5.JoinBy("+").MD5}</checksum>")
            Call fs.WriteLine("</vcXML>")
            Call fs.Flush()
        End Sub

        Private Sub writeOffsetIndex(name As String, offsets As List(Of XmlOffset))
            Call fs.WriteLine($"<index name=""{attr(name)}"" size=""{offsets.Count}"">")

            For Each o As XmlOffset In offsets
                Call fs.WriteLine(
                    $"<offset id=""{o.id}"" tick=""{o.tick}"" module=""{attr(o.module)}"" " &
                    $"species=""{attr(o.species)}"" voxelStart=""{o.voxelStart}"" voxelCount=""{o.voxelCount}"" " &
                    $"content_type=""{attr(o.content_type)}"">{o.offset}</offset>")
            Next

            Call fs.WriteLine("</index>")
        End Sub

        ''' <summary>XML 属性值转义</summary>
        Private Shared Function attr(value As String) As String
            Return SecurityElement.Escape(If(value, ""))
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Call writeIndex()
                    Call fs.Flush()
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
