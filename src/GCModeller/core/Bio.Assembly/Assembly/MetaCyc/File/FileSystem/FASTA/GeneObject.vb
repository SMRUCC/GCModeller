#Region "Microsoft.VisualBasic::936c3b0b0f5574b856f8553448aaacaa, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\FileSystem\FASTA\GeneObject.vb"

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

    '   Total Lines: 89
    '    Code Lines: 70
    ' Comment Lines: 3
    '   Blank Lines: 16
    '     File Size: 3.29 KB


    '     Class GeneObject
    ' 
    '         Properties: AccessionId, Location, ProductUniqueId, Species, UniqueId
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Sub: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class GeneObject : Inherits FastaSeq
        Implements IReadOnlyId

        Public ReadOnly Property UniqueId As String Implements IReadOnlyId.Identity
            Get
                Return Headers.Last.Split.First
            End Get
        End Property

        Public ReadOnly Property AccessionId As String
            Get
                Return Headers.Last.Split()(1)
            End Get
        End Property

        Public ReadOnly Property ProductUniqueId As String
            Get
                Return Headers.Last.Split()(2).Replace("""", "")
            End Get
        End Property

        '>gnl|ECOLI|EG10570 map "EG10570-MONOMER" (complement(189506..188712)) Escherichia coli K-12 substr. MG1655
        '>gnl|ECOLI|EG11769 ybbC "EG11769-MONOMER" 526805..527173 Escherichia coli K-12 substr. MG1655
        Public Const METACYC_LOCATION_REGX As String = "(\(complement\(\d+\.\.\d+\)\))|(\d+\.\.\d+)"

        Public ReadOnly Property Location As NucleotideLocation
            Get
                ' Dim strLocation As String = Attributes.Last.Split()(3)
                Dim strLocation As String = Regex.Match(Headers.Last, METACYC_LOCATION_REGX).Value
                Dim Tokens As String() = Nothing
                Dim Strand As Strands

                If InStr(strLocation, "complement") Then
                    strLocation = Regex.Match(strLocation, "\d+\.\.\d+").Value
                    Tokens = Strings.Split(strLocation, "..")
                    Strand = Strands.Reverse
                Else
                    Tokens = Strings.Split(strLocation, "..")
                    Strand = Strands.Forward
                End If

                Dim objLocation = New NucleotideLocation
                objLocation.Left = Val(Tokens(0))
                objLocation.Right = Val(Tokens(1))
                objLocation.Strand = Strand

                Return objLocation
            End Get
        End Property

        Public ReadOnly Property Species As String
            Get
                Dim sBuilder As StringBuilder = New StringBuilder(128)
                Dim Tokens As String() = Headers.Last.Split

                For i As Integer = 4 To Tokens.Length - 1
                    Call sBuilder.Append(Tokens(i) & " ")
                Next

                Return Trim(sBuilder.ToString)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(fa As FastaSeq)
            Headers = fa.Headers
            SequenceData = fa.SequenceData
        End Sub

        Public Overloads Shared Sub Save(Data As GeneObject(), FilePath As String)
            Dim FsaFile As FastaFile = New FastaFile With {
                .FilePath = FilePath,
                ._innerList = New List(Of FastaSeq)
            }
            Call FsaFile._innerList.AddRange(Data)
            Call FsaFile.Save(FilePath, Encoding.UTF8)
        End Sub
    End Class
End Namespace
