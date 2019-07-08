Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports r = System.Text.RegularExpressions.Regex

Namespace Pipeline

    Public Module KOAssignment

        ''' <summary>
        ''' 在这里主要是将hit_name之中的KO编号提取出来就好了
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="removesNohit">Removes all of the alignment that produce hit not found...</param>
        ''' <returns></returns>
        <Extension>
        Public Function KOAssignmentSBH(result As IEnumerable(Of BestHit), Optional removesNohit As Boolean = True) As BestHit()
            Return result _
                .Select(Function(align)
                            align.HitName = r.Match(align.HitName, "K\d+", RegexICSng).Value Or align.HitName.Split.First.AsDefault
                            Return align
                        End Function) _
                .Where(Function(align)
                           If removesNohit Then
                               If align.HitName.IsPattern("K\d+") Then
                                   Return True
                               Else
                                   Return False
                               End If
                           Else
                               Return True
                           End If
                       End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' KO number assignment in bbh method
        ''' </summary>
        ''' <param name="drf">Besthits in direction forward</param>
        ''' <param name="drr">Besthits in direction reverse</param>
        ''' <returns></returns>
        Public Function KOassignmentBBH(drf As BestHit(), drr As BestHit()) As BiDirectionalBesthit()

        End Function
    End Module
End Namespace