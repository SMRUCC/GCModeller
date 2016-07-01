Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DEG

    Public Class Annotations : Implements sIdEnumerable

        <Column("#DEG_AC")> Public Property DEG_AC As String Implements sIdEnumerable.Identifier
        <Column("#Gene_Name")> Public Property GeneName As String
        <Column("#Gene_Ref")> Public Property Gene_Ref As String
        <Column("#COG")> Public Property COG As String
        <Column("#Class")> Public Property [Class] As String
        <Column("#Function")> Public Property [Function] As String
        <Column("#Organism")> Public Property Organism As String
        <Column("#Refseq")> Public Property Refseq As String
        <Column("#Condition")> Public Property Condition As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", DEG_AC, Gene_Ref)
        End Function

        Public Shared Function Load(CsvData As DocumentStream.File) As DEG.Annotations()
            Dim ChunkBuffer As DEG.Annotations() = Reflector.Convert(Of DEG.Annotations)(CsvData.DataFrame(), False).ToArray
            Return ChunkBuffer
        End Function

        Public Shared Function GetSpeciesId(AnnotiationCollection As DEG.Annotations()) As String()
            Dim LQuery = (From item In AnnotiationCollection Where Not String.IsNullOrEmpty(item.Organism) Select item.Organism Distinct Order By Organism Ascending).ToArray
            Return LQuery
        End Function

        Public Shared Function Load(AnnotionFile As String) As DEG.Annotations()
            Dim CsvData = DataImports.Imports(AnnotionFile, vbTab)
            Dim ChunkBuffer As DEG.Annotations() = Reflector.Convert(Of DEG.Annotations)(CsvData.DataFrame(), False).ToArray
            Return ChunkBuffer
        End Function
    End Class

    Public Class DEG_AminoAcidSequence : Inherits FastaToken

        Public ReadOnly Property DEGAccessionId As String
        Public ReadOnly Property SpeciesId As String
        Public ReadOnly Property GeneName As String

        Public Overloads Shared Function Load(FilePath As String) As DEG.DEG_AminoAcidSequence()
            Dim LQuery = (From FsaObject As FastaToken
                          In FastaFile.Read(FilePath)
                          Select DEG.DEG_AminoAcidSequence.CreateObject(FsaObject)).ToArray
            Return LQuery
        End Function

        Public Shared Function CreateObject(FsaObject As FastaToken) As DEG.DEG_AminoAcidSequence
            Dim strData As String = FsaObject.Attributes.First
            Dim Tokens As String() = strData.Split("_"c)
            Dim Sequence As DEG.DEG_AminoAcidSequence = New DEG_AminoAcidSequence

            Call FsaObject.CopyTo(Sequence)
            Return Sequence
        End Function
    End Class
End Namespace