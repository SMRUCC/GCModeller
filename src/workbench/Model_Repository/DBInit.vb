Imports System.Data.SQLite.Linq.DataMapping.Interface.Reflector
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Model_Repository.SQLEngines

''' <summary>
''' 这个模块之中的命令当且仅当第一次创建数据库的时候使用，假若在已经创建了数据库的情况之下使用，则会重置整个数据库
''' </summary>
''' <remarks></remarks>
Public Module DBInit

    Public ReadOnly Property RepositoryEngine As SQLiteIndex

    Sub New()
        Call Settings.Initialize()
        DBInit.RepositoryEngine = New SQLiteIndex(Settings.SettingsFile.RepositoryRoot)
    End Sub

    Public Function InvokeOperation(Connection As SQLEngines.SQLiteIndex) As Boolean
        If Connection Is Nothing OrElse Connection.SQLiteEngine Is Nothing Then
            Return False
        Else
            Call Connection.Close()
            Call FileIO.FileSystem.DeleteFile(Connection.SQLiteEngine.URL, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
            Connection = New SQLEngines.SQLiteIndex(Connection.RepositoryRoot)  '重新连接数据并初始化数据库
        End If

        '导入现有的数据
        Dim GenbankDir As String = Connection.RepositoryRoot & "/Genbank/"
        Call DBInit.UpdateGenbankEntryInfo(GenbankDir, Cnn:=Connection)

        Return True
    End Function

    ''' <summary>
    ''' 由于文件已经存放于数据库的文件夹之中，故而不会再进行文件的复制操作，函数只需要生成数据库数据就可以了
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateGenbankEntryInfo(dir As String, Cnn As SQLEngines.SQLiteIndex) As Boolean
        Dim FileList = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.gb", "*.gbk")
        Dim EntryList = (From Path As String In FileList
                         Let Genbank As GBFF.File = GBFF.File.Read(Path)
                         Let Entry As Tables.GenbankEntryInfo =
                             New Tables.GenbankEntryInfo With {
                                .MD5Hash = SecurityString.GetFileHashString(Path),
                                .LocusID = Genbank.Locus.AccessionID,
                                .plasmid = Genbank.SourceFeature.Query("plasmid"),
                                .GI = Genbank.Version.GI,
                                .Definition = Genbank.Definition.Value.Replace("'", ""),
                                .Species = Genbank.Source.OrganismHierarchy.Categorys.Last
                            }
                         Select Cnn.SQLiteEngine.Insert(obj:=Entry)).ToArray
        Return True
    End Function
End Module
