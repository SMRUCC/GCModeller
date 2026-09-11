Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.SAM

''' <summary>
''' SAM / FastQ 通用读写辅助函数。
''' </summary>
''' <remarks>
''' 本模块保留了原 ``Perl.vb`` 之中与外部 Perl/R 无关的通用读写能力；
''' 原先通过调用外部 Perl/R 运行时实现 TSSAR 的代码已全部移除，
''' 改由纯 VB.NET 的 <see cref="TSSAR"/> 模块实现。
''' </remarks>
Public Module AlignmentIO

    ''' <summary>
    ''' 读取 FastQ 文件。
    ''' </summary>
    <ExportAPI("Read.Fastaq")>
    Public Function LoadFastaq(Path As String) As FastQFile
        Return FastQFile.Load(Path)
    End Function

    ''' <summary>
    ''' 读取 SAM 比对文件。
    ''' </summary>
    <ExportAPI("Read.SAM")>
    Public Function LoadSAM(Path As String) As SAM
        Return SAM.Load(Path)
    End Function

    ''' <summary>
    ''' 把比对片段导出为 CSV 文件。
    ''' </summary>
    <ExportAPI("Write.Csv.MappingReads")>
    Public Function SaveAlignmentReadsMapping(data As IEnumerable(Of AlignmentReads), SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function
End Module
