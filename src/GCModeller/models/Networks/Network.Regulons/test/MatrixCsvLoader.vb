Imports System.Globalization
Imports System.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 从 CSV 文件之中读取基因表达矩阵(行是基因、列是样本)
''' 
''' 真实的表达矩阵往往包含上万个基因与上千个样本，直接全部纳入调控网络的推断
''' 既没有必要、也不可能在合理的时间内完成，因此这里会先做三件事：
''' 
''' 1. 丢掉在大部分样本之中都没有表达的基因(由 <see cref="LoadOptions.minNonZeroRatio"/> 控制)
''' 2. 对表达量做 ``log2(x + 1)`` 变换，压缩量纲跨度
''' 3. 按照方差挑选表达变化最剧烈的若干个基因
''' </summary>
Module MatrixCsvLoader

    ''' <summary>
    ''' 表达矩阵的读取与预处理参数
    ''' </summary>
    Public Class LoadOptions

        ''' <summary>
        ''' 按照方差挑选的高变基因数量
        ''' </summary>
        ''' <returns></returns>
        Public Property topGenes As Integer = 150

        ''' <summary>
        ''' 只保留在至少这个比例的样本之中有表达的基因
        ''' </summary>
        ''' <returns></returns>
        Public Property minNonZeroRatio As Double = 0.5

        ''' <summary>
        ''' 是否对表达量做 ``log2(x + 1)`` 变换
        ''' </summary>
        ''' <returns></returns>
        Public Property logTransform As Boolean = True
    End Class

    ''' <summary>
    ''' 读取表达矩阵
    ''' </summary>
    ''' <param name="file">CSV 文件的路径，第一行是样本名称，第一列是基因编号</param>
    ''' <param name="options">预处理参数</param>
    ''' <returns>表达矩阵以及扫描/保留下来的基因数量</returns>
    Public Function Load(file As String, Optional options As LoadOptions = Nothing) As (matrix As Matrix, scanned As Integer, retained As Integer)
        Dim opt As LoadOptions = If(options, New LoadOptions())

        If String.IsNullOrEmpty(file) Then
            Throw New ArgumentNullException(NameOf(file))
        End If
        ' 注意：参数名 file 会遮蔽 System.IO.File 类型(VB 大小写不敏感)，这里必须写全名
        If Not System.IO.File.Exists(file) Then
            Throw New FileNotFoundException($"the expression matrix file is not found: {file}", file)
        End If

        Using reader As New StreamReader(file)
            Dim header As String = reader.ReadLine()

            If String.IsNullOrEmpty(header) Then
                Throw New InvalidDataException("the given expression matrix is empty!")
            End If

            Dim sampleNames As String() = header.Split(","c).Skip(1).ToArray
            Dim nSample As Integer = sampleNames.Length

            If nSample < 4 Then
                Throw New InvalidDataException($"the expression matrix requires at least 4 samples, but {nSample} was found!")
            End If

            Dim scanned As Integer = 0
            Dim candidates As New List(Of (symbol As String, values As Double(), variance As Double))
            Dim line As String

            While True
                line = reader.ReadLine()

                If line Is Nothing Then
                    Exit While
                End If
                If line.Length = 0 Then
                    Continue While
                End If

                Dim fields As String() = line.Split(","c)

                If fields.Length <> nSample + 1 Then
                    Continue While
                End If

                Dim values As Double() = New Double(nSample - 1) {}
                Dim parsed As Boolean = True

                For j As Integer = 1 To nSample
                    Dim value As Double

                    If Double.TryParse(fields(j), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                        values(j - 1) = If(opt.logTransform, Log2(value + 1.0R), value)
                    Else
                        parsed = False
                        Exit For
                    End If
                Next

                If Not parsed Then
                    Continue While
                End If

                scanned += 1

                ' 零表达比例与方差
                Dim nonZero As Integer = 0
                Dim mean As Double = 0.0R

                For j As Integer = 0 To nSample - 1
                    If values(j) > 0.0R Then
                        nonZero += 1
                    End If

                    mean += values(j)
                Next

                mean /= nSample

                Dim variance As Double = 0.0R

                For j As Integer = 0 To nSample - 1
                    variance += (values(j) - mean) ^ 2
                Next

                variance /= nSample

                If nonZero / nSample >= opt.minNonZeroRatio AndAlso variance > 0.0R Then
                    Call candidates.Add((fields(0), values, variance))
                End If
            End While

            Dim top As Integer = opt.topGenes

            If top <= 0 OrElse top > candidates.Count Then
                top = candidates.Count
            End If

            Dim selected As (symbol As String, values As Double(), variance As Double)() = candidates _
                .OrderByDescending(Function(r) r.variance) _
                .Take(top) _
                .ToArray
            Dim rows As DataFrameRow() = selected _
                .Select(Function(r) New DataFrameRow With {
                            .geneID = r.symbol,
                            .experiments = r.values
                        }) _
                .ToArray
            Dim matrix As New Matrix With {
                .tag = $"csv:{System.IO.Path.GetFileName(file)}",
                .sampleID = sampleNames,
                .expression = rows
            }

            Return (matrix, scanned, selected.Length)
        End Using
    End Function

    ''' <summary>
    ''' 以 2 为底的对数
    ''' </summary>
    Private Function Log2(x As Double) As Double
        If x <= 0.0R Then
            Return 0.0R
        End If

        Return std.Log(x) / std.Log(2.0R)
    End Function
End Module
