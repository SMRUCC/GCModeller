' ============================================================
' XML.vb - vcXML 2.0 (vcellXML_2.0) 文档模型
' ============================================================
' 文件结构（Writer 与 Reader 对称）：
'
'   <vcXML xmlns=".../vcellXML_2.0">
'     <cvList>           词表声明（VC/UO/MS）
'     <run>              运行标识 + 命令行参数
'     <vcExperiment>     实验级元数据（引擎/种子/时间轴/开关）
'     <speciesList>      物种清单（多细胞共培养）
'     <growthSpace>      生长空间（网格/形状/边界）
'     <cultureMedium>    培养基（pH/温度/组分/补料/扩散）
'     <perturbations>    虚拟扰动
'     <layout>           species × voxel × group 布局
'     <dataEntity>       每 (species, group) 的 id 列表 Base64(GZip)
'     <raw>              逐帧 Base64(GZip) 向量
'     <index>            帧索引
'     <index>            实体索引
'     <indexOffset>      索引起始位置
'     <checksum>         校验和
'   </vcXML>
'
' 关键：一个 <frame> 占一行（随机访问依赖按行 seek + 读一行）。
' ============================================================

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace vcXML.XML

    ''' <summary>
    ''' vcXML 2.0 的完整文档模型。
    ''' 
    ''' Writer/Reader 为了流式与随机访问并不整体使用该类，
    ''' 但保留完整映射便于反序列化、格式转换与测试。
    ''' </summary>
    Public Class vcXMLFile

        Public Property cvList As vcCVList
        Public Property run As run
        Public Property [experiment] As vcExperiment
        Public Property speciesList As vcSpeciesList
        Public Property growthSpace As vcGrowthSpace
        Public Property cultureMedium As vcMedium
        Public Property perturbations As vcPerturbationList
        Public Property layout As vcLayout
        Public Property dataEntity As dataEntity

        <XmlElement("raw")>
        Public Property raw As raw

        <XmlElement("index")>
        Public Property index As index()

        Public Property indexOffset As Long
        Public Property checksum As checksum

    End Class

    ''' <summary>
    ''' 运行标识与命令行参数（对应旧版的 &lt;run&gt; + &lt;parameters&gt;）。
    ''' </summary>
    <XmlRoot("run")>
    Public Class run

        <XmlAttribute>
        Public Property id As String
        <XmlAttribute>
        Public Property startTimeStamp As String
        <XmlAttribute>
        Public Property software As String
        <XmlAttribute>
        Public Property version As String

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        <XmlElement("userParam")>
        Public Property userParams As userParam()

        ''' <summary>动力学基线等键值参数（保留旧版 &lt;parameters&gt; 语义）</summary>
        Public Property parameters As parameters

    End Class

    Public Class parameters : Inherits ListOf(Of NamedValue)

        <XmlElement("argument")>
        Public Property args As NamedValue()

        Protected Overrides Function getSize() As Integer
            If args Is Nothing Then
                Return 0
            End If

            Return args.Length
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of NamedValue)
            Return args
        End Function

    End Class

    ''' <summary>
    ''' 某一 (species, group) 的实体 id 列表（Base64(GZip(ZeroTerminated String))）。
    ''' </summary>
    Public Class omicsDataEntities

        ''' <summary>组学模块：transcriptome / proteome / metabolome</summary>
        <XmlAttribute>
        Public Property [module] As String

        ''' <summary>物种 id；单物种文件可为空</summary>
        <XmlAttribute>
        Public Property species As String

        ''' <summary>内容类型：mass_profile / activity / flux_size</summary>
        <XmlAttribute>
        Public Property content_type As String

        ''' <summary>id 条数（用于与向量长度交叉校验）</summary>
        <XmlAttribute>
        Public Property count As Integer

        <XmlText>
        Public Property entities As String

    End Class

    Public Class dataEntity

        <XmlElement("omicsDataEntities")>
        Public Property entities As omicsDataEntities()

    End Class

    ''' <summary>
    ''' 一条数据帧。
    ''' 
    ''' <c>voxelStart</c>/<c>voxelCount</c> 指向 <c>layout</c> 的 voxel 轴：
    ''' <list type="bullet">
    ''' <item>sparse：voxelCount = 1，voxelStart = 该体素在 layout 中的序号</item>
    ''' <item>dense：voxelCount = 该帧覆盖的体素数，向量按体素连续展平</item>
    ''' </list>
    ''' </summary>
    Public Class frame

        <XmlAttribute> Public Property num As Integer

        ''' <summary>帧号（整数），用于索引与扰动匹配</summary>
        <XmlAttribute> Public Property tick As Integer

        ''' <summary>帧对应的模拟时间（浮点）</summary>
        <XmlAttribute> Public Property frameTime As Double

        <XmlAttribute> Public Property [module] As String

        ''' <summary>物种 id；单物种文件可为空</summary>
        <XmlAttribute> Public Property species As String

        <XmlAttribute> Public Property voxelStart As Integer
        <XmlAttribute> Public Property voxelCount As Integer

        ''' <summary>该 (species, group) 的实体条数；向量长度 = voxelCount × entityCount</summary>
        <XmlAttribute> Public Property entityCount As Integer

        Public Property vector As vector

    End Class

    Public Class raw

        <XmlElement("frame")>
        Public Property frames As frame()

    End Class

    ''' <summary>
    ''' 二进制向量块。
    ''' 
    ''' <c>precision</c>：64 = Double，32 = Single；
    ''' <c>byteOrder</c>：network = 大端，little = 小端；
    ''' <c>compressionType</c>：zlib / gzip / none。
    ''' </summary>
    Public Class vector

        <XmlAttribute> Public Property compressionType As String = "zlib"

        ''' <summary>压缩后的字节数（Base64 解码后的长度）</summary>
        <XmlAttribute> Public Property compressedLen As Integer

        ''' <summary>压缩前的字节数（解码后应等于 元素数 × precision/8）</summary>
        <XmlAttribute> Public Property uncompressedLen As Integer

        <XmlAttribute> Public Property precision As Integer = 64
        <XmlAttribute> Public Property byteOrder As String = "network"
        <XmlAttribute> Public Property contentType As String = "mass_profile"

        <XmlText>
        Public Property data As String

    End Class

    ''' <summary>
    ''' 索引块（<c>name</c> = frame 或 entity）。
    ''' </summary>
    Public Class index

        <XmlAttribute>
        Public Property name As String

        <XmlAttribute>
        Public Property size As Integer

        <XmlElement("offset")>
        Public Property offsets As offset()

    End Class

    ''' <summary>
    ''' 一个数据块的物理位置。
    ''' 
    ''' <c>id</c>/<c>tick</c>/<c>module</c>/<c>content_type</c>/<c>offset</c> 是 1.x 已有的字段，
    ''' 外部调用方（R# vcellkit / Extensions）直接使用，必须保留；
    ''' <c>species</c>/<c>voxelStart</c>/<c>voxelCount</c> 是 2.0 新增的时空维度。
    ''' </summary>
    Public Class offset

        <XmlAttribute> Public Property id As Integer
        <XmlAttribute> Public Property tick As Integer
        <XmlAttribute> Public Property [module] As String
        <XmlAttribute> Public Property content_type As String

        <XmlAttribute> Public Property species As String
        <XmlAttribute> Public Property voxelStart As Integer
        <XmlAttribute> Public Property voxelCount As Integer

        <XmlText>
        Public Property offset As Long

        Public Overrides Function ToString() As String
            Return $"[{id}] Dim {[module]} As {content_type} = &{offset}"
        End Function

        ''' <summary>格式化物种/体素维度，便于日志</summary>
        Public Function DescribeSpace() As String
            If String.IsNullOrEmpty(species) Then
                Return $"voxel[{voxelStart}+{voxelCount}]"
            Else
                Return $"{species} voxel[{voxelStart}+{voxelCount}]"
            End If
        End Function

    End Class

    ''' <summary>文件校验和</summary>
    Public Class checksum

        <XmlAttribute>
        Public Property type As String = "md5"

        <XmlText>
        Public Property value As String

    End Class

End Namespace
