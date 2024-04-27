#Region "Microsoft.VisualBasic::9dc4d37dfc095690e42a2516c5feb5e1, G:/GCModeller/src/GCModeller/analysis/Motifs/PrimerDesigner//Restriction_enzyme/WikiLoader.vb"

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

    '   Total Lines: 105
    '    Code Lines: 82
    ' Comment Lines: 8
    '   Blank Lines: 15
    '     File Size: 3.88 KB


    '     Module WikiLoader
    ' 
    '         Function: LoadResFile, ParseCutSites, ParseRecognition, PullAll, PullInternal
    ' 
    '     Class EnzymeTable
    ' 
    '         Properties: cut, enzyme, isoschizomers, organism, pdb
    '                     recognition
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Namespace Restriction_enzyme

    ''' <summary>
    ''' Parser for wikipadia page resource data
    ''' </summary>
    Public Module WikiLoader

        ''' <summary>
        ''' load all enzymne data from internal resource database
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PullAll() As IEnumerable(Of Enzyme)
            Return PullInternal.IteratesALL
        End Function

        Private Iterator Function PullInternal() As IEnumerable(Of Enzyme())
            Yield My.Resources.A.LoadResFile.ToArray
            Yield My.Resources.Ba_Bc.LoadResFile.ToArray
            Yield My.Resources.Bd_Bp.LoadResFile.ToArray
            Yield My.Resources.Bsa_Bso.LoadResFile.ToArray
            Yield My.Resources.Bsp_Bss.LoadResFile.ToArray
            Yield My.Resources.Bst_Bv.LoadResFile.ToArray
            Yield My.Resources.C_D.LoadResFile.ToArray
            Yield My.Resources.E_F.LoadResFile.ToArray
            Yield My.Resources.G_K.LoadResFile.ToArray
            Yield My.Resources.L_N.LoadResFile.ToArray
            Yield My.Resources.O_R.LoadResFile.ToArray
            Yield My.Resources.S.LoadResFile.ToArray
            Yield My.Resources.T_Z.LoadResFile.ToArray
        End Function

        <Extension>
        Private Iterator Function LoadResFile(file As String) As IEnumerable(Of Enzyme)
            Dim table As EnzymeTable() = DataFrame.Parse(content:=file) _
                .AsDataSource(Of EnzymeTable) _
                .ToArray

            For Each row As EnzymeTable In table
                Yield New Enzyme With {
                    .Enzyme = row.enzyme,
                    .Isoschizomers = row.isoschizomers.StringSplit(",\s*"),
                    .PDB = row.pdb,
                    .Source = row.organism,
                    .Recognition = ParseRecognition(row.recognition),
                    .Cut = ParseCutSites(row.cut).ToArray
                }
            Next
        End Function

        Private Iterator Function ParseCutSites(str As String) As IEnumerable(Of Cut)
            Dim si As String() = str.Matches("\d'\s+[-]+.*?[-]+\s+\d'").ToArray
            Dim reversed As Boolean
            Dim s2 As String()

            For Each s As String In si
                If s.StartsWith("3'") Then
                    reversed = True
                Else
                    reversed = False
                End If

                s = s.StringReplace("5'", "").StringReplace("3'", "").Trim(" "c, "-"c)
                s2 = s.Split

                Yield New Cut With {
                    .Reversed = reversed,
                    .CutSite1 = s2(0),
                    .CutSite2 = s2.ElementAtOrDefault(1)
                }
            Next
        End Function

        Private Function ParseRecognition(str As String) As Recognition
            Dim si As String() = str.Matches("\d'\s*\S+") _
                .Select(Function(s2)
                            Return s2.StringReplace("\d'", "").Trim
                        End Function) _
                .ToArray
            Dim r As New Recognition With {
                .Forwards = si(0),
                .Reversed = si(1)
            }

            Return r
        End Function
    End Module

    Friend Class EnzymeTable

        Public Property enzyme As String
        Public Property pdb As String
        Public Property organism As String
        Public Property recognition As String
        Public Property cut As String
        Public Property isoschizomers As String

    End Class
End Namespace
