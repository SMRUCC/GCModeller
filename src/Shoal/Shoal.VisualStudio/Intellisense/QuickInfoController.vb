'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
' Copyright (c) Microsoft Corporation.  All rights reserved.
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Utilities

Namespace VSLTK.Intellisense

    Friend Class TemplateQuickInfoController
        Implements IIntellisenseController

        Private _textView As ITextView
        Private _subjectBuffers As IList(Of ITextBuffer)
        Private _componentContext As TemplateQuickInfoControllerProvider

        Private _session As IQuickInfoSession

        Friend Sub New(textView As ITextView, subjectBuffers As IList(Of ITextBuffer), componentContext As TemplateQuickInfoControllerProvider)
            _textView = textView
            _subjectBuffers = subjectBuffers
            _componentContext = componentContext

            AddHandler _textView.MouseHover, AddressOf OnTextViewMouseHover
        End Sub

        Public Sub ConnectSubjectBuffer(subjectBuffer As ITextBuffer) Implements IIntellisenseController.ConnectSubjectBuffer

        End Sub

        Public Sub DisconnectSubjectBuffer(subjectBuffer As ITextBuffer) Implements IIntellisenseController.DisconnectSubjectBuffer

        End Sub

        Public Sub Detach(textView As ITextView) Implements IIntellisenseController.Detach
            If _textView Is textView Then
                RemoveHandler _textView.MouseHover, AddressOf OnTextViewMouseHover
                _textView = Nothing
            End If
        End Sub

        Private Sub OnTextViewMouseHover(sender As Object, e As MouseHoverEventArgs)
            Dim point? As SnapshotPoint = Me.GetMousePosition(New SnapshotPoint(_textView.TextSnapshot, e.Position))

            If point IsNot Nothing Then
                Dim triggerPoint As ITrackingPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive)
                ' Find the broker for this buffer
                If Not _componentContext.QuickInfoBroker.IsQuickInfoActive(_textView) Then
                    _session = _componentContext.QuickInfoBroker.CreateQuickInfoSession(_textView, triggerPoint, True)
                    _session.Start()
                End If
            End If
        End Sub

        Private Function GetMousePosition(topPosition As SnapshotPoint) As SnapshotPoint?
            ' Map this point down to the appropriate subject buffer.
            Return _textView.BufferGraph.MapDownToFirstMatch(topPosition, PointTrackingMode.Positive, AddressOf __match, PositionAffinity.Predecessor)
        End Function

        Private Function __match(snapshot As ITextSnapshot) As Boolean
            Return _subjectBuffers.Contains(snapshot.TextBuffer)
        End Function
    End Class
End Namespace