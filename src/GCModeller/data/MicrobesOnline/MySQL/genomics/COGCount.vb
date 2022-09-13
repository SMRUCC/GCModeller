#Region "Microsoft.VisualBasic::3f0f319a4e25a612e4753628833f1f19, GCModeller\data\MicrobesOnline\MySQL\genomics\COGCount.vb"

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

    '   Total Lines: 62
    '    Code Lines: 0
    ' Comment Lines: 55
    '   Blank Lines: 7
    '     File Size: 3.12 KB


    ' 
    ' /********************************************************************************/

#End Region

'REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
'REM  MYSQL Schema Mapper
'REM      for Microsoft VisualBasic.NET 

'REM  Dump @12/3/2015 8:30:29 PM


'Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

'Namespace MySQL.genomics

'''' <summary>
'''' 
'''' --
'''' 
'''' DROP TABLE IF EXISTS `cogcount`;
'''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
'''' /*!40101 SET character_set_client = utf8 */;
'''' CREATE TABLE `cogcount` (
''''   `count(l.locusId)` bigint(21) NOT NULL DEFAULT '0',
''''   `funCode` varchar(5) NOT NULL DEFAULT '',
''''   `taxonomyId` int(10) DEFAULT '0',
''''   `scaffoldId` int(10) unsigned DEFAULT NULL
'''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
'''' /*!40101 SET character_set_client = @saved_cs_client */;
'''' 
'''' --
'''' 
'''' </summary>
'''' <remarks></remarks>
'<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("cogcount")>
'Public Class cogcount: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
'#Region "Public Property Mapping To Database Fields"
'    <DatabaseField("count(l.locusId)"), NotNull, DataType(MySqlDbType.Int64, "21")> Public Property count(l.locusId) As Long
'    <DatabaseField("funCode"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property funCode As String
'    <DatabaseField("taxonomyId"), DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
'    <DatabaseField("scaffoldId"), DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
'#End Region
'#Region "Public SQL Interface"
'#Region "Interface SQL"
'    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `cogcount` (`count(l.locusId)`, `funCode`, `taxonomyId`, `scaffoldId`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
'    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `cogcount` (`count(l.locusId)`, `funCode`, `taxonomyId`, `scaffoldId`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
'    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `cogcount` WHERE ;</SQL>
'    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `cogcount` SET `count(l.locusId)`='{0}', `funCode`='{1}', `taxonomyId`='{2}', `scaffoldId`='{3}' WHERE ;</SQL>
'#End Region
'    Public Overrides Function GetDeleteSQL() As String
'        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
'    End Function
'    Public Overrides Function GetInsertSQL() As String
'        Return String.Format(INSERT_SQL, count(l.locusId), funCode, taxonomyId, scaffoldId)
'    End Function
'    Public Overrides Function GetReplaceSQL() As String
'        Return String.Format(REPLACE_SQL, count(l.locusId), funCode, taxonomyId, scaffoldId)
'    End Function
'    Public Overrides Function GetUpdateSQL() As String
'        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
'    End Function
'#End Region
'End Class


'End Namespace
