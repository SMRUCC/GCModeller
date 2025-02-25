Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.ProteinModel.ChouFasmanRules
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("proteinKit")>
Module proteinKit

    ''' <summary>
    ''' The Chou-Fasman method is a bioinformatics technique used for predicting the secondary structure of proteins. 
    ''' It was developed by Peter Y. Chou and Gerald D. Fasman in the 1970s. The method is based on the observation 
    ''' that certain amino acids have a propensity to form specific types of secondary structures, such as alpha-helices, 
    ''' beta-sheets, and turns.
    ''' 
    ''' Here's a brief overview of how the Chou-Fasman method works:
    ''' 
    ''' 1. **Amino Acid Propensities**: Each amino acid is assigned a set of probability values that reflect its 
    '''    tendency to be found in alpha-helices, beta-sheets, and turns. These values are derived from statistical 
    '''    analysis of known protein structures.
    ''' 2. **Sliding Window Technique**: A sliding window of typically 7 to 9 amino acids is moved along the protein 
    '''    sequence. At each position, the average propensity for each type of secondary structure is calculated 
    '''    for the amino acids within the window.
    ''' 3. **Thresholds and Rules**: The method uses predefined thresholds and rules to identify regions of the 
    '''    protein sequence that are likely to form alpha-helices or beta-sheets based on the calculated propensities. 
    '''    For example, a region with a high average propensity for alpha-helix and meeting certain criteria 
    '''    might be predicted to form an alpha-helix.
    ''' 4. **Secondary Structure Prediction**: The method predicts the secondary structure by identifying contiguous 
    '''    regions of the sequence that exceed the thresholds for helix or sheet formation. It also takes into 
    '''    account the likelihood of turns, which are important for the overall folding of the protein.
    ''' 5. **Refinement**: The initial predictions are often refined using additional rules and considerations, such 
    '''    as the tendency of certain amino acids to stabilize or destabilize specific structures, and the overall 
    '''    composition of the protein.
    '''    
    ''' The Chou-Fasman method was one of the first widely used techniques for predicting protein secondary structure
    ''' and played a significant role in the field of structural bioinformatics. However, it has largely been superseded
    ''' by more accurate methods, such as those based on machine learning and neural networks, which can take into
    ''' account more complex patterns and interactions within protein sequences.
    ''' 
    ''' Despite its limitations, the Chou-Fasman method remains a historical milestone in the understanding of 
    ''' protein structure and the development of computational methods for predicting it. It also serves as a 
    ''' foundational concept for those learning about protein structure prediction and bioinformatics.
    ''' </summary>
    ''' <param name="prot">a collection of the protein sequence data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("chou_fasman")>
    <RApiReturn(GetType(String), GetType(StructuralAnnotation))>
    Public Function ChouFasman(<RRawVectorArgument> prot As Object,
                               Optional polyaa As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        Dim seq = GetFastaSeq(prot, env)

        If seq Is Nothing Then
            Return Message.InCompatibleType(GetType(FastaSeq), prot.GetType, env)
        End If

        Dim pool As FastaSeq() = seq.ToArray

        If pool.Length = 1 Then
            Return pool(0).ChouFasman(polyaa)
        Else
            Dim result As list = list.empty

            For Each aa As FastaSeq In pool
                Call result.unique_add(aa.Title, aa.ChouFasman(polyaa))
            Next

            Return result
        End If
    End Function

    <Extension>
    Private Function ChouFasman(prot As FastaSeq, polyaa As Boolean) As Object
        Dim aa As AminoAcid() = ChouFasmanRules.Calculate(prot)

        If polyaa Then
            Return New StructuralAnnotation With {
                .polyseq = aa
            }
        Else
            Return ChouFasmanRules.ToString(aa)
        End If
    End Function

    ''' <summary>
    ''' read the protein database file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.pdb")>
    Public Function readPdb(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim is_path As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_path)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If


    End Function

End Module
