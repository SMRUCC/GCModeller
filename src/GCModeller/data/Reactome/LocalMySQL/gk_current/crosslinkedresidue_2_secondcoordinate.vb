#Region "Microsoft.VisualBasic::006a983c26416b8534b81fb240942edb, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\crosslinkedresidue_2_secondcoordinate.vb"

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

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `crosslinkedresidue_2_secondcoordinate`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `crosslinkedresidue_2_secondcoordinate` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `secondCoordinate_rank` int(10) unsigned DEFAULT NULL,
'''   `secondCoordinate` int(10) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `secondCoordinate` (`secondCoordinate`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("crosslinkedresidue_2_secondcoordinate", Database:="gk_current")>
Public Class crosslinkedresidue_2_secondcoordinate: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("secondCoordinate_rank"), DataType(MySqlDbType.Int64, "10")> Public Property secondCoordinate_rank As Long
    <DatabaseField("secondCoordinate"), DataType(MySqlDbType.Int64, "10")> Public Property secondCoordinate As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `crosslinkedresidue_2_secondcoordinate` (`DB_ID`, `secondCoordinate_rank`, `secondCoordinate`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `crosslinkedresidue_2_secondcoordinate` (`DB_ID`, `secondCoordinate_rank`, `secondCoordinate`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `crosslinkedresidue_2_secondcoordinate` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `crosslinkedresidue_2_secondcoordinate` SET `DB_ID`='{0}', `secondCoordinate_rank`='{1}', `secondCoordinate`='{2}' WHERE `DB_ID` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, secondCoordinate_rank, secondCoordinate)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, secondCoordinate_rank, secondCoordinate)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, secondCoordinate_rank, secondCoordinate, DB_ID)
    End Function
#End Region
End Class


End Namespace
