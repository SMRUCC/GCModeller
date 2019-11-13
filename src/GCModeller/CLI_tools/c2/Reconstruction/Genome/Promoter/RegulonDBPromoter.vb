#Region "Microsoft.VisualBasic::1dcc072b1ce4d52b3b97f14fc0927d08, CLI_tools\c2\Reconstruction\Genome\Promoter\RegulonDBPromoter.vb"

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

    '         Class RegulonDBPromoter
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: __exportFasta, GetSigmaFactors, Performance
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Reconstruction : Partial Class Promoters

        Public Class RegulonDBPromoter : Inherits c2.Reconstruction.Operation

            Dim MYSQL As String

            Sub New(Session As OperationSession)
                Call MyBase.New(Session)
                Session.MYSQL.Database = "regulondb"
                Me.MYSQL = Session.MYSQL.GetConnectionString
            End Sub

            Public Overrides Function Performance() As Integer
                Dim Db As Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector = New Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector(MYSQL)
                Dim Table = Db.Query(Of LANS.SystemsBiology.DatabaseServices.RegulonDB.Tables.promoter)("select * from promoter")

                Call Table.GetXml().SaveTo(MyBase.Workspace & "/regulondb_promoter.xml")
                Call __exportFasta(Table).Save(MyBase.Workspace & "/regulondb_promoter.fsa")

                Throw New NotImplementedException
            End Function

            Private Shared Function __exportFasta(promoters As Generic.IEnumerable(Of LANS.SystemsBiology.DatabaseServices.RegulonDB.Tables.promoter)) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
                Dim promoterArray = promoters.ToArray(Function(promoter) New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                                                          .SequenceData = promoter.promoter_sequence,
                                                          .Attributes = New String() {
                                                                promoter.promoter_id,
                                                                promoter.promoter_name,
                                                                promoter.basal_trans_val,
                                                                promoter.equilibrium_const,
                                                                promoter.key_id_org,
                                                                promoter.kinetic_const,
                                                                promoter.pos_1,
                                                                promoter.promoter_internal_comment,
                                                                promoter.promoter_note,
                                                                promoter.promoter_strand,
                                                                promoter.sigma_factor
                                                            }
                                                          })
                Return CType(promoterArray, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
            End Function

            Private Function GetSigmaFactors(Table As Generic.IEnumerable(Of LANS.SystemsBiology.DatabaseServices.RegulonDB.Tables.promoter)) As String()
                Dim LQuery = From Promoter In Table Select Promoter.sigma_factor '
                Return LQuery.ToArray
            End Function
        End Class
    End Class
End Namespace
