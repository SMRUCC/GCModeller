#Region "Microsoft.VisualBasic::0d18013e5ee7919ec5900910c44b3c71, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\GeneName.vb"

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

    '   Total Lines: 47
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.70 KB


    '     Class GeneName
    ' 
    '         Properties: description, EC, geneId, geneName, KO
    ' 
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlType("geneName")>
    Public Class GeneName : Implements INamedValue

        <XmlAttribute> Public Property geneId As String Implements INamedValue.Key
        <XmlAttribute> Public Property geneName As String
        <XmlText> Public Property description As String
        <XmlAttribute> Public Property KO As String
        <XmlAttribute> Public Property EC As String()

        Friend Shared Function Parse(text As String) As GeneName
            Dim data = text.GetTagValue(" ", trim:=True, failureNoName:=False)
            Dim geneId As String = data.Name
            Dim gene As New GeneName With {.geneId = geneId}
            Dim KO As String
            Dim EC As String

            data = data.Value.GetTagValue(";", trim:=True, failureNoName:=False)
            gene.geneName = data.Name
            text = data.Value
            KO = text.Match("\[KO[:](K\d+\s*)+\]")
            EC = text.Match("\[EC[:]([\d.\s]+)\]")

            If Not KO.StringEmpty Then text = text.Replace(KO, "")
            If Not EC.StringEmpty Then text = text.Replace(EC, "")

            text = text.Trim
            gene.description = text
            gene.KO = KO.Match("K\d+")
            gene.EC = EC.GetStackValue("[", "]") _
                .Split(":"c) _
                .LastOrDefault _
                .StringSplit("\s+")

            Return gene
        End Function

        Public Overrides Function ToString() As String
            Return description
        End Function

    End Class
End Namespace
