#Region "Microsoft.VisualBasic::1c8fd7206549597d4c7eb56230838376, data\ExternalDBSource\RegulonDB\Promoter.vb"

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

    '     Class Promoter
    ' 
    '         Properties: BasalTransValue, Comment, EquilibriumConst, ID, KeyIdOrg
    '                     KineticConst, Name, Note, Position, Sequence
    '                     SigmaFactor, Strand, StrengthSeq
    ' 
    '         Function: ExportFasta, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes

Namespace RegulonDB

    Public Class Promoter : Inherits LANS.SystemsBiology.ExternalDBSource.RegulonDB.ObjectItem
        <DatabaseField("promoter_id")> <Xml.Serialization.XmlElement("PROMOTER_ID")> Public Property ID As String
        <DatabaseField("promoter_name")> <Xml.Serialization.XmlElement("PROMOTER_NAME")> Public Property Name As String
        <DatabaseField("promoter_strand")> <Xml.Serialization.XmlElement("PROMOTER_STRAND")> Public Property Strand As String
        <DatabaseField("pos_1")> <Xml.Serialization.XmlElement("POS_1")> Public Property Position As Decimal
        <DatabaseField("sigma_factor")> <Xml.Serialization.XmlElement("SIGMA_FACTOR")> Public Property SigmaFactor As String
        <DatabaseField("basal_trans_val")> <Xml.Serialization.XmlElement("BASAL_TRANS_VAL")> Public Property BasalTransValue As Decimal
        <DatabaseField("equilibrium_const")> <Xml.Serialization.XmlElement("EQUILIBRIUM_CONST")> Public Property EquilibriumConst As Decimal
        <DatabaseField("kinetic_const")> <Xml.Serialization.XmlElement("KINETIC_CONST")> Public Property KineticConst As Decimal
        <DatabaseField("strength_seq")> <Xml.Serialization.XmlElement("STRENGTH_SEQ")> Public Property StrengthSeq As Decimal
        <DatabaseField("promoter_sequence")> <Xml.Serialization.XmlElement("PROMOTER_SEQUENCE")> Public Property Sequence As String
        <DatabaseField("promoter_note")> <Xml.Serialization.XmlElement("PROMOTER_NOTE")> Public Property Note As String
        <DatabaseField("key_id_org")> <Xml.Serialization.XmlElement("KEY_ID_ORG")> Public Property KeyIdOrg As String
        <DatabaseField("promoter_internal_comment")> <Xml.Serialization.XmlIgnore> Public Property Comment As String

        Public Shared Function ExportFasta(Table As Generic.IEnumerable(Of Promoter)) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim LQuery = From Promoter As Promoter In Table
                         Select New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject With
                                {
                                    .SequenceData = Promoter.Sequence,
                                    .Attributes = New String() {Promoter.ID, Promoter.Name, Promoter.SigmaFactor}
                                } '
            Return LQuery.ToList
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace
