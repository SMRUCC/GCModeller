#Region "Microsoft.VisualBasic::ddb27252f7fd643ac9220acc1108ec79, engine\IO\GCTabular\TranscriptRegulation.vb"

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

    ' Class TranscriptRegulation
    ' 
    '     Properties: BiologicalProcess, DoorId, Effectors, Ends, Information_Content
    '                 Log_Likelihood_Ratio, MAST_Evalue, MAST_Pvalue, MatchedMotif, MEME_Evalue
    '                 MEME_Pvalue, MotifId, ObjectId, OperonGeneIds, OperonPromoter
    '                 PccArray, RegpreciseRegulator, Relative_Entropy, Signature, Starts
    '                 Strand, TF, WGCNAWeight, Width
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Public Class TranscriptRegulation

    <Column("MatchedRegulator")> Public Property RegpreciseRegulator As String
    Public Property ObjectId As String
    Public Property Strand As String
    Public Property MatchedMotif As String
    Public Property DoorId As String
    <Column("MAST.E-value")> Public Property MAST_Evalue As Double
    <Column("MAST.P-value")> Public Property MAST_Pvalue As Double
    <CollectionAttribute("SequenceId", "; ")> Public Property OperonGeneIds As String()
    <Column("MEME.P-value")> Public Property MEME_Pvalue As Double
    Public Property Starts As Long
    Public Property Ends As Long
    Public Property MotifId As String
    <Column("MEME.E-value")> Public Property MEME_Evalue As Double
    Public Property Width As Integer
    Public Property Log_Likelihood_Ratio As Double
    Public Property Information_Content As Double
    Public Property Relative_Entropy As Double
    Public Property Signature As String

    ''' <summary>
    ''' 目标基因组内被匹配到的调控因子
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TF As String
    Public Property OperonPromoter As String

    <Column("BiologiclaProcess")> Public Property BiologicalProcess As String
    <CollectionAttribute("Effectors")> Public Property Effectors As String()

    <CollectionAttribute("WGCNA-weights", "; ")> Public Property WGCNAWeight As Double()
    ''' <summary>
    ''' <see cref="TF"></see>和<see cref="OperonPromoter"></see>
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Pcc.TF->Operon")> Public Property TFPcc As Double
    <CollectionAttribute("Pcc-array", "; ")> Public Property PccArray As Double()

    Public Overrides Function ToString() As String
        Return String.Format("{0} ({1}) --> {2}", TF, TFPcc, OperonPromoter)
    End Function
End Class
