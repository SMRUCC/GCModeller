#Region "Microsoft.VisualBasic::0913bc59a74837bef8cd594c787702c0, R#\gseakit\Utils\MapBackground.vb"

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

    '   Total Lines: 143
    '    Code Lines: 119 (83.22%)
    ' Comment Lines: 10 (6.99%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (9.79%)
    '     File Size: 6.21 KB


    ' Module MapBackground
    ' 
    '     Function: fromOrganismSpecificHtext, fromPtfAnnotation, fromUniprot, KOMaps
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

Module MapBackground

    ''' <summary>
    ''' try to map any terms to KO
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="geneId">the field name or regexp pattern for read gene id</param>
    ''' <param name="KO$"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' [geneId -> KO]
    ''' </returns>
    Public Function KOMaps(genes As Object, geneId$, KO$, name$, env As Environment) As Object
        If TypeOf genes Is list Then
            If KO.StringEmpty OrElse geneId.StringEmpty Then
                Return DirectCast(genes, list).slots _
                    .Select(Function(t)
                                Return New NamedValue(Of String) With {
                                    .Name = t.Key,
                                    .Value = Scripting.ToString([single](t.Value))
                                }
                            End Function) _
                    .ToArray
            Else
                Return DirectCast(genes, list).slots.Values _
                    .Select(Function(map)
                                Dim id As String = DirectCast(map, list).getValue(Of String)(geneId, env)
                                Dim koId As String = DirectCast(map, list).getValue(Of String)(KO, env)

                                Return New NamedValue(Of String)(id, koId)
                            End Function) _
                    .ToArray
            End If
        ElseIf TypeOf genes Is Rdataframe Then
            Dim idVec As String() = DirectCast(genes, Rdataframe).columns(geneId)
            Dim koVec As String() = DirectCast(genes, Rdataframe).columns(KO)

            Return idVec _
                .Select(Function(id, i)
                            Return New NamedValue(Of String) With {
                                .Name = id,
                                .Value = koVec(i)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf genes Is EntityObject() Then
            Return DirectCast(genes, EntityObject()) _
                .Select(Function(row)
                            Return New NamedValue(Of String) With {
                                .Name = row(geneId),
                                .Value = row(KO)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf genes Is PtfFile OrElse
               TypeOf genes Is ProteinAnnotation() OrElse
              (TypeOf genes Is pipeline AndAlso DirectCast(genes, pipeline).elementType Like GetType(ProteinAnnotation)) Then

            Return fromPtfAnnotation(genes, env)

        ElseIf TypeOf genes Is pipeline AndAlso DirectCast(genes, pipeline).elementType Like GetType(entry) Then
            Return DirectCast(genes, pipeline) _
                .populates(Of entry)(env) _
                .fromUniprot _
                .ToArray
        ElseIf TypeOf genes Is htext Then
            Return DirectCast(genes, htext).fromOrganismSpecificHtext(pattern:=geneId)
        Else
            Return Message.InCompatibleType(GetType(htext), genes.GetType, env)
        End If
    End Function

    <Extension>
    Private Function fromOrganismSpecificHtext(genes As htext, pattern$) As NamedValue(Of String)()
        Dim geneItems = genes.Deflate(pattern).ToArray
        Dim maps As NamedValue(Of String)() = geneItems _
            .Select(Function(i)
                        Dim KO As String = i.entry.Value.Split(ASCII.TAB)(1).Split.First

                        Return New NamedValue(Of String) With {
                            .Name = i.kegg_id,
                            .Value = KO,
                            .Description = i.entry.Value
                        }
                    End Function) _
            .ToArray

        Return maps
    End Function

    Private Function fromPtfAnnotation(genes As Object, env As Environment) As NamedValue(Of String)()
        Dim prot As ProteinAnnotation()

        If TypeOf genes Is PtfFile Then
            prot = DirectCast(genes, PtfFile).proteins
        ElseIf TypeOf genes Is pipeline Then
            prot = DirectCast(genes, pipeline).populates(Of ProteinAnnotation)(env).ToArray
        Else
            prot = DirectCast(genes, ProteinAnnotation())
        End If

        Return prot.Where(Function(p) p.has("ko")) _
            .Select(Function(protein)
                        Return protein.attributes("ko") _
                            .Select(Function(koid)
                                        Return New NamedValue(Of String) With {
                                            .Name = protein.geneId,
                                            .Value = koid,
                                            .Description = protein.description
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    <Extension>
    Private Iterator Function fromUniprot(entrylist As IEnumerable(Of entry)) As IEnumerable(Of NamedValue(Of String))
        For Each i As entry In From item As entry
                               In entrylist
                               Where Not item.KO Is Nothing

            Yield New NamedValue(Of String) With {
                .Name = i.accessions(Scan0),
                .Value = i.KO.id,
                .Description = i.name
            }
        Next
    End Function
End Module
