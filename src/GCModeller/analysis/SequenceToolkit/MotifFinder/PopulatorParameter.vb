#Region "Microsoft.VisualBasic::173581fcb2267f13e46646ef5b857874, analysis\SequenceToolkit\MotifScanner\Consensus\DATA.vb"

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

' Class PopulatorParameter
' 
'     Properties: maxW, minW, ScanCutoff, ScanMinW, seedingCutoff
'                 seedOccurances
' 
'     Function: DefaultParameter, ToString
' 
' Class SequenceMotif
' 
'     Properties: AverageScore, length, seeds
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports any = Microsoft.VisualBasic.Scripting

Public Class PopulatorParameter

    ''' <summary>
    ''' <see cref="Protocol.pairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property minW As Integer
    ''' <summary>
    ''' <see cref="Protocol.pairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property maxW As Integer
    ''' <summary>
    ''' <see cref="Protocol.pairwiseSeeding(FastaSeq, FastaSeq, PopulatorParameter)"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property seedingCutoff As Double
    ''' <summary>
    ''' [0, 1]，表示种子至少要在这个属性值所表示的百分比数量上的原始序列出现
    ''' </summary>
    ''' <returns></returns>
    Public Property seedOccurances As Double
    Public Property ScanMinW As Integer
    Public Property ScanCutoff As Double

    Public Property log As Action(Of Object)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function DefaultParameter() As [Default](Of PopulatorParameter)
        Return New PopulatorParameter With {
            .minW = 8,
            .maxW = 14,
            .seedingCutoff = 0.9,
            .ScanCutoff = 0.6,
            .ScanMinW = 6,
            .seedOccurances = 0.6,
            .log = Sub(msg) Call any.ToString(msg).__DEBUG_ECHO
        }
    End Function

End Class

