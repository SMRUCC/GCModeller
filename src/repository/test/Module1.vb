Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Data.Repository.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()

        'Call ASCII.Symbols.GetJson.__DEBUG_ECHO


        'Dim engine As New QueryEngine()

        'Dim size& = engine.ScanSeqDatabase("D:\GCModeller\src\repository\data\DATA\")

        'Dim fasta As New FastaFile(engine.Search("""1-OP3-PA-USA-2006"" OR ""C11-OP12-TX-USA-2007"""))

        'Call fasta.Save("x:\gggg.fa")


        'Pause()

        'Call testIndex()

        Dim cnn As New ConnectionUri With {
            .Database = "ncbi",
            .IPAddress = "127.0.0.1",
            .Password = "1234",
            .User = "root",
            .ServicesPort = 3306
        }



        'MsgBox(size)

        'Try
        '    Dim reader = "X:\cache".OpenBinaryReader
        'Catch ex As Exception
        '    Call ex.PrintException
        'End Try

        Dim mysql As New MySQL(cnn)

        Call mysql.[Imports]("D:\virus_nt\nt_NCBI_virus.fasta", "D:\virus_nt\$DATA\", False)
        Call testIndex()
    End Sub


    Sub testIndex()

        Dim file = "D:\GCModeller\src\repository\data\DATA\gb\gb-29.nt".ReadAllLines
        Dim nt = file.Select(Function(s) s.Split(ASCII.TAB)).ToDictionary(Function(x) x.First, Function(x) x.Last)  ' 直接文件读取然后建立字典
        Dim index As New Index("D:\GCModeller\src\repository\data\DATA\", "gb", "gb-29")  ' 打开数据库文件句柄，并建立索引

        For Each gi$ In nt.Keys
            Call (nt(gi$) = index.ReadNT_by_gi(gi$)).__DEBUG_ECHO  ' 测试NCBI数据库索引服务能否正确读取序列数据
        Next

        file = "D:\GCModeller\src\repository\data\DATA\headers\gb\gb-18.txt".ReadAllLines
        nt = file.Select(Function(s) s.Split(ASCII.TAB)).ToDictionary(Function(x) x.First, Function(x) x.Last)

        Dim titleIndex As New TitleIndex("D:\GCModeller\src\repository\data\DATA\", "gb", "gb-18")

        For Each s In titleIndex.EnumerateTitles
            Call s.GetJson.__DEBUG_ECHO
        Next

        For Each gi$ In titleIndex.giKeys
            Call titleIndex.ReadHeader_by_gi(gi$).__DEBUG_ECHO
        Next

        For Each tag In nt.Keys
            Call (nt(tag) = titleIndex.ReadHeader_by_locus_Tag(tag)).__DEBUG_ECHO
        Next

        Pause()
    End Sub
End Module
