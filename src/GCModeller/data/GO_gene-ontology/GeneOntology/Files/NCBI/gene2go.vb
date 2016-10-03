#Region "Microsoft.VisualBasic::72a1081f636e1235e8d01ab387fd32c3, ..\GCModeller\data\GO_gene-ontology\GeneOntology\Files\NCBI\gene2go.vb"

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

Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
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
                Select __create(tokens.MarshalAs)

            Return LQuery
        End Function

        Private Shared Function __create(tokens As Pointer(Of String)) As gene2go
            Dim gg As New gene2go With {
                .tax_id = (+tokens),
                .GeneID = (+tokens),
                .GO_ID = (+tokens),
                .Evidence = (+tokens),
                .Qualifier = (+tokens),
                .GO_term = (+tokens),
                .PubMed = (+tokens),
                .Category = (+tokens)
            }
            Return gg
        End Function
    End Class
End Namespace
