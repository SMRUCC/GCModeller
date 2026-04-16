#Region "Microsoft.VisualBasic::d2b130b91e6661648ae030849f97ac27, core\Bio.Assembly\Assembly\ELIXIR\UniProt\Fasta\TitleBuilder.vb"

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
    '    Code Lines: 48 (77.42%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (22.58%)
    '     File Size: 2.34 KB


    '     Class TitleBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Fasta, GetData
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.Uniprot

    Public Class TitleBuilder : Implements IKeyDataReader

        ReadOnly template As StringTemplate
        ReadOnly extractAll As Boolean

        Dim data As entry

        Sub New(template As StringTemplate, Optional extractAll As Boolean = False)
            Me.template = template
            Me.extractAll = extractAll
        End Sub

        Public Iterator Function Fasta(prot As entry) As IEnumerable(Of FastaSeq)
            Dim prot_seq As String = prot.ProteinSequence

            data = prot

            If extractAll Then
                Call template.SetDefaultKey("uniprot_id", prot.accessions.First)

                Yield New FastaSeq(prot_seq, title:=template.CreateString(Me))
            Else
                For Each id As String In prot.accessions
                    Call template.SetDefaultKey("uniprot_id", id)

                    Yield New FastaSeq With {
                        .SequenceData = prot_seq,
                        .Headers = {template.CreateString(Me)}
                    }
                Next
            End If
        End Function

        Public Function GetData(key As String) As String Implements IKeyDataReader.GetData
            Select Case key
                Case "fullname" : Return data.proteinFullName
                Case "name" : Return data.name
                Case "ncbi_taxid" : Return data.NCBITaxonomyId
                Case "organism" : Return data.OrganismScientificName
                Case "ec_number" : Return data.ECNumberList.JoinBy(",")
                Case "go_id" : Return data.GO.Select(Function(a) a.id).JoinBy(",")
                Case "gene_name" : Return data.geneName
                Case "ORF" : Return data.ORF
                Case "subcellular_location" : Return data.SubCellularLocations.JoinBy(",")

                Case Else
                    If data.xrefs.ContainsKey(key) Then
                        Return data.xrefs(key).Select(Function(a) a.id).JoinBy(",")
                    End If
            End Select

            Return Nothing
        End Function
    End Class
End Namespace
