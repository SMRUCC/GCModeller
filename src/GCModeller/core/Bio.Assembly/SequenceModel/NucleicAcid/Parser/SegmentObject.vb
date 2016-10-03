#Region "Microsoft.VisualBasic::61ea1d549a200366d312dc384385b77f, ..\GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Parser\SegmentObject.vb"

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

Imports System.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.ISequenceModel

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 片段数据，包含有在目标核酸链之上的位置信息以及用户给这个片段的自定义的标签信息
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SegmentObject : Inherits Location
        Implements I_PolymerSequenceModel

        ''' <summary>
        ''' This sequence segment object site is on the complement strand?
        ''' </summary>
        ''' <returns></returns>
        Public Property Complement As Boolean
        ''' <summary>
        ''' The sequence data of this site.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
        ''' <summary>
        ''' User tag data
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String = ""
        ''' <summary>
        ''' User data
        ''' </summary>
        ''' <returns></returns>
        Public Property Description As String = ""

        Public Overrides Function ToString() As String
            Dim l As String = String.Format("{0}..{1}", Left, Right)
            If Complement Then
                l = String.Format("complement({0})", l)
            End If
            Dim str As String = ""
            If Not String.IsNullOrEmpty(Title) Then
                str = Title & "|"
            End If
            If Not String.IsNullOrEmpty(Description) Then
                str &= Description & "|"
            End If
            If String.IsNullOrEmpty(str) Then
                str = "NULL_LOCI_INFO|"
            End If

            Return String.Format("> {0}{1}", str, l)
        End Function

        ''' <summary>
        ''' 将当前的核酸片段数据对象转换为FASTA对象
        ''' </summary>
        ''' <param name="title">If this value is not presents, then the function will using the location info as default.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFasta(Optional title As String = "") As FASTA.FastaToken
            Dim attrs As String()

            If String.IsNullOrEmpty(title) Then
                Dim l As String = String.Format("{0}..{1}", Left, Right)
                If Complement Then
                    l = String.Format("complement({0})", l)
                End If
                attrs = New String() {title, Description, l}
            Else
                attrs = title.Split("|"c)
            End If

            Return New FASTA.FastaToken With {
                .Attributes = attrs,
                .SequenceData = SequenceData
            }
        End Function

        Sub New()
        End Sub

        Sub New(SequenceData As String, Left As Integer)
            Me.SequenceData = SequenceData
            Me.Left = Left
            Me.Right = Left + FragmentSize
            Me.Complement = False
        End Sub

        Sub New(SequenceData As I_PolymerSequenceModel, Left As Integer)
            Me.SequenceData = SequenceData.SequenceData
            Me.Left = Left
            Me.Right = Left + FragmentSize
            Me.Complement = False
        End Sub

        Sub New(SequenceData As String, loci As NucleotideLocation)
            Me.SequenceData = SequenceData
            Me.Left = loci.Left
            Me.Right = loci.Right
            Me.Complement = loci.Strand = Strands.Reverse
        End Sub

        Public Function ToLoci() As NucleotideLocation
            Return New NucleotideLocation(Left, Right, Complement)
        End Function

#Region "GC Property for current segment sequence..."

        ''' <summary>
        ''' 获取当前核酸片段的GC含量值，假设已经全部转换为大写字母
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GC_Content As Double
            Get
                If String.IsNullOrEmpty(SequenceData) Then
                    Return -1
                End If

                Dim LQuery = (From ch In SequenceData Where ch = "G"c OrElse ch = "C"c Select 1).Sum
                Return LQuery / Len(SequenceData)
            End Get
        End Property

        ''' <summary>
        ''' 获取目标核酸片段的GC含量值，假设已经全部转换为大写字母
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Get_GCContent(SequenceData As String) As Double
            If String.IsNullOrEmpty(SequenceData) Then
                Return -1
            End If

            Dim LQuery = (From ch In SequenceData Where ch = "G"c OrElse ch = "C"c Select 1).Sum
            Return LQuery / Len(SequenceData)
        End Function

        Public Shared Function Get_GCContent(SequenceData As I_PolymerSequenceModel) As Double
            Return Get_GCContent(SequenceData.SequenceData)
        End Function
#End Region

    End Class
End Namespace
