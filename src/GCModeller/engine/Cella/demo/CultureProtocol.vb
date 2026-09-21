' ============================================================
' CultureProtocol.vb - 类器官培养方案（换液时相）
' ============================================================
' 真实的人肾类器官分化高度依赖分阶段的培养基配方：
'
'   阶段 I   （早期）      CHIR99021（Wnt 激动）+ Activin A
'                          → 诱导后肾间充质 / 肾单位祖细胞
'   阶段 II  （中期）      FGF2 + 视黄酸(RA) + 低剂量 BMP4
'                          → 肾单位分段分化（近端小管 / 远端小管 / 足细胞）
'   阶段 III （后期）      GDNF + EGF
'                          → 输尿管芽分支与集合管成熟
'
' 换液在环境侧实现为：更新培养基模板 → 表层格点按新配方一阶松弛，
' 内部格点只能靠扩散获得新配方成分，因此因子梯度天然存在。
' ============================================================

Imports Cella
Imports Microsoft.VisualBasic.Linq

''' <summary>培养方案的一个阶段</summary>
Public Class CulturePhase

    Public Property name As String
    ''' <summary>本阶段在累计时间上持续到该时刻（左闭右开）</summary>
    Public Property until As Double
    ''' <summary>生长因子浓度：信号通道 → 浓度</summary>
    Public Property factors As Dictionary(Of String, Double)

End Class

Public Module CultureProtocol

    ''' <summary>基础培养基配方（不含生长因子）</summary>
    Public ReadOnly Property Basal As Dictionary(Of String, Double)
        Get
            Return New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
                {"glc_e", 15.0},
                {"o2_e", 6.0},
                {"gln_e", 4.0},
                {"aa_e", 3.0},
                {"pi_e", 3.0},
                {"lac_e", 0.0},
                {"nh4_e", 0.0},
                {"hco3_e", 1.5},
                {"co2_e", 0.0},
                {"matrix_e", 0.0}
            }
        End Get
    End Property

    ''' <summary>三个培养阶段</summary>
    Public ReadOnly Property Phases As CulturePhase()
        Get
            Return {
                New CulturePhase With {
                    .name = "阶段 I 诱导后肾间充质",
                    .until = 8.0,
                    .factors = Factors(("chir", 6.0), ("activin", 4.0), ("fgf2", 1.0))
                },
                New CulturePhase With {
                    .name = "阶段 II 肾单位分段分化",
                    .until = 30.0,
                    .factors = Factors(("chir", 0.5), ("fgf2", 5.0), ("ra", 2.0), ("bmp4", 1.0), ("egf", 0.5))
                },
                New CulturePhase With {
                    .name = "阶段 III 输尿管芽分支 / 集合管成熟",
                    .until = Double.MaxValue,
                    .factors = Factors(("fgf2", 1.0), ("ra", 0.5), ("gdnef", 3.0), ("egf", 2.0))
                }
            }
        End Get
    End Property

    Private Function Factors(ParamArray items As (channel As String, level As Double)()) As Dictionary(Of String, Double)
        Dim map As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For Each item In items
            map(item.channel) = item.level
        Next

        Return map
    End Function

    ''' <summary>当前时刻生效的阶段</summary>
    Public Function PhaseAt(time As Double) As CulturePhase
        For Each phase As CulturePhase In Phases
            If time < phase.until Then
                Return phase
            End If
        Next

        Return Phases.Last()
    End Function

    ''' <summary>当前时刻的完整培养基配方（基础配方 + 生长因子）</summary>
    Public Function Recipe(time As Double) As Dictionary(Of String, Double)
        Dim formula As New Dictionary(Of String, Double)(Basal, StringComparer.OrdinalIgnoreCase)
        Dim phase As CulturePhase = PhaseAt(time)

        ' 未列入当前阶段的因子显式置零：换液意味着把旧因子洗掉
        For Each gf As GrowthFactor In HumanKidney.GrowthFactors
            formula(gf.medium) = 0.0
        Next

        For Each item In phase.factors
            Dim gf As GrowthFactor = HumanKidney.GrowthFactors _
                .FirstOrDefault(Function(g) String.Equals(g.channel, item.Key, StringComparison.OrdinalIgnoreCase))

            If gf IsNot Nothing Then
                formula(gf.medium) = item.Value
            End If
        Next

        Return formula
    End Function

    ''' <summary>
    ''' 把培养方案应用到环境：更新培养基模板与补料清单，并对表层格点立即换液
    ''' </summary>
    Public Sub Apply(env As Environment, time As Double, Optional surfaceRelax As Double = 0.9)
        Dim formula As Dictionary(Of String, Double) = Recipe(time)
        Dim components As String() = formula.Keys.ToArray()

        env.MediumTemplate = formula
        env.Diffusion.FeedMetabolites = components
        env.Diffusion.FeedRate = 0.35
        env.Diffusion.FeedLevel = 1.0

        For Each spot As Spot In env.GetAllSpots()
            If spot.Medium Is Nothing Then
                Continue For
            End If

            ' 保证所有配方成分都有键（扩散只处理存在的键）
            For Each id As String In components
                If Not spot.Medium.ContainsKey(id) Then
                    spot.Medium(id) = 0.0
                End If
            Next

            ' 换液：只有直接浸润在培养基中的表层格点被立即拉到新配方
            If Not spot.IsSurface Then
                Continue For
            End If

            For Each id As String In components
                Dim target As Double = formula(id)
                Dim level As Double = spot.Medium(id)

                spot.Medium(id) = level + surfaceRelax * (target - level)
            Next
        Next
    End Sub

End Module
