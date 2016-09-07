#Region "Microsoft.VisualBasic::e14b52a9b63f629c846e11bb398c0ddc, ..\GCModeller\data\GO_gene-ontology\AnnotationFile\Obo\AnnotationFile.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace OBO

    ''' <summary>
    ''' go.obo/go-basic.obo(Go注释功能定义文件)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AnnotationFile

        Public Property header As header
        Public Property Terms As Term()

        Public Function Save(path As String) As Boolean
            Dim bufs As List(Of String) = New List(Of String)
            Dim schema = LoadClassSchema(Of Term)()
            Dim LQuery = From x As Term
                     In Terms
                         Select x.ToLines(schema)

            Call bufs.AddRange(header.ToLines)
            Call bufs.Add("")

            For Each x In LQuery
                Call bufs.Add(Term.Term)
                Call bufs.AddRange(x)
                Call bufs.Add("")
            Next

            Return bufs.SaveTo(path, Encodings.ASCII.GetEncodings)
        End Function

        Public Shared Function LoadDocument(path As String) As AnnotationFile

        End Function
    End Class
End Namespace