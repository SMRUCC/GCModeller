#Region "Microsoft.VisualBasic::b896887548c61e2687f7fca7a801a91a, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\RepeatsLoci.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically

    ''' <summary>
    ''' The reversed repeats.(反向重复序列)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RevRepeats : Inherits Repeats

        Public Property RevSegment As String
        ''' <summary>
        ''' The left loci of the repeat sequence.(正向的序列片段的位置集合)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RepeatLoci As Integer()

        Public Shared Function GenerateFromBase(obj As Repeats) As RevRepeats
            Dim seq As String = New String(obj.SequenceData.ToArray.Reverse.ToArray)
            Return New RevRepeats With {
                .Locations = obj.Locations,
                .SequenceData = NucleicAcid.Complement(seq),
                .RevSegment = obj.SequenceData
            }
        End Function

        ''' <summary>
        ''' {Repeats, Rev-Repeats, Loci_left}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GenerateDocumentSegment() As Topologically.RepeatsLoci()
            Dim LQuery As RepeatsLoci() = LinqAPI.Exec(Of RepeatsLoci) <=
 _
                From revLoci As Integer
                In Me.Locations
                Select From loci As Integer
                       In Me.RepeatLoci
                       Let site = New Topologically.RevRepeatsLoci With {
                           .RepeatLoci = Me.SequenceData,
                           .LociLeft = loci,
                           .RevRepeats = Me.RevSegment,
                           .RevLociLeft = revLoci
                       }
                       Select DirectCast(site, Topologically.RepeatsLoci)

            Return LQuery
        End Function

        Public Overloads Shared Function CreateDocument(RevData As IEnumerable(Of RevRepeats)) As Topologically.RevRepeatsLoci()
            Dim LQuery As IEnumerable(Of RepeatsLoci) =
                LinqAPI.Exec(Of RepeatsLoci) <= From line As RevRepeats
                                                In RevData.AsParallel
                                                Select line.GenerateDocumentSegment
            Return (From loci As RepeatsLoci
                    In LQuery
                    Where Not loci Is Nothing
                    Select loci
                    Group loci By loci.__hash Into Group).ToArray(Function(loci) DirectCast(loci.Group.First, RevRepeatsLoci))
        End Function
    End Class

    ''' <summary>
    ''' The sequence segment of the nucleotide repeats.(重复的核酸片段)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Repeats : Implements IPolymerSequenceModel

        ''' <summary>
        ''' The Repeats sequence data.(重复序列)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        ''' <summary>
        ''' 重复序列的左端的位置的集合
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Locations As Integer()

        Public ReadOnly Property NumberOfRepeats As Integer
            Get
                If Locations.IsNullOrEmpty Then
                    Return 0
                Else
                    Return Locations.Count
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return SequenceData
        End Function

        ''' <summary>
        ''' 视图2：生成文档片段(Repeats, loci_left)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GenerateDocumentSegment() As Topologically.RepeatsLoci()
            Return Me.Locations.ToArray(
                Function(loci) New Topologically.RepeatsLoci With {
                    .LociLeft = loci,
                    .RepeatLoci = Me.SequenceData})
        End Function

        Public Shared Function CreateDocument(data As IEnumerable(Of Repeats)) As Topologically.RepeatsLoci()
            Dim LQuery = (From line As Repeats
                          In data.AsParallel
                          Select line.GenerateDocumentSegment).ToArray.Unlist
            Return (From loci As RepeatsLoci
                    In LQuery
                    Where Not loci Is Nothing
                    Select loci
                    Group loci By loci.__hash Into Group).ToArray(
                        Function(loci) loci.Group.First)
        End Function
    End Class
End Namespace
