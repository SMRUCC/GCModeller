Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.CRISPR.SearchingModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService
Imports LANS.SystemsBiology
Imports SMRUCC.genomics.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.ComponentModel.Loci
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