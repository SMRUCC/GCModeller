Imports LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic
Imports System.Text
Imports Oracle.LinuxCompatibility.MySQL

Namespace Regtransbase

    Public Class Database

        Dim DbReflector As Oracle.LinuxCompatibility.MySQL.MySQL

        Sub New(MySQL As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
            DbReflector = New Oracle.LinuxCompatibility.MySQL.MySQL(MySQL)
        End Sub

        Public Function GetGenes() As Regtransbase.StructureObjects.Gene()
            Return DbReflector.Query(Of Regtransbase.StructureObjects.Gene)("select * from genes").ToArray
        End Function

        Public Function GetSites() As Regtransbase.StructureObjects.Sites()
            Return DbReflector.Query(Of Regtransbase.StructureObjects.Sites)("select * from sites").ToArray
        End Function

        Public Function GetRegulators() As Regtransbase.StructureObjects.Regulator()
            Return DbReflector.Query(Of Regtransbase.StructureObjects.Regulator)("select * from regulators").ToArray
        End Function

        Public Function ExportBindingSites(Optional TryAutoFix As Boolean = False) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of Regtransbase.StructureObjects.Sites)("select * from sites")
            Dim LQuery = (From site As Regtransbase.StructureObjects.Sites In Table
                          Let Fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = StructureObjects.Sites.ExportFasta(site, TryAutoFix)
                          Where Not Fsa Is Nothing
                          Select Fsa).ToArray
            Return LQuery
        End Function

        Public Function ExportRegulators(Optional TryAutoFix As Boolean = False) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of Regtransbase.StructureObjects.Regulator)("select * from regulators")
            Dim Genes = DbReflector.Query(Of Regtransbase.StructureObjects.Gene)("select * from genes").ToArray '在其中居然会有以TGA开头的基因序列
            Dim LQuery = (From regulator As Regtransbase.StructureObjects.Regulator
                          In Table
                          Let fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = Regtransbase.StructureObjects.Regulator.ExportFasta(regulator, Genes, TryAutoFix)
                          Where Not fsa Is Nothing AndAlso Len(fsa.SequenceData) > 0
                          Select fsa).ToArray
            Return CType(LQuery, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
        End Function

        Private Shared Function Trim(str As String) As String
            Dim sBuilder As List(Of Char) = New List(Of Char)
            For Each ch In str
                If ch = " "c Then
                Else
                    Call sBuilder.Add(ch)
                End If
            Next

            Return sBuilder.ToArray
        End Function
    End Class
End Namespace