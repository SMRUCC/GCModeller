Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' FastQ toolkit
''' </summary>
''' <remarks>
''' FASTQ format Is a text-based format For storing both a biological sequence 
''' (usually nucleotide sequence) And its corresponding quality scores. Both 
''' the sequence letter And quality score are Each encoded With a Single ASCII 
''' character For brevity. It was originally developed at the Wellcome Trust 
''' Sanger Institute To bundle a FASTA formatted sequence And its quality data, 
''' but has recently become the de facto standard For storing the output Of 
''' high-throughput sequencing instruments such As the Illumina Genome 
''' Analyzer.
''' </remarks>
<Package("FastQ")>
Public Module FastQ

    ''' <summary>
    ''' In FASTQ files, quality scores are encoded into a compact form, 
    ''' which uses only 1 byte per quality value. In this encoding, the 
    ''' quality score is represented as the character with an ASCII 
    ''' code equal to its value + 33.
    ''' </summary>
    ''' <param name="q"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("quality_score")>
    <RApiReturn(GetType(Double))>
    Public Function GetQualityScore(q As Object, Optional env As Environment = Nothing) As Object
        If q Is Nothing Then
            Return Nothing
        End If

        If TypeOf q Is String Then
            Return FQ.FastQ _
                .GetQualityOrder(CStr(q)) _
                .Select(Function(d) CDbl(d)) _
                .ToArray
        ElseIf TypeOf q Is FastQFile Then
            Return New list With {
                .slots = DirectCast(q, FastQFile) _
                    .ToDictionary(Function(i) i.Title,
                                  Function(i)
                                      Dim scores = FQ.FastQ _
                                         .GetQualityOrder(i.Quality) _
                                         .Select(Function(d) CDbl(d)) _
                                         .ToArray

                                      Return CObj(scores)
                                  End Function)
            }
        ElseIf TypeOf q Is FQ.FastQ Then
            Return FQ.FastQ _
                .GetQualityOrder(DirectCast(q, FQ.FastQ).Quality) _
                .Select(Function(d) CDbl(d)) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(FQ.FastQ), q.GetType, env)
        End If
    End Function
End Module
