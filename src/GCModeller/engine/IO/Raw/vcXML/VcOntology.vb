' ============================================================
' VcOntology.vb - vcXML 2.0 受控词表常量
' ============================================================
' 说明：
'   * VC 是本项目自定义的虚拟细胞词表（accession 形如 VC:0000xxx），
'     URI 指向规划中的 vc.obo；不冒充任何官方本体。
'   * UO/MS 的 accession 参照 OBO 常用条目，若与目标版本有出入，
'     以官方发布为准，本文件的常量仅作为「默认建议值」。
' ============================================================

Namespace vcXML

    ''' <summary>
    ''' vcXML 使用的词表标识、词条 accession 与单位常量。
    ''' </summary>
    Public Module VcOntology

        ' ==================== 词表标识 ====================

        Public Const CV_ID As String = "VC"
        Public Const CV_NAME As String = "GCModeller Virtual Cell Ontology"
        Public Const CV_VERSION As String = "2.0"
        Public Const CV_URI As String = "https://biocad.gcmodeller.org/ontology/vc.obo"

        Public Const UO_ID As String = "UO"
        Public Const UO_NAME As String = "Unit Ontology"
        Public Const UO_VERSION As String = "2024-01-01"
        Public Const UO_URI As String = "http://purl.obolibrary.org/obo/uo.obo"

        Public Const MS_ID As String = "MS"
        Public Const MS_NAME As String = "PSI-MS"
        Public Const MS_VERSION As String = "4.1.0"
        Public Const MS_URI As String = "https://purl.obolibrary.org/obo/ms.obo"

        ' ==================== 实验级词条（VC:0000001 ~ 0000019） ====================

        ''' <summary>虚拟细胞模拟</summary>
        Public Const Simulation As String = "VC:0000001"
        ''' <summary>模拟引擎</summary>
        Public Const SimulationEngine As String = "VC:0000010"
        ''' <summary>随机种子</summary>
        Public Const RandomSeed As String = "VC:0000011"
        ''' <summary>总模拟时长</summary>
        Public Const TotalTime As String = "VC:0000012"
        ''' <summary>时间步长</summary>
        Public Const TimeStep As String = "VC:0000013"
        ''' <summary>帧数</summary>
        Public Const FrameCount As String = "VC:0000014"
        ''' <summary>被动扩散开关</summary>
        Public Const PassiveDiffusion As String = "VC:0000015"
        ''' <summary>边界推导方式</summary>
        Public Const BoundaryDerivation As String = "VC:0000016"

        ' ==================== 物种词条（VC:0000020 ~ 0000029） ====================

        ''' <summary>物种</summary>
        Public Const Species As String = "VC:0000020"
        ''' <summary>分类学谱系</summary>
        Public Const Taxonomy As String = "VC:0000021"
        ''' <summary>菌株/品系</summary>
        Public Const Strain As String = "VC:0000022"
        ''' <summary>来源模型文件</summary>
        Public Const ModelFile As String = "VC:0000023"
        ''' <summary>基因数</summary>
        Public Const GeneCount As String = "VC:0000024"

        ' ==================== 空间词条（VC:0000030 ~ 0000039） ====================

        ''' <summary>体素网格</summary>
        Public Const VoxelGrid As String = "VC:0000030"
        ''' <summary>网格宽度</summary>
        Public Const GridWidth As String = "VC:0000031"
        ''' <summary>网格高度</summary>
        Public Const GridHeight As String = "VC:0000032"
        ''' <summary>网格深度</summary>
        Public Const GridDepth As String = "VC:0000033"
        ''' <summary>体素尺寸</summary>
        Public Const VoxelSize As String = "VC:0000034"
        ''' <summary>被形状覆盖的体素数</summary>
        Public Const OccupiedVoxels As String = "VC:0000035"
        ''' <summary>立方体边界</summary>
        Public Const BoundaryCube As String = "VC:0000036"
        ''' <summary>STL 网格边界</summary>
        Public Const BoundaryStl As String = "VC:0000037"
        ''' <summary>球体边界</summary>
        Public Const BoundarySphere As String = "VC:0000038"

        ' ==================== 培养基词条（VC:0000040 ~ 0000049） ====================

        ''' <summary>培养基</summary>
        Public Const CultureMedium As String = "VC:0000040"
        ''' <summary>pH</summary>
        Public Const Ph As String = "VC:0000041"
        ''' <summary>温度</summary>
        Public Const Temperature As String = "VC:0000042"
        ''' <summary>初始浓度</summary>
        Public Const InitialConcentration As String = "VC:0000043"
        ''' <summary>补料模式</summary>
        Public Const FeedingMode As String = "VC:0000044"
        ''' <summary>稀释率</summary>
        Public Const DilutionRate As String = "VC:0000045"
        ''' <summary>扩散系数</summary>
        Public Const DiffusionCoefficient As String = "VC:0000046"

        ' ==================== 扰动词条（VC:0000050 ~ 0000059） ====================

        ''' <summary>基因敲除</summary>
        Public Const GeneKnockout As String = "VC:0000050"
        ''' <summary>过表达</summary>
        Public Const Overexpression As String = "VC:0000051"
        ''' <summary>培养基切换</summary>
        Public Const MediumShift As String = "VC:0000052"
        ''' <summary>温度切换</summary>
        Public Const TemperatureShift As String = "VC:0000053"
        ''' <summary>扰动起始帧</summary>
        Public Const PerturbationStart As String = "VC:0000054"
        ''' <summary>扰动结束帧</summary>
        Public Const PerturbationEnd As String = "VC:0000055"
        ''' <summary>扰动倍数因子</summary>
        Public Const PerturbationFactor As String = "VC:0000056"

        ' ==================== 布局与组学词条（VC:0000060 ~ 0000079） ====================

        ''' <summary>时空布局</summary>
        Public Const SpatiotemporalLayout As String = "VC:0000060"
        ''' <summary>逐体素稀疏布局</summary>
        Public Const FrameModeSparse As String = "VC:0000061"
        ''' <summary>整帧稠密布局</summary>
        Public Const FrameModeDense As String = "VC:0000062"
        ''' <summary>转录组</summary>
        Public Const Transcriptome As String = "VC:0000070"
        ''' <summary>蛋白质组</summary>
        Public Const Proteome As String = "VC:0000071"
        ''' <summary>代谢组</summary>
        Public Const Metabolome As String = "VC:0000072"
        ''' <summary>质量谱</summary>
        Public Const MassProfile As String = "VC:0000073"
        ''' <summary>活性</summary>
        Public Const Activity As String = "VC:0000074"
        ''' <summary>通量规模</summary>
        Public Const FluxSize As String = "VC:0000075"

        ' ==================== UO 单位 ====================

        Public Const UnitSecond As String = "UO:0000010"
        Public Const UnitMinute As String = "UO:0000031"
        Public Const UnitHour As String = "UO:0000032"
        Public Const UnitCelsius As String = "UO:0000027"
        Public Const UnitMillimetre As String = "UO:0000016"
        Public Const UnitMillilitre As String = "UO:0000098"
        Public Const UnitMillimolePerLitre As String = "UO:0000063"
        Public Const UnitPixel As String = "UO:0000189"
        Public Const UnitDimensionless As String = "UO:0000191"

        ' ==================== 辅助构造 ====================

        ''' <summary>
        ''' vcXML 2.0 默认的 <c>cvList</c>（VC / UO / MS）。
        ''' </summary>
        Public Function CvList() As vcCVList
            Return vcCVList.Create(
                New vcCV With {
                    .id = CV_ID,
                    .fullName = CV_NAME,
                    .version = CV_VERSION,
                    .URI = CV_URI
                },
                New vcCV With {
                    .id = UO_ID,
                    .fullName = UO_NAME,
                    .version = UO_VERSION,
                    .URI = UO_URI
                },
                New vcCV With {
                    .id = MS_ID,
                    .fullName = MS_NAME,
                    .version = MS_VERSION,
                    .URI = MS_URI
                })
        End Function

        ''' <summary>
        ''' 构造一个不带单位的 cvParam。
        ''' </summary>
        Public Function Param(accession As String, name As String, value As String) As cvParam
            Return New cvParam With {
                .accession = accession,
                .name = name,
                .value = value
            }
        End Function

        ''' <summary>
        ''' 构造一个带 UO 单位的 cvParam。
        ''' </summary>
        Public Function Param(accession As String, name As String, value As String,
                              unitAccession As String, unitName As String) As cvParam

            Return New cvParam With {
                .accession = accession,
                .name = name,
                .value = value,
                .unitCvRef = UO_ID,
                .unitAccession = unitAccession,
                .unitName = unitName
            }
        End Function

        ''' <summary>带 UO 单位的数值参数（统一用不变文化格式化）</summary>
        Public Function Number(accession As String, name As String, value As Double,
                               unitAccession As String, unitName As String) As cvParam

            Return Param(accession, name,
                         value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                         unitAccession, unitName)
        End Function

        ''' <summary>布尔型参数（true / false）</summary>
        Public Function Flag(accession As String, name As String, value As Boolean) As cvParam
            Return Param(accession, name, If(value, "true", "false"))
        End Function

    End Module

End Namespace
