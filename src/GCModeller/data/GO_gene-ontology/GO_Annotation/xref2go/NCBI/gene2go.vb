#Region "Microsoft.VisualBasic::5aa1bea998b53d9133a948ebca266305, data\GO_gene-ontology\GO_Annotation\xref2go\NCBI\gene2go.vb"

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

    '   Total Lines: 57
    '    Code Lines: 41
    ' Comment Lines: 9
    '   Blank Lines: 7
    '     File Size: 2.01 KB


    '     Class gene2go
    ' 
    '         Properties: Category, Evidence, GeneID, GO_ID, GO_term
    '                     PubMed, Qualifier, tax_id
    ' 
    '         Function: createMaps, LoadDoc, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI

    ''' <summary>
    ''' #Format: tax_id GeneID GO_ID Evidence Qualifier GO_term PubMed Category (tab is used as a separator, pound sign - start of a comment)
    ''' </summary>
    <Serializable> Public Class gene2go

        ' #Format: (TAB Is used As a separator, pound sign - start Of a comment)
        Public Property tax_id As String
        Public Property GeneID As String
        Public Property GO_ID As String
        Public Property Evidence As String
        Public Property Qualifier As String
        Public Property GO_term As String
        Public Property PubMed As String
        Public Property Category As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' Load ncbi gene2go.txt document
        ''' </summary>
        ''' <param name="path">The doc path</param>
        ''' <returns></returns>
        Public Shared Function LoadDoc(path As String) As gene2go()
            Dim lines As String() = path.ReadAllLines
            Dim LQuery As gene2go() = LinqAPI.Exec(Of gene2go) <=
 _
                From line As String
                In lines.AsParallel
                Let tokens As String() = Strings.Split(line, vbTab)
                Select createMaps(tokens.MarshalAs)

            Return LQuery
        End Function

        Private Shared Function createMaps(i As Pointer(Of String)) As gene2go
            Dim g2g As New gene2go With {
                .tax_id = ++i,
                .GeneID = ++i,
                .GO_ID = ++i,
                .Evidence = ++i,
                .Qualifier = ++i,
                .GO_term = ++i,
                .PubMed = ++i,
                .Category = ++i
            }
            Return g2g
        End Function
    End Class
End Namespace
