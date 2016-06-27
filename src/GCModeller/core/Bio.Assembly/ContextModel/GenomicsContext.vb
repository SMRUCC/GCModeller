Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci

Namespace ContextModel

    Public Interface IGenomicsContextProvider(Of T As IGeneBrief)

        ''' <summary>
        ''' Listing all of the features loci sites on the genome. 
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property AllFeatures As T()
        Default ReadOnly Property Feature(locus_tag As String) As T

        Function GetByName(locus_tag As String) As T

        ''' <summary>
        ''' 获取某一个位点在位置上有相关联系的基因
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="unstrand"></param>
        ''' <param name="ATGDist"></param>
        ''' <returns></returns>
        Function GetRelatedGenes(loci As NucleotideLocation,
                                 Optional unstrand As Boolean = False,
                                 Optional ATGDist As Integer = 500) As Relationship(Of T)()

        ''' <summary>
        ''' Gets all of the feature sites on the specific <see cref="Strands"/> nucleotide sequence
        ''' </summary>
        ''' <param name="strand"></param>
        ''' <returns></returns>
        Function GetStrandFeatures(strand As Strands) As T()
    End Interface
End Namespace