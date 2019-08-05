#Region "Microsoft.VisualBasic::e64ca7744a81f7aa9d9529177cb32f0d, meme_suite\MEME\Analysis\MotifScanning\MotifScan\TestAPI.vb"

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

    '     Module TestAPI
    ' 
    '         Function: (+5 Overloads) DeltaSimilarity, Nt, (+2 Overloads) PWM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.SequenceModel

Namespace Analysis.MotifScans

    <Package("MotifScansTools.Similarity", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module TestAPI

        <ExportAPI("PWM")>
        Public Function PWM(<Parameter("Nt.Seq", "ATCG base, nucleotide sequence.")> sequence As String) As MotifPM()
            Dim Nt = New FASTA.FastaSeq With {.SequenceData = sequence}
            Return MotifDeltaSimilarity.PWM(Nt)
        End Function

        <ExportAPI("NT")>
        Public Function Nt(sequence As String) As NucleotideModels.NucleicAcid
            Return New NucleotideModels.NucleicAcid(sequence)
        End Function

        <ExportAPI("PWM")>
        Public Function PWM(sequence As FASTA.FastaSeq) As MotifPM()
            Return MotifDeltaSimilarity.PWM(sequence)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As IEnumerable(Of MotifPM), g As IEnumerable(Of MotifPM)) As Double
            Return MotifDeltaSimilarity.Sigma(f.ToArray, g.ToArray)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As FASTA.FastaSeq, g As FASTA.FastaSeq) As Double
            Return DifferenceMeasurement.Sigma(f, g)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As NucleotideModels.NucleicAcid, g As NucleotideModels.NucleicAcid) As Double
            Return DifferenceMeasurement.Sigma(f, g)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As AnnotationModel, g As AnnotationModel) As Double
            Return MotifDeltaSimilarity.Sigma(f.PspMatrix, g.PspMatrix)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As AnnotationModel, g As NucleotideModels.NucleicAcid) As Double
            Return MEME_Suite.Analysis.Similarity.Sigma(f.PspMatrix, g:=MEME_Suite.Analysis.Similarity.PWM(g.ToArray))
        End Function
    End Module
End Namespace
