Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Class ProteinDescription : Implements INamedValue

        ''' <summary>
        ''' #Replicon Name
        ''' </summary>
        ''' <returns></returns>
        Public Property RepliconName As String
        ''' <summary>
        ''' Replicon Accession
        ''' </summary>
        ''' <returns></returns>
        Public Property RepliconAccession As String
        Public Property Start As Integer
        Public Property [Stop] As Integer
        Public Property Strand As String
        Public Property GeneID As String
        Public Property Locus As String
        ''' <summary>
        ''' Locus tag.(基因号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Locus_tag As String Implements INamedValue.Key
        ''' <summary>
        ''' Protein product
        ''' </summary>
        ''' <returns></returns>
        Public Property Product As String
        Public Property Length As Integer
        ''' <summary>
        ''' COG(s)
        ''' </summary>
        ''' <returns></returns>
        Public Property COG As String
        ''' <summary>
        ''' Protein name
        ''' </summary>
        ''' <returns></returns>
        Public Property ProteinName As String

        Public Function ToPTTGene() As GeneBrief
            Return New GeneBrief With {
                .Code = Me.Product,
                .COG = Me.COG,
                .Gene = Me.GeneID,
                .Location = GetLoci(),
                .PID = Me.GeneID,
                .Product = Me.ProteinName,
                .Synonym = Me.Locus_tag
            }
        End Function

        Public Function GetLoci() As NucleotideLocation
            Dim strand = GetStrand(Me.Strand)
            Dim loci As New NucleotideLocation(Me.Start, Me.Stop, strand)
            Return loci
        End Function

        Public Overrides Function ToString() As String
            Return Locus_tag & vbTab & GetLoci.ToString
        End Function
    End Class
End Namespace