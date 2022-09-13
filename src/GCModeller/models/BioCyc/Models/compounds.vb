#Region "Microsoft.VisualBasic::de2e5bb65be6c903323e0d364341bab2, GCModeller\models\BioCyc\Models\compounds.vb"

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

    '   Total Lines: 29
    '    Code Lines: 26
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 1015 B


    ' Class compounds
    ' 
    '     Properties: atomCharges, chemicalFormula, componentOf, dbLinks, exactMass
    '                 Gibbs0, InChI, InChIKey, molecularWeight, nonStandardInChI
    '                 SMILES
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("compounds.dat")>
Public Class compounds : Inherits Model

    <AttributeField("ATOM-CHARGES")>
    Public Property atomCharges As String()
    <AttributeField("DBLINKS")>
    Public Property dbLinks As String()
    <AttributeField("NON-STANDARD-INCHI")>
    Public Property nonStandardInChI As String()
    <AttributeField("SMILES")>
    Public Property SMILES As String
    <AttributeField("CHEMICAL-FORMULA")>
    Public Property chemicalFormula As String()
    <AttributeField("GIBBS-0")>
    Public Property Gibbs0 As Double
    <AttributeField("INCHI")>
    Public Property InChI As String
    <AttributeField("INCHI-KEY")>
    Public Property InChIKey As String
    <AttributeField("MOLECULAR-WEIGHT")>
    Public Property molecularWeight As Double
    <AttributeField("MONOISOTOPIC-MW")>
    Public Property exactMass As Double
    <AttributeField("COMPONENT-OF")>
    Public Property componentOf As String()

End Class
