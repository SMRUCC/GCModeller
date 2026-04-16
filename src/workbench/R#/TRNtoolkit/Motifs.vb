#Region "Microsoft.VisualBasic::1e3eea934af2f3da2da08571113c83b9, R#\TRNtoolkit\Motifs.vb"

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

    '   Total Lines: 66
    '    Code Lines: 51 (77.27%)
    ' Comment Lines: 5 (7.58%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (15.15%)
    '     File Size: 2.32 KB


    ' Module MotifsTool
    ' 
    '     Function: load_motifs, open_meme_dir, pwm_table, read_meme, save_meme
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Interops.NBCR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("motif_tool")>
Module MotifsTool

    Sub Main()
        Call Converts.makeDataframe.addHandler(GetType(Probability), AddressOf pwm_table)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function pwm_table(pwm As Probability, args As list, env As Environment) As dataframe
        Dim alphabets As String() = pwm.region _
            .Select(Function(r) r.frequency.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim tbl As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        For Each c As String In alphabets
            Call tbl.add(c, From r As Residue
                            In pwm.region
                            Select r.frequency.TryGetValue(c))
        Next

        Return tbl
    End Function

    <ExportAPI("load_motifs")>
    <RApiReturn(GetType(Probability))>
    Public Function load_motifs(db As SequencePatterns.PWMDatabase, name As String) As Object
        Return db.LoadFamilyMotifs(name).ToArray
    End Function

    ''' <summary>
    ''' read meme motif text file
    ''' </summary>
    ''' <param name="file">file path to the meme motif text file(*.meme)</param>
    ''' <returns></returns>
    <ExportAPI("read_meme")>
    Public Function read_meme(file As String) As MotifPWM()
        Return MEME_Suite.ParsePWMFile(file).ToArray
    End Function

    <ExportAPI("save_meme")>
    Public Function save_meme(motif As Probability, file As String) As Object
        Return motif.SaveToMeme(file)
    End Function

    <ExportAPI("open_meme_dir")>
    Public Function open_meme_dir(dir As String) As MEMEMotifRepository
        Return New MEMEMotifRepository(dir)
    End Function

End Module

