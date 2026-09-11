Imports System.Collections.Generic
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
' 项目根命名空间与文档类型同名，这里使用别名消除歧义
Imports CircosDoc = SMRUCC.genomics.Visualize.Circos.Configurations.Circos

Namespace GdiPlus

    ''' <summary>
    ''' 一条染色体（ideogram 区段）在圆环之上的角度布局信息
    ''' </summary>
    Public Class ChromosomeBand

        Public Property Name As String
        Public Property Entry As KaryotypeEntry
        ''' <summary>
        ''' 有效长度（nt），已经扣除了 <see cref="KaryotypeSkeleton.loopHole"/>
        ''' </summary>
        Public Property Length As Integer
        ''' <summary>
        ''' 起始角度（度）。0 度为正上方（12 点钟方向），顺时针方向为正
        ''' </summary>
        Public Property StartAngle As Double
        ''' <summary>
        ''' 跨越的角度（度）
        ''' </summary>
        Public Property SweepAngle As Double

        Public ReadOnly Property EndAngle As Double
            Get
                Return StartAngle + SweepAngle
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Name} [{StartAngle:0.##}°..{EndAngle:0.##}°]"
        End Function
    End Class

    ''' <summary>
    ''' circos 文档对象到圆环几何（角度/半径）的映射。
    ''' 
    ''' 负责：
    ''' 
    ''' + 按染色体的长度与 ``ideogram.spacing`` 分配角度区间
    ''' + 处理单染色体模型之中的 ``loopHole`` 缺口
    ''' + 将基因组坐标映射为角度
    ''' </summary>
    Public Class CircosLayout

        ''' <summary>
        ''' 图像半径（像素）
        ''' </summary>
        Public ReadOnly Property ImageRadius As Double
        ''' <summary>
        ''' 参与绘制的染色体（按照它们在 karyotype 文件之中的顺序）
        ''' </summary>
        Public ReadOnly Property Bands As List(Of ChromosomeBand)

        Private _GenomeSize As Integer
        Private _SpacingAngle As Double

        ''' <summary>
        ''' 所有染色体有效长度的总和（nt）
        ''' </summary>
        Public ReadOnly Property GenomeSize As Integer
            Get
                Return _GenomeSize
            End Get
        End Property

        ''' <summary>
        ''' 染色体之间的角度间距（度）
        ''' </summary>
        Public ReadOnly Property SpacingAngle As Double
            Get
                Return _SpacingAngle
            End Get
        End Property
        ''' <summary>
        ''' 是否成功的解析出了染色体骨架信息
        ''' </summary>
        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return Bands Is Nothing OrElse Bands.Count = 0
            End Get
        End Property

        Public Sub New(circos As CircosDoc, imageRadius As Double)
            Me.ImageRadius = imageRadius
            Me.Bands = New List(Of ChromosomeBand)

            Dim karyos As New List(Of KaryotypeEntry)
            Dim loopHole As Integer = 0

            If circos.skeletonKaryotype IsNot Nothing Then
                For Each k As KaryotypeEntry In circos.skeletonKaryotype.Karyotypes
                    If k IsNot Nothing Then
                        karyos.Add(k)
                    End If
                Next

                loopHole = circos.skeletonKaryotype.loopHole
            End If

            If karyos.Count = 0 Then
                Return
            End If

            ' 只绘制 circos.chromosomes 所指定的染色体子集
            Dim filter = parseChromosomes(circos)

            If filter IsNot Nothing Then
                karyos = karyos.Where(Function(k) filter(k.chrName)).ToList()
            End If

            If karyos.Count = 0 Then
                Return
            End If

            Dim units As Double = CircosUnits.ParseNumber(circos.chromosomes_units, 1)

            If units <= 0 Then
                units = 1
            End If

            Dim lengths As New List(Of Integer)

            For i As Integer = 0 To karyos.Count - 1
                Dim len As Integer = Math.Max(0, karyos(i).end - karyos(i).start)

                ' 单染色体模型：在染色体的末尾预留出 loopHole 的缺口
                If i = karyos.Count - 1 AndAlso loopHole > 0 Then
                    len = Math.Max(0, len - loopHole)
                End If

                lengths.Add(len)
            Next

            Dim totalLen As Integer = lengths.Sum()

            If totalLen <= 0 Then
                Return
            End If

            _GenomeSize = totalLen

            Dim ideogram = circos.Ideogram
            Dim spacingExpr$ = If(ideogram Is Nothing, "1u", ideogram.Ideogram.Spacing.default)

            _SpacingAngle = Math.Max(0, CircosUnits.ParseSpacingAngle(spacingExpr, totalLen, units))

            Dim totalSpacing As Double = _SpacingAngle * Math.Max(0, karyos.Count - 1)
            Dim available As Double = 360.0 - totalSpacing

            If available <= 1 Then
                ' 间距过大，退化为没有间距
                _SpacingAngle = 0
                available = 360.0
            End If

            Dim angle As Double = 0

            For i As Integer = 0 To karyos.Count - 1
                Dim sweep As Double = available * lengths(i) / totalLen

                Bands.Add(New ChromosomeBand With {
                    .Name = karyos(i).chrName,
                    .Entry = karyos(i),
                    .Length = lengths(i),
                    .StartAngle = angle,
                    .SweepAngle = sweep
                })

                angle += sweep + _SpacingAngle
            Next
        End Sub

        Public Function TryGetBand(chr As String, ByRef band As ChromosomeBand) As Boolean
            If Not String.IsNullOrEmpty(chr) Then
                For Each b As ChromosomeBand In Bands
                    If String.Equals(b.Name, chr, StringComparison.OrdinalIgnoreCase) Then
                        band = b
                        Return True
                    End If
                Next
            End If

            band = Nothing
            Return False
        End Function

        ''' <summary>
        ''' 将基因组坐标映射为角度（度，0 = 正上方，顺时针）
        ''' </summary>
        Public Function AngleOf(band As ChromosomeBand, position As Integer) As Double
            If band Is Nothing OrElse band.Length <= 0 Then
                Return If(band Is Nothing, 0, band.StartAngle)
            End If

            Dim f As Double = (position - band.Entry.start) / CDbl(band.Length)

            If f < 0 Then f = 0
            If f > 1 Then f = 1

            Return band.StartAngle + f * band.SweepAngle
        End Function

        ''' <summary>
        ''' 计算一个基因组区段（start..end）所对应的角度区间
        ''' </summary>
        Public Function AngleRangeOf(band As ChromosomeBand, start As Integer, [end] As Integer, ByRef startAngle As Double, ByRef sweepAngle As Double) As Boolean
            If band Is Nothing OrElse band.Length <= 0 Then
                Return False
            End If

            Dim a0 As Double = AngleOf(band, start)
            Dim a1 As Double = AngleOf(band, [end])

            startAngle = Math.Min(a0, a1)
            sweepAngle = Math.Abs(a1 - a0)

            Return sweepAngle > 0
        End Function

        ''' <summary>
        ''' 解析 ``chromosomes`` 选择子。当 ``chromosomes_display_default`` 为 yes 的时候返回 Nothing（表示全部显示）
        ''' </summary>
        Private Function parseChromosomes(circos As CircosDoc) As Func(Of String, Boolean)
            If CircosUnits.IsYes(circos.chromosomes_display_default) Then
                Return Nothing
            End If

            Dim spec$ = circos.chromosomes

            If String.IsNullOrWhiteSpace(spec) Then
                Return Nothing
            End If

            Dim names = spec _
                .Split(CChar(";"), CChar(",")) _
                .Select(Function(s) s.Trim()) _
                .Where(Function(s) s.Length > 0) _
                .ToArray()

            If names.Length = 0 Then
                Return Nothing
            End If

            Return Function(chr As String) names.Any(Function(n) String.Equals(n, chr, StringComparison.OrdinalIgnoreCase))
        End Function
    End Class
End Namespace
