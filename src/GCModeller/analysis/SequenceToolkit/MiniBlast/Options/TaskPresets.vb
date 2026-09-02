' ============================================================================
' TaskPresets.vb — 任务预设（CLI 与测试共用的唯一参数来源）
' ----------------------------------------------------------------------------
' [README §2.1 / §3.1] 各任务的 word / 打分 / gap 预设。
'
' 预设原先硬编码在 Program.ParseArgs 的私有分支里，测试无法复用；而
' BlastOptions 的字段默认值（WordSize=11、GapOpen=5、GapExtend=2、
' XdropGapFinal=100）对 blastp 全是错的 —— 直接构造 BlastOptions 会以
' W=11 + T=11 触发蛋白邻域词枚举的组合爆炸（24^11 量级、剪枝失效）。
'
' 抽成本模块后，CLI 与自检走同一套参数；命令行显式给出的参数在 Apply 之后覆盖。
' ============================================================================

Namespace Options

    Public Module TaskPresets

        ''' <summary>dc-megablast 默认模板（Ma, Xu &amp; Altschul 2003 的 11/18 coding 模板）</summary>
        Public Const DcTemplateCoding As String = "101101100101101101"

        ''' <summary>dc-megablast 备选模板（optimal 11/18）</summary>
        Public Const DcTemplateOptimal As String = "111010010110010111"

        ''' <summary>
        ''' [式2-1] megablast 动态 gap 延伸代价 = |2·penalty − reward| / 2。
        ''' 保留 Double：reward=1 / penalty=−5 → 5.5（README §2.1 同款示例）；
        ''' 取整会与文档自相矛盾，也会让 x.5 的代价失去意义。
        ''' </summary>
        Public Function MegablastGapExtend(reward As Double, penalty As Double) As Double
            Return Math.Abs(2.0 * penalty - reward) / 2.0
        End Function

        ''' <summary>
        ''' 按 opts.Task 就地覆盖全部预设参数。未知 task 名回落到程序默认值，
        ''' 保证「只设 Program 不设 Task」也能拿到自洽参数。
        ''' </summary>
        Public Sub Apply(opts As BlastOptions)
            If opts.Program = "blastn" Then
                Select Case opts.Task
                    Case "megablast", "dc-megablast", "blastn", "blastn-short"
                        ' ok
                    Case Else
                        opts.Task = "blastn"
                End Select

                Select Case opts.Task
                    Case "megablast"
                        opts.WordSize = 28
                        opts.Reward = 1.0
                        opts.Penalty = -2.0
                        opts.GapOpen = 0.0
                        opts.GapExtend = MegablastGapExtend(opts.Reward, opts.Penalty)
                        opts.Dust = False
                    Case "dc-megablast"
                        opts.WordSize = 11       ' 非连续（11/18 模板）
                        opts.Reward = 2.0
                        opts.Penalty = -3.0
                        opts.GapOpen = 5.0
                        opts.GapExtend = 2.0
                        opts.Dust = True
                    Case "blastn"
                        opts.WordSize = 11
                        opts.Reward = 2.0
                        opts.Penalty = -3.0
                        opts.GapOpen = 5.0
                        opts.GapExtend = 2.0
                        opts.Dust = True
                    Case "blastn-short"
                        opts.WordSize = 7
                        opts.Reward = 1.0
                        opts.Penalty = -3.0
                        opts.GapOpen = 5.0
                        opts.GapExtend = 2.0
                        opts.Dust = False
                End Select
            Else
                Select Case opts.Task
                    Case "blastp", "blastp-short"
                        ' ok
                    Case Else
                        opts.Task = "blastp"
                End Select

                Select Case opts.Task
                    Case "blastp"
                        opts.WordSize = 3
                        opts.Matrix = "BLOSUM62"
                        opts.Threshold = 11
                        opts.GapOpen = 11.0
                        opts.GapExtend = 1.0
                        opts.Seg = True
                        opts.CompBasedStats = 0
                        opts.XdropGapFinal = 25.0
                    Case "blastp-short"
                        opts.WordSize = 2
                        opts.Matrix = "BLOSUM80"
                        opts.Threshold = 13
                        opts.GapOpen = 10.0
                        opts.GapExtend = 1.0
                        opts.Seg = False
                        opts.CompBasedStats = 0
                        opts.XdropGapFinal = 25.0
                End Select
            End If
        End Sub

    End Module

End Namespace
