#Region "Microsoft.VisualBasic::1fd68fe2b01ebb17b661b2500c9d00d6, analysis\SequenceToolkit\TRNScanner\ScannerTask.vb"

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

    '   Total Lines: 58
    '    Code Lines: 48 (82.76%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (17.24%)
    '     File Size: 2.85 KB


    ' Module ScannerTask
    ' 
    '     Function: ScanSites, ScanTask
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports batch
Imports Darwinism.HPC.Parallel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module ScannerTask

    <Extension>
    Public Function ScanSites(regions As IEnumerable(Of FastaSeq), motif_db As String,
                              Optional n_threads As Integer = 8,
                              Optional identities_cutoff As Double = 0.8,
                              Optional minW As Double = 0.85,
                              Optional top As Integer = 3,
                              Optional permutation As Integer = 2500) As IEnumerable(Of MotifMatch)

        Dim task As New Func(Of FastaSeq, String, Double, Double, Integer, Integer, MotifMatch())(AddressOf ScanTask)

        DarwinismEnvironment.SetThreads(n_threads)
        DarwinismEnvironment.SetLibPath(App.HOME)

        Dim env As Argument = DarwinismEnvironment.GetEnvironmentArguments
        Dim source As FastaSeq() = regions.ToArray
        Dim result = Host.ParallelFor(Of FastaSeq, MotifMatch())(env, task, source, motif_db,
                                                                 identities_cutoff,
                                                                 minW,
                                                                 top,
                                                                 permutation).ToArray
        Return result.IteratesALL
    End Function

    <EmitStream(GetType(MotifSiteFile), Target:=GetType(MotifMatch()))>
    <EmitStream(GetType(FastaSocketFile), Target:=GetType(FastaSeq))>
    Public Function ScanTask(region As FastaSeq, motif_db As String,
                             identities_cutoff As Double,
                             minW As Double,
                             top As Integer,
                             permutation As Integer) As MotifMatch()

        Dim database = MotifDatabase.OpenReadOnly(motif_db.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        Dim pwm As Dictionary(Of String, Probability()) = database.LoadMotifs()
        Dim result As New List(Of MotifMatch)

        For Each family_pwm As KeyValuePair(Of String, Probability()) In pwm
            Call result.AddRange(region.ScanRegion(family_pwm.Key,
                                                   family_pwm.Value,
                                                   identities_cutoff:=identities_cutoff,
                                                   minW:=minW,
                                                   top:=top,
                                                   permutation:=permutation))
        Next

        Return result.ToArray
    End Function

End Module

