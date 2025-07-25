﻿#Region "Microsoft.VisualBasic::cd01e50ed55045636e51d12966debadb, models\BioCyc\Workspace.vb"

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

    '   Total Lines: 142
    '    Code Lines: 106 (74.65%)
    ' Comment Lines: 11 (7.75%)
    '    - Xml Docs: 63.64%
    ' 
    '   Blank Lines: 25 (17.61%)
    '     File Size: 5.25 KB


    ' Class Workspace
    ' 
    '     Properties: compounds, enzrxns, genes, IWorkspace_Workspace, pathways
    '                 proteins, reactions, species, transunits
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateSequenceIndex, getFileName, Open, openFile, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.File.FileSystem.FastaObjects
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' A work directory object for read the biocyc database
''' </summary>
Public Class Workspace : Implements IWorkspace

    Friend ReadOnly dir As String

    Dim m_reactions As Lazy(Of AttrDataCollection(Of reactions))
    Dim m_pathways As Lazy(Of AttrDataCollection(Of pathways))
    Dim m_enzrxns As Lazy(Of AttrDataCollection(Of enzrxns))
    Dim m_compounds As Lazy(Of AttrDataCollection(Of compounds))
    Dim m_genes As Lazy(Of AttrDataCollection(Of genes))
    Dim m_proteins As Lazy(Of AttrDataCollection(Of proteins))
    Dim m_transunits As Lazy(Of AttrDataCollection(Of transunits))
    Dim m_protligandcplxes As Lazy(Of AttrDataCollection(Of protligandcplxes))
    Dim m_rnas As Lazy(Of AttrDataCollection(Of rnas))
    Dim m_regulation As Lazy(Of AttrDataCollection(Of regulation))

    Dim m_species As species

    Dim m_fasta As Lazy(Of FastaCollection)

    Public ReadOnly Property regulation As AttrDataCollection(Of regulation)
        Get
            Return m_regulation.Value
        End Get
    End Property

    Public ReadOnly Property rnas As AttrDataCollection(Of rnas)
        Get
            Return m_rnas.Value
        End Get
    End Property

    Public ReadOnly Property protligandcplxes As AttrDataCollection(Of protligandcplxes)
        Get
            Return m_protligandcplxes.Value
        End Get
    End Property

    Public ReadOnly Property transunits As AttrDataCollection(Of transunits)
        Get
            Return m_transunits.Value
        End Get
    End Property

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

    Public ReadOnly Property genes As AttrDataCollection(Of genes)
        Get
            Return m_genes.Value
        End Get
    End Property

    Public ReadOnly Property proteins As AttrDataCollection(Of proteins)
        Get
            Return m_proteins.Value
        End Get
    End Property

    ''' <summary>
    ''' the organism taxonomy species information
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property species As species
        Get
            Return m_species
        End Get
    End Property

    Public ReadOnly Property fastaSeq As FastaCollection
        Get
            Return m_fasta.Value
        End Get
    End Property

    Private ReadOnly Property IWorkspace_Workspace As String Implements IWorkspace.Workspace
        Get
            Return dir
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
        m_genes = New Lazy(Of AttrDataCollection(Of genes))(Function() openFile(Of genes)())
        m_proteins = New Lazy(Of AttrDataCollection(Of proteins))(Function() openFile(Of proteins)())
        m_transunits = New Lazy(Of AttrDataCollection(Of transunits))(Function() openFile(Of transunits)())
        m_protligandcplxes = New Lazy(Of AttrDataCollection(Of protligandcplxes))(Function() openFile(Of protligandcplxes)())
        m_rnas = New Lazy(Of AttrDataCollection(Of rnas))(Function() openFile(Of rnas)())
        m_regulation = New Lazy(Of AttrDataCollection(Of regulation))(Function() openFile(Of regulation)())

        ' scalar data object
        m_species = openFile(Of species).features.FirstOrDefault

        m_fasta = New Lazy(Of FastaCollection)(Function() New FastaCollection(Me))

        If m_species Is Nothing Then
            Call "missing the organism taxonomy species information file in current model!".Warning
        End If
    End Sub

    Public Function checkValid() As Boolean
        Return {"species.dat", "compounds.dat", "reactions.dat", "proteins.dat"}.All(Function(name) $"{dir}/{name}".FileExists(True))
    End Function

    Private Function openFile(Of T As Model)() As AttrDataCollection(Of T)
        Dim fileName As String = getFileName(Of T)()
        Dim fullName As String = $"{dir}/{fileName}".GetFullPath

        Call VBDebugger.EchoLine($"[biocyc_open] {fullName}")

        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of T).LoadFile(file)
        End Using
    End Function

    Private Shared Function getFileName(Of T As Model)() As String
        Dim attrs As Object() = GetType(T).GetCustomAttributes(inherit:=True).ToArray
        Dim ref = From attr As Object In attrs Where TypeOf attr Is XrefAttribute
        Dim fileName As XrefAttribute = ref.FirstOrDefault

        If fileName Is Nothing Then
            Throw New MissingFieldException("no file name attribute tag for the given biocyc element model!")
        Else
            Return fileName.Name
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return dir
    End Function

    Public Shared Function CreateSequenceIndex(seq As FastaFile) As Dictionary(Of String, FastaSeq)
        Return seq.ToDictionary(Function(a) a.Headers(2).Split(" "c).First)
    End Function

    Public Shared Function Open(dir As String) As Workspace
        Return New Workspace(dir)
    End Function

End Class
