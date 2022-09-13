#Region "Microsoft.VisualBasic::7f3a370a0cd7090c7c166cde18df7172, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\genes.vb"

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

    '   Total Lines: 208
    '    Code Lines: 110
    ' Comment Lines: 76
    '   Blank Lines: 22
    '     File Size: 14.35 KB


    ' Class genes
    ' 
    '     Properties: art_guid, descript, ferment_num, fl_real_name, gene_function
    '                 gene_guid, genome_guid, last_update, location, metabol_path
    '                 name, pkg_guid, ref_bank1, ref_bank2, ref_bank3
    '                 ref_bank4, signature
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `genes` (
'''   `gene_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `location` varchar(50) DEFAULT NULL,
'''   `ref_bank1` varchar(255) DEFAULT NULL,
'''   `ref_bank2` varchar(255) DEFAULT NULL,
'''   `ref_bank3` varchar(255) DEFAULT NULL,
'''   `ref_bank4` varchar(255) DEFAULT NULL,
'''   `signature` text,
'''   `metabol_path` varchar(100) DEFAULT NULL,
'''   `ferment_num` varchar(20) DEFAULT NULL,
'''   `gene_function` varchar(100) DEFAULT NULL,
'''   `descript` blob,
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`gene_guid`),
'''   KEY `FK_genes-pkg_guid` (`pkg_guid`),
'''   KEY `FK_genes-art_guid` (`art_guid`),
'''   KEY `FK_genes-genome_guid` (`genome_guid`),
'''   CONSTRAINT `FK_genes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_genes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_genes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("genes", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `genes` (
  `gene_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `location` varchar(50) DEFAULT NULL,
  `ref_bank1` varchar(255) DEFAULT NULL,
  `ref_bank2` varchar(255) DEFAULT NULL,
  `ref_bank3` varchar(255) DEFAULT NULL,
  `ref_bank4` varchar(255) DEFAULT NULL,
  `signature` text,
  `metabol_path` varchar(100) DEFAULT NULL,
  `ferment_num` varchar(20) DEFAULT NULL,
  `gene_function` varchar(100) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`gene_guid`),
  KEY `FK_genes-pkg_guid` (`pkg_guid`),
  KEY `FK_genes-art_guid` (`art_guid`),
  KEY `FK_genes-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_genes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_genes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_genes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class genes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="gene_guid"), XmlAttribute> Public Property gene_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="name")> Public Property name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1"), Column(Name:="fl_real_name")> Public Property fl_real_name As Long
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="genome_guid")> Public Property genome_guid As Long
    <DatabaseField("location"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="location")> Public Property location As String
    <DatabaseField("ref_bank1"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ref_bank1")> Public Property ref_bank1 As String
    <DatabaseField("ref_bank2"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ref_bank2")> Public Property ref_bank2 As String
    <DatabaseField("ref_bank3"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ref_bank3")> Public Property ref_bank3 As String
    <DatabaseField("ref_bank4"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ref_bank4")> Public Property ref_bank4 As String
    <DatabaseField("signature"), DataType(MySqlDbType.Text), Column(Name:="signature")> Public Property signature As String
    <DatabaseField("metabol_path"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="metabol_path")> Public Property metabol_path As String
    <DatabaseField("ferment_num"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="ferment_num")> Public Property ferment_num As String
    <DatabaseField("gene_function"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="gene_function")> Public Property gene_function As String
    <DatabaseField("descript"), DataType(MySqlDbType.Blob), Column(Name:="descript")> Public Property descript As Byte()
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="last_update")> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `genes` WHERE `gene_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `genes` SET `gene_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `location`='{6}', `ref_bank1`='{7}', `ref_bank2`='{8}', `ref_bank3`='{9}', `ref_bank4`='{10}', `signature`='{11}', `metabol_path`='{12}', `ferment_num`='{13}', `gene_function`='{14}', `descript`='{15}', `last_update`='{16}' WHERE `gene_guid` = '{17}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `genes` WHERE `gene_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gene_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(INSERT_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{gene_guid}', '{pkg_guid}', '{art_guid}', '{name}', '{fl_real_name}', '{genome_guid}', '{location}', '{ref_bank1}', '{ref_bank2}', '{ref_bank3}', '{ref_bank4}', '{signature}', '{metabol_path}', '{ferment_num}', '{gene_function}', '{descript}', '{last_update}')"
        Else
            Return $"('{gene_guid}', '{pkg_guid}', '{art_guid}', '{name}', '{fl_real_name}', '{genome_guid}', '{location}', '{ref_bank1}', '{ref_bank2}', '{ref_bank3}', '{ref_bank4}', '{signature}', '{metabol_path}', '{ferment_num}', '{gene_function}', '{descript}', '{last_update}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `genes` (`gene_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `signature`, `metabol_path`, `ferment_num`, `gene_function`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(REPLACE_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `genes` SET `gene_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `location`='{6}', `ref_bank1`='{7}', `ref_bank2`='{8}', `ref_bank3`='{9}', `ref_bank4`='{10}', `signature`='{11}', `metabol_path`='{12}', `ferment_num`='{13}', `gene_function`='{14}', `descript`='{15}', `last_update`='{16}' WHERE `gene_guid` = '{17}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, gene_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature, metabol_path, ferment_num, gene_function, descript, MySqlScript.ToMySqlDateTimeString(last_update), gene_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As genes
                         Return DirectCast(MyClass.MemberwiseClone, genes)
                     End Function
End Class


End Namespace
