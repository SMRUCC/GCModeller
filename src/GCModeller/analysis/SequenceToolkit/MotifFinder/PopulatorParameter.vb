#Region "Microsoft.VisualBasic::6042f0ec3d3ab44c2244d7da08d93dc6, GCModeller\analysis\SequenceToolkit\MotifFinder\PopulatorParameter.vb"

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

'   Total Lines: 62
'    Code Lines: 38
' Comment Lines: 16
'   Blank Lines: 8
'     File Size: 2.02 KB


' Class PopulatorParameter
' 
'     Properties: log, maxW, minW, ScanCutoff, ScanMinW
'                 seedingCutoff, seedOccurances
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: DefaultParameter, ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports any = Microsoft.VisualBasic.Scripting

Public Class PopulatorParameter

    ''' <summary>
    ''' <see cref="MotifSeeds.PairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property minW As Integer
    ''' <summary>
    ''' <see cref="MotifSeeds.PairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property maxW As Integer
    ''' <summary>
    ''' <see cref="MotifSeeds.PairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property seedingCutoff As Double
    ''' <summary>
    ''' [0, 1]，表示种子至少要在这个属性值所表示的百分比数量上的原始序列出现
    ''' </summary>
    ''' <returns></returns>
    Public Property seedOccurances As Double
    Public Property seedScanner As Scanners = Scanners.TreeScan
    Public Property ScanMinW As Integer
    Public Property ScanCutoff As Double
    Public Property significant_sites As Integer = 4

    Public Property log As Action(Of Object)

    Sub New()
    End Sub

    Sub New(clone As PopulatorParameter)
        minW = clone.minW
        maxW = clone.maxW
        seedingCutoff = clone.seedingCutoff
        seedOccurances = clone.seedOccurances
        ScanMinW = clone.ScanMinW
        ScanCutoff = clone.ScanCutoff
        log = clone.log
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub logText(x As Object)
        Call _log(x)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Function GetScanner() As Type
        Select Case seedScanner
            Case Scanners.TreeScan : Return GetType(TreeScan)
            Case Scanners.FullScan : Return GetType(FullScan)
            Case Else
                Throw New NotImplementedException(seedScanner.Description)
        End Select
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function DefaultParameter() As [Default](Of PopulatorParameter)
        Return New PopulatorParameter With {
            .minW = 8,
            .maxW = 14,
            .seedingCutoff = 0.9,
            .ScanCutoff = 0.6,
            .ScanMinW = 6,
            .seedOccurances = 0.6,
            .log = Sub(msg) Call any.ToString(msg).__DEBUG_ECHO,
            .significant_sites = 4,
            .seedScanner = Scanners.TreeScan
        }
    End Function

End Class
