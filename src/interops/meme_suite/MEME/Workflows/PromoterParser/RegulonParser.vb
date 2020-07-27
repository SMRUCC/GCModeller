#Region "Microsoft.VisualBasic::22fcf39f171cdfce283cdf33d901c4f1, meme_suite\MEME\Workflows\PromoterParser\RegulonParser.vb"

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

    '     Class RegulonParser
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+3 Overloads) RegulonParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Workflows.PromoterParser

    Public Class RegulonParser : Inherits PromoterRegionParser

        ReadOnly _DOOR As DOOR

        ''' <summary>
        ''' 基因组的Fasta核酸序列
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <remarks></remarks>
        Sub New(Fasta As FastaSeq, PTT As PTT, DOOR As DOOR)
            Call MyBase.New(Fasta, PTT)
            _DOOR = DOOR
        End Sub

        Public Function RegulonParser(regulon As Regprecise.Regulator, len As Integer, Optional method As GetLocusTags = GetLocusTags.UniDOOR) As FastaFile
            Dim regulates As String() = regulon.Regulates.Select(Function(x) x.locusId).Distinct.ToArray
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(_DOOR, method)
            Dim uniOpr As String() = (From sId As String
                                      In regulates
                                      Select GetDOORUni(sId)).IteratesALL.Distinct.ToArray
            Dim fa = Me.GetSequenceById(uniOpr, len)
            Return fa
        End Function

        Public Function RegulonParser(genome As Regprecise.BacteriaRegulome, outDIR As String) As Boolean
            For Each len As Integer In PromoterRegionParser.PrefixLengths
                For Each regulon In genome.regulome.regulators
                    Dim fa = RegulonParser(regulon, len)
                    Dim path As String = $"{outDIR}/{len}/{regulon.LocusId.NormalizePathString(True)}.{regulon.locus_tag.text.NormalizePathString(True)}.fasta"
                    Call fa.Save(path)
                Next
            Next

            Return True
        End Function

        Public Function RegulonParser(inDIR As String, outDIR As String) As Boolean
            Dim genomes = inDIR.LoadSourceEntryList({"*.xml"}).Select(Function(x) x.Value.LoadXml(Of Regprecise.BacteriaRegulome))

            For Each genome As Regprecise.BacteriaRegulome In genomes
                If Not genome.regulome Is Nothing AndAlso Not genome.regulome.regulators.IsNullOrEmpty Then
                    Call RegulonParser(genome, outDIR)
                End If

                Call genome.genome.name.__DEBUG_ECHO
            Next

            Return True
        End Function
    End Class
End Namespace
