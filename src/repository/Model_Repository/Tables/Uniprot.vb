#Region "Microsoft.VisualBasic::86da48461b4ebd9ffe8130f327b5d3c2, Model_Repository\Tables\Uniprot.vb"

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

' Class Uniprot
' 
'     Properties: COG
' 
'     Function: CreateObject, GetPath
' 
' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
' Imports System.Data.SQLite.Linq.QueryBuilder
Imports Oracle.LinuxCompatibility.MySQL.Scripting
Imports SMRUCC.genomics.Assembly.Uniprot

Public Class Uniprot : Inherits DbFileSystemObject
    Implements DbFileSystemObject.ProteinDescriptionModel

    <Column(Name:="uniprot_id", IsPrimaryKey:=True, DbType:="varchar(64)")> Public Property UniprotID As String
    <Column(Name:="Organism_Species", DbType:="varchar(2048)")> Public Property OrganismSpecies As String Implements DbFileSystemObject.ProteinDescriptionModel.OrganismSpecies
    ''' <summary>
    ''' GeneName(基因名称)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column(Name:="gene_name", DbType:="varchar(1024)")> Public Property GeneName As String
    <Column(Name:="pe", DbType:="varchar(64)")> Public Property PE As String
    <Column(Name:="sv", DbType:="varchar(64)")> Public Property SV As String

    Public Overrides Function GetPath(RepositoryRoot As String) As String
        Dim FilePath As String = RepositoryRoot & "/" & OrganismSpecies.ToUpper.First & "/" & OrganismSpecies.NormalizePathString() & ".fasta"
        Return FilePath
    End Function

    Public Shared Function CreateObject(fasta As UniprotFasta, MD5 As String) As Uniprot
        Dim header = fasta.UniProtHeader
        Dim entry As New Uniprot With {
            .Definition = header.ProtName.MySqlEscaping,
            .GeneName = header.GN,
            .LocusID = header.EntryName,
            .MD5Hash = MD5,
            .OrganismSpecies = header.OrgnsmSpName.MySqlEscaping,
            .PE = header.PE,
            .SV = header.SV,
            .UniprotID = fasta.UniprotID
        }

        Return entry
    End Function

    Public Property COG As String Implements DbFileSystemObject.ProteinDescriptionModel.COG
End Class
