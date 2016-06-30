Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Namespace DataStorage.FileModel

    ''' <summary>
    ''' GCModeller的内核驱动程序的数据服务
    ''' </summary>
    ''' <remarks></remarks>
    <[PackageNamespace]("Kernel_Driver.Data_Services", Publisher:="GCModeller Virtual Cell System")>
    Public Module Extensions

        ''' <summary>
        ''' 加载<see cref="Integer"></see>类型的计算数据
        ''' </summary>
        ''' <param name="Csv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Load.Csv.Integer_Samples")>
        <Extension> Public Function LoadData_Integer(Csv As File) As DataSerials(Of Integer)()
            Dim LQuery As DataSerials(Of Integer)() = (From row As RowObject In Csv.Skip(1).ToArray.AsParallel
                                                       Let sample As Integer() = (From s As String In row.Skip(1).ToArray Let n As Integer = CInt(Val(s)) Select n).ToArray
                                                       Let x As DataSerials(Of Integer) = New DataSerials(Of Integer) With {
                                                       .UniqueId = row.First,
                                                       .Samples = sample
                                                   }
                                                       Select x).ToArray
            Return LQuery.AddHandle.ToArray
        End Function

        <ExportAPI("Load.Csv.Double_Samples")>
        <Extension> Public Function LoadData_Double(Csv As File) As DataSerials(Of Double)()
            Dim LQuery As DataSerials(Of Double)() = (
        From item
        In Csv.Skip(1).ToArray.AsParallel
        Select New DataSerials(Of Double) With {
            .UniqueId = item.First,
            .Samples = (From s As String In item.Skip(1) Select Val(s)).ToArray}).ToArray
            Return LQuery.AddHandle.ToArray
        End Function

        <ExportAPI("Load.Csv.Boolean_Samples")>
        <Extension> Public Function LoadData_Boolean(Csv As File) As DataSerials(Of Boolean)()
            Dim LQuery As DataSerials(Of Boolean)() = (
        From item
        In Csv.Skip(1).ToArray.AsParallel
        Select New DataSerials(Of Boolean) With {
            .UniqueId = item.First,
            .Samples = (From s As String In item.Skip(1) Select CBool(s)).ToArray}).ToArray
            Return LQuery.AddHandle.ToArray
        End Function

        <Extension> Public Function TakeSamples(Of T)(data As IEnumerable(Of DataSerials(Of T)),
                                                  start As Integer,
                                                  Optional counts As Integer = -1) As DataSerials(Of T)()
            Return DataSerials(Of T).GetRanges(data, start, counts)
        End Function

        ''' <summary>
        ''' 获取实验数据的采样数目
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function SampleCounts(Of T)(data As IEnumerable(Of DataSerials(Of T))) As Integer
            If data.IsNullOrEmpty Then
                Return 0
            End If
            Return data.First.Samples.GetElementCounts
        End Function
    End Module
End Namespace