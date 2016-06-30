Imports LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
Imports Microsoft.VisualBasic.Extensions
Imports System.Text

Namespace Regtransbase

    Public Class Database : Inherits MySQLDbReflector

        Sub New(MySQL As Oracle.LinuxCompatibility.MySQL.Client.ConnectionUri)
            Call MyBase.New(MySQL)
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

        Public Function ExportBindingSites(Optional TryAutoFix As Boolean = False) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of Regtransbase.StructureObjects.Sites)("select * from sites")
            Dim LQuery = (From site As Regtransbase.StructureObjects.Sites In Table
                          Let Fsa As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject = StructureObjects.Sites.ExportFasta(site, TryAutoFix)
                          Where Not Fsa Is Nothing
                          Select Fsa).ToArray
            Return LQuery
        End Function

        Public Function ExportRegulators(Optional TryAutoFix As Boolean = False) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of Regtransbase.StructureObjects.Regulator)("select * from regulators")
            Dim Genes = DbReflector.Query(Of Regtransbase.StructureObjects.Gene)("select * from genes").ToArray '在其中居然会有以TGA开头的基因序列
            Dim LQuery = (From regulator As Regtransbase.StructureObjects.Regulator
                          In Table
                          Let fsa As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject = Regtransbase.StructureObjects.Regulator.ExportFasta(regulator, Genes, TryAutoFix)
                          Where Not fsa Is Nothing AndAlso Len(fsa.SequenceData) > 0
                          Select fsa).ToArray
            Return CType(LQuery, LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile)
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