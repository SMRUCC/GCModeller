#Region "Microsoft.VisualBasic::ca8b4df2feaa1752bc8202a225e4086d, GCModeller\data\RegulonDatabase\Regprecise\AnalysisResult\RegPreciseOperon.vb"

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

'   Total Lines: 112
'    Code Lines: 61
' Comment Lines: 40
'   Blank Lines: 11
'     File Size: 3.84 KB


'     Class RegPreciseOperon
' 
'         Properties: bbh, bbhUID, BiologicalProcess, Effector, Operon
'                     Pathway, Regulators, source, Strand, TF_trace
' 
'         Constructor: (+3 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Regprecise

    ''' <summary>
    ''' Operon regulon model that reconstructed from the RegPrecise database.
    ''' (使用RegPrecise数据库重构出来的Regulon数据)
    ''' </summary>
    Public Class RegPreciseOperon

        ''' <summary>
        ''' Mapping from <see cref="TF_trace"/> by using protein ortholog analysis, such as ``bbh`` method.
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulators As String()
        ''' <summary>
        ''' Active the regulation from <see cref="TF_trace"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Effector As String
        Public Property Pathway As String
        ''' <summary>
        ''' Operon regulator TF
        ''' </summary>
        ''' <returns></returns>
        Public Property TF_trace As String
        Public Property BiologicalProcess As String
        Public Property source As String
        ''' <summary>
        ''' Operon members
        ''' </summary>
        ''' <returns></returns>
        Public Property Operon As String()
        Public Property Strand As String
        Public Property bbh As String()
            Get
                Return __bbh
            End Get
            Set(value As String())
                __bbh = value

                If __bbh Is Nothing Then
                    _bbhUID = ""
                Else
                    _bbhUID = String.Join(", ", value.OrderBy(Function(x) x).ToArray)
                End If
            End Set
        End Property

        Dim __bbh As String()

        ''' <summary>
        ''' Using for the CORN analysis or distinct
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        <Ignored>
        <XmlIgnore>
        Public ReadOnly Property bbhUID As String

        Sub New()
        End Sub

        ''' <summary>
        ''' Copy regulon definition from <see cref="Regulator"/>
        ''' </summary>
        ''' <param name="regulon"></param>
        ''' <param name="TF"></param>
        ''' <param name="members"></param>
        ''' <param name="cstrand"></param>
        ''' <param name="bbhHits"></param>
        Sub New(regulon As Regulator, TF As String(), members As String(), cstrand As String, bbhHits As String())
            Operon = members
            TF_trace = regulon.LocusId
            Regulators = TF
            Effector = regulon.effector
            Pathway = regulon.pathway
            BiologicalProcess = regulon.biological_process.JoinBy("; ")
            source = regulon.regulog.name
            Strand = cstrand
            bbh = bbhHits
        End Sub

        ''' <summary>
        ''' Copy regulon definition from <see cref="Regulator"/>
        ''' </summary>
        ''' <param name="regulon"></param>
        ''' <param name="TF"></param>
        ''' <param name="members"></param>
        ''' <param name="strand"></param>
        ''' <param name="bbhHits"></param>
        Sub New(regulon As Regulator, TF As String(), members As String(), strand As Strands, bbhHits As String())
            Call Me.New(regulon,
                        TF, members,
                        If(strand = Strands.Forward, "+", "-"),
                        bbhHits)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
