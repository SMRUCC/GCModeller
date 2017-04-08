
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' 生成参考所使用的外标尺核酸片段
    ''' </summary>
    Public Module ReferenceRule

        ''' <summary>
        ''' 获取默认的外标尺：基因组之中的dnaA-gyrB之间的序列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Get.Ref_Rule",
                   Info:="Gets the segment betweens the dnaA and gyrB nucleotide sequence as the default reference rule for the homogeneity measuring.")>
        <Extension>
        Public Function GetReferenceRule(nt As FastaToken, PTT As PTT) As FastaToken
            Dim dnaA = PTT.MatchGene("dnaA", {"chromosomal replication initiator protein DnaA", "chromosomal replication initiator"})
            Dim gyrB = PTT.MatchGene("gyrB", {"DNA gyrase B subunit", "DNA gyrase, B subunit"})

            If (dnaA Is Nothing OrElse gyrB Is Nothing) Then
                Call $"Could not found gene dnaA or gyrB on {nt.Title}".PrintException
                Return Nothing
            End If

            ' 默认dnaA - gyrB这个基因簇是位于正义链的
            Dim St As Integer = dnaA.Location.Left
            Dim Sp As Integer = gyrB.Location.Right

            ' 但是有些基因组或者由于测序的原因，位于负义链。。。
            If dnaA.Location.Strand = Strands.Reverse Then
                St = gyrB.Location.Left
                Sp = dnaA.Location.Right
            End If

            Dim RuleSegment As NucleotideModels.NucleicAcid
            ' 构建基因组外标尺片段的计算模型
            Try
                RuleSegment = New NucleotideModels.NucleicAcid(nt.CutSequenceLinear(St, Sp - St))
                If RuleSegment.Length > 10 * 1000 Then
                    Call $"Location exception on (""{nt.Title}"") parsing segment.".PrintException
                    Return Nothing
                End If
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.PrintException
                Return Nothing
            End Try

            Return New FastaToken With {
                .Attributes = New String() {"dnaA-gyrB", nt.Title},
                .SequenceData = RuleSegment.SequenceData
            }
        End Function
    End Module
End Namespace