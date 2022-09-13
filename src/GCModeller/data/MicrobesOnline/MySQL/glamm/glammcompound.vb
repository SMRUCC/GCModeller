#Region "Microsoft.VisualBasic::01901e1959834fd1fa82d939f3c44930, GCModeller\data\MicrobesOnline\MySQL\glamm\glammcompound.vb"

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

    '   Total Lines: 73
    '    Code Lines: 41
    ' Comment Lines: 25
    '   Blank Lines: 7
    '     File Size: 3.98 KB


    ' Class glammcompound
    ' 
    '     Properties: commonName, created, formula, guid, inchi
    '                 mass, priority, smiles, version
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:22:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.glamm

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `glammcompound`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `glammcompound` (
'''   `guid` bigint(10) unsigned NOT NULL,
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `priority` tinyint(3) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `commonName` text NOT NULL,
'''   `mass` float DEFAULT NULL,
'''   `formula` varchar(255) DEFAULT NULL,
'''   `smiles` text,
'''   `inchi` text,
'''   PRIMARY KEY (`guid`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("glammcompound", Database:="glamm")>
Public Class glammcompound: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property guid As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("priority"), DataType(MySqlDbType.Int64, "3")> Public Property priority As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("commonName"), NotNull, DataType(MySqlDbType.Text)> Public Property commonName As String
    <DatabaseField("mass"), DataType(MySqlDbType.Double)> Public Property mass As Double
    <DatabaseField("formula"), DataType(MySqlDbType.VarChar, "255")> Public Property formula As String
    <DatabaseField("smiles"), DataType(MySqlDbType.Text)> Public Property smiles As String
    <DatabaseField("inchi"), DataType(MySqlDbType.Text)> Public Property inchi As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `glammcompound` (`guid`, `version`, `priority`, `created`, `commonName`, `mass`, `formula`, `smiles`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `glammcompound` (`guid`, `version`, `priority`, `created`, `commonName`, `mass`, `formula`, `smiles`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `glammcompound` WHERE `guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `glammcompound` SET `guid`='{0}', `version`='{1}', `priority`='{2}', `created`='{3}', `commonName`='{4}', `mass`='{5}', `formula`='{6}', `smiles`='{7}', `inchi`='{8}' WHERE `guid` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), commonName, mass, formula, smiles, inchi)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), commonName, mass, formula, smiles, inchi)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), commonName, mass, formula, smiles, inchi, guid)
    End Function
#End Region
End Class


End Namespace
