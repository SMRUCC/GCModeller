Imports System.Runtime.CompilerServices
Imports GeneticComponents
Imports Microsoft.VisualBasic.Data.IO
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Module Module1

    Sub Main()
        Dim database = UniProtXML.EnumerateEntries(path:="K:\uniprot-taxonomy%3A2.xml")

        Using writer As New BinaryDataWriter("X:/test.bin".Open(, True))
            For Each node In database.CreateDump.Where(Function(n) Not n.Accession.StringEmpty)
                Call writer.writeBin(node)
            Next
        End Using
    End Sub

    <Extension>
    Private Sub writeBin(writer As BinaryDataWriter, node As GeneticNode)
        Call writer.Write(node.ID, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Accession, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.KO, BinaryStringFormat.ByteLengthPrefix)
        Call writer.Write(node.GO.JoinBy("|"), BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Function, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Xref, BinaryStringFormat.DwordLengthPrefix)
        Call writer.Write(node.Nt.Length)
        Call writer.Write(node.Nt)
        Call writer.Write(node.Sequence.Length)
        Call writer.Write(node.Sequence)
    End Sub
End Module
