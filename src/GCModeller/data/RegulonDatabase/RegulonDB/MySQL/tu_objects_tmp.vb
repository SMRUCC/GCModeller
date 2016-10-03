#Region "Microsoft.VisualBasic::01c596255f383c9bff70bb2818733ea1, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\tu_objects_tmp.vb"

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
''' DROP TABLE IF EXISTS `tu_objects_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tu_objects_tmp` (
'''   `transcription_unit_id` char(12) DEFAULT NULL,
'''   `numtu` decimal(10,0) DEFAULT NULL,
'''   `tu_posleft` decimal(10,0) DEFAULT NULL,
'''   `tu_posright` decimal(10,0) DEFAULT NULL,
'''   `tu_type` varchar(10) DEFAULT NULL,
'''   `tu_object_class` char(2) DEFAULT NULL,
'''   `tu_object_id` char(12) DEFAULT NULL,
'''   `tu_object_name` varchar(200) DEFAULT NULL,
'''   `tu_object_posleft` decimal(10,0) DEFAULT NULL,
'''   `tu_object_posright` decimal(10,0) DEFAULT NULL,
'''   `tu_object_strand` char(1) DEFAULT NULL,
'''   `tu_object_colorclass` varchar(12) DEFAULT NULL,
'''   `tu_object_description` varchar(2000) DEFAULT NULL,
'''   `tu_object_sigma` varchar(100) DEFAULT NULL,
'''   `tu_object_evidence` varchar(2000) DEFAULT NULL,
'''   `tu_object_ri_type` varchar(100) DEFAULT NULL,
'''   `tu_object_type` varchar(10) DEFAULT NULL,
'''   `evidence` char(1) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tu_objects_tmp", Database:="regulondb_7_5")>
Public Class tu_objects_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("transcription_unit_id"), DataType(MySqlDbType.VarChar, "12")> Public Property transcription_unit_id As String
    <DatabaseField("numtu"), DataType(MySqlDbType.Decimal)> Public Property numtu As Decimal
    <DatabaseField("tu_posleft"), DataType(MySqlDbType.Decimal)> Public Property tu_posleft As Decimal
    <DatabaseField("tu_posright"), DataType(MySqlDbType.Decimal)> Public Property tu_posright As Decimal
    <DatabaseField("tu_type"), DataType(MySqlDbType.VarChar, "10")> Public Property tu_type As String
    <DatabaseField("tu_object_class"), DataType(MySqlDbType.VarChar, "2")> Public Property tu_object_class As String
    <DatabaseField("tu_object_id"), DataType(MySqlDbType.VarChar, "12")> Public Property tu_object_id As String
    <DatabaseField("tu_object_name"), DataType(MySqlDbType.VarChar, "200")> Public Property tu_object_name As String
    <DatabaseField("tu_object_posleft"), DataType(MySqlDbType.Decimal)> Public Property tu_object_posleft As Decimal
    <DatabaseField("tu_object_posright"), DataType(MySqlDbType.Decimal)> Public Property tu_object_posright As Decimal
    <DatabaseField("tu_object_strand"), DataType(MySqlDbType.VarChar, "1")> Public Property tu_object_strand As String
    <DatabaseField("tu_object_colorclass"), DataType(MySqlDbType.VarChar, "12")> Public Property tu_object_colorclass As String
    <DatabaseField("tu_object_description"), DataType(MySqlDbType.VarChar, "2000")> Public Property tu_object_description As String
    <DatabaseField("tu_object_sigma"), DataType(MySqlDbType.VarChar, "100")> Public Property tu_object_sigma As String
    <DatabaseField("tu_object_evidence"), DataType(MySqlDbType.VarChar, "2000")> Public Property tu_object_evidence As String
    <DatabaseField("tu_object_ri_type"), DataType(MySqlDbType.VarChar, "100")> Public Property tu_object_ri_type As String
    <DatabaseField("tu_object_type"), DataType(MySqlDbType.VarChar, "10")> Public Property tu_object_type As String
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "1")> Public Property evidence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tu_objects_tmp` (`transcription_unit_id`, `numtu`, `tu_posleft`, `tu_posright`, `tu_type`, `tu_object_class`, `tu_object_id`, `tu_object_name`, `tu_object_posleft`, `tu_object_posright`, `tu_object_strand`, `tu_object_colorclass`, `tu_object_description`, `tu_object_sigma`, `tu_object_evidence`, `tu_object_ri_type`, `tu_object_type`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tu_objects_tmp` (`transcription_unit_id`, `numtu`, `tu_posleft`, `tu_posright`, `tu_type`, `tu_object_class`, `tu_object_id`, `tu_object_name`, `tu_object_posleft`, `tu_object_posright`, `tu_object_strand`, `tu_object_colorclass`, `tu_object_description`, `tu_object_sigma`, `tu_object_evidence`, `tu_object_ri_type`, `tu_object_type`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tu_objects_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tu_objects_tmp` SET `transcription_unit_id`='{0}', `numtu`='{1}', `tu_posleft`='{2}', `tu_posright`='{3}', `tu_type`='{4}', `tu_object_class`='{5}', `tu_object_id`='{6}', `tu_object_name`='{7}', `tu_object_posleft`='{8}', `tu_object_posright`='{9}', `tu_object_strand`='{10}', `tu_object_colorclass`='{11}', `tu_object_description`='{12}', `tu_object_sigma`='{13}', `tu_object_evidence`='{14}', `tu_object_ri_type`='{15}', `tu_object_type`='{16}', `evidence`='{17}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, transcription_unit_id, numtu, tu_posleft, tu_posright, tu_type, tu_object_class, tu_object_id, tu_object_name, tu_object_posleft, tu_object_posright, tu_object_strand, tu_object_colorclass, tu_object_description, tu_object_sigma, tu_object_evidence, tu_object_ri_type, tu_object_type, evidence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, transcription_unit_id, numtu, tu_posleft, tu_posright, tu_type, tu_object_class, tu_object_id, tu_object_name, tu_object_posleft, tu_object_posright, tu_object_strand, tu_object_colorclass, tu_object_description, tu_object_sigma, tu_object_evidence, tu_object_ri_type, tu_object_type, evidence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
