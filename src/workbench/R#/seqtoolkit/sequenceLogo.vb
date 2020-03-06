#Region "Microsoft.VisualBasic::74cd7244918386352d065a33b096be11, R#\seqtoolkit\sequenceLogo.vb"

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

' Module sequenceLogo
' 
'     Function: DrawLogo
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

<Package("bioseq.sequenceLogo")>
Module sequenceLogo

    ''' <summary>
    ''' Drawing the sequence logo just simply modelling this motif site from the clustal multiple sequence alignment.
    ''' </summary>
    ''' <param name="MSA"></param>
    ''' <param name="title"></param>
    ''' <returns></returns>
    <ExportAPI("plot.seqLogo")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function DrawLogo(<RRawVectorArgument> MSA As Object, Optional title$ = "", Optional env As Environment = Nothing) As Object
        If MSA Is Nothing Then
            Return REnv.debug.stop("MSA is nothing!", env)
        End If

        Dim data As IEnumerable(Of FastaSeq) = GetFastaSeq(MSA)

        If data Is Nothing Then
            Dim type As Type = MSA.GetType

            Select Case type
                Case GetType(SequenceMotif)
                    data = DirectCast(MSA, SequenceMotif).seeds.ToFasta
                Case Else
                    Return REnv.debug.stop(New InvalidProgramException, env)
            End Select
        End If

        Return DrawingDevice.DrawFrequency(New FastaFile(data), title)
    End Function
End Module

