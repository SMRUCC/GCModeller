#Region "Microsoft.VisualBasic::aeb9064f010570c1da24f4849ad38522, DataMySql\CEG\MySQL\ceg_core.vb"

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

    ' Class ceg_core
    ' 
    '     Properties: access_num, aevalue, ahit_ref, ascore, cogid
    '                 degid, gid, hprd_aid, hprd_nid, koid
    '                 nevalue, nhit_ref, nscore, organismid
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:39


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace CEG.MySQL

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ceg_core", Database:="ceg", SchemaSQL:="
CREATE TABLE `ceg_core` (
  `access_num` varchar(50) DEFAULT NULL,
  `gid` varchar(25) NOT NULL DEFAULT '',
  `koid` varchar(30) DEFAULT NULL,
  `cogid` varchar(255) NOT NULL,
  `hprd_nid` varchar(12) DEFAULT NULL,
  `nhit_ref` varchar(12) DEFAULT NULL,
  `nevalue` varchar(12) DEFAULT NULL,
  `nscore` varchar(20) DEFAULT NULL,
  `hprd_aid` varchar(20) NOT NULL,
  `ahit_ref` varchar(20) NOT NULL,
  `aevalue` varchar(20) NOT NULL,
  `ascore` varchar(20) NOT NULL,
  `degid` varchar(15) NOT NULL,
  `organismid` int(4) NOT NULL,
  PRIMARY KEY (`gid`),
  FULLTEXT KEY `gid` (`gid`,`access_num`)
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;")>
Public Class ceg_core: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("access_num"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="access_num")> Public Property access_num As String
    <DatabaseField("gid"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25"), Column(Name:="gid"), XmlAttribute> Public Property gid As String
    <DatabaseField("koid"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="koid")> Public Property koid As String
    <DatabaseField("cogid"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="cogid")> Public Property cogid As String
    <DatabaseField("hprd_nid"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="hprd_nid")> Public Property hprd_nid As String
    <DatabaseField("nhit_ref"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="nhit_ref")> Public Property nhit_ref As String
    <DatabaseField("nevalue"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="nevalue")> Public Property nevalue As String
    <DatabaseField("nscore"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="nscore")> Public Property nscore As String
    <DatabaseField("hprd_aid"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="hprd_aid")> Public Property hprd_aid As String
    <DatabaseField("ahit_ref"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="ahit_ref")> Public Property ahit_ref As String
    <DatabaseField("aevalue"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="aevalue")> Public Property aevalue As String
    <DatabaseField("ascore"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="ascore")> Public Property ascore As String
    <DatabaseField("degid"), NotNull, DataType(MySqlDbType.VarChar, "15"), Column(Name:="degid")> Public Property degid As String
    <DatabaseField("organismid"), NotNull, DataType(MySqlDbType.Int64, "4"), Column(Name:="organismid")> Public Property organismid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `ceg_core` WHERE `gid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `ceg_core` SET `access_num`='{0}', `gid`='{1}', `koid`='{2}', `cogid`='{3}', `hprd_nid`='{4}', `nhit_ref`='{5}', `nevalue`='{6}', `nscore`='{7}', `hprd_aid`='{8}', `ahit_ref`='{9}', `aevalue`='{10}', `ascore`='{11}', `degid`='{12}', `organismid`='{13}' WHERE `gid` = '{14}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `ceg_core` WHERE `gid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
        Else
        Return String.Format(INSERT_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{access_num}', '{gid}', '{koid}', '{cogid}', '{hprd_nid}', '{nhit_ref}', '{nevalue}', '{nscore}', '{hprd_aid}', '{ahit_ref}', '{aevalue}', '{ascore}', '{degid}', '{organismid}')"
        Else
            Return $"('{access_num}', '{gid}', '{koid}', '{cogid}', '{hprd_nid}', '{nhit_ref}', '{nevalue}', '{nscore}', '{hprd_aid}', '{ahit_ref}', '{aevalue}', '{ascore}', '{degid}', '{organismid}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `ceg_core` (`access_num`, `gid`, `koid`, `cogid`, `hprd_nid`, `nhit_ref`, `nevalue`, `nscore`, `hprd_aid`, `ahit_ref`, `aevalue`, `ascore`, `degid`, `organismid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
        Else
        Return String.Format(REPLACE_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `ceg_core` SET `access_num`='{0}', `gid`='{1}', `koid`='{2}', `cogid`='{3}', `hprd_nid`='{4}', `nhit_ref`='{5}', `nevalue`='{6}', `nscore`='{7}', `hprd_aid`='{8}', `ahit_ref`='{9}', `aevalue`='{10}', `ascore`='{11}', `degid`='{12}', `organismid`='{13}' WHERE `gid` = '{14}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, access_num, gid, koid, cogid, hprd_nid, nhit_ref, nevalue, nscore, hprd_aid, ahit_ref, aevalue, ascore, degid, organismid, gid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As ceg_core
                         Return DirectCast(MyClass.MemberwiseClone, ceg_core)
                     End Function
End Class


End Namespace
