Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports LANS.SystemsBiology.ProteinModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Public Module LinqProvider

    <LinqEntity("GeneObject", GetType(FileStream.GeneObject))>
    Public Function GetTabularGenes(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.GeneObject)(False)
    End Function

    <LinqEntity("Metabolite", GetType(FileStream.Metabolite))>
    Public Function GetTabularMetabolites(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.Metabolite)(False)
    End Function

    <LinqEntity("TranscriptUnit", GetType(FileStream.TranscriptUnit))>
    Public Function GetTabularTranscriptUnit(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.TranscriptUnit)(False)
    End Function

    <LinqEntity("Transcript", GetType(FileStream.Transcript))>
    Public Function GetTabularTranscripts(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.Transcript)(False)
    End Function

    <LinqEntity("ProteinModel", GetType(Protein))>
    Public Function GetSMARTLDM(path As String) As IEnumerable
        Return path.LoadCsv(Of Protein)
    End Function
End Module
