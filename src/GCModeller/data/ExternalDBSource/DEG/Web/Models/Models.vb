#Region "Microsoft.VisualBasic::59db93d6a61c2f121e62f7e3e071f97f, data\ExternalDBSource\DEG\Web\Models\Models.vb"

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

    '     Class EssentialGene
    ' 
    '         Properties: Aa, COG, Condition, FuncClass, FunctionDescrib
    '                     geneRef, GO, ID, Name, Nt
    '                     Organism, Reference, RefSeq, UniProt
    ' 
    '     Class Genome
    ' 
    '         Properties: Conditions, EssentialGenes, ID, numOfDEG, Organism
    '                     Reference, summary
    ' 
    '         Function: GenericEnumerator, GetEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq

Namespace DEG.Web

    <XmlType("essentialGene")> Public Class EssentialGene

        <XmlAttribute> Public Property ID As String
        <XmlAttribute> Public Property Name As String

        Public Property FunctionDescrib As String
        Public Property Organism As String
        Public Property geneRef As String
        Public Property RefSeq As String
        Public Property UniProt As String
        Public Property COG As String
        Public Property GO As String()
        Public Property FuncClass As String
        Public Property Reference As String
        Public Property Condition As String
        Public Property Nt As String
        Public Property Aa As String

    End Class

    <XmlRoot("genome")> Public Class Genome : Inherits XmlDataModel
        Implements Enumeration(Of EssentialGene)

        <XmlAttribute> Public Property ID As String
        Public Property Organism As String
        <XmlAttribute> Public Property numOfDEG As Integer
        <XmlAttribute> Public Property Conditions As String
        Public Property Reference As String

        Public Property summary As Summary

        <XmlElement>
        Public Property EssentialGenes As EssentialGene()

        Public Overrides Function ToString() As String
            Return Organism
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of EssentialGene) Implements Enumeration(Of EssentialGene).GenericEnumerator
            For Each gene As EssentialGene In EssentialGenes
                Yield gene
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of EssentialGene).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

End Namespace
