' ============================================================================
' SelfTest.vb — EmMotif 自检编排器
' ----------------------------------------------------------------------------
' 只负责「分组调度 + 计时 + 汇总」，具体用例分布在：
'   TestAlphabet.vb  字母表 / 编码 / 歧义 / 反向互补 / 序列类型
'   TestEmMath.vb    E 步 / M 步 / 似然 / 一致序列（含独立 Oracle 交叉验证）
'   TestEmSearch.vb  搜索编排的集成测试（恢复、双链、多 motif、宽度、边界）
'   TestChiSquare.vb 不完全伽马 / χ² / E-value
'   TestAssert.vb    断言原语
'   TestData.vb      确定性数据工厂
'
' 用法：
'   EmMotif selftest                 全部用例
'   EmMotif selftest --only 双链     只跑用例组名包含「双链」的组
'   EmMotif selftest --only M 步     只跑 M 步相关组（便于定位单个缺陷）
'
' 退出码 = 失败断言条数（上限 125，符合 POSIX 惯例）。
' ============================================================================

Option Strict On

Imports System.Diagnostics

Namespace EmMotif

    Public Module SelfTest

        Private Function RunGroup(name As String, body As Action, filter As String) As Boolean
            If filter IsNot Nothing AndAlso name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0 Then
                Return False
            End If

            Console.WriteLine()
            Console.WriteLine($"### {name}")
            Dim sw = Stopwatch.StartNew()
            TestAssert.Guard(name, body)
            sw.Stop()
            Console.WriteLine($"    （{name} 用时 {sw.Elapsed.TotalSeconds:F2}s）")
            Return True
        End Function

        ''' <summary>
        ''' 执行全部（或筛选后的）用例组。
        ''' </summary>
        ''' <param name="filter">用例组名关键字；Nothing 或空串表示全部</param>
        ''' <returns>失败断言条数</returns>
        Public Function RunAll(Optional filter As String = Nothing) As Integer
            If filter IsNot Nothing AndAlso filter.Trim().Length = 0 Then filter = Nothing

            TestAssert.Reset()
            Console.WriteLine("=== EmMotif SelfTest ===")
            If filter IsNot Nothing Then Console.WriteLine($"=== 仅执行匹配 [{filter}] 的用例组 ===")

            Dim swAll = Stopwatch.StartNew()

            ' ---- 单元层：字母表与 EM 数学 ----
            RunGroup("字母表与编码", AddressOf TestAlphabet.RunAll, filter)
            RunGroup("种子初始化 InitFromSeed", AddressOf TestEmMath.TestInitFromSeed, filter)
            RunGroup("M 步加权计数与归一化", AddressOf TestEmMath.TestMStep, filter)
            RunGroup("一致序列 Consensus", AddressOf TestEmMath.TestConsensus, filter)
            RunGroup("PWM 变化量 MaxDeltaTo", AddressOf TestEmMath.TestMaxDeltaTo, filter)
            RunGroup("窗口似然比 WindowLogR", AddressOf TestEmMath.TestWindowLogR, filter)
            RunGroup("E 步后验约束（三模型）", AddressOf TestEmMath.TestEStepConstraints, filter)
            RunGroup("E 步后验 vs Oracle", AddressOf TestEmMath.TestEStepVsOracle, filter)
            RunGroup("似然的链模式显式化", AddressOf TestEmMath.TestFullLogLikIndependence, filter)
            RunGroup("全似然 vs Oracle", AddressOf TestEmMath.TestFullLogLikVsOracle, filter)
            RunGroup("EM 单调收敛", AddressOf TestEmMath.TestMonotoneConvergence, filter)
            RunGroup("λ 更新", AddressOf TestEmMath.TestLambdaUpdate, filter)
            RunGroup("伪计数平滑作用", AddressOf TestEmMath.TestPseudocountEffect, filter)
            RunGroup("反向互补不变性", AddressOf TestEmMath.TestRevcompInvariance, filter)
            RunGroup("χ² 生存函数与 E-value", AddressOf TestChiSquare.RunAll, filter)

            ' ---- 集成层：搜索编排 ----
            RunGroup("DNA ZOOPS 种植恢复", AddressOf TestEmSearch.TestDnaRecovery, filter)
            RunGroup("DNA OOPS 种植恢复", AddressOf TestEmSearch.TestOopsRecovery, filter)
            RunGroup("蛋白种植恢复", AddressOf TestEmSearch.TestProteinRecovery, filter)
            RunGroup("双链扫描恢复", AddressOf TestEmSearch.TestRevcompRecovery, filter)
            RunGroup("ANR 多位点", AddressOf TestEmSearch.TestAnrMultipleSites, filter)
            RunGroup("多 motif 屏蔽重跑", AddressOf TestEmSearch.TestMultiMotif, filter)
            RunGroup("种子初始化策略", AddressOf TestEmSearch.TestSeedStrategies, filter)
            RunGroup("结果可复现性", AddressOf TestEmSearch.TestDeterminism, filter)
            RunGroup("宽度范围择优", AddressOf TestEmSearch.TestWidthSelection, filter)
            RunGroup("边界与异常输入", AddressOf TestEmSearch.TestEdgeCases, filter)
            RunGroup("FASTA 端到端与 JSON", AddressOf TestEmSearch.TestFastaEndToEnd, filter)
            RunGroup("数值回归快照", AddressOf TestEmMath.TestGoldenSnapshot, filter)

            swAll.Stop()

            ' ---- 汇总 ----
            Dim failures = TestAssert.FailureCount
            Console.WriteLine()
            Console.WriteLine("================================================================")
            Console.WriteLine($"断言总数 {TestAssert.CheckCount}，失败 {failures}，用时 {swAll.Elapsed.TotalSeconds:F1}s")
            If failures = 0 Then
                Console.WriteLine("=== ALL TESTS PASSED ===")
            Else
                Console.WriteLine($"=== {failures} ASSERTION(S) FAILED ===")
                Console.WriteLine()
                Console.WriteLine("失败清单：")
                For i = 0 To TestAssert.FailureList.Count - 1
                    Console.WriteLine($"  {i + 1,3}. {TestAssert.FailureList(i)}")
                Next
            End If
            Console.WriteLine("================================================================")

            Return Math.Min(125, failures)
        End Function

    End Module

End Namespace
