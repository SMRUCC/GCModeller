Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.DATA

Partial Module CLI

    ''' <summary>
    ''' 将多个实验设计得到的指定时间点的模拟计算数据提取出来然后合并为一个矩阵进行后续分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/union")>
    <Usage("/union /raw <*.csv data_directory> /time <timepoint> [/out <union_matrix.csv>]")>
    Public Function UnionCompareMatrix(args As CommandLine) As Integer
        Dim raw$ = args <= "/raw"
        Dim time$ = args <= "/time"
        Dim out$ = args("/out") Or $"{raw.TrimDIR},time={time}.matrix.csv"
        Dim matrix As New DataFrame
    End Function
End Module