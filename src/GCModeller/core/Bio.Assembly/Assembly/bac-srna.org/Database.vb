#Region "Microsoft.VisualBasic::75e3ed44aab3481f97069a8c47481433, ..\GCModeller\core\Bio.Assembly\Assembly\bac-srna.org\Database.vb"

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

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
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
