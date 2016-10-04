Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Data.Repository.NCBI

Module Module1

    Sub Main()

        'Try
        '    Dim reader = "X:\cache".OpenBinaryReader
        'Catch ex As Exception
        '    Call ex.PrintException
        'End Try


        Call testIndex()

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


    Sub testIndex()

        Dim file = "D:\GCModeller\src\repository\data\DATA\gb\gb-29.nt".ReadAllLines
        Dim nt = file.Select(Function(s) s.Split(ASCII.TAB)).ToDictionary(Function(x) x.First, Function(x) x.Last)  ' 直接文件读取然后建立字典
        Dim index As New Index("D:\GCModeller\src\repository\data\DATA\", "gb", "gb-29")  ' 打开数据库文件句柄，并建立索引

        For Each gi$ In nt.Keys
            Call (nt(gi$) = index.ReadNT_by_gi(gi$)).__DEBUG_ECHO  ' 测试NCBI数据库索引服务能否正确读取序列数据
        Next

        Pause()
    End Sub
End Module
