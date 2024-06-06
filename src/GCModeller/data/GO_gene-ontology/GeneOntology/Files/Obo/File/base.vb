#Region "Microsoft.VisualBasic::00d373ade274544f9b877e7c35eb93a6, data\GO_gene-ontology\GeneOntology\Files\Obo\File\base.vb"

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

    '   Total Lines: 31
    '    Code Lines: 15 (48.39%)
    ' Comment Lines: 11 (35.48%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (16.13%)
    '     File Size: 1.51 KB


    '     Class base
    ' 
    '         Properties: [namespace], id, name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection

Namespace OBO

    Public MustInherit Class base
        Implements INamedValue

        ''' <summary>
        ''' Every term has a term name—e.g. mitochondrion, glucose transport, amino acid binding—and a unique zero-padded seven digit 
        ''' identifier (often called the term accession or term accession number) prefixed by GO:, e.g. GO:0005125 or GO:0060092. 
        ''' The numerical portion of the ID has no inherent meaning or relation to the position of the term in the ontologies. 
        ''' Ranges of GO IDs are assigned to individual ontology editors or editing groups, and can thus be used to trace who added the term.
        ''' </summary>
        ''' <returns></returns>
        <Field("id")> Public Property id As String Implements INamedValue.Key
        <Field("name")> Public Property name As String
        ''' <summary>
        ''' Denotes which of the three sub-ontologies—cellular component, biological process or molecular function—the term belongs to.
        ''' </summary>
        ''' <returns></returns>
        <Field("namespace")> Public Property [namespace] As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{id}: {name}"
        End Function

    End Class
End Namespace
