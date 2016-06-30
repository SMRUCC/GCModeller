Imports System.Data.Linq.Mapping

''' <summary>
''' The object which its entry information was stores in the repository database but data stores on the filesystem. 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class DbFileSystemObject

    Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.sIdEnumerable
    Implements DbFileSystemObject.DescriptionData

    <Column(dbtype:="varchar(2048)", name:="definition")> Public Property Definition As String Implements DescriptionData.Description
    <Column(DbType:="varchar(128)", Name:="locus_id")> Public Property LocusID As String Implements DescriptionData.locusId, Microsoft.VisualBasic.ComponentModel.Collection.Generic.sIdEnumerable.Identifier
    <Column(dbtype:="varchar(1024)", name:="md5_hash")> Public Property MD5Hash As String

    ''' <summary>
    ''' 本对象所指向的文件不存在或者哈希值比对不上，都会返回False
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifyData(RepositoryRoot As String, Query As String) As Boolean
        Dim Path As String = GetPath(RepositoryRoot)
        If Not FileIO.FileSystem.FileExists(Path) Then
            Return False
        Else
            Return String.Equals(MD5Hash, SecurityString.GetFileHashString(Query))
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <param name="RepositoryRoot">数据源的根目录</param>
    ''' <remarks></remarks>
    Public MustOverride Function GetPath(RepositoryRoot As String) As String

    Public Overrides Function ToString() As String
        Return String.Format("{0}:  {1};  {2}", LocusID, Definition, MD5Hash)
    End Function

    Public Interface DescriptionData

        ''' <summary>
        ''' 基因号或者数据库之中的编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property locusId As String

        ''' <summary>
        ''' 功能注释
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Description As String
    End Interface

    ''' <summary>
    ''' 用于描述一个蛋白质的注释源信息的数据模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ProteinDescriptionModel : Inherits DescriptionData

        Property COG As String

        ''' <summary>
        ''' 物种来源
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property OrganismSpecies As String
    End Interface
End Class
