Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

''' <summary>
''' 正态分布数值函数（erf / erfc / Φ / Φ⁻¹ / 尾概率）的验收测试。
''' </summary>
''' <remarks>
''' 参考值取自 scipy.special.erf / scipy.stats.norm 的双精度输出；
''' 全部为"解析或权威实现已知值"，不依赖本机安装 Python。
''' </remarks>
Friend Module NormalAudit

        Private Const Section As String = "normal"

        Friend Function Run() As Integer
            Dim failures As Integer = 0

            ' ---- erf / erfc 参考值 ----
            failures += AssertClose("erf(0)", NormalDistribution.Erf(0.0), 0.0, 1.0E-16, Section)
            failures += AssertClose("erf(0.5)", NormalDistribution.Erf(0.5), 0.5204998778130465, 1.0E-15, Section)
            failures += AssertClose("erf(1)", NormalDistribution.Erf(1.0), 0.8427007929497149, 1.0E-15, Section)
            failures += AssertClose("erf(-1)", NormalDistribution.Erf(-1.0), -0.8427007929497149, 1.0E-15, Section)
            failures += AssertClose("erf(2.5)", NormalDistribution.Erf(2.5), 0.999593047982555, 1.0E-14, Section)
            failures += AssertClose("erfc(1)", NormalDistribution.Erfc(1.0), 0.15729920705028513, 1.0E-15, Section)
            failures += AssertClose("erfc(2.5)", NormalDistribution.Erfc(2.5), 0.0004069520174449589, 1.0E-14, Section)
            failures += AssertClose("erf(x)+erfc(x)", NormalDistribution.Erf(1.3) + NormalDistribution.Erfc(1.3), 1.0, 1.0E-15, Section)

            ' ---- Φ / φ 参考值 ----
            failures += AssertClose("Φ(0)", NormalDistribution.CDF(0.0), 0.5, 1.0E-16, Section)
            failures += AssertClose("Φ(1.96)", NormalDistribution.CDF(1.96), 0.9750021048517795, 1.0E-14, Section)
            failures += AssertClose("Φ(-1.96)", NormalDistribution.CDF(-1.96), 0.024997895148220435, 1.0E-14, Section)
            failures += AssertClose("Φ(z)+Φ(-z)", NormalDistribution.CDF(2.3) + NormalDistribution.CDF(-2.3), 1.0, 1.0E-15, Section)
            failures += AssertClose("φ(0)", NormalDistribution.PDF(0.0), 0.3989422804014327, 1.0E-15, Section)
            failures += AssertClose("φ(1)", NormalDistribution.PDF(1.0), 0.24197072451914337, 1.0E-15, Section)
            failures += AssertClose("CCDF(1.96)", NormalDistribution.CCDF(1.96), 0.024997895148220435, 1.0E-14, Section)

            ' ---- 尾概率（相对精度）----
            Dim tail As Double = NormalDistribution.CCDF(10.0)
            Dim tailExpected As Double = 7.619853024160595E-24

            failures += AssertClose("CCDF(10)", tail / tailExpected, 1.0, 1.0E-10, Section)

            ' ---- 双侧 P 值 ----
            failures += AssertClose("2-sided p at 1.96", NormalDistribution.TwoSidedPValue(1.96), 0.04999579029644087, 1.0E-13, Section)
            failures += AssertClose("2-sided p at 0", NormalDistribution.TwoSidedPValue(0.0), 1.0, 1.0E-16, Section)
            failures += Assert("2-sided p is symmetric",
                               std.Abs(NormalDistribution.TwoSidedPValue(-1.2) - NormalDistribution.TwoSidedPValue(1.2)) < 1.0E-15)

            ' ---- Φ⁻¹ ----
            failures += AssertClose("Φ⁻¹(0.5)", NormalDistribution.InvCDF(0.5), 0.0, 1.0E-15, Section)
            failures += AssertClose("Φ⁻¹(0.975)", NormalDistribution.InvCDF(0.975), 1.959963984540054, 1.0E-12, Section)
            failures += AssertClose("Φ⁻¹(0.025)", NormalDistribution.InvCDF(0.025), -1.959963984540054, 1.0E-12, Section)
            failures += AssertClose("Φ⁻¹(1e-10)", NormalDistribution.InvCDF(0.0000000001), -6.361340902404056, 1.0E-9, Section)
            failures += Assert("Φ⁻¹(0) is -inf", Double.IsNegativeInfinity(NormalDistribution.InvCDF(0.0)))
            failures += Assert("Φ⁻¹(1) is +inf", Double.IsPositiveInfinity(NormalDistribution.InvCDF(1.0)))

            ' ---- 往返一致性：Φ⁻¹(Φ(z)) = z ----
            ' 约定：取"尾部的那一侧"做往返——负 z 用 Φ(z)（小概率），正 z 用 1-Φ(z)（小概率），
            ' 这样输入概率始终是中/小量，往返不会因为 1-p 的舍入而丢精度
            For Each z As Double In {-12.0, -8.0, -6.5, -6.0, -3.5, -1.0, -0.25}
                Dim back As Double = NormalDistribution.InvCDF(NormalDistribution.CDF(z))
                Dim tolerance As Double = 1.0E-9 * std.Max(1.0, std.Abs(z))

                failures += AssertClose($"lower-tail roundtrip z={z}", back, z, tolerance, Section)
            Next

            For Each z As Double In {0.25, 1.0, 3.5, 6.0, 6.5, 8.0, 12.0}
                Dim back As Double = NormalDistribution.InvCCDF(NormalDistribution.CCDF(z))
                Dim tolerance As Double = 1.0E-9 * std.Max(1.0, std.Abs(z))

                failures += AssertClose($"upper-tail roundtrip z={z}", back, z, tolerance, Section)
            Next

            failures += AssertClose("InvCCDF(0.025)", NormalDistribution.InvCCDF(0.025), 1.959963984540054, 1.0E-11, Section)

            ' ---- 由双侧 P 值反解 ----
            failures += AssertClose("ZFromTwoSidedPValue(0.05)", NormalDistribution.ZFromTwoSidedPValue(0.05), 1.959963984540054, 1.0E-11, Section)

            ' ---- 带符号分数 ----
            failures += AssertClose("signed score (positive effect)",
                                   NormalDistribution.SignedScore(0.05, 0.7),
                                   1.959963984540054, 1.0E-11, Section)
            failures += AssertClose("signed score (negative effect)",
                                   NormalDistribution.SignedScore(0.05, -0.7),
                                   -1.959963984540054, 1.0E-11, Section)

            ' ---- 与旧 Normal 类的一致性（委托改造后的回归）----
            Dim normal As New Distributions.MethodOfMoments.Normal(2.0, 3.0)

            failures += AssertClose("Normal(2,3).GetCDF(2)",
                                   normal.GetCDF(2.0), 0.5, 1.0E-15, Section)
            failures += AssertClose("Normal(2,3).GetCDF(5)",
                                   normal.GetCDF(5.0), 0.8413447460685429, 1.0E-14, Section)
            failures += AssertClose("Normal(2,3).GetPDF(2)",
                                   normal.GetPDF(2.0), 0.1329807601338109, 1.0E-15, Section)
            failures += AssertClose("Normal(2,3).GetInvCDF(0.975)",
                                   normal.GetInvCDF(0.975), 2.0 + 3.0 * 1.959963984540054, 1.0E-11, Section)

            Call DebugAudit.Mark($"{Section} audit completed", failures)

            Return failures
        End Function

End Module
