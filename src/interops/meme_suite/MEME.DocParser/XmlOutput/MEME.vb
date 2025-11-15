#Region "Microsoft.VisualBasic::79900489206d5b6dc0102e4baf22e434, meme_suite\MEME.DocParser\XmlOutput\MEME.vb"

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

    '     Class Model
    ' 
    '         Properties: BackgroundFrequencies, Beta, Distance, EndGaps, EValueThreshold
    '                     Maxiter, MaxSites, MaxWidth, MiniC, MinSites
    '                     MinWidth, NumMotifs, NumPositions, NumSequences, ObjectFunction
    '                     Prior, PriorsFile, Prob, ReasonForStopping, Seed
    '                     SeqFrac, SpFuzz, SpMap, Strands, Type
    '                     Wg, WnSites, Ws
    ' 
    '     Class BackgroundFrequencies
    ' 
    '         Properties: AlphabetArray, Source
    ' 
    '         Function: ToString
    ' 
    '     Class AlphabetArray
    ' 
    '         Properties: Values
    ' 
    '         Function: GetValue
    '         Class Value
    ' 
    '             Properties: LetterId, Value
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    '     Class Scores
    ' 
    '         Properties: AlphabetMatrix
    ' 
    '     Class Motif
    ' 
    '         Properties: BayesThreshold, ContributingSites, ElapsedTime, EValue, Ic
    '                     Id, Llr, Name, Probabilities, Re
    '                     RegularExpression, Scores, Sites, Width
    ' 
    '         Function: GetEvalue, ToString
    '         Class ProbabilitiesArray
    ' 
    '             Properties: AlphabetMatrix
    ' 
    '         Class ContributingSite
    ' 
    '             Properties: Id, LeftFlank, Position, PValue, RightFlank
    '                         Site, Strand
    ' 
    '             Function: ToString
    '             Class LetterRef
    ' 
    '                 Properties: Id
    ' 
    '                 Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class MEME
    ' 
    '         Properties: Model, Motifs, ScannedSitesSummary, TrainingSet
    ' 
    '         Function: LoadDocument, ToMEMEHtml, ToString
    ' 
    '     Class ScannedSitesSummary
    ' 
    '         Properties: pThresh, ScannedSites
    '         Class ScannedSite
    ' 
    '             Properties: Id, NumSites, PValue, ScannedSites
    '             Class ScannedSite
    ' 
    '                 Properties: MotifId, Position, PValue, Strand
    ' 
    '                 Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class TrainingSet
    ' 
    '         Properties: Alphabet, Ambigs, DataFile, Length, LetterFrequencies
    '                     Sequences
    ' 
    '         Function: GetObject, ToString
    '         Class AmbigsArray
    ' 
    '             Properties: Letters
    ' 
    '         Class Sequence
    ' 
    '             Properties: Id, Length, Name, Weight
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    '     Class LetterFrequencies
    ' 
    '         Properties: AlphabetArray
    ' 
    '     Class Alphabet
    ' 
    '         Properties: Id, Length, Letters
    ' 
    '         Function: ToString
    '         Class Letter
    ' 
    '             Properties: Id, Symbol
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME

Namespace DocumentFormat.XmlOutput.MEME

    ''' <summary>
    ''' Model elements
    ''' </summary>
    <XmlType("model")> Public Class Model : Inherits XmlOutput.ModelBase

        <XmlElement("type")> Public Property Type As String
        <XmlElement("nmotifs")> Public Property NumMotifs As Integer
        <XmlElement("evalue_threshold")> Public Property EValueThreshold As String
        <XmlElement("object_function")> Public Property ObjectFunction As String
        <XmlElement("min_width")> Public Property MinWidth As String
        <XmlElement("max_width")> Public Property MaxWidth As String
        <XmlElement("minic")> Public Property MiniC As String
        <XmlElement("wg")> Public Property Wg As String
        <XmlElement("ws")> Public Property Ws As String
        <XmlElement("endgaps")> Public Property EndGaps As String
        <XmlElement("minsites")> Public Property MinSites As String
        <XmlElement("maxsites")> Public Property MaxSites As String
        <XmlElement("wnsites")> Public Property WnSites As String
        <XmlElement("prob")> Public Property Prob As String
        <XmlElement("spmap")> Public Property SpMap As String
        <XmlElement("spfuzz")> Public Property SpFuzz As String
        <XmlElement("prior")> Public Property Prior As String
        <XmlElement("beta")> Public Property Beta As String
        <XmlElement("maxiter")> Public Property Maxiter As String
        <XmlElement("distance")> Public Property Distance As String
        <XmlElement("num_sequences")> Public Property NumSequences As String
        <XmlElement("num_positions")> Public Property NumPositions As String
        <XmlElement("seed")> Public Property Seed As String
        <XmlElement("seqfrac")> Public Property SeqFrac As String
        <XmlElement("strands")> Public Property Strands As String
        <XmlElement("priors_file")> Public Property PriorsFile As String
        <XmlElement("reason_for_stopping")> Public Property ReasonForStopping As String
        <XmlElement("background_frequencies")> Public Property BackgroundFrequencies As BackgroundFrequencies

    End Class

    <XmlType("background_frequencies")> Public Class BackgroundFrequencies
        <XmlAttribute("source")> Public Property Source As String
        <XmlElement("alphabet_array")> Public Property AlphabetArray As AlphabetArray

        Public Overrides Function ToString() As String
            Return String.Format("<background_frequencies source=""{0}"">", Source)
        End Function


    End Class

    <XmlType("alphabet_array")> Public Class AlphabetArray
        <XmlElement("value")> Public Property Values As Value()

        Public Function GetValue(alphabet As String) As String
            Dim LQuery = From value In Values Where String.Equals(value.LetterId, alphabet) Select value.Value '
            Try
                Return LQuery.First
            Catch ex As Exception
                Return ""
            End Try
        End Function

        <XmlType("value")> Public Class Value
            <XmlAttribute("letter_id")> Public Property LetterId As String
            <XmlText()> Public Property Value As String

            Public Overrides Function ToString() As String
                Return String.Format("<value letter_id=""{0}"">{1}</value>", LetterId, Value)
            End Function
        End Class
    End Class

    Public Class Scores
        <XmlArray("alphabet_matrix")> Public Property AlphabetMatrix As XmlOutput.MEME.AlphabetArray()
    End Class

    ''' <summary>
    ''' Motif elements
    ''' </summary>
    <XmlType("motif")> Public Class Motif
        <XmlAttribute("id")> Public Property Id As String
        <XmlAttribute("name")> Public Property Name As String
        <XmlAttribute("width")> Public Property Width As String
        <XmlAttribute("sites")> Public Property Sites As String
        <XmlAttribute("ic")> Public Property Ic As String
        <XmlAttribute("re")> Public Property Re As String
        <XmlAttribute("llr")> Public Property Llr As String
        <XmlAttribute("e_value")> Public Property EValue As String
        <XmlAttribute("bayes_threshold")> Public Property BayesThreshold As String
        <XmlAttribute("elapsed_time")> Public Property ElapsedTime As String
        <XmlElement("scores")> Public Property Scores As Scores
        <XmlElement("probabilities")> Public Property Probabilities As ProbabilitiesArray

        Public Shared Function GetEvalue(motifId As String, Motifs As Motif()) As Double
            Dim LQuery = (From motif In Motifs Where String.Equals(motifId, motif.Id) Select motif.EValue).ToArray
            If LQuery.IsNullOrEmpty Then
                Return -1
            Else
                Return LQuery.First
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("motif:={0};", Name)
        End Function

        Public Class ProbabilitiesArray
            <XmlArray("alphabet_matrix")> Public Property AlphabetMatrix As XmlOutput.MEME.AlphabetArray()
        End Class

        Dim _InternalExpression As String

        <XmlElement("regular_expression")> Public Property RegularExpression As String
            Get
                Return _InternalExpression
            End Get
            Set(value As String)
                _InternalExpression = value.Replace(vbCr, "").Replace(vbLf, "")
            End Set
        End Property

        ''' <summary>
        ''' Contributing site elements
        ''' Contributing sites are motif occurences found during the motif discovery phase
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("contributing_sites")> Public Property ContributingSites As ContributingSite()

        <XmlType("contributing_site")> Public Class ContributingSite
            <XmlAttribute("sequence_id")> Public Property Id As String
            <XmlAttribute("position")> Public Property Position As String
            <XmlAttribute("strand")> Public Property Strand As String
            <XmlAttribute("pvalue")> Public Property PValue As String
            ''' <summary>
            ''' The left_flank contains the sequence for 10 bases to the left of the motif start
            ''' </summary>
            ''' <returns></returns>
            <XmlElement("left_flank")> Public Property LeftFlank As String
            ''' <summary>
            ''' The site contains the sequence for the motif instance
            ''' </summary>
            ''' <returns></returns>
            <XmlArray("site")> Public Property Site As LetterRef()
            ''' <summary>
            ''' The right_flank contains the sequence for 10 bases to the right of the motif end
            ''' </summary>
            ''' <returns></returns>
            <XmlElement("right_flank")> Public Property RightFlank As String

            <XmlType("letter_ref")> Public Class LetterRef
                <XmlAttribute("letter_id")> Public Property Id As String

                Public Overrides Function ToString() As String
                    Return Id.Replace("letter_", "")
                End Function
            End Class

            Public Overrides Function ToString() As String
                Return $"<contributing_site sequence_id=""{Id}"" position=""{Position}"" strand=""{Strand}"" pvalue=""{PValue}"" >"
            End Function
        End Class
    End Class

    ''' <summary>
    ''' &lt;MEME version="4.10.2" release="Thu Sep 03 15:00:54 2015 -0700">.
    ''' </summary>
    ''' <remarks>
    ''' (MEME程序的输出文件的XML文档)
    ''' </remarks>
    <XmlRoot("MEME", DataType:="MEME", IsNullable:=True)> Public Class MEMEXml : Inherits XmlOutput.MEMEXmlBase

        ''' <summary>
        ''' Training-set elements
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("training_set")> Public Property TrainingSet As TrainingSet

        Public Overrides Function ToString() As String
            Return String.Format("<MEME version=""{0}"" release=""{1}"">", Version, Release)
        End Function

        <XmlArray("motifs")> Public Property Motifs As Motif()
        <XmlElement("model")> Public Property Model As Model

        ''' <summary>
        ''' Scanned site elements
        ''' Scanned sites are motif occurences found during the sequence scan phase
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("scanned_sites_summary")> Public Property ScannedSitesSummary As ScannedSitesSummary

        Public Shared Function LoadDocument(path As String) As MEMEXml
            Dim Text As String = FileIO.FileSystem.ReadAllText(path)
            Text = Regex.Replace(Text, "<!DOCTYPE.+?]>", "")
            Return Text.LoadFromXml(Of MEMEXml)()
        End Function

        Public Function ToMEMEHtml() As HTML.MEMEHtml
            Dim Html As HTML.MEMEHtml = New HTML.MEMEHtml With {
                .ObjectId = BaseName(Me.TrainingSet.DataFile)
            }
            Html.Motifs = (From MotifObject As Motif
                           In Me.Motifs
                           Let motif As HTML.Motif = New HTML.Motif With {
                               .Evalue = MotifObject.EValue,
                               .Id = Val(MotifObject.Id.Replace("motif_", "")),
                               .LogLikelihoodRatio = MotifObject.Llr,
                               .RegularExpression = MotifObject.RegularExpression,
                               .Width = MotifObject.Width
                           }
                           Select motif).ToArray
            Return Html
        End Function
    End Class

    <XmlType("scanned_sites_summary")> Public Class ScannedSitesSummary
        <XmlAttribute("p_thresh")> Public Property pThresh As String
        <XmlElement("scanned_sites")> Public Property ScannedSites As ScannedSite()

        <XmlType("scanned_sites")> Public Class ScannedSite
            <XmlAttribute("sequence_id")> Public Property Id As String
            <XmlAttribute("pvalue")> Public Property PValue As String
            <XmlAttribute("num_sites")> Public Property NumSites As String
            <XmlElement("scanned_site")> Public Property ScannedSites As ScannedSite()

            <XmlType("scanned_site")> Public Class ScannedSite
                <XmlAttribute("motif_id")> Public Property MotifId As String
                <XmlAttribute("strand")> Public Property Strand As String
                <XmlAttribute("position")> Public Property Position As String
                <XmlAttribute("pvalue")> Public Property PValue As String

                Public Overrides Function ToString() As String
                    Return String.Format("<scanned_site motif_id=""{0}"" strand=""{1}"" position=""{2}"" pvalue=""{3}""/>", MotifId, Strand, Position, PValue)
                End Function
            End Class
        End Class
    End Class

    ''' <summary>
    ''' Training-set elements
    ''' </summary>
    Public Class TrainingSet

        <XmlAttribute("datafile")> Public Property DataFile As String
        <XmlAttribute("length")> Public Property Length As String
        <XmlElement("alphabet")> Public Property Alphabet As Alphabet
        <XmlElement("ambigs")> Public Property Ambigs As AmbigsArray
        <XmlElement("sequence")> Public Property Sequences As Sequence()
        <XmlElement("letter_frequencies")> Public Property LetterFrequencies As LetterFrequencies

        Public Function GetObject(Id As String) As Sequence
            Dim LQuery = (From seq In Sequences Where String.Equals(Id, seq.Id) Select seq).ToArray
            Return LQuery.First
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("<training_set datafile=""{0}"" length=""{1}"">", DataFile, Length)
        End Function

        Public Class AmbigsArray
            <XmlElement("letter")> Public Property Letters As Alphabet.Letter()
        End Class

        Public Class Sequence
            <XmlAttribute("id")> Public Property Id As String
            <XmlAttribute("name")> Public Property Name As String
            <XmlAttribute("length")> Public Property Length As String
            <XmlAttribute("weight")> Public Property Weight As String

            Public Overrides Function ToString() As String
                Return Name
            End Function
        End Class
    End Class

    Public Class LetterFrequencies
        <XmlElement("alphabet_array")> Public Property AlphabetArray As AlphabetArray
    End Class

    Public Class Alphabet
        <XmlAttribute("id")> Public Property Id As String
        <XmlAttribute("length")> Public Property Length As String
        <XmlElement("letter")> Public Property Letters As Letter()

        Public Overrides Function ToString() As String
            Return String.Format("<alphabet id=""{0}"" length=""{1}"">", Id, Length)
        End Function

        Public Class Letter
            <XmlAttribute("id")> Public Property Id As String
            <XmlAttribute("symbol")> Public Property Symbol As String

            Public Overrides Function ToString() As String
                Return String.Format("<letter id=""{0}"" symbol=""{1}"">", Id, Symbol)
            End Function
        End Class
    End Class
End Namespace
