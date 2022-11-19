#Region "Microsoft.VisualBasic::e8294b0975a09d078b97ee233bc9478b, GCModeller\models\Networks\Microbiome\PathwayProfile\Profile.vb"

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

    '   Total Lines: 54
    '    Code Lines: 28
    ' Comment Lines: 18
    '   Blank Lines: 8
    '     File Size: 1.68 KB


    '     Class Profile
    ' 
    '         Properties: pct, Profile, RankGroup, Taxonomy
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace PathwayProfile

    ''' <summary>
    ''' A profile matrix model
    ''' </summary>
    Public Class Profile

        ''' <summary>
        ''' 物种分类信息
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Ignored>
        Public ReadOnly Property Taxonomy As Taxonomy
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return BIOMTaxonomyParser.Parse(biomString:=RankGroup)
            End Get
        End Property

        ''' <summary>
        ''' The profile matrix data row
        ''' 
        ''' (该分类下的所有的具有覆盖度结果的KEGG编号的列表和相对应的覆盖度值)
        ''' </summary>
        ''' <returns></returns>
        Public Property Profile As Dictionary(Of String, Double)
        ''' <summary>
        ''' 这个物种的相对百分比含量
        ''' </summary>
        ''' <returns></returns>
        Public Property pct As Double
        Public Property RankGroup As String

        Sub New()
        End Sub

        Sub New(tax As Taxonomy, profile As Dictionary(Of String, Double), pct#)
            Me.RankGroup = tax.ToString(BIOMstyle:=True)
            Me.Profile = profile
            Me.pct = pct
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{(pct * 100).ToString("F2")}%] {Profile.Where(Function(p) p.Value > 0).ToDictionary.GetJson}"
        End Function
    End Class

End Namespace
