#Region "Microsoft.VisualBasic::827ddb54b5871ad64cb73792ee19a3f1, ncbi\gi2taxid.vb"

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

    ' Class gi2taxid
    ' 
    '     Properties: gi, taxid
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @2016/10/4 20:02:15


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace mysql.NCBI

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `gi2taxid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gi2taxid` (
'''   `gi` int(11) NOT NULL,
'''   `taxid` int(11) NOT NULL,
'''   PRIMARY KEY (`gi`,`taxid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gi2taxid", Database:="ncbi", SchemaSQL:="
CREATE TABLE `gi2taxid` (
  `gi` int(11) NOT NULL,
  `taxid` int(11) NOT NULL,
  PRIMARY KEY (`gi`,`taxid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gi2taxid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gi"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gi As Long
    <DatabaseField("taxid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property taxid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gi2taxid` (`gi`, `taxid`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gi2taxid` (`gi`, `taxid`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gi2taxid` WHERE `gi`='{0}' and `taxid`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gi2taxid` SET `gi`='{0}', `taxid`='{1}' WHERE `gi`='{2}' and `taxid`='{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gi2taxid` WHERE `gi`='{0}' and `taxid`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gi, taxid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gi2taxid` (`gi`, `taxid`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gi, taxid)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `gi2taxid` (`gi`, `taxid`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gi, taxid)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gi2taxid` SET `gi`='{0}', `taxid`='{1}' WHERE `gi`='{2}' and `taxid`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, gi, taxid, gi, taxid)
    End Function
#End Region
End Class


End Namespace
