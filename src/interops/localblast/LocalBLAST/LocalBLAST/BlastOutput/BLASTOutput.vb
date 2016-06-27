Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput

    ''' <summary>
    ''' Blast程序结果对外输出的统一接口类型对象
    ''' </summary>
    ''' <remarks>
    ''' Reader文件夹之下为各种格式的日志文件的读取类对象
    ''' 对于BLAST日志文件，则有一个BlastLogFile对象作为对外保存和其他程序读取的统一接口
    ''' </remarks>
    Public MustInherit Class IBlastOutput : Inherits ITextFile

        Public Const HITS_NOT_FOUND As String = "HITS_NOT_FOUND"

        <XmlIgnore> <ScriptIgnore>
        Public Shadows Property FilePath As String
            Get
                Return MyBase.FilePath
            End Get
            Set(value As String)
                MyBase.FilePath = value
            End Set
        End Property

        Public MustOverride Function Grep(Query As TextGrepMethod, Hits As TextGrepMethod) As IBlastOutput
        ''' <summary>
        ''' 仅导出每条记录的第一个最佳匹配的结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
        Public MustOverride Function ExportOverview() As Overview
        ''' <summary>
        ''' 导出每条记录中的所有最佳的匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()

        <XmlElement("BlastOutput_db")>
        Public Overridable Property Database As String

        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        Public MustOverride Function CheckIntegrity(QuerySource As FastaFile) As Boolean
    End Class
End Namespace