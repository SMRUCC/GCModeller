Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.LDM
Imports Microsoft.VisualBasic

Namespace Kernel

    ''' <summary>
    ''' The data reading adapter for the GCModeller calculation engine.(计算引擎的数据采集卡)
    ''' </summary>
    ''' <typeparam name="ModelType"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class DataAdapter(Of ModelType As ModelBaseType)

        Protected Friend _kernelModule As Framework.Kernel_Driver.IterationMathEngine(Of ModelType)
        ''' <summary>
        ''' {RTime, {Handle, Value}}，每一个对象都是按照Handle排列的
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _innerBuffer As List(Of KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())) =
            New List(Of KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)()))

        Sub New(Kernel As Framework.Kernel_Driver.IterationMathEngine(Of ModelType))
            Me._kernelModule = Kernel
            Kernel.__runDataAdapter = Sub() Call Me.Tick()
        End Sub

        Public Overridable Function Tick() As Integer
            Call Me._innerBuffer.Add(GetDataPackage)
            Return 0
        End Function

        ''' <summary>
        ''' {RTime, {Handle, Value}}()，每一个元素都是按照Handle排列顺序的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function GetDataPackage() As KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())
            Dim LQuery = (From Equation As Expression In Me._kernelModule.get_Expressions
                          Let Data0Expr = New KeyValuePair(Of Long, Double)(Equation.Handle, Equation.Value)
                          Select Data0Expr).ToArray
            Dim DeltaData = New KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())(_kernelModule.RuntimeTicks, LQuery)
            Return DeltaData
        End Function

        Public Overridable Function CreateDataPackage() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim CsvData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
                New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Call CsvData.Add(New String() {"RTimeTicks"})
            Call CsvData.First.AddRange(__getHeaders)
            Dim LQuery = (From item In Me._innerBuffer.AsParallel
                          Select RowData = __createRow(item)
                          Order By RowData.First Ascending).ToArray
            Call CsvData.AppendRange(LQuery)

            Return CsvData
        End Function

        Private Shared Function __createRow(x As KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
            Dim Row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject = New String() {}
            Call Row.Add(x.Key)
            Call Row.AddRange((From it In x.Value Select CStr(it.Value)).ToArray)
            Return Row
        End Function

        ''' <summary>
        ''' 按照Handle顺序排列Headers字符串
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function __getHeaders() As String()
    End Class
End Namespace
