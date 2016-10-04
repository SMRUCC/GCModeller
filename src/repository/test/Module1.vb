Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Data.Repository.NCBI

Module Module1

    Sub Main()
        Dim cnn As New ConnectionUri With {
            .Database = "ncbi",
            .IPAddress = "127.0.0.1",
            .Password = "1234",
            .User = "root",
            .ServicesPort = 3306
        }
        Dim mysql As New MySQL(cnn)

        Call mysql.[Imports]("D:\GCModeller\src\repository\data\test_virus_nt.fna", "D:\GCModeller\src\repository\data\DATA\")
    End Sub
End Module
