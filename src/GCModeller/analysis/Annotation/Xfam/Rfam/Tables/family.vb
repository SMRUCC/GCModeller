#Region "Microsoft.VisualBasic::ba60e6cd4191477df7515c38d2e063a5, ..\GCModeller\analysis\Annotation\Xfam\Rfam\Tables\family.vb"

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
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("family")>
Public Class family: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), NotNull, DataType(MySqlDbType.VarChar, "40")> Public Property rfam_id As String
    <DatabaseField("auto_wiki"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property auto_wiki As Long
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "75")> Public Property description As String
    <DatabaseField("author"), DataType(MySqlDbType.Text)> Public Property author As String
    <DatabaseField("seed_source"), DataType(MySqlDbType.Text)> Public Property seed_source As String
    <DatabaseField("gathering_cutoff"), DataType(MySqlDbType.Double)> Public Property gathering_cutoff As Double
    <DatabaseField("trusted_cutoff"), DataType(MySqlDbType.Double)> Public Property trusted_cutoff As Double
    <DatabaseField("noise_cutoff"), DataType(MySqlDbType.Double)> Public Property noise_cutoff As Double
    <DatabaseField("comment"), DataType(MySqlDbType.Text)> Public Property comment As String
    <DatabaseField("previous_id"), DataType(MySqlDbType.Text)> Public Property previous_id As String
    <DatabaseField("cmbuild"), DataType(MySqlDbType.Text)> Public Property cmbuild As String
    <DatabaseField("cmcalibrate"), DataType(MySqlDbType.Text)> Public Property cmcalibrate As String
    <DatabaseField("cmsearch"), DataType(MySqlDbType.Text)> Public Property cmsearch As String
    <DatabaseField("num_seed"), DataType(MySqlDbType.Int64, "20")> Public Property num_seed As Long
    <DatabaseField("num_full"), DataType(MySqlDbType.Int64, "20")> Public Property num_full As Long
    <DatabaseField("num_genome_seq"), DataType(MySqlDbType.Int64, "20")> Public Property num_genome_seq As Long
    <DatabaseField("num_refseq"), DataType(MySqlDbType.Int64, "20")> Public Property num_refseq As Long
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "50")> Public Property type As String
    <DatabaseField("structure_source"), DataType(MySqlDbType.Text)> Public Property structure_source As String
    <DatabaseField("number_of_species"), DataType(MySqlDbType.Int64, "20")> Public Property number_of_species As Long
    <DatabaseField("number_3d_structures"), DataType(MySqlDbType.Int64, "11")> Public Property number_3d_structures As Long
    <DatabaseField("tax_seed"), DataType(MySqlDbType.Text)> Public Property tax_seed As String
    <DatabaseField("ecmli_lambda"), DataType(MySqlDbType.Double)> Public Property ecmli_lambda As Double
    <DatabaseField("ecmli_mu"), DataType(MySqlDbType.Double)> Public Property ecmli_mu As Double
    <DatabaseField("ecmli_cal_db"), DataType(MySqlDbType.Int64, "9")> Public Property ecmli_cal_db As Long
    <DatabaseField("ecmli_cal_hits"), DataType(MySqlDbType.Int64, "9")> Public Property ecmli_cal_hits As Long
    <DatabaseField("maxl"), DataType(MySqlDbType.Int64, "9")> Public Property maxl As Long
    <DatabaseField("clen"), DataType(MySqlDbType.Int64, "9")> Public Property clen As Long
    <DatabaseField("match_pair_node"), DataType(MySqlDbType.Int64, "1")> Public Property match_pair_node As Long
    <DatabaseField("hmm_tau"), DataType(MySqlDbType.Double)> Public Property hmm_tau As Double
    <DatabaseField("hmm_lambda"), DataType(MySqlDbType.Double)> Public Property hmm_lambda As Double
    <DatabaseField("created"), NotNull, DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("updated"), NotNull, DataType(MySqlDbType.DateTime)> Public Property updated As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `family` (`rfam_acc`, `rfam_id`, `auto_wiki`, `description`, `author`, `seed_source`, `gathering_cutoff`, `trusted_cutoff`, `noise_cutoff`, `comment`, `previous_id`, `cmbuild`, `cmcalibrate`, `cmsearch`, `num_seed`, `num_full`, `num_genome_seq`, `num_refseq`, `type`, `structure_source`, `number_of_species`, `number_3d_structures`, `tax_seed`, `ecmli_lambda`, `ecmli_mu`, `ecmli_cal_db`, `ecmli_cal_hits`, `maxl`, `clen`, `match_pair_node`, `hmm_tau`, `hmm_lambda`, `created`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `family` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `auto_wiki`='{2}', `description`='{3}', `author`='{4}', `seed_source`='{5}', `gathering_cutoff`='{6}', `trusted_cutoff`='{7}', `noise_cutoff`='{8}', `comment`='{9}', `previous_id`='{10}', `cmbuild`='{11}', `cmcalibrate`='{12}', `cmsearch`='{13}', `num_seed`='{14}', `num_full`='{15}', `num_genome_seq`='{16}', `num_refseq`='{17}', `type`='{18}', `structure_source`='{19}', `number_of_species`='{20}', `number_3d_structures`='{21}', `tax_seed`='{22}', `ecmli_lambda`='{23}', `ecmli_mu`='{24}', `ecmli_cal_db`='{25}', `ecmli_cal_hits`='{26}', `maxl`='{27}', `clen`='{28}', `match_pair_node`='{29}', `hmm_tau`='{30}', `hmm_lambda`='{31}', `created`='{32}', `updated`='{33}' WHERE `rfam_acc` = '{34}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, DataType.ToMySqlDateTimeString(created), DataType.ToMySqlDateTimeString(updated))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, DataType.ToMySqlDateTimeString(created), DataType.ToMySqlDateTimeString(updated))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfam_id, auto_wiki, description, author, seed_source, gathering_cutoff, trusted_cutoff, noise_cutoff, comment, previous_id, cmbuild, cmcalibrate, cmsearch, num_seed, num_full, num_genome_seq, num_refseq, type, structure_source, number_of_species, number_3d_structures, tax_seed, ecmli_lambda, ecmli_mu, ecmli_cal_db, ecmli_cal_hits, maxl, clen, match_pair_node, hmm_tau, hmm_lambda, DataType.ToMySqlDateTimeString(created), DataType.ToMySqlDateTimeString(updated), rfam_acc)
    End Function
#End Region
End Class


End Namespace

