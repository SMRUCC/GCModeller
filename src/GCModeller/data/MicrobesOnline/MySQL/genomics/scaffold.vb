#Region "Microsoft.VisualBasic::d3e4e3f8a732580aab8adba7d238c623, ..\GCModeller\data\MicrobesOnline\MySQL\genomics\scaffold.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
''' DROP TABLE IF EXISTS `scaffold`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `scaffold` (
'''   `scaffoldId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `chr_num` int(10) unsigned DEFAULT '1',
'''   `isCircular` int(1) DEFAULT NULL,
'''   `length` int(10) unsigned DEFAULT NULL,
'''   `file` varchar(32) DEFAULT NULL,
'''   `isGenomic` int(1) DEFAULT NULL,
'''   `gi` int(10) unsigned DEFAULT NULL,
'''   `taxonomyId` int(10) DEFAULT '0',
'''   `comment` varchar(255) DEFAULT NULL,
'''   `isActive` int(1) DEFAULT '1',
'''   `isPartial` int(1) DEFAULT '0',
'''   `created` date DEFAULT '0000-00-00',
'''   `allowUpdates` tinyint(3) unsigned NOT NULL DEFAULT '1',
'''   `ncbiProjectId` int(10) DEFAULT NULL,
'''   `md5` char(32) DEFAULT NULL,
'''   PRIMARY KEY (`scaffoldId`),
'''   KEY `Indx_Scaffold_file` (`file`),
'''   KEY `comment` (`comment`),
'''   KEY `taxonomyId` (`taxonomyId`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=952581 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("scaffold")>
Public Class scaffold: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("scaffoldId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("chr_num"), DataType(MySqlDbType.Int64, "10")> Public Property chr_num As Long
    <DatabaseField("isCircular"), DataType(MySqlDbType.Int64, "1")> Public Property isCircular As Long
    <DatabaseField("length"), DataType(MySqlDbType.Int64, "10")> Public Property length As Long
    <DatabaseField("file"), DataType(MySqlDbType.VarChar, "32")> Public Property file As String
    <DatabaseField("isGenomic"), DataType(MySqlDbType.Int64, "1")> Public Property isGenomic As Long
    <DatabaseField("gi"), DataType(MySqlDbType.Int64, "10")> Public Property gi As Long
    <DatabaseField("taxonomyId"), DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
    <DatabaseField("comment"), DataType(MySqlDbType.VarChar, "255")> Public Property comment As String
    <DatabaseField("isActive"), DataType(MySqlDbType.Int64, "1")> Public Property isActive As Long
    <DatabaseField("isPartial"), DataType(MySqlDbType.Int64, "1")> Public Property isPartial As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("allowUpdates"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property allowUpdates As Long
    <DatabaseField("ncbiProjectId"), DataType(MySqlDbType.Int64, "10")> Public Property ncbiProjectId As Long
    <DatabaseField("md5"), DataType(MySqlDbType.VarChar, "32")> Public Property md5 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `scaffold` (`chr_num`, `isCircular`, `length`, `file`, `isGenomic`, `gi`, `taxonomyId`, `comment`, `isActive`, `isPartial`, `created`, `allowUpdates`, `ncbiProjectId`, `md5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `scaffold` (`chr_num`, `isCircular`, `length`, `file`, `isGenomic`, `gi`, `taxonomyId`, `comment`, `isActive`, `isPartial`, `created`, `allowUpdates`, `ncbiProjectId`, `md5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `scaffold` WHERE `scaffoldId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `scaffold` SET `scaffoldId`='{0}', `chr_num`='{1}', `isCircular`='{2}', `length`='{3}', `file`='{4}', `isGenomic`='{5}', `gi`='{6}', `taxonomyId`='{7}', `comment`='{8}', `isActive`='{9}', `isPartial`='{10}', `created`='{11}', `allowUpdates`='{12}', `ncbiProjectId`='{13}', `md5`='{14}' WHERE `scaffoldId` = '{15}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, scaffoldId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, chr_num, isCircular, length, file, isGenomic, gi, taxonomyId, comment, isActive, isPartial, DataType.ToMySqlDateTimeString(created), allowUpdates, ncbiProjectId, md5)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, chr_num, isCircular, length, file, isGenomic, gi, taxonomyId, comment, isActive, isPartial, DataType.ToMySqlDateTimeString(created), allowUpdates, ncbiProjectId, md5)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, scaffoldId, chr_num, isCircular, length, file, isGenomic, gi, taxonomyId, comment, isActive, isPartial, DataType.ToMySqlDateTimeString(created), allowUpdates, ncbiProjectId, md5, scaffoldId)
    End Function
#End Region
End Class


End Namespace
