Imports System.Text.RegularExpressions
Imports System.Text
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class FastaCollection

        Public Property DNAseq As GeneObject()
        Public Property protseq As Proteins()

        Public Property ProteinSourceFile As String
        Public Property DNASourceFilePath As String

        ''' <summary>
        ''' The complete genome sequence of the target species.(目标对象的全基因组序列)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Origin As FastaToken
        Public Property OriginSourceFile As String

        Public Overloads Shared Function Load(Of T As FastaToken)(FilePath As String, Optional Explicit As Boolean = True) As T()
            Dim FASTA As FastaFile = FastaFile.Read(FilePath, Explicit)
            Dim type As Type = GetType(T)
            Dim LQuery As T() = (From Fa As FastaToken
                                 In FASTA.AsParallel
                                 Select DirectCast(Activator.CreateInstance(type, {Fa}), T)).ToArray
            Return LQuery
        End Function

        Public Shared Function LoadGeneObjects(file As String, Optional explicit As Boolean = True) As GeneObject()
            Return Load(Of GeneObject)(file, explicit)
        End Function

        Public Shared Function LoadProteins(file As String, Optional explicit As Boolean = True) As Proteins()
            Return Load(Of Proteins)(file, explicit)
        End Function
    End Class
End Namespace