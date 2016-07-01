Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.DatabaseServices
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Workflows.PromoterParser

    Public Class RegulonParser : Inherits GenePromoterParser

        ReadOnly _DOOR As DOOR

        ''' <summary>
        ''' 基因组的Fasta核酸序列
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <remarks></remarks>
        Sub New(Fasta As FastaToken, PTT As PTT, DOOR As DOOR)
            Call MyBase.New(Fasta, PTT)
            _DOOR = DOOR
        End Sub

        Public Function RegulonParser(regulon As Regprecise.Regulator, len As Integer, Optional method As GetLocusTags = GetLocusTags.UniDOOR) As FastaFile
            Dim regulates As String() = regulon.Regulates.ToArray(Function(x) x.LocusId).Distinct.ToArray
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(_DOOR, method)
            Dim uniOpr As String() = (From sId As String
                                      In regulates
                                      Select GetDOORUni(sId)).MatrixAsIterator.Distinct.ToArray
            Dim fa = Me.GetSequenceById(uniOpr, len)
            Return fa
        End Function

        Public Function RegulonParser(genome As Regprecise.BacteriaGenome, outDIR As String) As Boolean
            For Each len As Integer In GenePromoterParser.PrefixLength
                For Each regulon In genome.Regulons.Regulators
                    Dim fa = RegulonParser(regulon, len)
                    Dim path As String = $"{outDIR}/{len}/{regulon.LocusId.NormalizePathString(True)}.{regulon.LocusTag.Value.NormalizePathString(True)}.fasta"
                    Call fa.Save(path)
                Next
            Next

            Return True
        End Function

        Public Function RegulonParser(inDIR As String, outDIR As String) As Boolean
            Dim genomes = inDIR.LoadSourceEntryList({"*.xml"}).ToArray(Function(x) x.Value.LoadXml(Of Regprecise.BacteriaGenome))

            For Each genome As Regprecise.BacteriaGenome In genomes
                If Not genome.Regulons Is Nothing AndAlso Not genome.Regulons.Regulators.IsNullOrEmpty Then
                    Call RegulonParser(genome, outDIR)
                End If

                Call genome.BacteriaGenome.name.__DEBUG_ECHO
            Next

            Return True
        End Function
    End Class
End Namespace