#Region "Microsoft.VisualBasic::3126d8237d66eeb00f22bbf93c182b1a, GCModeller\analysis\SequenceToolkit\SequencePatterns\Motif\MotifLog.vb"

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

    '   Total Lines: 69
    '    Code Lines: 45
    ' Comment Lines: 16
    '   Blank Lines: 8
    '     File Size: 2.06 KB


    ' Class MotifLog
    ' 
    '     Properties: ATGDist, BiologicalProcess, Family, InPromoterRegion, Location
    '                 Regulog, tag, tags, Taxonomy
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Web.Script.Serialization
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
#End If
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

''' <summary>
''' Simple site information of the TF motif site.
''' (简单的Motif位点)
''' </summary>
Public Class MotifLog : Inherits SimpleSegment
    Implements INumberTag

    Public Property Family As String
    Public Property BiologicalProcess As String
    Public Property Regulog As String
    Public Property Taxonomy As String
    Public Property ATGDist As Integer Implements INumberTag.Tag
    ''' <summary>
    ''' 基因组上下文之中的位置的描述
    ''' </summary>
    ''' <returns></returns>
    Public Property Location As String
    Public Property tag As String

    ''' <summary>
    ''' 当前的这个位点对象是否是在启动子区
    ''' </summary>
    ''' <returns></returns>
    <ScriptIgnore>
    <Ignored>
    Public ReadOnly Property InPromoterRegion As Boolean
        Get
            If Location Is Nothing Then
                Return False
            End If
            Return InStr(Location, "In the promoter region of", CompareMethod.Text) > 0 OrElse
                InStr(Location, "Overlap on up_stream with", CompareMethod.Text) > 0
        End Get
    End Property

    <Meta(GetType(String))>
    Public Property tags As Dictionary(Of String, String)

    Sub New()
    End Sub

    ''' <summary>
    ''' 复制
    ''' </summary>
    ''' <param name="loci"></param>
    Sub New(loci As MotifLog)
        Call MyBase.New(loci)

        Family = loci.Family
        BiologicalProcess = loci.BiologicalProcess
        Regulog = loci.Regulog
        Taxonomy = loci.Taxonomy
        ATGDist = loci.ATGDist
        Location = loci.Location
    End Sub

    Sub New(loci As SimpleSegment)
        Call MyBase.New(loci)
    End Sub
End Class
