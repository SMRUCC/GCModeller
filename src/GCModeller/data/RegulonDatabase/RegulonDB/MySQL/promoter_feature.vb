#Region "Microsoft.VisualBasic::74753242962fc2c39fff2316d932bf1f, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\promoter_feature.vb"

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

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `promoter_feature`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `promoter_feature` (
'''   `promoter_feature_id` char(12) DEFAULT NULL,
'''   `promoter_id` char(12) DEFAULT NULL,
'''   `box_10_left` decimal(10,0) DEFAULT NULL,
'''   `box_10_right` decimal(10,0) DEFAULT NULL,
'''   `box_35_left` decimal(10,0) DEFAULT NULL,
'''   `box_35_right` decimal(10,0) DEFAULT NULL,
'''   `box_10_sequence` varchar(100) DEFAULT NULL,
'''   `box_35_sequence` varchar(100) DEFAULT NULL,
'''   `score` decimal(4,2) DEFAULT NULL,
'''   `relative_box_10_left` decimal(10,0) DEFAULT NULL,
'''   `relative_box_10_right` decimal(10,0) DEFAULT NULL,
'''   `relative_box_35_left` decimal(10,0) DEFAULT NULL,
'''   `relative_box_35_right` decimal(10,0) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("promoter_feature", Database:="regulondb_7_5")>
Public Class promoter_feature: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("promoter_feature_id"), DataType(MySqlDbType.VarChar, "12")> Public Property promoter_feature_id As String
    <DatabaseField("promoter_id"), DataType(MySqlDbType.VarChar, "12")> Public Property promoter_id As String
    <DatabaseField("box_10_left"), DataType(MySqlDbType.Decimal)> Public Property box_10_left As Decimal
    <DatabaseField("box_10_right"), DataType(MySqlDbType.Decimal)> Public Property box_10_right As Decimal
    <DatabaseField("box_35_left"), DataType(MySqlDbType.Decimal)> Public Property box_35_left As Decimal
    <DatabaseField("box_35_right"), DataType(MySqlDbType.Decimal)> Public Property box_35_right As Decimal
    <DatabaseField("box_10_sequence"), DataType(MySqlDbType.VarChar, "100")> Public Property box_10_sequence As String
    <DatabaseField("box_35_sequence"), DataType(MySqlDbType.VarChar, "100")> Public Property box_35_sequence As String
    <DatabaseField("score"), DataType(MySqlDbType.Decimal)> Public Property score As Decimal
    <DatabaseField("relative_box_10_left"), DataType(MySqlDbType.Decimal)> Public Property relative_box_10_left As Decimal
    <DatabaseField("relative_box_10_right"), DataType(MySqlDbType.Decimal)> Public Property relative_box_10_right As Decimal
    <DatabaseField("relative_box_35_left"), DataType(MySqlDbType.Decimal)> Public Property relative_box_35_left As Decimal
    <DatabaseField("relative_box_35_right"), DataType(MySqlDbType.Decimal)> Public Property relative_box_35_right As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `promoter_feature` (`promoter_feature_id`, `promoter_id`, `box_10_left`, `box_10_right`, `box_35_left`, `box_35_right`, `box_10_sequence`, `box_35_sequence`, `score`, `relative_box_10_left`, `relative_box_10_right`, `relative_box_35_left`, `relative_box_35_right`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `promoter_feature` (`promoter_feature_id`, `promoter_id`, `box_10_left`, `box_10_right`, `box_35_left`, `box_35_right`, `box_10_sequence`, `box_35_sequence`, `score`, `relative_box_10_left`, `relative_box_10_right`, `relative_box_35_left`, `relative_box_35_right`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `promoter_feature` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `promoter_feature` SET `promoter_feature_id`='{0}', `promoter_id`='{1}', `box_10_left`='{2}', `box_10_right`='{3}', `box_35_left`='{4}', `box_35_right`='{5}', `box_10_sequence`='{6}', `box_35_sequence`='{7}', `score`='{8}', `relative_box_10_left`='{9}', `relative_box_10_right`='{10}', `relative_box_35_left`='{11}', `relative_box_35_right`='{12}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, promoter_feature_id, promoter_id, box_10_left, box_10_right, box_35_left, box_35_right, box_10_sequence, box_35_sequence, score, relative_box_10_left, relative_box_10_right, relative_box_35_left, relative_box_35_right)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, promoter_feature_id, promoter_id, box_10_left, box_10_right, box_35_left, box_35_right, box_10_sequence, box_35_sequence, score, relative_box_10_left, relative_box_10_right, relative_box_35_left, relative_box_35_right)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
