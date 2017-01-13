Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SSDB

    ''' <summary>
    ''' 这个API只适合小批量数据的获取
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="geneIDs"></param>
    ''' <param name="len%"></param>
    ''' <param name="save$"></param>
    ''' <param name="code$"></param>
    ''' <param name="[overrides]"></param>
    <Extension>
    Public Sub CutSequence_Upstream(genome As PTT, geneIDs As IEnumerable(Of String), len%, save$, code$, Optional [overrides] As Boolean = False)
        Dim genes = genome.ToDictionary
        Dim cuts As New FastaFile(save, throwEx:=False)
        Dim titles As New IndexOf(Of String)(cuts.Select(Function(f) f.Title))

        Using write As StreamWriter = save.OpenWriter(Encodings.ASCII)
            For Each fa In cuts
                Call write.WriteLine(fa.GenerateDocument(60))
            Next

            For Each id$ In geneIDs
                If Not genes.ContainsKey(id) Then
                    Continue For
                End If

                Dim loci = genes(id).Location
                Dim region As Location

                With loci.Normalization
                    If loci.Strand = Strands.Reverse Then
                        region = New Location(.Right, .Right + len) ' ATG 向右平移
                    Else
                        region = New Location(.Left - len, .Left)
                    End If
                End With

                Dim title$ = id & " " & loci.ToString

                If titles(title) > -1 AndAlso Not [overrides] Then
                    Call $"Skip existed {title}...".__DEBUG_ECHO
                    Continue For
                End If

                Dim seq As FastaToken =
                    SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.CutSequence(
                    region,
                    org:=code,
                    vector:=loci.Strand)

                seq.Attributes = {title}

                Call write.WriteLine(seq.GenerateDocument(60))
                Call Thread.Sleep(1500)
            Next
        End Using
    End Sub
End Module
