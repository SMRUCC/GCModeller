Imports System.Text
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports CSVFile = Microsoft.VisualBasic.Data.Framework.IO.File
Imports PhylipMatrix = SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile.MatrixFile
Imports PhylipNeighbor = SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile.NeighborMatrix

Namespace Evolution.Models

    ''' <summary>
    ''' 对称的距离方阵（square distance matrix）：<c>names + Double()()</c>。
    ''' </summary>
    ''' <remarks>
    ''' 该类型为距离法（UPGMA / NJ）的共同输入模型，并提供与 PHYLIP 邻接矩阵
    ''' （<see cref="PhylipNeighbor"/>）之间的双向转换，从而可以复用现有的
    ''' <c>gendist</c> / <c>neighbor</c> 工作流产出的矩阵文件。
    ''' </remarks>
    Public Class DistanceMatrix

        Public ReadOnly Property Names As String()

        ''' <summary>
        ''' 对称方阵，<c>Matrix(i)(j)</c> 为第 i 与第 j 个分类单元之间的距离。
        ''' </summary>
        Public ReadOnly Property Matrix As Double()()

        Private Sub New(names As String(), matrix As Double()())
            Me.Names = names
            Me.Matrix = matrix
        End Sub

        Default Public ReadOnly Property Item(i As Integer, j As Integer) As Double
            Get
                Return Matrix(i)(j)
            End Get
        End Property

        Public ReadOnly Property Size As Integer
            Get
                Return Names.Length
            End Get
        End Property

        ''' <summary>
        ''' 由距离方阵与名称向量直接构建（会校验维度并强制对角为零、矩阵对称）。
        ''' </summary>
        Public Shared Function FromMatrix(names As String(), matrix As Double()()) As DistanceMatrix
            If names Is Nothing OrElse matrix Is Nothing Then
                Throw New ArgumentNullException("names/matrix")
            End If
            If names.Length <> matrix.Length Then
                Throw New ArgumentException($"名称数目({names.Length})与距离矩阵维度({matrix.Length})不一致！")
            End If

            Dim n As Integer = names.Length
            Dim m(n - 1)() As Double

            For i As Integer = 0 To n - 1
                If matrix(i) Is Nothing OrElse matrix(i).Length <> n Then
                    Throw New ArgumentException($"距离矩阵不是方阵：第 {i} 行的列数为 {If(matrix(i) Is Nothing, 0, matrix(i).Length)}，期望 {n}。")
                End If

                m(i) = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    If i = j Then
                        m(i)(j) = 0
                    Else
                        ' 强制对称：取两端平均，规避第三方程序输出的微小数值误差
                        m(i)(j) = (matrix(i)(j) + matrix(j)(i)) / 2
                    End If
                Next
            Next

            Return New DistanceMatrix(names.ToArray, m)
        End Function

        ''' <summary>
        ''' 由 PHYLIP 邻接矩阵（<see cref="PhylipNeighbor"/>）构建。
        ''' </summary>
        Public Shared Function FromNeighbor(m As PhylipNeighbor) As DistanceMatrix
            Dim raw As CSVFile = m.MATRaw

            If raw Is Nothing OrElse raw.Count = 0 Then
                Throw New ArgumentException("输入的 PHYLIP 邻接矩阵为空！")
            End If

            Dim header As RowObject = raw(0)
            Dim offset As Integer = If(String.IsNullOrEmpty(header(0)), 1, 0)
            Dim names As String() = header.Skip(offset).ToArray
            Dim n As Integer = names.Length
            Dim matrix(n - 1)() As Double

            If raw.Count - 1 < n Then
                Throw New ArgumentException($"PHYLIP 邻接矩阵的行数({raw.Count - 1})少于分类单元数目({n})！")
            End If

            For i As Integer = 0 To n - 1
                Dim row As RowObject = raw(i + 1)
                matrix(i) = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    matrix(i)(j) = Val(row(j + offset))
                Next
            Next

            Return FromMatrix(names, matrix)
        End Function

        ''' <summary>
        ''' 由 PHYLIP 方阵文本文件构建。
        ''' </summary>
        Public Shared Function ReadPhylip(path As String) As DistanceMatrix
            Dim lines As String() = IO.File.ReadAllLines(path).
                Select(Function(s) s.Trim()).
                Where(Function(s) Not s.StringEmpty).
                ToArray()

            If lines.Length = 0 Then
                Throw New ArgumentException($"距离矩阵文件为空：{path}")
            End If

            Dim n As Integer = CInt(Val(lines(0).Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)(0)))
            Dim names(n - 1) As String
            Dim matrix(n - 1)() As Double

            For i As Integer = 0 To n - 1
                Dim line As String = lines(i + 1)
                Dim name As String = Mid(line, 1, 10).Trim
                Dim data As String() = Mid(line, 11).Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)

                names(i) = name
                matrix(i) = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    matrix(i)(j) = If(j < data.Length, Val(data(j)), 0)
                Next
            Next

            Return FromMatrix(names, matrix)
        End Function

        ''' <summary>
        ''' 转换为 PHYLIP 邻接矩阵（可直接传入 neighbor / fitch / kitsch 等程序）。
        ''' </summary>
        Public Function ToNeighbor() As PhylipNeighbor
            Dim raw As New CSVFile
            Dim head As New RowObject
            Dim n As Integer = Size

            Call head.Add("")

            For i As Integer = 0 To n - 1
                Call head.Add(Names(i))
            Next

            Call raw.Add(head)

            For i As Integer = 0 To n - 1
                Dim row As New RowObject
                Call row.Add(Names(i))

                For j As Integer = 0 To n - 1
                    Call row.Add(Matrix(i)(j).ToString("F6"))
                Next

                Call raw.Add(row)
            Next

            Return PhylipNeighbor.CreateObject(raw)
        End Function

        ''' <summary>
        ''' 生成 PHYLIP 方阵文本（第一行给出分类单元数目）。
        ''' </summary>
        Public Function ToPhylipDocument() As String
            Dim sb As New StringBuilder(1024)
            Dim n As Integer = Size

            Call sb.AppendLine("   " & n)

            For i As Integer = 0 To n - 1
                Call sb.Append(PhylipMatrix.MAT_ID(Names(i)))
                Call sb.Append(" ")

                For j As Integer = 0 To n - 1
                    Call sb.Append(PhylipMatrix.RoundNumber(Matrix(i)(j).ToString("F6"), 6))

                    If j < n - 1 Then
                        Call sb.Append(" ")
                    End If
                Next

                Call sb.AppendLine()
            Next

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"{Size} x {Size} distance matrix"
        End Function
    End Class

End Namespace
