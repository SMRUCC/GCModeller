#Region "Microsoft.VisualBasic::8788b86452adedc1f7b7766e94f29b27, GCModeller\data\MicrobesOnline\MySQL\genomics\taxnode.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 84
    '    Code Lines: 45
    ' Comment Lines: 32
    '   Blank Lines: 7
    '     File Size: 4.97 KB


    ' Class taxnode
    ' 
    '     Properties: comments, divId, embl, flag1, flag2
    '                 flag3, flag4, flag5, flag6, flag7
    '                 parentId, rank, taxonomyId
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `taxnode`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxnode` (
'''   `taxonomyId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `parentId` int(10) unsigned DEFAULT NULL,
'''   `rank` varchar(50) DEFAULT NULL,
'''   `embl` varchar(10) DEFAULT NULL,
'''   `divId` int(3) unsigned NOT NULL DEFAULT '0',
'''   `flag1` int(1) unsigned NOT NULL DEFAULT '0',
'''   `flag2` int(1) unsigned NOT NULL DEFAULT '0',
'''   `flag3` int(1) unsigned NOT NULL DEFAULT '0',
'''   `flag4` int(1) unsigned NOT NULL DEFAULT '0',
'''   `flag5` int(1) unsigned NOT NULL DEFAULT '0',
'''   `flag6` int(1) unsigned NOT NULL DEFAULT '0',
'''   `flag7` int(1) unsigned NOT NULL DEFAULT '0',
'''   `comments` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`taxonomyId`),
'''   KEY `parentId` (`parentId`),
'''   KEY `rank` (`rank`),
'''   KEY `divisionId` (`divId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxnode")>
Public Class taxnode: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("taxonomyId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
    <DatabaseField("parentId"), DataType(MySqlDbType.Int64, "10")> Public Property parentId As Long
    <DatabaseField("rank"), DataType(MySqlDbType.VarChar, "50")> Public Property rank As String
    <DatabaseField("embl"), DataType(MySqlDbType.VarChar, "10")> Public Property embl As String
    <DatabaseField("divId"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property divId As Long
    <DatabaseField("flag1"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag1 As Long
    <DatabaseField("flag2"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag2 As Long
    <DatabaseField("flag3"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag3 As Long
    <DatabaseField("flag4"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag4 As Long
    <DatabaseField("flag5"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag5 As Long
    <DatabaseField("flag6"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag6 As Long
    <DatabaseField("flag7"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property flag7 As Long
    <DatabaseField("comments"), DataType(MySqlDbType.VarChar, "255")> Public Property comments As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxnode` (`taxonomyId`, `parentId`, `rank`, `embl`, `divId`, `flag1`, `flag2`, `flag3`, `flag4`, `flag5`, `flag6`, `flag7`, `comments`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxnode` (`taxonomyId`, `parentId`, `rank`, `embl`, `divId`, `flag1`, `flag2`, `flag3`, `flag4`, `flag5`, `flag6`, `flag7`, `comments`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxnode` WHERE `taxonomyId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxnode` SET `taxonomyId`='{0}', `parentId`='{1}', `rank`='{2}', `embl`='{3}', `divId`='{4}', `flag1`='{5}', `flag2`='{6}', `flag3`='{7}', `flag4`='{8}', `flag5`='{9}', `flag6`='{10}', `flag7`='{11}', `comments`='{12}' WHERE `taxonomyId` = '{13}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, taxonomyId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, taxonomyId, parentId, rank, embl, divId, flag1, flag2, flag3, flag4, flag5, flag6, flag7, comments)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, taxonomyId, parentId, rank, embl, divId, flag1, flag2, flag3, flag4, flag5, flag6, flag7, comments)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, taxonomyId, parentId, rank, embl, divId, flag1, flag2, flag3, flag4, flag5, flag6, flag7, comments, taxonomyId)
    End Function
#End Region
End Class


End Namespace
