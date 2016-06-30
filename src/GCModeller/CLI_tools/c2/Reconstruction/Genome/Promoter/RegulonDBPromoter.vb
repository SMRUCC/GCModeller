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