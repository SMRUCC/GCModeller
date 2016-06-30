Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation
Imports LANS.SystemsBiology.SequenceModel

Namespace DocumentFormat.MAST.HTML

    Public Class FastaToken : Inherits FASTA.FastaToken
        Implements ILocationNucleotideSegment

        Public ReadOnly Property Strand As Strands Implements ILocationNucleotideSegment.Strand
        Public ReadOnly Property Location As LANS.SystemsBiology.ComponentModel.Loci.Location Implements ILocationSegment.Location
        Public ReadOnly Property UniqueId As String Implements ILocationSegment.UniqueId
        Public ReadOnly Property DOOR As String
        Public ReadOnly Property GeneList As String

        Public Overloads Shared Function Load(FastaFile As FASTA.FastaFile) As FastaToken()
            Dim LQuery = (From Fsa In FastaFile.AsParallel Select __createObject(Fsa)).ToArray
            Return LQuery
        End Function

        Private Shared Function __createObject(fa As FASTA.FastaToken) As FastaToken
            Dim FsaObject As FastaToken = New FastaToken
            Dim Title As String = fa.Title
            FsaObject._UniqueId = Title.Split.First
            FsaObject._GeneList = Regex.Match(Title, "OperonGenes=[^]]+").Value.Split(CChar("=")).Last
            FsaObject._Location = New LANS.SystemsBiology.ComponentModel.Loci.Location

            Dim strLocation As String = Regex.Match(Title, "\d+ ==> \d+ #(Reverse|Forward)").Value
            Dim Tokens As String() = strLocation.Split
            FsaObject._Location = New LANS.SystemsBiology.ComponentModel.Loci.Location With {
                .Left = Tokens(0),
                .Right = Tokens(2)
            }
            FsaObject._Strand = If(String.Equals(Tokens.Last, "#Forward"), Strands.Forward, Strands.Reverse)
            FsaObject.SequenceData = fa.SequenceData
            FsaObject._DOOR = Regex.Match(Title, "AssociatedOperon=[^]]+").Value.Split(CChar("=")).Last
            If String.IsNullOrEmpty(FsaObject._DOOR) Then
                FsaObject._DOOR = Title.Split.First
            End If

            Return FsaObject
        End Function
    End Class
End Namespace