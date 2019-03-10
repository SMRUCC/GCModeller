#Region "Microsoft.VisualBasic::ea2df8c77b9c714a07c9995b387cf8b1, CLI_tools\c2\Reconstruction\Genome\Genes.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports Microsoft.VisualBasic.Extensions
'Imports Microsoft.VisualBasic.consoledevice.stdio  

'Namespace Reconstruction : Partial Class TranscriptUnit

'        Protected Friend Class Genes : Inherits c2.Reconstruction.Operation

'            Dim rctGenes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes

'            Sub New(Session As OperationSession)
'                Call MyBase.New(Session)
'                rctGenes = MyBase.Reconstructed.GetGenes
'            End Sub

'            Public Overrides Function Performance() As Integer
'                Dim LQuery = From Gene As LANS.SystemsBiology.Assembly.FASTA In MyBase.Reconstructed.Database.dnaseq
'                             Let Segment As LANS.SystemsBiology.Assembly.NucleicAcid.Segment = c2.Reconstruction.Promoters.PromoterFinder.TryParse(Gene)
'                             Let UniqueId As String = Segment.Title.Split.First
'                             Where rctGenes.IndexOf(UniqueId) = -1
'                             Select NewGeneObject(Segment, UniqueId) '

'                Dim Result = LQuery.ToArray
'                Call rctGenes.AddRange(Result)

'                Return Result.Count
'            End Function

'            Private Function NewGeneObject(Segment As LANS.SystemsBiology.Assembly.NucleicAcid.Segment, UniqueId As String) As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene
'                Dim GeneObj As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene =
'                        New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene
'                GeneObj.UniqueId = UniqueId
'                GeneObj.Accession1 = Segment.Title.Split.Last
'                GeneObj.Product = New List(Of String) From {Segment.Description.Replace("""", "")}
'                GeneObj.LeftEndPosition = Segment.Left
'                GeneObj.RightEndPosition = Segment.Right
'                GeneObj.TranscriptionDirection = If(Segment.Complement = True, "+", "-")

'                Return GeneObj
'            End Function
'        End Class
'    End Class
'End Namespace
