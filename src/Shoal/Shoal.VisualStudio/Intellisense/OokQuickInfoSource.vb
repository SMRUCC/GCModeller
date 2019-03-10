﻿Imports System.Text
Imports Microsoft.VisualStudio.Language.Intellisense
Imports System.Collections.ObjectModel
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Tagging
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Utilities
Imports System.Runtime.InteropServices

Namespace OokLanguage

    <Export(GetType(IQuickInfoSourceProvider)), ContentType("ook!"), Name("ookQuickInfo")>
    Friend Class OokQuickInfoSourceProvider
        Implements IQuickInfoSourceProvider

        <Import()>
        Private aggService As IBufferTagAggregatorFactoryService = Nothing

        Public Function TryCreateQuickInfoSource(textBuffer As ITextBuffer) As IQuickInfoSource Implements IQuickInfoSourceProvider.TryCreateQuickInfoSource
            Return New OokQuickInfoSource(textBuffer, aggService.CreateTagAggregator(Of OokTokenTag)(textBuffer))
        End Function

    End Class

    Friend Class OokQuickInfoSource
        Implements IQuickInfoSource

        Private _aggregator As ITagAggregator(Of OokTokenTag)
        Private _buffer As ITextBuffer
        Private _disposed As Boolean = False

        Public Sub New(buffer As ITextBuffer, aggregator As ITagAggregator(Of OokTokenTag))
            _aggregator = aggregator
            _buffer = buffer
        End Sub

        Public Sub AugmentQuickInfoSession(session As IQuickInfoSession, quickInfoContent As IList(Of Object), <Out()> ByRef applicableToSpan As ITrackingSpan) Implements IQuickInfoSource.AugmentQuickInfoSession
            applicableToSpan = Nothing

            If _disposed Then
                Throw New ObjectDisposedException("TestQuickInfoSource")
            End If

            Dim triggerPoint = CType(session.GetTriggerPoint(_buffer.CurrentSnapshot), SnapshotPoint)
            If triggerPoint = Nothing Then
                Return
            End If

            For Each curTag As IMappingTagSpan(Of OokTokenTag) In _aggregator.GetTags(New SnapshotSpan(triggerPoint, triggerPoint))
                If curTag.Tag.type = Scripting.ShoalShell.Interpreter.LDM.Expressions.ExpressionTypes.Die Then
                    Dim tagSpan = curTag.Span.GetSpans(_buffer).First()
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive)
                    quickInfoContent.Add("Throw the exception!")
                End If
            Next curTag
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _disposed = True
        End Sub

    End Class
End Namespace

