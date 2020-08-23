#Region "Microsoft.VisualBasic::93b30f4972efaa4507119cfb56c010d4, comparative_genomics\DNA_Comparative.vb"

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

    ' Module DNA_Comparative
    ' 
    '     Function: dist, dnaA_gyrB, Sigma
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("DNA_Comparative")>
Public Module DNA_Comparative

    ''' <summary>
    ''' Create a distance matrix for a given sequence collection
    ''' </summary>
    ''' <param name="seqs"></param>
    ''' <returns></returns>
    <ExportAPI("seq.dist")>
    Public Function dist(<RRawVectorArgument> seqs As Object) As DistanceMatrix
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
    ''' is the average absolute dinucleotide relative abundance difference calculated as
    '''
    ''' ```
    ''' sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
    ''' ```
    ''' 
    ''' where the sum extends over all dinucleotides (abbreviated sigma-differences).
    ''' </summary>
    ''' <param name="f"></param>
    ''' <param name="g"></param>
    ''' <returns></returns>
    <ExportAPI("sigma")>
    Public Function Sigma(f As FastaSeq, g As FastaSeq) As vector
        Dim dist As Double = DifferenceMeasurement.Sigma(f, g)
        Dim desc As String = DifferenceMeasurement.SimilarDescription(dist).ToString

        Return vector.asVector({dist}, New unit(desc))
    End Function

    ''' <summary>
    ''' Using the DNA segment between the ``dnaA`` and ``gyrB`` as the reference rule.
    ''' </summary>
    ''' <param name="nt">
    ''' a fasta sequence object or NCBI genbank database object.
    ''' </param>
    ''' <param name="context"></param>
    ''' <returns></returns>
    <ExportAPI("dnaA_gyrB")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function dnaA_gyrB(nt As Object, Optional context As PTT = Nothing, Optional env As Environment = Nothing) As Object
        If nt Is Nothing Then
            Return Nothing
        End If

        If TypeOf nt Is GBFF.File Then
            Return DirectCast(nt, GBFF.File).dnaA_gyrB
        ElseIf TypeOf nt Is FastaSeq Then
            Return DirectCast(nt, FastaSeq).dnaA_gyrB(proteins:=context)
        Else
            Return Internal.debug.stop({
                "invalid object type for data sequence input...",
                "require: fasta",
                "given: " & nt.GetType.FullName
            }, env)
        End If
    End Function
End Module
