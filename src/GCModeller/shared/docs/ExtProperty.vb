#Region "Microsoft.VisualBasic::54c8c31b835fc37e7aaf65100ccee962, shared\docs\ExtProperty.vb"

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

    '     Module ExtProperty
    ' 
    '         Function: AssignTSSsId, BoundaryOverlaps, IsPossibleRNA, IsRNA, PredictedsiRNA
    '                   TSSsOverlapsATG, TTSsOverlapsTGA
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.Language

Namespace DocumentFormat

    Public Module ExtProperty

        <Extension> Public Function IsPossibleRNA(transcript As Transcript) As Boolean
            If transcript.Length < Math.Abs(transcript.ATG - transcript.TGA) Then
                Return True
            ElseIf transcript._5UTR >= 1000 Then
                Return True
            Else
                Return False
            End If
        End Function

        <Extension> Public Function IsRNA(transcript As Transcript) As Boolean
            Return transcript.TSSs <> 0 AndAlso transcript.ATG = 0
        End Function

        <Extension> Public Function TSSsOverlapsATG(tr As Transcript) As Boolean
            Return tr.TSSs = tr.ATG
        End Function

        <Extension> Public Function TTSsOverlapsTGA(tr As Transcript) As Boolean
            Return tr.TTSs = tr.TGA
        End Function

        ''' <summary>
        ''' 上下游都分别重合不能够延伸
        ''' </summary>
        ''' <returns></returns>
        <Extension> Public Function BoundaryOverlaps(tr As Transcript) As Boolean
            Return tr.TTSsOverlapsTGA AndAlso tr.TSSsOverlapsATG
        End Function

        <Extension> Public Function PredictedsiRNA(tr As Transcript) As Boolean
            Return tr.BoundaryOverlaps AndAlso tr.MappingLocation.FragmentSize <= 150
        End Function

        <Extension> Public Function AssignTSSsId(source As Generic.IEnumerable(Of Transcript), Optional prefix As String = "TSS_") As Transcript()
            Dim array As Transcript() = source.ToArray
            Dim i As i32 = 1

            For Each transcript As Transcript In array
                If transcript._5UTR <> 0 Then
                    transcript.TSS_ID = prefix & STDIO.ZeroFill(++i, 4)
                Else
                    If transcript.TSSsShared >= 30 Then
                        transcript.TSS_ID = prefix & STDIO.ZeroFill(++i, 4)
                    End If
                End If
            Next

            Return array
        End Function
    End Module
End Namespace
