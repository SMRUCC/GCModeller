﻿#Region "Microsoft.VisualBasic::ae28a79e97cb80b49d34fe71a17b3473, models\BioCyc\Models\compounds.vb"

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

    '   Total Lines: 69
    '    Code Lines: 56 (81.16%)
    ' Comment Lines: 4 (5.80%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (13.04%)
    '     File Size: 2.67 KB


    ' Class compounds
    ' 
    '     Properties: atomCharges, chemicalFormula, componentOf, dbLinks, exactMass
    '                 Gibbs0, InChI, InChIKey, molecularWeight, nonStandardInChI
    '                 regulates, SMILES
    ' 
    '     Function: FormulaString, GetDbLinks, (+2 Overloads) OpenFile, ParseText
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' the compound metabolite data model from the metacyc/biocyc database
''' </summary>
<Xref("compounds.dat")>
Public Class compounds : Inherits Model

    <AttributeField("ATOM-CHARGES")>
    Public Property atomCharges As String()

    ''' <summary>
    ''' cross reference to the external database of current metabolite model
    ''' </summary>
    ''' <returns></returns>
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
    <AttributeField("REGULATES")>
    Public Property regulates As String()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FormulaString(meta As compounds) As String
        Return meta.chemicalFormula _
            .Select(Function(d)
                        Return d.Trim(" "c, "("c, ")"c).Replace(" ", "")
                    End Function) _
            .JoinBy("")
    End Function

    Public Overloads Shared Function GetDbLinks(meta As compounds) As IEnumerable(Of DBLink)
        Return GetDbLinks(meta.dbLinks)
    End Function

    Public Shared Function OpenFile(fullName As String) As AttrDataCollection(Of compounds)
        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of compounds).LoadFile(file)
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As Stream) As AttrDataCollection(Of compounds)
        Return AttrDataCollection(Of compounds).LoadFile(file)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ParseText(data As String) As AttrDataCollection(Of compounds)
        Return AttrDataCollection(Of compounds).LoadFile(New StringReader(data))
    End Function
End Class
