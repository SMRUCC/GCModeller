#Region "Microsoft.VisualBasic::8ce567e0cbb93a56cea3f83018b3677c, R#\seqtoolkit\Annotations\uniprot.vb"

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

    ' Module uniprot
    ' 
    '     Function: getProteinSeq, openUniprotXmlAssembly
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("uniprot", Category:=APICategories.UtilityTools)>
Module uniprot

    ''' <summary>
    ''' open a uniprot database file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="isUniParc"></param>
    ''' <returns></returns>
    <ExportAPI("open.uniprot")>
    Public Function openUniprotXmlAssembly(file As String, Optional isUniParc As Boolean = False) As pipeline
        Return UniProtXML _
            .EnumerateEntries(file, isUniParc) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' populate all protein fasta sequence from the given uniprot database reader
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <param name="extractAll"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("protein.seqs")>
    Public Function getProteinSeq(<RRawVectorArgument> uniprot As Object,
                                  Optional extractAll As Boolean = False,
                                  Optional env As Environment = Nothing) As pipeline

        If uniprot Is Nothing Then
            Return Nothing
        End If

        Dim protFa = Iterator Function(prot As entry) As IEnumerable(Of FastaSeq)
                         If extractAll Then
                             For Each accid As String In prot.accessions
                                 Yield New FastaSeq With {
                                    .Headers = {accid},
                                    .SequenceData = prot.ProteinSequence
                                 }
                             Next
                         Else
                             Yield New FastaSeq With {
                                .Headers = {prot.accessions(Scan0)},
                                .SequenceData = prot.ProteinSequence
                             }
                         End If
                     End Function

        If TypeOf uniprot Is entry() Then
            Return DirectCast(uniprot, entry()) _
                .Select(Function(prot)
                            Return protFa(prot)
                        End Function) _
                .IteratesALL _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf TypeOf uniprot Is pipeline AndAlso DirectCast(uniprot, pipeline).elementType Like GetType(entry) Then
            Return DirectCast(uniprot, pipeline) _
                .populates(Of entry) _
                .Select(Function(prot) protFa(prot)) _
                .IteratesALL _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Internal.debug.stop($"invalid data source input: {uniprot.GetType.FullName}!", env)
        End If
    End Function
End Module

