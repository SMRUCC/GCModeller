#Region "Microsoft.VisualBasic::d6afc15717d281da9ab89a18396734c6, GCModeller\data\SABIO-RK\Dumps\Enzyme.vb"

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

    '   Total Lines: 38
    '    Code Lines: 23
    ' Comment Lines: 9
    '   Blank Lines: 6
    '     File Size: 1.60 KB


    '     Class EnzymeModifier
    ' 
    '         Properties: CommonName, SequenceData, Uniprot
    ' 
    '         Function: ConvertToFastaObject, CreateObjects, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace TabularDump

    Public Class EnzymeModifier : Inherits SabiorkEntity
        Implements IPolymerSequenceModel, INamedValue

        Public Property Uniprot As String
        Public Property CommonName As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Uniprot, CommonName)
        End Function

        Public Shared Function CreateObjects(SABIORK_DATA As SabiorkSBML) As EnzymeModifier()
            'Dim LQuery = (From cs As SBMLParser.CompoundSpecie
            '              In SABIORK_DATA.CompoundSpecies
            '              Let uniprot = GetIdentifier(cs.Identifiers, "uniprot")
            '              Where Not String.IsNullOrEmpty(uniprot)
            '              Select New EnzymeModifier With {
            '                  .CommonName = cs.Name,
            '                  .Uniprot = uniprot,
            '                  .SabiorkId = cs.Id}).ToArray
            'Return LQuery
        End Function

        Public Function ConvertToFastaObject() As FastaSeq
            Return New FastaSeq With {
                .SequenceData = SequenceData,
                .Headers = New String() {Me.Uniprot, Me.CommonName}
            }
        End Function
    End Class
End Namespace
