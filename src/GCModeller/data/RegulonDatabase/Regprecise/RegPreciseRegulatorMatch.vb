#Region "Microsoft.VisualBasic::4d6565eaab7c000eae0c762da1a306dc, data\RegulonDatabase\Regprecise\RegPreciseRegulatorMatch.vb"

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

    '     Class RegPreciseRegulatorMatch
    ' 
    '         Properties: biological_process, Description, effector, Family, Identities
    '                     mode, Query, Regulator, Regulog, RegulonSites
    '                     species
    ' 
    '     Class MotifSiteMatch
    ' 
    '         Properties: hits, ID, left, right, src
    '                     strand
    ' 
    '         Function: __getMappingLoci
    ' 
    '     Class RegulationFootprint
    ' 
    '         Properties: biological_process, distance, effector, family, identities
    '                     mode, motif, product, regprecise, regulated
    '                     regulator, regulog, replicon, sequenceData, site
    '                     species
    ' 
    '         Function: ToString
    ' 
    '     Class FootprintSite
    ' 
    '         Properties: distance, gene, location, product, replicon
    '                     sequenceData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Regprecise

    Public Class RegPreciseRegulatorMatch

        ''' <summary>
        ''' The genome query.(待注释的目标基因组的蛋白编号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Query As String

        ''' <summary>
        ''' The regprecise regulator name
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulator As String
        Public Property Family As String
        Public Property Description As String

        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <returns></returns>
        Public Property Identities As Double

        Public Property Regulog As String
        Public Property biological_process As String
        Public Property effector As String
        Public Property mode As String
        ''' <summary>
        ''' 注释来源的基因组名称
        ''' </summary>
        ''' <returns></returns>
        Public Property species As String

        ''' <summary>
        ''' 这个<see cref="Regulator"/>所调控的位点集合
        ''' </summary>
        ''' <returns></returns>
        Public Property RegulonSites As String()
    End Class

    Public Class MotifSiteMatch : Inherits Contig
        Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property left As Integer
        Public Property right As Integer
        Public Property strand As String

        <Collection("src", "|")> Public Property src As String()

        Public ReadOnly Property hits As Integer
            Get
                Return src.Length
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(left, right, strand)
        End Function
    End Class

    ''' <summary>
    ''' 调控关系，这个对象相当于网络之中的边对象
    ''' </summary>
    Public Class RegulationFootprint : Implements IPolymerSequenceModel

        ''' <summary>
        ''' 预测出来的基因组之中的调控因子的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property regulator As String
        Public Property family As String
        ''' <summary>
        ''' 预测出来的被调控的基因
        ''' </summary>
        ''' <returns></returns>
        Public Property regulated As String

        ''' <summary>
        ''' motif位点的基因组坐标位置
        ''' </summary>
        ''' <returns></returns>
        <Column("motif-context", GetType(NucleotideLocationParser))>
        Public Property motif As NucleotideLocation

        ''' <summary>
        ''' 这个位置上的序列片段数据
        ''' </summary>
        ''' <returns></returns>
        Public Property sequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' motif位点到被调控的基因之间的最短距离
        ''' </summary>
        ''' <returns></returns>
        Public Property distance As Integer
        ''' <summary>
        ''' 这个位点所属的复制子的编号，这个属性是为了将基因组的染色体DNA和质粒DNA区分开
        ''' </summary>
        ''' <returns></returns>
        Public Property replicon As String
        ''' <summary>
        ''' 受调控的基因的功能
        ''' </summary>
        ''' <returns></returns>
        Public Property product As String
        Public Property regulog As String
        Public Property biological_process As String
        Public Property effector As String
        Public Property mode As String

        ''' <summary>
        ''' 来自于RegPrecise数据库之中的调控因子注释来源
        ''' </summary>
        ''' <returns></returns>
        Public Property regprecise As String
        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <returns></returns>
        Public Property identities As Double

        ''' <summary>
        ''' 注释来源的基因组名称
        ''' </summary>
        ''' <returns></returns>
        Public Property species As String

        ''' <summary>
        ''' motif位点的预测来源
        ''' </summary>
        ''' <returns></returns>
        Public Property site As String

        Public Overrides Function ToString() As String
            Return $"[{family}]..{regulog}..{regulator} ({mode}) {regulated}"
        End Function

    End Class

    Public Class FootprintSite : Inherits MotifSiteMatch
        Implements IPolymerSequenceModel

        ''' <summary>
        ''' 当前的这个基因组位点相关的在一定长度范围内的下游基因
        ''' </summary>
        ''' <returns></returns>
        Public Property gene As String
        ''' <summary>
        ''' 这个下游基因的位置
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Column("location", GetType(NucleotideLocationParser))>
        Public Property location As NucleotideLocation
        ''' <summary>
        ''' 这个位点所属的复制子的编号，这个属性是为了将基因组的染色体DNA和质粒DNA区分开
        ''' </summary>
        ''' <returns></returns>
        Public Property replicon As String
        ''' <summary>
        ''' 这个基因的转录起始位点到目标位点之间的最小距离
        ''' </summary>
        ''' <returns></returns>
        Public Property distance As Integer
        ''' <summary>
        ''' 基因的功能描述
        ''' </summary>
        ''' <returns></returns>
        Public Property product As String
        ''' <summary>
        ''' 这个motif位点区域上的序列信息
        ''' </summary>
        ''' <returns></returns>
        Public Property sequenceData As String Implements IPolymerSequenceModel.SequenceData
    End Class
End Namespace
