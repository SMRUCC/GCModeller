#Region "Microsoft.VisualBasic::451574193990b17f616e12cb10ee3569, ..\GCModeller\data\ExternalDBSource\SABIORK KineticLaws\Dumps\Enzyme.vb"

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

Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace SabiorkKineticLaws.TabularDump

    Public Class EnzymeModifier : Inherits SabiorkEntity
        Implements I_PolymerSequenceModel, INamedValue

        Public Property Uniprot As String
        Public Property CommonName As String
        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Uniprot, CommonName)
        End Function

        Public Shared Function CreateObjects(SABIORK_DATA As SABIORK) As EnzymeModifier()
            Dim LQuery = (From cs As SBMLParser.CompoundSpecie
                          In SABIORK_DATA.CompoundSpecies
                          Let uniprot = GetIdentifier(cs.Identifiers, "uniprot")
                          Where Not String.IsNullOrEmpty(uniprot)
                          Select New EnzymeModifier With {
                              .CommonName = cs.Name,
                              .Uniprot = uniprot,
                              .SabiorkId = cs.Id}).ToArray
            Return LQuery
        End Function

        Public Function ConvertToFastaObject() As FastaToken
            Return New FastaToken With {
                .SequenceData = SequenceData,
                .Attributes = New String() {Me.Uniprot, Me.CommonName}
            }
        End Function
    End Class
End Namespace
