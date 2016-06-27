Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.Bac_sRNA.org

    Public Class Database

        Public Property Interactions As Interaction()
        Public Property Sequences As Sequence()
        Public ReadOnly Property lastUpdate As String

        Sub New(dataDIR As String)
            Call Me.New(dataDIR & "/sRNA-target interaction.txt",
                        dataDIR & "/BSRD_sRNA_sequences.txt")
        End Sub

        Sub New(interaction As String, seq As String)
            Me.Interactions = ImportsInteraction(interaction)
            Me.Sequences = FASTA.FastaFile.Read(seq) _
                .ToArray(AddressOf Sequence.CType)
        End Sub

        Public Overrides Function ToString() As String
            Return $"http://bac-srna.org; {lastUpdate}: {Interactions.Length} {NameOf(Interactions)} & {Sequences.Length} {NameOf(Sequences)}"
        End Function

        Public Shared Function ImportsInteraction(path As String) As Interaction()
            Dim File As String() = IO.File.ReadAllLines(path)
            Dim LQuery As Interaction() =
                LinqAPI.Exec(Of Interaction) <= From line As String
                                                In File.Skip(3).AsParallel
                                                Let Tokens As String() = Strings.Split(line, vbTab)
                                                Let Interaction = New Bac_sRNA.org.Interaction With {
                                                    .sRNAid = Tokens(0),
                                                    .Organism = Tokens(1),
                                                    .Name = Tokens(2),
                                                    .Regulation = Tokens(3),
                                                    .TargetName = Tokens(4),
                                                    .Reference = Tokens(5)
                                                }
                                                Select Interaction
                                                Order By Interaction.sRNAid
            Return LQuery
        End Function
    End Class
End Namespace