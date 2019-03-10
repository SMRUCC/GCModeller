#Region "Microsoft.VisualBasic::efd18058644081208505d28dee6f6660, meme_suite\MEME.DocParser\TomTom\Xml.vb"

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

    '     Class TOMXml
    ' 
    '         Properties: model, queries, runtime, targets
    ' 
    '         Function: HaveData, ToString
    ' 
    '     Class runtime
    ' 
    '         Properties: cycles, seconds
    ' 
    '         Function: ToString
    ' 
    '     Class target_file
    ' 
    '         Properties: excluded, index, loaded, motif
    ' 
    '     Class queries
    ' 
    '         Properties: query_file
    ' 
    '     Class MotifFile
    ' 
    '         Properties: last_mod_date, name, source
    ' 
    '         Function: ToString
    ' 
    '     Class query_file
    ' 
    '         Properties: query
    ' 
    '     Class query
    ' 
    '         Properties: match, motif
    ' 
    '     Class match
    ' 
    '         Properties: evalue, offset, orientation, pvalue, qvalue
    '                     target
    ' 
    '         Function: ToString
    ' 
    '     Class motif
    ' 
    '         Properties: evalue, id, length, name, nsites
    '                     pos
    ' 
    '     Class pos
    ' 
    '         Properties: A, C, G, i, T
    ' 
    '         Function: ToString
    ' 
    '     Class Model
    ' 
    '         Properties: [when], background, distance_measure, threshold
    ' 
    '     Class value
    ' 
    '         Properties: value
    ' 
    '         Function: ToString
    ' 
    '     Class threshold
    ' 
    '         Properties: type, value
    ' 
    '         Function: ToString
    ' 
    '     Class background
    ' 
    '         Properties: A, C, from, G, T
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DocumentFormat.TomTOM

    <XmlType("tomtom")>
    Public Class TOMXml : Inherits XmlOutput.MEMEXmlBase
        Public Property model As Model
        Public Property targets As target_file()
        Public Property queries As queries
        Public Property runtime As runtime

        Public Overrides Function ToString() As String
            Return model.ToString
        End Function

        Public Shared Function HaveData(x As TOMXml) As Boolean
            If x Is Nothing Then
                Return False
            End If
            If x.targets.IsNullOrEmpty Then
                Return False
            End If
            If x.queries Is Nothing Then
                Return False
            End If
            If x.queries.query_file.query.IsNullOrEmpty Then
                Return False
            End If

            Return True
        End Function
    End Class

    Public Class runtime
        <XmlAttribute> Public Property cycles As Long
        <XmlAttribute> Public Property seconds As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class target_file : Inherits MotifFile
        <XmlAttribute> Public Property index As String
        <XmlAttribute> Public Property loaded As String
        <XmlAttribute> Public Property excluded As String

        <XmlElement> Public Property motif As motif()
    End Class

    Public Class queries
        Public Property query_file As query_file
    End Class

    Public MustInherit Class MotifFile
        <XmlAttribute> Public Property source As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property last_mod_date As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class query_file : Inherits MotifFile
        <XmlElement> Public Property query As query()
    End Class

    Public Class query
        Public Property motif As motif
        <XmlElement> Public Property match As match()
    End Class

    Public Class match
        <XmlAttribute> Public Property target As String
        <XmlAttribute> Public Property orientation As String
        <XmlAttribute> Public Property offset As String
        <XmlAttribute> Public Property pvalue As String
        <XmlAttribute> Public Property evalue As String
        <XmlAttribute> Public Property qvalue As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class motif
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property length As String
        <XmlAttribute> Public Property evalue As Double
        <XmlAttribute> Public Property nsites As Integer
        <XmlElement> Public Property pos As pos()
    End Class

    Public Class pos
        <XmlAttribute> Public Property i As Integer
        <XmlAttribute> Public Property A As Double
        <XmlAttribute> Public Property C As Double
        <XmlAttribute> Public Property G As Double
        <XmlAttribute> Public Property T As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    <XmlType("model")>
    Public Class Model : Inherits XmlOutput.ModelBase
        Public Property distance_measure As value
        Public Property threshold As threshold
        Public Property background As background
        Public Property [when] As String
    End Class

    Public Class value
        <XmlAttribute> Public Property value As String

        Public Overrides Function ToString() As String
            Return value
        End Function
    End Class

    Public Class threshold
        <XmlAttribute> Public Property type As String
        <XmlText> Public Property value As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class background
        <XmlAttribute> Public Property from As String
        <XmlAttribute> Public Property A As Double
        <XmlAttribute> Public Property C As Double
        <XmlAttribute> Public Property G As Double
        <XmlAttribute> Public Property T As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
