' ============================================================
' Experiment.vb - vcXML 2.0 实验元数据 DTO
' ============================================================
' 仿 mzML 的组织方式：所有实验字段用 cvParam（受控词条 + 单位）表达，
' 非标准字段用 userParam。这些类型只负责「格式契约」，不依赖 Cella，
' 由调用方（R# vcellkit / Cella 导出器）自行映射后填充。
'
' 元数据块在文件中的固定顺序（Writer 与 Reader 对称）：
'
'   cvList -> run -> vcExperiment -> speciesList -> growthSpace
'          -> cultureMedium -> perturbations -> layout -> dataEntity -> raw
' ============================================================

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace vcXML

    ''' <summary>
    ''' 受控词条参数（对应 mzML 的 <c>cvParam</c>）。
    ''' 
    ''' <c>unitCvRef</c> / <c>unitAccession</c> / <c>unitName</c> 三者同时出现时表示该值带单位。
    ''' </summary>
    Public Class cvParam

        ''' <summary>词条 accession，例如 <c>VC:0000011</c> 或者 <c>UO:0000031</c></summary>
        <XmlAttribute>
        Public Property accession As String

        ''' <summary>词条名称，便于人读与缺失本体时降级</summary>
        <XmlAttribute>
        Public Property name As String

        ''' <summary>取值；纯标记型词条留空</summary>
        <XmlAttribute>
        Public Property value As String

        ''' <summary>单位所属词表（例如 UO）</summary>
        <XmlAttribute>
        Public Property unitCvRef As String

        ''' <summary>单位 accession，例如 <c>UO:0000031</c></summary>
        <XmlAttribute>
        Public Property unitAccession As String

        ''' <summary>单位名称，例如 minute</summary>
        <XmlAttribute>
        Public Property unitName As String

        Public Overrides Function ToString() As String
            Dim text As String = If(String.IsNullOrEmpty(value), name, $"{name}={value}")

            If String.IsNullOrEmpty(unitName) Then
                Return $"{accession} {text}"
            Else
                Return $"{accession} {text} {unitName}"
            End If
        End Function

    End Class

    ''' <summary>
    ''' 非标准字段（对应 mzML 的 <c>userParam</c>），用于承载尚未进入 VC 词表的扩展信息。
    ''' </summary>
    Public Class userParam

        <XmlAttribute>
        Public Property name As String

        <XmlAttribute>
        Public Property value As String

        Public Overrides Function ToString() As String
            Return $"{name}={value}"
        End Function

    End Class

    ''' <summary>词表声明（对应 mzML 的 <c>cv</c>）</summary>
    Public Class vcCV

        <XmlAttribute>
        Public Property id As String
        <XmlAttribute>
        Public Property fullName As String
        <XmlAttribute>
        Public Property version As String
        <XmlAttribute>
        Public Property URI As String

    End Class

    ''' <summary>词表清单（对应 mzML 的 <c>cvList</c>）</summary>
    <XmlRoot("cvList")>
    Public Class vcCVList

        <XmlAttribute>
        Public Property count As Integer

        <XmlElement("cv")>
        Public Property items As vcCV()

        Public Shared Function Create(ParamArray cvs As vcCV()) As vcCVList
            If cvs Is Nothing Then
                cvs = New vcCV() {}
            End If

            Return New vcCVList With {
                .count = cvs.Length,
                .items = cvs
            }
        End Function

    End Class

    ''' <summary>
    ''' 实验级元数据：运行引擎、随机性、时间轴与运行开关。
    ''' </summary>
    <XmlRoot("vcExperiment")>
    Public Class vcExperiment

        ''' <summary>模拟引擎名称，例如 Cella / BootstrapLoader</summary>
        <XmlAttribute>
        Public Property engine As String

        ''' <summary>随机种子；无法确定时写 <c>auto</c></summary>
        <XmlAttribute>
        Public Property seed As String

        ''' <summary>总模拟时长（单位见 <see cref="timeUnit"/>）</summary>
        <XmlAttribute>
        Public Property totalTime As Double

        ''' <summary>时间步长（单位见 <see cref="timeUnit"/>）</summary>
        <XmlAttribute>
        Public Property timeStep As Double

        ''' <summary>帧数</summary>
        <XmlAttribute>
        Public Property frameCount As Integer

        ''' <summary>时间单位 accession（默认 UO:0000031 minute）</summary>
        <XmlAttribute>
        Public Property timeUnit As String = VcOntology.UnitMinute

        ''' <summary>时间单位名称</summary>
        <XmlAttribute>
        Public Property timeUnitName As String = "minute"

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        <XmlElement("userParam")>
        Public Property userParams As userParam()

        ''' <summary>按 accession 查找参数值</summary>
        Public Function GetParam(accession As String) As cvParam
            If cvParams.IsNullOrEmpty Then
                Return Nothing
            End If

            Return cvParams _
                .Where(Function(p) String.Equals(p.accession, accession, StringComparison.OrdinalIgnoreCase)) _
                .FirstOrDefault
        End Function

    End Class

    ''' <summary>
    ''' 物种（多细胞共培养时每个物种一条）。
    ''' </summary>
    Public Class vcSpecies

        ''' <summary>文件内唯一 id，例如 <c>sp1</c></summary>
        <XmlAttribute>
        Public Property id As String

        ''' <summary>学名</summary>
        <XmlAttribute>
        Public Property name As String

        ''' <summary>分类学谱系（Cella 的 taxonomy 字符串）</summary>
        <XmlAttribute>
        Public Property taxonomy As String

        ''' <summary>菌株/品系</summary>
        <XmlAttribute>
        Public Property strain As String

        ''' <summary>来源模型文件名</summary>
        <XmlAttribute>
        Public Property model As String

        ''' <summary>转录组 id 数量</summary>
        <XmlAttribute>
        Public Property mrna As Integer

        ''' <summary>蛋白质组 id 数量</summary>
        <XmlAttribute>
        Public Property protein As Integer

        ''' <summary>代谢组 id 数量</summary>
        <XmlAttribute>
        Public Property metabolite As Integer

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        Public Overrides Function ToString() As String
            Return $"{id}: {name}"
        End Function

    End Class

    ''' <summary>物种清单</summary>
    <XmlRoot("speciesList")>
    Public Class vcSpeciesList

        <XmlAttribute>
        Public Property count As Integer

        <XmlElement("species")>
        Public Property items As vcSpecies()

    End Class

    ''' <summary>
    ''' 生长空间的一个体素（只列「被空间形状覆盖 / 有数据」的体素，避免 50³ 全量枚举）。
    ''' </summary>
    Public Class vcVoxel

        ''' <summary>在 layout 中的顺序号（frame 的 voxelStart 指向它）</summary>
        <XmlAttribute>
        Public Property index As Integer

        <XmlAttribute>
        Public Property x As Integer
        <XmlAttribute>
        Public Property y As Integer
        <XmlAttribute>
        Public Property z As Integer

    End Class

    ''' <summary>
    ''' 二进制块：Base64(GZip(原始字节))。
    ''' 
    ''' 用于空间占用掩码等可选的大块数据。
    ''' </summary>
    Public Class vcBinaryBlock

        <XmlAttribute>
        Public Property compressionType As String = "zlib"

        ''' <summary>元素位宽（8 = bool/byte，32 = float，64 = double）</summary>
        <XmlAttribute>
        Public Property precision As Integer = 8

        <XmlAttribute>
        Public Property compressedLen As Integer

        <XmlAttribute>
        Public Property uncompressedLen As Integer

        <XmlText>
        Public Property data As String

    End Class

    ''' <summary>
    ''' 空间形状：网格尺寸、体素体积与占用统计。
    ''' </summary>
    Public Class vcShape

        <XmlAttribute>
        Public Property width As Integer
        <XmlAttribute>
        Public Property height As Integer
        <XmlAttribute>
        Public Property depth As Integer

        ''' <summary>体素边长（立方体假设），单位见 <see cref="voxelSizeUnit"/></summary>
        <XmlAttribute>
        Public Property voxelSize As Double

        <XmlAttribute>
        Public Property voxelSizeUnit As String = VcOntology.UnitMillimetre

        <XmlAttribute>
        Public Property voxelSizeUnitName As String = "millimetre"

        ''' <summary>被形状覆盖的体素数</summary>
        <XmlAttribute>
        Public Property occupied As Integer

        ''' <summary>网格体素总数（width × height × depth）</summary>
        <XmlAttribute>
        Public Property total As Integer

        ''' <summary>可选的占用掩码（按 (x*height+y)*depth+z 展平的 bool）</summary>
        <XmlElement("mask")>
        Public Property mask As vcBinaryBlock

    End Class

    ''' <summary>空间边界（立方体 / STL 网格 / 球）</summary>
    Public Class vcBoundary

        <XmlAttribute>
        Public Property type As String

        ''' <summary>网格文件名（type=stl 时使用）</summary>
        <XmlAttribute>
        Public Property file As String

        ''' <summary>球半径（type=sphere 时使用）</summary>
        <XmlAttribute>
        Public Property radius As Double

        <XmlAttribute>
        Public Property unitCvRef As String = VcOntology.UO_ID

        <XmlAttribute>
        Public Property unitAccession As String

        <XmlAttribute>
        Public Property unitName As String

    End Class

    ''' <summary>
    ''' 生长空间模型：网格、形状、边界与体素清单。
    ''' </summary>
    <XmlRoot("growthSpace")>
    Public Class vcGrowthSpace

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        <XmlElement("shape")>
        Public Property shape As vcShape

        <XmlElement("boundary")>
        Public Property boundary As vcBoundary

    End Class

    ''' <summary>培养基的一个组分</summary>
    Public Class vcMediumComponent

        ''' <summary>模型内的化合物 id</summary>
        <XmlAttribute>
        Public Property id As String

        ''' <summary>初始浓度（单位由 cvParams 中的 concentration 词条给出）</summary>
        <XmlAttribute>
        Public Property concentration As Double

        <XmlAttribute>
        Public Property unitCvRef As String = VcOntology.UO_ID

        <XmlAttribute>
        Public Property unitAccession As String = VcOntology.UnitMillimolePerLitre

        <XmlAttribute>
        Public Property unitName As String = "millimole per litre"

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

    End Class

    ''' <summary>培养基组分清单</summary>
    Public Class vcMediumComponentList

        <XmlAttribute>
        Public Property count As Integer

        <XmlElement("component")>
        Public Property items As vcMediumComponent()

    End Class

    ''' <summary>补料/恒化器参数</summary>
    Public Class vcFeeding

        ''' <summary>batch / fed-batch / chemostat</summary>
        <XmlAttribute>
        Public Property mode As String = "batch"

        <XmlAttribute>
        Public Property dilutionRate As Double

        <XmlAttribute>
        Public Property unitCvRef As String = VcOntology.UO_ID

        <XmlAttribute>
        Public Property unitAccession As String

        <XmlAttribute>
        Public Property unitName As String

    End Class

    ''' <summary>格点间扩散参数</summary>
    Public Class vcDiffusion

        <XmlAttribute>
        Public Property coefficient As Double

        <XmlAttribute>
        Public Property unitCvRef As String = VcOntology.UO_ID

        <XmlAttribute>
        Public Property unitAccession As String

        <XmlAttribute>
        Public Property unitName As String

    End Class

    ''' <summary>
    ''' 培养基环境：名称、pH、温度、组分与传递参数。
    ''' </summary>
    <XmlRoot("cultureMedium")>
    Public Class vcMedium

        <XmlAttribute>
        Public Property name As String

        <XmlAttribute>
        Public Property ph As Double

        ''' <summary>温度（单位见 <see cref="temperatureUnit"/>）</summary>
        <XmlAttribute>
        Public Property temperature As Double

        <XmlAttribute>
        Public Property temperatureUnit As String = VcOntology.UnitCelsius

        <XmlAttribute>
        Public Property temperatureUnitName As String = "degree Celsius"

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        <XmlElement("components")>
        Public Property components As vcMediumComponentList

        <XmlElement("feeding")>
        Public Property feeding As vcFeeding

        <XmlElement("diffusion")>
        Public Property diffusion As vcDiffusion

    End Class

    ''' <summary>扰动靶点：按物种与分子定位，factor 为作用倍数（敲除 = 0）</summary>
    Public Class vcPerturbationTarget

        ''' <summary>目标物种 id；空表示作用于所有物种</summary>
        <XmlAttribute>
        Public Property species As String

        ''' <summary>目标分子/基因/反应 id</summary>
        <XmlAttribute>
        Public Property molecule As String

        ''' <summary>作用倍数：0 = 敲除，1 = 无变化，>1 = 增强</summary>
        <XmlAttribute>
        Public Property factor As Double

    End Class

    ''' <summary>
    ''' 虚拟扰动：基因敲除/过表达、培养基切换、温度切换等。
    ''' </summary>
    Public Class vcPerturbation

        <XmlAttribute>
        Public Property id As String

        ''' <summary>gene-knockout / overexpression / medium-shift / temperature-shift / custom</summary>
        <XmlAttribute>
        Public Property type As String

        ''' <summary>起始帧号（含）</summary>
        <XmlAttribute>
        Public Property startTick As Integer

        ''' <summary>结束帧号（含）；0 表示持续到结束</summary>
        <XmlAttribute>
        Public Property endTick As Integer

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        <XmlElement("target")>
        Public Property targets As vcPerturbationTarget()

        ''' <summary>该扰动在指定帧是否生效</summary>
        Public Function IsActive(tick As Integer) As Boolean
            If tick < startTick Then
                Return False
            End If

            Return endTick <= 0 OrElse tick <= endTick
        End Function

    End Class

    ''' <summary>扰动清单</summary>
    <XmlRoot("perturbations")>
    Public Class vcPerturbationList

        <XmlAttribute>
        Public Property count As Integer

        <XmlElement("perturbation")>
        Public Property items As vcPerturbation()

    End Class

    ''' <summary>
    ''' layout 轴上的一个条目。
    ''' 
    ''' species 轴用 <see cref="reference"/> 指向物种 id；
    ''' voxel 轴用 x/y/z；
    ''' group 轴用 <see cref="value"/> 表示 transcriptome/proteome/metabolome。
    ''' </summary>
    Public Class vcAxisItem

        <XmlAttribute>
        Public Property index As Integer

        <XmlAttribute>
        Public Property reference As String

        <XmlAttribute>
        Public Property value As String

        <XmlAttribute>
        Public Property x As Integer
        <XmlAttribute>
        Public Property y As Integer
        <XmlAttribute>
        Public Property z As Integer

        Public Overrides Function ToString() As String
            If Not String.IsNullOrEmpty(reference) Then
                Return $"[{index}] {reference}"
            ElseIf Not String.IsNullOrEmpty(value) Then
                Return $"[{index}] {value}"
            Else
                Return $"[{index}] ({x},{y},{z})"
            End If
        End Function

    End Class

    ''' <summary>layout 的一个轴（species / voxel / group）</summary>
    Public Class vcAxis

        <XmlAttribute>
        Public Property name As String

        <XmlAttribute>
        Public Property count As Integer

        <XmlElement("item")>
        Public Property items As vcAxisItem()

    End Class

    ''' <summary>
    ''' 多物种 × 体素 × 组学的数据布局。
    ''' 
    ''' <c>frameMode</c>：
    ''' <list type="bullet">
    ''' <item><c>dense</c>：每 (tick, species, group) 一条 frame，向量 = voxelCount × entityCount 展平</item>
    ''' <item><c>sparse</c>：每 (tick, species, group, voxel) 一条 frame，voxelCount = 1</item>
    ''' </list>
    ''' 
    ''' <c>blockOrder</c> 描述向量内部的展平顺序，默认 <c>voxel,molecule</c>。
    ''' </summary>
    <XmlRoot("layout")>
    Public Class vcLayout

        <XmlAttribute>
        Public Property frameMode As String = "sparse"

        <XmlAttribute>
        Public Property blockOrder As String = "voxel,molecule"

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()

        <XmlElement("axis")>
        Public Property axes As vcAxis()

        ''' <summary>按名称取轴（大小写不敏感）</summary>
        Public Function GetAxis(name As String) As vcAxis
            If axes.IsNullOrEmpty Then
                Return Nothing
            End If

            Return axes _
                .Where(Function(a) String.Equals(a.name, name, StringComparison.OrdinalIgnoreCase)) _
                .FirstOrDefault
        End Function

        Public ReadOnly Property SpeciesAxis As vcAxis
            Get
                Return GetAxis("species")
            End Get
        End Property

        Public ReadOnly Property VoxelAxis As vcAxis
            Get
                Return GetAxis("voxel")
            End Get
        End Property

        Public ReadOnly Property GroupAxis As vcAxis
            Get
                Return GetAxis("group")
            End Get
        End Property

        ''' <summary>是否为逐体素的稀疏布局</summary>
        Public ReadOnly Property IsSparse As Boolean
            Get
                Return String.Equals(frameMode, "sparse", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

    End Class

End Namespace
