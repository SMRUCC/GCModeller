Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.CRISPR.SearchingModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.gbExportService
Imports LANS.SystemsBiology
Imports LANS.SystemsBiology.NCBI.Extensions.Analysis
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Namespace Output

    Public Class Loci : Implements I_PolymerSequenceModel

        ''' <summary>
        ''' 起始位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Left As Integer
        <XmlAttribute> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        Public ReadOnly Property Right As Integer
            Get
                Return Left + Len(SequenceData)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("")
        End Function
    End Class
End Namespace