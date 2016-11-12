#Region "Microsoft.VisualBasic::697ce25c0b748f0a8bfdbe4a19cf9334, ..\GCModeller\analysis\annoTools\DataMySql\CEG\MySQL\CEG\ceg_core.vb"

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

REM  Dump @12/3/2015 8:51:02 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace CEG.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `ceg_core`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `ceg_core` (
'''   `access_num` varchar(50) DEFAULT NULL,
'''   `gid` varchar(25) NOT NULL DEFAULT '',
'''   `koid` varchar(30) DEFAULT NULL,
'''   `cogid` varchar(255) NOT NULL,
'''   `hprd_nid` varchar(12) DEFAULT NULL,
'''   `nhit_ref` varchar(12) DEFAULT NULL,
'''   `nevalue` varchar(12) DEFAULT NULL,
'''   `nscore` varchar(20) DEFAULT NULL,
'''   `hprd_aid` varchar(20) NOT NULL,
'''   `ahit_ref` varchar(20) NOT NULL,
'''   `aevalue` varchar(20) NOT NULL,
'''   `ascore` varchar(20) NOT NULL,
'''   `degid` varchar(15) NOT NULL,
'''   `organismid` int(4) NOT NULL,
'''   PRIMARY KEY (`gid`),
'''   FULLTEXT KEY `gid` (`gid`,`access_num`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ceg_core", Database:="ceg")>
Public Class ceg_core: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("access_num"), DataType(MySqlDbType.VarChar, "50")> Public Property access_num As String
    <DatabaseField("gid"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25")> Public Property gid As String
    <DatabaseField("koid"), DataType(MySqlDbType.VarChar, "30")> Public Property koid As String
    <DatabaseField("cogid"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property cogid As String
    <DatabaseField("hprd_nid"), DataType(MySqlDbType.VarChar, "12")> Public Property hprd_nid As String
    <DatabaseField("nhit_ref"), DataType(MySqlDbType.VarChar, "12")> Public Property nhit_ref As String
    <DatabaseField("nevalue"), DataType(MySqlDbType.VarChar, "12")> Public Property nevalue As String
    <DatabaseField("nscore"), DataType(MySqlDbType.VarChar, "20")> Public Property nscore As String
    <DatabaseField("hprd_aid"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property hprd_aid As String
    <DatabaseField("ahit_ref"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property ahit_ref As String
    <DatabaseField("aevalue"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property aevalue As String
    <DatabaseField("ascore"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property ascore As String
    <DatabaseField("degid"), NotNull, DataType(MySqlDbType.VarChar, "15")> Public Property degid As String
    <DatabaseField("organismid"), NotNull, DataType(MySqlDbType.Int64, "4")> Public Property organismid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ceg_core` WHERE `gid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ceg_core` SET `access_num`='{0}', `gid`='{1}', `koid`='{2}', `cogid`='{3}', `hprd_nid`='{4}', `nhit_ref`='{5}', `nevalue`='{6}', `nscore`='{7}', `hprd_aid`='{8}', `ahit_ref`='{9}', `aevalue`='{10}', `ascore`='{11}', `degid`='{12}', `organismid`='{13}' WHERE `gid` = '{14}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid, gid)
    End Function
#End Region
End Class


End Namespace
