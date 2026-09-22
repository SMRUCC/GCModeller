Namespace Perturbation

    ''' <summary>扰动类型（对应 README 中"基因敲除、药物处理等离散条件"）。</summary>
    Public Enum PerturbationKind
        ''' <summary>对照（未扰动）。</summary>
        Control = 0

        ''' <summary>基因敲除。</summary>
        Knockout = 1

        ''' <summary>基因过表达。</summary>
        Overexpress = 2

        ''' <summary>药物 / 刺激处理。</summary>
        DrugTreatment = 3
    End Enum

    ''' <summary>
    ''' 一个扰动的元信息。
    ''' SquiDiff 本身在隐空间中把扰动表示为一个方向向量 <c>Δz_sem</c>，
    ''' 因此本类型只是把"扰动名称 / 类型 / 靶基因"与数据集中该组样本对应起来。
    ''' </summary>
    Public Class PerturbationSpec

        ''' <summary>扰动名（与数据集中的分组标签一致）。</summary>
        Public Property Name As String

        ''' <summary>扰动类型。</summary>
        Public Property Kind As PerturbationKind = PerturbationKind.Control

        ''' <summary>靶基因（组合扰动时可有多个）。</summary>
        Public Property TargetGenes As String()

        ''' <summary>备注（例如"由 A 与 B 不可加叠加"）。</summary>
        Public Property Description As String

        ''' <summary>是否为对照组。</summary>
        Public ReadOnly Property IsControl As Boolean
            Get
                Return Kind = PerturbationKind.Control
            End Get
        End Property

        ''' <summary>构造一个对照扰动。</summary>
        Public Shared Function Control(name As String) As PerturbationSpec
            Return New PerturbationSpec With {
                .Name = name,
                .Kind = PerturbationKind.Control,
                .TargetGenes = New String() {}
            }
        End Function

        Public Function Describe() As String
            Dim targets = If(TargetGenes Is Nothing OrElse TargetGenes.Length = 0,
                             "-",
                             String.Join("+", TargetGenes))

            Return $"{Name} ({Kind}; 靶基因={targets})"
        End Function

        Public Overrides Function ToString() As String
            Return Describe()
        End Function
    End Class
End Namespace
