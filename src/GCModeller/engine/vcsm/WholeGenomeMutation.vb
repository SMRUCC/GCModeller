Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine

Public Class WholeGenomeMutation

    Dim OriginalCommandl As String

    Sub New(CommandLine As String)
        Me.OriginalCommandl = CommandLine
    End Sub

    Public Function Invoke() As Integer
        Dim CmdlData = CommandLine.TryParse(OriginalCommandl)
        Dim ModelFilePath As String = CmdlData.Item("-i")
        Dim ModelDir As String = FileIO.FileSystem.GetParentPath(ModelFilePath)
        '    Dim GeneObjects = ModelFilePath.LoadXml(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.XmlFormat.CellSystemXmlModel)().Transcript.GetFullPath(ModelDir).LoadCsv(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.Transcript)(False)

        'For Each Gene In GeneObjects
        '    Dim CommandLine As String = String.Format("{0} -gene_mutations {1}|{2}", OriginalCommandl, Gene.Template, CmdlData.Item("-factor"))

        'Next

        Return 0
    End Function
End Class
