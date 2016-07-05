#Region "Microsoft.VisualBasic::ed7399411849e640dc49539985061088, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\frontpage_2_frontpageitem.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `frontpage_2_frontpageitem`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `frontpage_2_frontpageitem` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `frontPageItem_rank` int(10) unsigned DEFAULT NULL,
'''   `frontPageItem` int(10) unsigned DEFAULT NULL,
'''   `frontPageItem_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `frontPageItem` (`frontPageItem`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("frontpage_2_frontpageitem", Database:="gk_current")>
Public Class frontpage_2_frontpageitem: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("frontPageItem_rank"), DataType(MySqlDbType.Int64, "10")> Public Property frontPageItem_rank As Long
    <DatabaseField("frontPageItem"), DataType(MySqlDbType.Int64, "10")> Public Property frontPageItem As Long
    <DatabaseField("frontPageItem_class"), DataType(MySqlDbType.VarChar, "64")> Public Property frontPageItem_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `frontpage_2_frontpageitem` (`DB_ID`, `frontPageItem_rank`, `frontPageItem`, `frontPageItem_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `frontpage_2_frontpageitem` (`DB_ID`, `frontPageItem_rank`, `frontPageItem`, `frontPageItem_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `frontpage_2_frontpageitem` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `frontpage_2_frontpageitem` SET `DB_ID`='{0}', `frontPageItem_rank`='{1}', `frontPageItem`='{2}', `frontPageItem_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, frontPageItem_rank, frontPageItem, frontPageItem_class)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, frontPageItem_rank, frontPageItem, frontPageItem_class)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, frontPageItem_rank, frontPageItem, frontPageItem_class, DB_ID)
    End Function
#End Region
End Class


End Namespace

