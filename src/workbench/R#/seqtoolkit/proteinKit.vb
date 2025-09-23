#Region "Microsoft.VisualBasic::ae5a7cd9c004ad7c345fd5ecd51d7a60, R#\seqtoolkit\proteinKit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 287
    '    Code Lines: 121 (42.16%)
    ' Comment Lines: 138 (48.08%)
    '    - Xml Docs: 91.30%
    ' 
    '   Blank Lines: 28 (9.76%)
    '     File Size: 12.96 KB


    ' Module proteinKit
    ' 
    '     Function: (+2 Overloads) ChouFasman, kmer_fingerprint, kmer_graph, parsePdb, pdbModels
    '               readPdb
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.RCSB.PDB
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Kmer
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.ProteinModel.ChouFasmanRules
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' A computational biology toolkit for protein structural analysis and sequence-based modeling. 
''' This module provides R-language interfaces for predicting secondary structures, parsing molecular 
''' structure files, and generating graph-based protein sequence fingerprints.
''' 
''' Key functionalities include:
''' 1. Chou-Fasman secondary structure prediction algorithm implementation
''' 2. Protein Data Bank (PDB) file format parsing
''' 3. K-mer graph construction for sequence pattern analysis
''' 4. Morgan fingerprint generation for structural similarity comparison
''' </summary>
''' <remarks>
''' This module bridges biological sequence analysis with graph theory concepts, enabling:
''' - Rapid prediction of alpha-helices and beta-sheets from amino acid sequences
''' - Structural feature extraction from PDB files
''' - Topological representation of proteins as k-mer adjacency graphs
''' - Fixed-length hashing of structural patterns for machine learning applications
''' 
''' Dependencies: 
''' - Requires R# runtime environment for interop
''' - Relies on SMRUCC.genomics libraries for core bioinformatics operations
''' </remarks>
''' <example>
''' # Basic workflow example:
''' let seqs = read.fasta("./proteome.fa");
''' 
''' # 1. Secondary structure prediction
''' let structures = sapply(seqs, x => chou_fasman(x));
''' 
''' # 2. Create k-mer graphs
''' let graphs = kmer_graph(seqs, k = 3);
''' 
''' # 3. Generate fingerprints
''' let fingerprints = lapply(graphs, g => kmer_fingerprint(g, radius = 2));
''' 
''' </example>
''' <seealso cref="ChouFasmanRules"/> Class implementing core prediction logic
''' <seealso cref="KMerGraph"/> Data structure for k-mer adjacency relationships
''' <seealso cref="PDB"/> Protein Data Bank object model
''' <author type="copyright">SMRUCC genomics Institute</author>
''' <version>1.6.0</version>
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
    ''' <param name="polyaa">
    ''' returns <see cref="StructuralAnnotation"/> clr object model if this parameter is set TRUE, otherwise returns 
    ''' the string representitive of the chou-fasman structure information.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' print(chou_fasman("AAABAAGKKKJLLMMMMMM"));
    ''' </example>
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
        Dim aa As ChouFasmanRules.AminoAcid() = ChouFasmanRules.Calculate(prot)

        If polyaa Then
            Return New StructuralAnnotation With {
                .polyseq = aa
            }
        Else
            Return ChouFasmanRules.ToString(aa)
        End If
    End Function

    ''' <summary>
    ''' parse the pdb struct data from a given document text data
    ''' </summary>
    ''' <param name="pdb_txt"></param>
    ''' <param name="safe"></param>
    ''' <returns></returns>
    <ExportAPI("parse_pdb")>
    <RApiReturn(GetType(PDB))>
    Public Function parsePdb(pdb_txt As String, Optional safe As Boolean = False, Optional verbose As Boolean = False) As Object
        If safe Then
            Try
                Return PDB.Parse(pdb_txt, verbose)
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.Message.warning

                Return Nothing
            End Try
        Else
            Return PDB.Parse(pdb_txt, verbose)
        End If
    End Function

    ''' <summary>
    ''' Reads a Protein Data Bank (PDB) file and parses it into a PDB object model.
    ''' </summary>
    ''' <param name="file">A file path string or Stream object representing the PDB file to read.</param>
    ''' <param name="env">The R runtime environment for error handling and resource management.</param>
    ''' <returns>Returns a parsed <see cref="PDB"/> object if successful. Returns a <see cref="Message"/> 
    ''' error object if file loading fails due to invalid path or format issues.</returns>
    ''' <example>
    ''' let pdb = read.pdb("1abc.pdb");
    ''' </example>
    <ExportAPI("read.pdb")>
    <RApiReturn(GetType(PDB))>
    Public Function readPdb(<RRawVectorArgument>
                            file As Object,
                            Optional safe As Boolean = False,
                            Optional env As Environment = Nothing) As Object

        Dim is_path As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_path)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim pdb As PDB() = Nothing

        If safe Then
            Try
                pdb = RCSB.PDB.PDB.Load(s.TryCast(Of Stream)).ToArray
            Catch ex As Exception
                Call ex.Message.warning
                Call App.LogException(ex)
            End Try
        Else
            pdb = RCSB.PDB.PDB.Load(s.TryCast(Of Stream)).ToArray
        End If
        If is_path Then
            Call s.TryCast(Of Stream).Dispose()
        End If

        Return pdb
    End Function

    ''' <summary>
    ''' get structure models inside the given pdb object
    ''' </summary>
    ''' <param name="pdb"></param>
    ''' <returns></returns>
    <ExportAPI("pdb_models")>
    <RApiReturn(GetType(Atom))>
    Public Function pdbModels(pdb As PDB) As Object
        Return pdb.AsEnumerable.ToArray
    End Function

    <ExportAPI("pdb_centroid")>
    <RApiReturn(GetType(Point3D), GetType(Double))>
    Public Function pdb_centroid(pdb As PDB, Optional as_vector As Boolean = False) As Object
        If as_vector Then
            With pdb.ModelCentroid
                Return New Double() { .X, .Y, .Z}
            End With
        Else
            Return pdb.ModelCentroid
        End If
    End Function

    <ExportAPI("ligands")>
    <RApiReturn(GetType(Het.HETRecord))>
    Public Function ligands(pdb As PDB, Optional key As String = Nothing, Optional number As Integer = -1)
        If key.StringEmpty(, True) OrElse number <= 0 Then
            ' get all 
            Return pdb.ListLigands.Values.ToArray
        Else
            Return pdb.ListLigands _
                .Where(Function(li)
                           Return li.Name = key AndAlso
                               li.Value.SequenceNumber = number
                       End Function) _
                .FirstOrDefault
        End If
    End Function

    ''' <summary>
    ''' Constructs k-mer adjacency graphs from protein sequence data. Nodes represent k-length 
    ''' subsequences, edges connect k-mers appearing consecutively in the sequence.
    ''' </summary>
    ''' <param name="prot">A FASTA sequence or collection of FASTA sequences to process.</param>
    ''' <param name="k">The subsequence length parameter for k-mer generation. Default is 3.</param>
    ''' <param name="env">The R runtime environment for error handling and resource cleanup.</param>
    ''' <returns>Returns a single <see cref="KMerGraph"/> for single sequence input. Returns a named list 
    ''' of KMerGraph objects for multiple sequences. Returns error message for invalid inputs.</returns>
    ''' <example>
    ''' let graphs = kmer_graph(sequences, k = 3);
    ''' </example>
    <ExportAPI("kmer_graph")>
    <RApiReturn(GetType(KMerGraph))>
    Public Function kmer_graph(<RRawVectorArgument>
                               prot As Object,
                               Optional k As Integer = 3,
                               Optional env As Environment = Nothing) As Object

        Dim seq = GetFastaSeq(prot, env)

        If seq Is Nothing Then
            Return Message.InCompatibleType(GetType(FastaSeq), prot.GetType, env)
        End If

        Dim pool As FastaSeq() = seq.ToArray

        If pool.Length = 1 Then
            Return KMerGraph.FromSequence(seq(0), k)
        Else
            Dim graphSet As list = list.empty

            For Each p As FastaSeq In seq
                Call graphSet.unique_add(p.Title, KMerGraph.FromSequence(p, k))
            Next

            Return graphSet
        End If
    End Function

    ''' <summary>
    ''' Calculate the morgan fingerprint based on the k-mer graph data 
    ''' 
    ''' Generates fixed-length molecular fingerprint vectors from k-mer graphs using 
    ''' Morgan algorithm with circular topology hashing.
    ''' </summary>
    ''' <param name="graph">The k-mer graph object to fingerprint</param>
    ''' <param name="radius">Neighborhood radius for structural feature capture. Larger values 
    ''' consider more distant node relationships. Default is 3.</param>
    ''' <param name="len">Output vector length (uses modulo hashing). Default 4096.</param>
    ''' <returns>Integer array fingerprint where indices represent structural features</returns>
    ''' <example>
    ''' let seqs = read.fasta("./proteins.fa");
    ''' # create protein sequence graph based on k-mer(k=3)
    ''' let g = kmer_graph(seqs, k = 3);
    ''' 
    ''' for(let kmer in g) {
    '''     print(kmer |> kmer_fingerprint());
    ''' }
    ''' </example>
    <ExportAPI("kmer_fingerprint")>
    Public Function kmer_fingerprint(graph As KMerGraph,
                                     Optional radius As Integer = 3,
                                     Optional len As Integer = 4096) As Object

        Return graph.GetFingerprint(radius, len)
    End Function

    <ExportAPI("enzyme_builder")>
    <RApiReturn(GetType(TransformerModel))>
    Public Function enzymeBuilder(<RRawVectorArgument> enzymes As Object,
                                  Optional kmer As Integer = 3,
                                  Optional env As Environment = Nothing) As Object

        Dim seq = GetFastaSeq(enzymes, env)

        If seq Is Nothing Then
            Return Message.InCompatibleType(GetType(FastaSeq), enzymes.GetType, env)
        End If

        Return seq.MakeModel(kmer)
    End Function

    <ExportAPI("predict_sequence")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function predict_sequence(model As TransformerModel, <RRawVectorArgument> ec_number As Object, Optional env As Environment = Nothing) As Object
        Return model.BuildProteinSequence(CLRVector.asCharacter(ec_number)).ToArray
    End Function
End Module
