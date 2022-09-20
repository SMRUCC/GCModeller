#Region "Microsoft.VisualBasic::5e12d7c9baf42f585fff26ca1cd35552, R#\annotationKit\OBO_DAG.vb"

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

'   Total Lines: 21
'    Code Lines: 10
' Comment Lines: 8
'   Blank Lines: 3
'     File Size: 578 B


' Module OBO_DAG
' 
'     Function: readOboDAG
' 
' /********************************************************************************/

#End Region


Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' The Open Biological And Biomedical Ontology (OBO) Foundry
''' </summary>
<Package("OBO")>
Module OBO_DAG

    ''' <summary>
    ''' parse the obo file 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.obo")>
    Public Function readOboDAG(path As String) As GO_OBO
        Return GO_OBO.LoadDocument(path)
    End Function

    <ExportAPI("filter.is_obsolete")>
    Public Function filterObsolete(obo As GO_OBO) As GO_OBO
        obo = New GO_OBO With {
            .headers = obo.headers,
            .typedefs = obo.typedefs,
            .terms = obo.terms _
                .Where(Function(t) Not t.is_obsolete.TextEquals("true")) _
                .ToArray
        }

        Return obo
    End Function

    <ExportAPI("write.obo")>
    Public Function saveObo(obo As GO_OBO, path As String, Optional excludes As String() = Nothing) As Boolean
        Using file As Stream = path.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call obo.Save(file, excludes)
        End Using

        Return True
    End Function
End Module

