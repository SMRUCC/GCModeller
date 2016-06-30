Namespace RegulonDB

    Public Class RegulonDB : Inherits LANS.SystemsBiology.ExternalDBSource.MySQLDbReflector

        Sub New(MySQL As Oracle.LinuxCompatibility.MySQL.Client.ConnectionUri)
            Call MyBase.New(MySQL)
        End Sub

        Public Function ExportSites() As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of ExternalDBSource.RegulonDB.Site)("select * from site")
            Dim File = LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaExportMethods.Export(Table.ToArray)
            Return File
        End Function
    End Class
End Namespace