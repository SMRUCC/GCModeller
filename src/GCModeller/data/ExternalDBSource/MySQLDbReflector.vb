Public MustInherit Class MySQLDbReflector

    Protected Friend DbReflector As Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector
    '  Protected Friend Export As SMRUCC.genomics.SequenceModel.FASTA.FsaExport = New SequenceModel.FASTA.FsaExport

    Protected Friend Sub New(MySQL As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
        DbReflector = New Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector(MySQL.GetConnectionString)
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0}::{1}", Me.GetType.Name, DbReflector.ToString)
    End Function
End Class
