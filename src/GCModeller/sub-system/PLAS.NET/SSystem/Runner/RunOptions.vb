Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports Microsoft.VisualBasic.Text

Namespace Runner

    ''' <summary>
    ''' The S-system model simulation configuration object.
    ''' (S-system 模型仿真的配置对象)
    ''' </summary>
    ''' <remarks>
    ''' 本对象用于替代原先直接把命令行参数对象传递到算法库内部的写法：算法库只依赖本配置对象，
    ''' 命令行语法到配置对象的映射统一由调用方（例如 PLAS 的 CLI 适配层）负责完成。
    ''' </remarks>
    Public Class RunOptions

        ''' <summary>
        ''' 自动推导积分步数时的上限保护值
        ''' </summary>
        Public Const MaxAutoSteps As Integer = 1000000

        ''' <summary>
        ''' The file path of the input model.(输入模型文件的文件路径)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 依据入口的不同，这里可以是 PLAS 脚本文本、编译好的 xml 模型文件或者 sbml 文件
        ''' </remarks>
        Public Property Input As String

        ''' <summary>
        ''' The file path of the output csv data file, if this property is empty, 
        ''' then the output file will be the input file its TrimSuffix value plust 
        ''' the ``.out.Csv`` file extension name.
        ''' (输出数据文件的文件路径，默认为输入文件的扩展名经过去除之后再加上 ``.out.Csv`` 后缀)
        ''' </summary>
        ''' <returns></returns>
        Public Property Output As String

        ''' <summary>
        ''' 仿真退出的终止时间，小于等于零的时候表示使用模型自身所携带的 FINALTIME 配置值
        ''' </summary>
        ''' <returns></returns>
        Public Property FinalTime As Double = 0

        ''' <summary>
        ''' 是否使用常微分方程组求解器进行仿真计算？当本参数为假的时候会使用内核的离散迭代进行计算
        ''' </summary>
        ''' <returns></returns>
        Public Property UseODEs As Boolean = False

        ''' <summary>
        ''' 仿真的步长或者积分精度，这个值越小则仿真的结果越精细，所需要消耗的计算时间也就越多
        ''' </summary>
        ''' <returns></returns>
        Public Property Precise As Double = 0.1

        ''' <summary>
        ''' 严格模式：为真的时候每一个变量都必须显式的申明通量值的上下区间，否则使用默认的 ``-5..5`` 区间
        ''' </summary>
        ''' <returns></returns>
        Public Property Strict As Boolean = False

        ''' <summary>
        ''' 当脚本之中存在缺失的变量申明的时候，是否自动修复？
        ''' </summary>
        ''' <returns></returns>
        Public Property AutoFix As Boolean = False

        ''' <summary>
        ''' 是否在计算结束之后显示图表窗口
        ''' </summary>
        ''' <returns></returns>
        Public Property Chart As Boolean = False

        ''' <summary>
        ''' 自定义的通量值上下区间，在 Strict 模式之下可以为每一个变量指定区间范围
        ''' </summary>
        ''' <returns></returns>
        Public Property Bounds As Dictionary(Of String, DoubleRange)

        ''' <summary>
        ''' The ODEs integration solver engine.(常微分方程组数值积分的求解引擎)
        ''' </summary>
        ''' <returns></returns>
        Public Property Solver As ODESolverMethod = ODESolverMethod.RK4

        ''' <summary>
        ''' 用户在命令行之上传入的求解器名称字符串，仅用于日志诊断
        ''' </summary>
        ''' <returns></returns>
        Public Property SolverName As String

        ''' <summary>
        ''' 求解器的可选参数：自适应容差、牛顿迭代次数等
        ''' </summary>
        ''' <returns></returns>
        Public Property SolverTuning As ODESolverOptions

        ''' <summary>
        ''' The number of the sampling points of the ODEs integration, zero means
        ''' auto derive from the <see cref="FinalTime"/> and <see cref="Precise"/> value.
        ''' (积分的采样点数，零表示依据终止时间与步长自动推导)
        ''' </summary>
        ''' <returns></returns>
        Public Property Steps As Integer = 0

        ''' <summary>
        ''' 是否限制_system节点的状态值不可以小于零？(S-system 幂律之中的负数会越界扩散 NaN，默认为真)
        ''' </summary>
        ''' <returns></returns>
        Public Property NonNegative As Boolean = True

        ''' <summary>
        ''' 从 SBML 编译得到模型的时候，当物种缺失初始量属性的时候所使用默认初始量
        ''' </summary>
        ''' <returns></returns>
        Public Property InitialAmountFallback As Double = 1.0R

        ''' <summary>
        ''' 对缺失初始量的物种附加的确定性抖动幅度（相对比例），用于避免全模型初始值完全一致的时候
        ''' 幂律项的乘积互相抵消而恒等于零，设置为零则关闭抖动
        ''' </summary>
        ''' <returns></returns>
        Public Property InitialJitter As Double = 0.01R

        Sub New()
            SolverTuning = New Microsoft.VisualBasic.Math.Calculus.Dynamics.ODESolverOptions()
        End Sub

        ''' <summary>
        ''' Gets the final time value that will be used in the simulation.
        ''' (获取仿真计算实际所会使用的终止时间)
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Public Function GetFinalTime(model As Script.Model) As Double
            If FinalTime > 0 Then
                Return FinalTime
            End If
            If Not model Is Nothing AndAlso model.FinalTime > 0 Then
                Return CDbl(model.FinalTime)
            End If

            Return 100
        End Function

        ''' <summary>
        ''' Gets the output csv file path, this function returns the user specific value
        ''' when the <see cref="Output"/> is not empty.
        ''' (获取输出数据文件的文件路径)
        ''' </summary>
        ''' <returns></returns>
        Public Function GetOutputPath() As String
            If Not String.IsNullOrWhiteSpace(Output) Then
                Return Output
            End If

            Return Input.TrimSuffix & ".out.Csv"
        End Function

        ''' <summary>
        ''' Gets the sampling point count of the ODEs integration.
        ''' (获取常微分方程组积分的采样点数)
        ''' </summary>
        ''' <param name="finalTime"></param>
        ''' <returns></returns>
        Public Function GetSteps(finalTime As Double) As Integer
            If Steps > 0 Then
                Return Steps
            End If

            Dim precision As Double = If(Precise > 0, Precise, 0.1)
            Dim points As Long = CLng(System.Math.Ceiling(finalTime / precision))

            If points < 1 Then
                points = 1
            ElseIf points > MaxAutoSteps Then
                points = MaxAutoSteps
            End If

            Return CInt(points)
        End Function

        ''' <summary>
        ''' Check the config value is legal or not.(检查配置值是否合法)
        ''' </summary>
        Public Sub Validate()
            If String.IsNullOrWhiteSpace(Input) Then
                Throw New ArgumentException("The input model file path can not be empty!", NameOf(Input))
            End If
            If FinalTime < 0 Then
                Throw New ArgumentException("The simulation final time value must be a positive number!", NameOf(FinalTime))
            End If
            If Not (Precise > 0) Then
                Throw New ArgumentException("The simulation precise value must be a positive number!", NameOf(Precise))
            End If
        End Sub

        ''' <summary>
        ''' View of the current simulation configuration.(打印当前的配置信息)
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim display As String = If(String.IsNullOrWhiteSpace(SolverName), Solver.ToString(), SolverName)

            Return $"PLAS run options: input={Input}, output={GetOutputPath()}, final_time={FinalTime}, " &
                   $"solve_mode={If(UseODEs, "ODEs", "discrete-kernel")}, solver={display}, " &
                   $"precise={Precise}"
        End Function
    End Class
End Namespace
