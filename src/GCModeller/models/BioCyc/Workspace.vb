#Region "Microsoft.VisualBasic::9ab88914a82d06d99823ae0de19351c7, GCModeller\models\BioCyc\Workspace.vb"

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

    '   Total Lines: 80
    '    Code Lines: 60
    ' Comment Lines: 3
    '   Blank Lines: 17
    '     File Size: 2.88 KB


    ' Class Workspace
    ' 
    '     Properties: compounds, enzrxns, pathways, reactions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getFileName, openFile, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Public Class Workspace

    ReadOnly dir As String

    Dim m_reactions As Lazy(Of AttrDataCollection(Of reactions))
    Dim m_pathways As Lazy(Of AttrDataCollection(Of pathways))
    Dim m_enzrxns As Lazy(Of AttrDataCollection(Of enzrxns))
    Dim m_compounds As Lazy(Of AttrDataCollection(Of compounds))

    Public ReadOnly Property compounds As AttrDataCollection(Of compounds)
        Get
            Return m_compounds.Value
        End Get
    End Property

    Public ReadOnly Property reactions As AttrDataCollection(Of reactions)
        Get
            Return m_reactions.Value
        End Get
    End Property

    Public ReadOnly Property pathways As AttrDataCollection(Of pathways)
        Get
            Return m_pathways.Value
        End Get
    End Property

    Public ReadOnly Property enzrxns As AttrDataCollection(Of enzrxns)
        Get
            Return m_enzrxns.Value
        End Get
    End Property

    Sub New(dir As String)
        Me.dir = dir.GetDirectoryFullPath

        ' 20220401 when commit the data base into git reposiotry
        ' some empty folder may be missing from the repository
        ' just check these three main folder
        If {"data", "input", "kb"}.All(Function(d) $"{dir}/{d}".DirectoryExists) Then
            Me.dir = $"{Me.dir}/data/"
        End If

        m_enzrxns = New Lazy(Of AttrDataCollection(Of enzrxns))(Function() openFile(Of enzrxns)())
        m_reactions = New Lazy(Of AttrDataCollection(Of reactions))(Function() openFile(Of reactions)())
        m_pathways = New Lazy(Of AttrDataCollection(Of pathways))(Function() openFile(Of pathways)())
        m_compounds = New Lazy(Of AttrDataCollection(Of compounds))(Function() openFile(Of compounds)())
    End Sub

    Private Function openFile(Of T As Model)() As AttrDataCollection(Of T)
        Dim fileName As String = getFileName(Of T)()
        Dim fullName As String = $"{dir}/{fileName}".GetFullPath

        Call Console.WriteLine($"[biocyc_open] {fullName}")

        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of T).LoadFile(file)
        End Using
    End Function

    Private Shared Function getFileName(Of T As Model)() As String
        Dim attrs As Object() = GetType(T).GetCustomAttributes(inherit:=True).ToArray
        Dim ref = From attr As Object In attrs Where TypeOf attr Is XrefAttribute
        Dim fileName As XrefAttribute = ref.FirstOrDefault

        If fileName Is Nothing Then
            Throw New MissingFieldException
        Else
            Return fileName.Name
        End If
    End Function

    Public Overrides Function ToString() As String
        Return dir
    End Function

End Class
