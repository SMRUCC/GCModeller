Imports System.Data.Linq.Mapping
Imports System.Data.SQLite.Linq.DataMapping.Interface.Reflector

Namespace Tables

    <Table(name:="genbank_entry_info")>
    Public Class GenbankEntryInfo : Inherits DbFileSystemObject

        <Column(dbtype:="varchar(128)", name:="gi", isprimarykey:=True)> Public Property GI As String
        <Column(dbtype:="varchar(2048)", name:="species")> Public Property Species As String
        <Column(dbtype:="varchar(1024)", name:="plasmid")> Public Property plasmid As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}/Genbank/{1}/{2}/{3}.gbk", "RepositoryRoot://", Species, LocusID.First.ToString, LocusID)
        End Function

        ''' <summary>
        ''' Genbank文件的文件相对路径
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="RepositoryRoot">数据源的根目录</param>
        ''' <remarks></remarks>
        Public Overrides Function GetPath(RepositoryRoot As String) As String
            Dim Path As String = String.Format("{0}/Genbank/{1}/{2}/{3}.gbk", RepositoryRoot, Species, LocusID.First.ToString, LocusID)
            Return FileIO.FileSystem.GetFileInfo(Path).FullName
        End Function

        Private Function InternalUpdateInfo(RepositoryRoot As String) As GenbankEntryInfo
            Dim File As String = GetPath(RepositoryRoot)

            If Not File.FileExists() Then
                Return Nothing  '数据入口点已经失效了
            End If

            Dim Genbank As LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File =
                LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File.Read(Path:=File)
            Me.LocusID = Genbank.Locus.AccessionID
            Me.GI = Genbank.Version.GI
            Me.Definition = Genbank.Definition.Value.Replace("'", "")
            Me.Species = Genbank.Source.OrganismHierarchy.Categorys.Last
            Me.plasmid = Genbank.SourceFeature.Query("plasmid")
            Me.MD5Hash = SecurityString.GetFileHashString(File)

            Return Me
        End Function

        ''' <summary>
        ''' Only using for the development when you have change the definition of this table, then using this function to update the database.(在更新了数据表的结构之后，请使用本方法更新数据库)
        ''' </summary>
        ''' <param name="DB"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function UpdateTableSchema(DB As SQLEngines.SQLiteIndex) As Boolean
            Dim TableSchema = GetType(GenbankEntryInfo)
            Dim Data = (From item In System.Data.SQLite.Linq.DataMapping.Interface.Reflector.Load(DB.SQLiteEngine, TableSchema) Select DirectCast(item, GenbankEntryInfo)).ToArray
            Data = (From item As GenbankEntryInfo In Data.AsParallel Let obj = item.InternalUpdateInfo(DB.RepositoryRoot) Where Not obj Is Nothing Select obj).ToArray '更新数据
            '删除原来的表
            Call DB.SQLiteEngine.DeleteTable(TableSchema)
            Call DB.SQLiteEngine.CreateTableFor(TableSchema)
            '插入新的数据
            Return DB.SQLiteEngine.Insert(data:=Data)
        End Function
    End Class
End Namespace
