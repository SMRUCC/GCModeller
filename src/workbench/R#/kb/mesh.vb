#Region "Microsoft.VisualBasic::268fdd3eefc38dc70137f4e015b2c1df, R#\kb\mesh.vb"

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

    '   Total Lines: 49
    '    Code Lines: 40
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.68 KB


    ' Module meshTools
    ' 
    '     Function: loadMeshTree, loadMeshXml, mesh_category
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mesh")>
<RTypeExport("ncbi_mesh", GetType(MeSH.Tree.Term))>
Module meshTools

    Sub Main()

    End Sub

    <ExportAPI("read.mesh_xml")>
    Public Function loadMeshXml(file As String) As DescriptorRecord()
        Return DescriptorRecordSet.ReadTerms(file).ToArray
    End Function

    <ExportAPI("mesh_category")>
    Public Function mesh_category(term As MeSH.Tree.Term) As MeshCategory()
        Return term.category.ToArray
    End Function

    <ExportAPI("read.mesh_tree")>
    <RApiReturn(GetType(MeSH.Tree.Term))>
    Public Function loadMeshTree(<RRawVectorArgument> file As Object,
                                 Optional as_tree As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Using buf As Stream = s.TryCast(Of Stream)
            If as_tree Then
                Return MeSH.Tree.ParseTree(buf)
            Else
                Return MeSH.Tree.ReadTerms(buf).ToArray
            End If
        End Using
    End Function
End Module
