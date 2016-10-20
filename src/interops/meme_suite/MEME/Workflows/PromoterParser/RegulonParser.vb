#Region "Microsoft.VisualBasic::be838455c8cfc77769d95a254c2ae2aa, ..\interops\meme_suite\MEME\Workflows\PromoterParser\RegulonParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Workflows.PromoterParser

    Public Class RegulonParser : Inherits GenePromoterParser

        ReadOnly _DOOR As DOOR

        ''' <summary>
        ''' 基因组的Fasta核酸序列
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <remarks></remarks>
        Sub New(Fasta As FastaToken, PTT As PTT, DOOR As DOOR)
            Call MyBase.New(Fasta, PTT)
            _DOOR = DOOR
        End Sub

        Public Function RegulonParser(regulon As Regprecise.Regulator, len As Integer, Optional method As GetLocusTags = GetLocusTags.UniDOOR) As FastaFile
            Dim regulates As String() = regulon.Regulates.ToArray(Function(x) x.LocusId).Distinct.ToArray
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(_DOOR, method)
            Dim uniOpr As String() = (From sId As String
                                      In regulates
                                      Select GetDOORUni(sId)).IteratesALL.Distinct.ToArray
            Dim fa = Me.GetSequenceById(uniOpr, len)
            Return fa
        End Function

        Public Function RegulonParser(genome As Regprecise.BacteriaGenome, outDIR As String) As Boolean
            For Each len As Integer In GenePromoterParser.PrefixLength
                For Each regulon In genome.Regulons.Regulators
                    Dim fa = RegulonParser(regulon, len)
                    Dim path As String = $"{outDIR}/{len}/{regulon.LocusId.NormalizePathString(True)}.{regulon.LocusTag.Value.NormalizePathString(True)}.fasta"
                    Call fa.Save(path)
                Next
            Next

            Return True
        End Function

        Public Function RegulonParser(inDIR As String, outDIR As String) As Boolean
            Dim genomes = inDIR.LoadSourceEntryList({"*.xml"}).ToArray(Function(x) x.Value.LoadXml(Of Regprecise.BacteriaGenome))

            For Each genome As Regprecise.BacteriaGenome In genomes
                If Not genome.Regulons Is Nothing AndAlso Not genome.Regulons.Regulators.IsNullOrEmpty Then
                    Call RegulonParser(genome, outDIR)
                End If

                Call genome.BacteriaGenome.name.__DEBUG_ECHO
            Next

            Return True
        End Function
    End Class
End Namespace
